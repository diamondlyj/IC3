using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using System.Xml.XPath;

namespace MIX2.Acquisition.LDAP
{
    public class ObjectFactory : IMIXFactory
    {
        private bool eod;
        private string sourceName;
        private MIX2.Acquisition.DataSourceSchema.ObjectClass objClass;
        private MapSchema mapSchema;
        private Connection connection;
        private List<SearchResultCollection> results;
        private int listPos;
        private int itemPos;
        //private Queue<System.DirectoryServices.SearchResult> entries;

        private Dictionary<string, Expression> rootExpressions;
        private Expression idExpression;
        private Expression parentExpression;

        //Below variable are used to map to a property. 
        //The only reason they exist is to be able to map multi value properties 
        //to the single value attributes of MIX1

        private MIX2.Data.LocalObject current;
        private string queriedAttribute;
        private object[] queriedAttributeValues;
        private Dictionary<string, object[]> queriedPropVals;
        private int queriedPropPos;
        private object baseID;
        private object[] ids;
        private bool useIDs;
        private object[] parentIDs;
        private bool useParentIDs;

        public ObjectFactory(string sourceName, object connection)
        {
            this.sourceName = sourceName;
            this.connection = (Connection)connection;
            this.eod = true;
        }

        public bool IsAlive(out string message)
        {
            message = "OK";
            return true;
        }

        public void Open(MIX2.Acquisition.DataSourceSchema.ObjectClass objectClass)
        {
            this.results = new List<SearchResultCollection>();
            this.listPos = 0;
            this.itemPos = 0;
            this.queriedAttribute = null;
            this.queriedAttributeValues = new object[0];
            this.ids = new object[0];
            this.useIDs = false;
            this.parentIDs = new object[0];
            this.useParentIDs = false;

            this.queriedPropPos = 0;
            this.queriedPropVals = new Dictionary<string,object[]>();
            this.current = null;
            this.baseID = null;

            this.rootExpressions = new Dictionary<string, Expression>();
            this.parentExpression = null;

            this.objClass = objectClass;
            this.mapSchema = new MapSchema(this.objClass.Map);

            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("@\\w+$");

            string query = this.mapSchema.Query.Trim();

            //We need to check if the query to get the objects from LDAP has a attribute directive at the end (e.g. @myLDAPAttribue)
            //If it does, we extract what that attribute is and remove it from the actual LDAP query sent to the server.
            //The attute is then use in GetNext() to turn each value of the LDAP attribute into a seperate LocalObject.
            System.Text.RegularExpressions.Match ma = re.Match(query);

            if (ma.Success)
            {
                this.queriedAttribute = ma.Value;
                query = query.Substring(0, ma.Index);
            }

            Parser parser = new Parser();

            if (this.mapSchema.LocalID != null)
            {
                string idQuery = this.mapSchema.LocalID.Query;

                //Check if the query for the LocalID has the attribute defined by the attribute directive in the base query (see above)
                //If it does, each LocalID will be constructed based on the value evaluated by the scripting for each attribute instance in LDAP (rather than the full value of the attribute itself)
                if ( ma.Success && idQuery.Contains(this.queriedAttribute) )
                    this.useIDs = true;

                Token[] idTokens = Tokenizer.Tokenize(idQuery);
                this.idExpression = parser.Parse(idTokens);
            }
            else
                this.idExpression = null;

            if (this.mapSchema.Parent != null)
            {
                string parQuery = this.mapSchema.Parent.Query;

                //Check if the query for the ParentID has the attribute defined by the attribute directive in the base query (see above)
                //If it does, each ParentID will be different for each attribute instance turned into a separate LocalObject
                if (ma.Success && parQuery.Contains(this.queriedAttribute))
                    this.useParentIDs = true;

                Token[] tokens = Tokenizer.Tokenize(parQuery);
                this.parentExpression = parser.Parse(tokens);
            }

            //Tokenize and parse script
            foreach (MapSchema.Property prop in this.mapSchema.Properties)
            {
                string propQuery = prop.Query;
                Token[] tokens = Tokenizer.Tokenize(propQuery);

                this.rootExpressions.Add(prop.Name, parser.Parse(tokens));

                if( ma.Success && propQuery.Contains(this.queriedAttribute) )
                    this.queriedPropVals.Add(prop.Name, new object[0]);

            }

            if (this.queriedAttribute != null)
                this.queriedAttribute = this.queriedAttribute.Substring(1);


            //Get releveant entries
            //this.entries = new Queue<System.DirectoryServices.SearchResult>();

            string filter = @"(&(cn>=*)(" + query + "))";


            System.DirectoryServices.SearchResultCollection res = this.connection.GetData( filter );

            int n = res.Count;

            if( n == 0 )
            {
                Console.WriteLine("Search with filter " + filter + " returned no  " + this.objClass.Name + " objects.");
                this.eod = true;
                return;
            }

            this.eod = false;

            if (n == 1)
                n++;

            object startVal = new object();

            int m = 0;

            do
            {
                Console.WriteLine(objClass.Name +  ": Retrieving objects " + m.ToString() + " to " + (m +res.Count-1).ToString() );

                m += res.Count;
                
                /*
                for (int i = 0; i < n - 1; i++)
                {
                    this.entries.Enqueue(res[i]);
                }
                */

                //If more than 1 directory entry is returned, then add the result to the collection of results.
                //Otherwise, the entry is the last item in the directory (which still has to be added as a single item collection)
                if (res.Count > 1)
                {
                    this.results.Add(res);

                    //Console.WriteLine(res[res.Count - 1].Properties["cn"][0]);
                    //Console.WriteLine(startVal);

                    if (!res[res.Count - 1].Properties["cn"][0].Equals(startVal) )
                    {
                        //Console.WriteLine("THEY ARE NOT EQUAL");
                        startVal = res[res.Count - 1].Properties["cn"][0];

                        filter = @"(&(cn>=" + startVal + ")(" + query + "))";

                        res = this.connection.GetData(filter);


                        n = res.Count;

                        if (n == 1)
                            n++;
                    }
                    else
                        n = 0;
                }
                else
                {
                    //This gurantees that the last entry is processed as well.
                    this.results.Add(res);
                    n = 0;
                }


            } while (n > 0);

            if (this.results.Count == 0)
                this.eod = true;
        }

        public void Close()
        {
            foreach (SearchResultCollection coll in this.results)
                coll.Dispose();

            return;
        }

        public bool EOD
        {
            get { return this.eod; }
        }

        public MIX2.Data.LocalObject GetNext()
        {
            //Perhaps we should throw an error instead if EOD
            if (this.eod)
                return new MIX2.Data.LocalObject();

            MIX2.Data.LocalObject obj;

            if (this.current != null)
            {
                obj = this.current;
            }
            else
            {
            GetData:
                obj = new MIX2.Data.LocalObject(this.sourceName, this.objClass.Domain, this.objClass.Name);               

                System.DirectoryServices.SearchResult res = this.results[listPos][this.itemPos];

                MovePositions();

                /*
                if (this.entries.Count == 0 && this.queriedAttribute == null )
                    this.eod = true;
                 * */

                System.DirectoryServices.DirectoryEntry self = res.GetDirectoryEntry();                

                //If a LocalObject is being mapped to an LDAP property (and not an entity), 
                //then store the LocalObject in memory, since it will be returned several times (each time with a changed property value)
                //and store the values of the attribute
                if (this.queriedAttribute != null)
                {
                    this.current = obj;

                    if (self.Properties.Contains(this.queriedAttribute))
                    {
                        Type t = self.Properties[this.queriedAttribute].Value.GetType();

                        if (t == typeof(object[]))
                        {
                            object[] vals = (object[])self.Properties[this.queriedAttribute].Value;
                            this.queriedAttributeValues = vals;
                        }
                        else
                        {
                            this.queriedAttributeValues = new object[] { self.Properties[this.queriedAttribute] };
                        }
                    }
                    else
                    {
                        //If there are no values for the LDAP attribute, move on to the next LDAP object.
                        //If we have reached the end of the data, dispose of the DirectoryEntry and return an empty object.

                        if (!this.eod)
                        {
                            this.current = null;

                            MovePositions();

                            goto GetData;
                        }
                        else
                        {
                            self.Dispose();
                            return new MIX2.Data.LocalObject();
                        }

                        /*
                        if (this.entries.Count > 0)
                            goto GetData;
                        else
                            return new MIX2.Data.LocalObject();
                         * */
                    }
                }

                foreach (MapSchema.Property prop in this.mapSchema.Properties)
                {
                    MIX2.Data.LocalObject.Property objProp = new MIX2.Data.LocalObject.Property(prop.Name);

                    this.rootExpressions[prop.Name].Evaluate(self);

                    object value = this.rootExpressions[prop.Name].Value;

                    Type t = value.GetType();

                    if (t == typeof(object[]))
                    {                        
                        object[] vals = (object[])value;

                        //If a LocalObject is being mapped to an LDAP property (and not an entity), 
                        // then store in memory the vals of the property if it is the one being mapped to
                        // Otherwise, just place the vals in the LocalObject that will be returned.
                        if (this.queriedAttribute != null && this.queriedPropVals.ContainsKey(prop.Name))
                        {                            
                            this.queriedPropVals[prop.Name] = vals;
                        }
                        else
                        {

                            for (int i = 0; i < vals.Length; i++)
                            {
                                string val = Converter.ToString(vals[i]);

                                if (val != string.Empty)
                                    objProp.Values.Add(val);
                            }
                        }
                    }
                    else
                    {
                        string val = Converter.ToString(value);

                        if (val != string.Empty)
                            objProp.Values.Add(val);
                    }


                    obj.Properties.Add(objProp);

                }

                if (this.idExpression != null)
                {
                    this.idExpression.Evaluate(self);

                    Type t = this.idExpression.Value.GetType();

                    if (this.useIDs)
                    {
                        if (t == typeof(object[]))
                            this.ids = (object[])this.parentExpression.Value;
                        else
                            this.ids = new object[] { this.parentExpression.Value };
                    }
                    else
                    {

                        obj.LocalID = string.Empty;

                        if (t == typeof(object[]))
                        {
                            object[] ids = (object[])this.idExpression.Value;

                            for (int i = 0; i < ids.Length; i++)
                            {
                                if (i > 0)
                                    obj.LocalID += ";";

                                obj.LocalID += Converter.ToString(ids[i]);
                            }
                        }
                        else
                        {
                            obj.LocalID = Converter.ToString(this.idExpression.Value);
                        }

                        this.baseID = obj.LocalID;
                    }
                }
                else
                {
                    obj.LocalID = self.Path;
                    this.baseID = self.Path;
                }

                if (this.parentExpression != null)
                {
                    this.parentExpression.Evaluate(self);

                    Type t = this.parentExpression.Value.GetType();

                    obj.ParentClass = this.mapSchema.Parent.ObjectClass;

                    if (this.useParentIDs)
                    {
                        if (t == typeof(object[]))
                            this.parentIDs = (object[])this.parentExpression.Value;
                        else
                            this.parentIDs = new object[] { this.parentExpression.Value };

                    }
                    else
                    {
                        obj.ParentID = string.Empty;

                        if (t == typeof(object[]))
                        {
                            object[] ids = (object[])this.parentExpression.Value;

                            for (int i = 0; i < ids.Length; i++)
                            {
                                if (i > 0)
                                    obj.ParentID += ";";

                                obj.ParentID += Converter.ToString(ids[i]);
                            }
                        }
                        else
                        {
                            obj.ParentID = Converter.ToString(this.parentExpression.Value);
                        }
                    }
                }

                self.Dispose();
            }


            //If a LocalObject is being mapped to an LDAP property (and not an entity), 
            //then replace the mapped property with the next value
            if (this.queriedAttribute != null && this.queriedAttributeValues.Length > 0)
            {
                if (this.useIDs)
                    this.current.LocalID = this.ids[this.queriedPropPos] + ";" + this.queriedPropPos.ToString();
                else
                    this.current.LocalID = this.baseID + ";" + this.queriedPropPos.ToString(); //Converter.ToString(this.queriedAttributeValues[this.queriedPropPos]);
                    

                foreach (string key in this.queriedPropVals.Keys)
                {
                    foreach (MIX2.Data.LocalObject.Property prop in this.current.Properties)
                    {
                        if (prop.Name == key)
                        {
                            object[] vals = this.queriedPropVals[key];
                            prop.Values.Clear();
                            
                            if(this.queriedPropPos < vals.Length )
                                prop.Values.Add( Converter.ToString(vals[this.queriedPropPos]) );
                        }
                    }
                }
                
                if (this.useParentIDs)
                    this.current.ParentID = Converter.ToString(this.parentIDs[this.queriedPropPos]);

                if (this.queriedPropPos == this.queriedAttributeValues.Length-1)
                {
                    this.current = null;
                    this.queriedPropPos = 0;

                    //If the querried property has no more values and we have reached the end of the search results, declare EOD.
                    if (this.itemPos == this.results[listPos].Count && this.listPos == this.results.Count)
                        this.eod = true;
                }
                else
                    this.queriedPropPos++;

            }


            return obj;
        }

        private void MovePositions()
        {
            this.itemPos++;

            if (this.itemPos == this.results[listPos].Count)
            {
                this.itemPos = 0;
                this.listPos++;

                if (this.listPos == this.results.Count)
                    this.eod = true;
            }
        }
    }


}
