using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace MIX2.Acquisition.IBM.BigFix
{
    public class Factory2: IMIXFactory
    {
        string sourceName;
        DataSourceSchema.ObjectClass currentClass;
        MIX2.Acquisition.MapSchema map;
        WebConnection connection;
        int chunkCount;

        string[] ids;
        XDocument idXml;
        IEnumerable<XElement> objectIDs;
        IEnumerator<XElement> idEnumerator;
        int count;

        IEnumerable<NonLocusObject> nonLocusObjects;
        IEnumerator<NonLocusObject> nonLocusEnumerator;

        int idPosition;
        IEnumerator<XElement> posEnumerator;
        XDocument nolidXml;
        IEnumerator<XElement> nolidEnumerator;
        string[,] nonLocusIDs;
        int nonLocusPosition;
        bool hasParent;
        bool parentIsLocus;
        string[] eprops;
        string[] eids;
        string firstID, lastID;

        //string[][] props;
        XDocument[] propXmls;
        IEnumerator<XElement>[] propEnumumerators;

        int[] propPosition;
        //The multidimensional array will be filled by bes computer id + property value.
        string[,] buffer;
        
        int position;
        int chunkSize;
        bool eod;

        public Factory2(string source, object connection)
        {
            this.connection = (WebConnection)connection;
            this.connection.RequestMethod = "GET";

            this.sourceName = source;
            //this.ids = new string[] { };
            this.eprops = new string[] { };
            this.eids = new string[] { };
            this.idPosition = -1;
            this.nonLocusPosition = -1;
            this.chunkSize = -1;
            this.hasParent = false;
            this.eod = true;
            this.parentIsLocus = false;
            this.count = 0;
        }

        public bool IsAlive(out string mes)
        {
            mes = "OK";

            try
            {
                System.IO.Stream stream = this.connection.GetRESTData("login");
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);

                mes = reader.ReadToEnd();

                //mes = this.sourceName + " contains no computer data.";
            }
            catch(Exception exc) 
            {
                mes = exc.Message;
            }

            if (mes.ToLower() == "ok")
                return true;

            return false;
        }

        public void Open(DataSourceSchema.ObjectClass objectClass)
        {
            /*
            System.IO.Stream stream = this.connection.GetRESTData("computers");
            System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(stream);

            stream.Close();

            string f,z;
            f = "";
            z = "";

            int n = 0;

            foreach(XElement computer in doc.Descendants("Computer"))
            {
                XElement id = computer.Element("ID");
                XElement time = computer.Element("LastReportTime");

                if(n == 0)
                    f = id.Value;

                if(n == 5000)
                {
                    z = id.Value;
                    break;
                }

                n++;
            }

            try
            {
                
                string query = "(id of it,(values of it) of ((property results of it) whose (((name of it) of property of it) = \"Installed Applications - Windows\"))) of(bes computers whose(id of it is greater than or equal to " + f + " and id of it is less than " + z + "))";

                stream = this.connection.GetRESTData("query?relevance=" + System.Web.HttpUtility.UrlEncode(query) );
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);

                Console.WriteLine( reader.ReadToEnd() );

                stream.Close();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            */

            
            this.currentClass = objectClass;
            this.chunkSize = objectClass.ChunkSize;
            this.chunkCount = 0;

            this.map = new MapSchema(objectClass.Map);

            if (this.map.Parent != null)
            {
                this.hasParent = true;
            }

            string locusValue = this.map.Locus.ToLower();

            if (locusValue == "parent")
            {
                this.parentIsLocus = true;
            }
            else if( locusValue != string.Empty && locusValue != "object")
            {
                throw new Exception("The locus of the map(" + locusValue + ") is not a recognized.");
            }


            Console.Write("Bypassing SSL check...");

            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            Console.Write("Getting objects...");

            string query = "query?relevance=";

            if (!parentIsLocus)
            {
               query += System.Web.HttpUtility.UrlEncode(map.Query);
            }
            else
            {
                if (map.Parent == null && map.Parent.Query == null)
                    throw new Exception("The locus is parent but no query was defined for parent objects.");

                query += System.Web.HttpUtility.UrlEncode(map.Parent.Query);
            }


            System.IO.Stream stream = this.connection.GetRESTData(query);
            this.idXml = System.Xml.Linq.XDocument.Load(stream);
            
            stream.Close();

            XElement result = this.idXml.Root.Element("Query").Element("Result");

            this.count = result.Descendants("Answer").Count();

            if (this.map.ExtractionProperties != string.Empty)
            {
                this.eprops = this.map.ExtractionProperties.Split(',');

                for (int i = 0; i < eprops.Length; i++)
                {
                    this.eprops[i] = this.eprops[i].Trim();

                    if (this.eprops[i] == string.Empty)
                        throw new Exception("The name of properties to extract cannot be empty.");
                }

            }

            if (this.map.LocalID != string.Empty)
            {
                this.eids = this.map.LocalID.Split(',');

                for (int i = 0; i < eids.Length; i++)
                {
                    this.eids[i] = this.eids[i].Trim();

                    if (this.eids[i] == string.Empty)
                        throw new Exception("A local id property cannot be empty.");
                }

            }


            if (this.count == 0)
            {
                this.idPosition = -1;
                this.nonLocusPosition = -1;
                this.eod = true;

                Console.WriteLine("...NO OBJECTS RETURNED");

                return;
            }

            this.objectIDs = result.Elements("Answer");
            this.idEnumerator = this.objectIDs.GetEnumerator();
            this.posEnumerator = this.objectIDs.GetEnumerator();
            this.idEnumerator.MoveNext();
            this.eod = !posEnumerator.MoveNext();

            Console.WriteLine(this.count + "...DONE");

            int len = map.Properties.Count();

            //this.props = new string[len][];
            this.propXmls = new XDocument[len];
            this.propEnumumerators = new IEnumerator<XElement>[len];
            this.propPosition = new int[len];
            this.buffer = new string[len,2];

            if ( this.chunkSize < 1)
            {
                this.chunkSize = this.count;
            }

            this.idPosition = 0;
            this.nonLocusPosition = 0;
            this.eod = false;
            this.currentClass = objectClass;

            //Console.WriteLine("Press a key....");
            //Console.ReadKey();

            //System.Threading.Thread.Sleep(180000);

            if (this.parentIsLocus || this.map.Properties.Count() > 0 || (this.map.Parent != null && this.map.Parent.Query != null) )
            {
                this.NextChunk();
                this.GetChunk();
            }

            return;
        }

        public void Close()
        {
            Console.WriteLine("Returned objects:" + this.idPosition);
            return;
        }

        public bool EOD
        {
            get
            {
                return this.eod;
            }
        }

        public MIX2.Data.LocalObject GetNext()
        {
            Data.LocalObject obj = new Data.LocalObject( this.sourceName, this.currentClass.Domain, this.currentClass.Name);
            obj.ObjectClass = this.currentClass.Name;            

            string id = this.posEnumerator.Current.Value;
            
            string rawval = string.Empty; 

            if (!this.parentIsLocus)
            {
                if (this.eprops.Length > 0)
                {
                    //Very similar code to below
                    string[] vals = this.ids[this.idPosition].Split(',');
                   
                    if (hasParent)
                    {
                        obj.ParentClass = this.map.Parent.ObjectClass;

                        if (this.map.Parent.Query == null)
                        {
                            obj.ParentID = vals[0];
                        }
                        else
                        {
                            while (this.nonLocusPosition < this.nonLocusIDs.GetLength(0) && this.nonLocusIDs[this.nonLocusPosition, 0] != vals[0])
                                this.nonLocusPosition++;

                            if (this.nonLocusPosition != this.nonLocusIDs.GetLength(0))
                                obj.ParentID = this.nonLocusIDs[this.nonLocusPosition, 1];
                        }
                    }

                    obj.LocalID = this.ids[this.idPosition];

                    //Identical code to below -- excpets iteration starts at 1 and eprops is shifted
                    for (int i = 1; i < this.eprops.Length + 1 && i < vals.Length; i++)
                    {
                        Data.LocalObject.Property objProp = obj.Properties.FirstOrDefault(p => p.Name == eprops[i - 1]);

                        //Console.WriteLine(eprops[i] + ":" + vals[i].Trim());

                        bool isNew = false;

                        if (objProp == null)
                        {
                            objProp = new Data.LocalObject.Property(eprops[i - 1]);
                            isNew = true;
                        }

                        string[] subVals = vals[i].Split(';');

                        for (int j = 0; j < subVals.Length; j++)
                        {
                            string val = subVals[j].Trim();

                            if (this.eids.Contains(this.eprops[i - 1]))
                                obj.LocalID += ";" + val;

                            if (val != string.Empty)
                            {
                                objProp.Values.Add(val);
                            }
                        }
                        //If there is at least one value for the property, then it gets appended to the MIX object
                        if (isNew && objProp.Values.Count > 0)
                        {
                            obj.Properties.Add(objProp);
                        }

                    }
                }
                else
                {
                    obj.LocalID = id;
                    
                    /*
                    string[] vals = this.ids[this.idPosition].Split(',');

                    if( vals.Length > 1 )
                        rawval = vals[1].Trim();
                    */


                    if (hasParent)
                    {
                        obj.ParentClass = this.map.Parent.ObjectClass;

                        if (this.map.Parent.Query == null)
                        {
                            obj.ParentID = id;
                        }
                        else
                        {

                            //Looking for the parent id. Need to reset enumerator if we reach end and havent found it. Make a second pass.

                            /*
                            if (this.nolidEnumerator.Current != null)
                            {
                            CheckParentID:
                                //If the end of the prop enumerator has been reached, this code uncessarily checks for id agreement!

                                IEnumerable<XElement> answers = this.nolidEnumerator.Current.Elements("Answer");
                                IEnumerator<XElement> enumerator = answers.GetEnumerator();

                                string childObjectID = string.Empty;
                                string val = string.Empty;

                                if (enumerator.MoveNext())
                                {
                                    childObjectID = enumerator.Current.Value;

                                    if (childObjectID == id && enumerator.MoveNext())
                                    {
                                        val = enumerator.Current.Value;

                                        if (this.nolidEnumerator.MoveNext())
                                            goto CheckParentID;
                                    }
                                    else
                                    {
                                        if (this.nolidEnumerator.MoveNext())
                                            goto CheckParentID;
                                    }
                                }
                            }
                            */

                            /*
                            while (this.nonLocusPosition < this.nonLocusIDs.GetLength(0) && this.nonLocusIDs[this.nonLocusPosition, 0] != this.ids[this.idPosition])
                                this.nonLocusPosition++;

                            if (this.nonLocusPosition != this.nonLocusIDs.GetLength(0))
                                obj.ParentID = this.nonLocusIDs[this.nonLocusPosition, 1];
                            */
                        }
                    }
                    
                }

                //What are the manually set values?
                
                //NEW CODE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                ProcessValue( rawval, obj );

                //Console.WriteLine(obj.LocalID);
            }

            //Extract Property values from the object query if parent is the locus.
            //Write code here

            else
            {
                obj.LocalID = this.nonLocusIDs[this.nonLocusPosition, 0] + ";" + this.nonLocusIDs[this.nonLocusPosition, 1];
                obj.ParentID = this.nonLocusIDs[this.nonLocusPosition, 0];
                obj.ParentClass = this.map.Parent.ObjectClass;

                //Split. Move the extraction method.

                string delimiterString = this.map.ExtractionDelimiter;

                if (String.IsNullOrEmpty(delimiterString))
                    delimiterString = "|";

                char[] delimiters = delimiterString.ToCharArray();

                string[] vals = this.nonLocusIDs[this.nonLocusPosition, 1].Split(delimiters);

                //Identical code to above
                for (int i = 0; i < this.eprops.Length && i < vals.Length; i++)
                {
                    //This code is very similar to below.

                    string val = vals[i].Trim();

                    if (val != string.Empty)
                    {
                        Data.LocalObject.Property objProp = obj.Properties.FirstOrDefault(p => p.Name == eprops[i]);

                        //Console.WriteLine(eprops[i] + ":" + vals[i].Trim());

                        bool isNew = false;

                        if (objProp == null)
                        {
                            objProp = new Data.LocalObject.Property(eprops[i]);
                            isNew = true;
                        }

                        objProp.Values.Add(vals[i].Trim());

                        //If there is at least one value for the property, then it gets appended to the MIX object
                        if (isNew && objProp.Values.Count > 0)
                        {
                            obj.Properties.Add(objProp);
                        }

                    }
                }

                ProcessValue(this.nonLocusIDs[this.nonLocusPosition, 1], obj);
            }

            int n = 0;


            //Get all the values from the Property elements
            foreach (MapSchema.Property prop in this.map.Properties)
            {
                if (prop.Query != null)
                {
                    Data.LocalObject.Property objProp = obj.Properties.FirstOrDefault(p => p.Name == prop.Name);

                    bool isNew = false;

                    if (objProp == null)
                    {
                        objProp = new Data.LocalObject.Property(prop.Name);
                        isNew = true;
                    }

                    //Fill property untill next id is found


                    /*
                    while (this.propPosition[n] < this.props[n].Length && buffer[n, 0] == this.ids[this.idPosition])
                    {

                        objProp.Values.Add(buffer[n, 1]);

                        FillBuffer(n, this.props[n][this.propPosition[n]]);

                        this.propPosition[n]++;
                    }
                    */

                    // Messy code. Overly convuloluted! Looking for properties in the chunk that have the same object id as the current object.
                    if (this.propEnumumerators[n].Current != null)
                    {
                        CheckProp:
                            //If the end of the prop enumerator has been reached, this code uncessarily checks for id agreement!

                            IEnumerable<XElement> answers = this.propEnumumerators[n].Current.Elements("Answer");
                            IEnumerator < XElement > enumerator = answers.GetEnumerator();

                            string propObjectID = string.Empty;
                            string propValue = string.Empty;

                            if (enumerator.MoveNext())
                            {
                                propObjectID = enumerator.Current.Value;

                                if (propObjectID == id && enumerator.MoveNext())
                                {
                                    propValue = enumerator.Current.Value;
                                    objProp.Values.Add(propValue);

                                    if (this.propEnumumerators[n].MoveNext())
                                        goto CheckProp;
                                }
                            }
                    }
                    
                    foreach (MapSchema.Value val in prop.Values)
                    {
                        if (val.Action == "set")
                        {
                            objProp.Values.Add(val.InnerValue);
                        }
                    }

                    //If there is at least one value for the property, then it gets appended to the MIX object
                    if (isNew && objProp.Values.Count > 0)
                    {
                        obj.Properties.Add(objProp);
                    }
                    

                }

                n++;
            }


            if( !this.parentIsLocus )
                this.idPosition++;
            
            this.nonLocusPosition++;

            this.eod = !this.posEnumerator.MoveNext();

            
            if (!this.eod == true)
            {
                if (!this.parentIsLocus)
                {

                    if (this.nonLocusPosition == this.chunkSize)
                    {
                        this.nonLocusPosition = 0;
                        this.NextChunk();
                        this.GetChunk();
                    }
                }
                else if( this.nonLocusPosition == this.nonLocusIDs.GetLength(0) )
                {
                    //If the parent is the locus, we have to up the parent ids after checking if we have no more objects to create.
                    //Make sure the next parent object exists.

                    int nextID = this.idPosition + this.chunkSize;

                    if (nextID >= this.ids.Length)
                    {
                        this.eod = true;
                    }
                    else
                    {
                        this.idPosition = nextID;
                        this.nonLocusPosition = 0;

                        this.GetChunk();
                    }
                }
            }
            

            return obj;
        }

        private void GetChunk()
        {
        GetChunk:

            bool returnedData = false;

            this.chunkCount++;

            //Console.WriteLine("CHUNK " + this.chunkCount);

            this.nonLocusPosition = 0;

            //Get the object based on the locus (the type of objects in this.ids).
            if ((!parentIsLocus && this.map.Parent != null && this.map.Parent.Query != null) || (parentIsLocus && this.map.Query != null))
            {
                string objectQuery = string.Empty;

                if (!this.parentIsLocus)
                {
                    Console.Write("Getting parents...");
                    objectQuery = this.ReplaceVariables(this.map.Parent.Query);
                }
                else
                {
                    Console.Write("Getting objects...");
                    objectQuery = this.ReplaceVariables(this.map.Query);
                }

                //Console.WriteLine(objectQuery);

                //string[] nlids = (string[])this.connection.GetData("GetRelevanceResult", new object[] { objectQuery, "[$Username]", "[$Password]" });

                string query = "query?relevance=";
                query += System.Web.HttpUtility.UrlEncode(objectQuery);

                System.IO.Stream stream = this.connection.GetRESTData(query);
                this.nolidXml = System.Xml.Linq.XDocument.Load(stream);

                XElement result = this.nolidXml.Root.Element("Query").Element("Result");
                //Console.WriteLine(result.ToString());

                
                int nolidCount = 0;
                
                if (!this.parentIsLocus)
                {
                    this.nonLocusObjects = result.Elements("Tuple").Select( obj  => new NonLocusObject( obj.Elements("Answer").ElementAt(0).Value, obj.Elements("Answer").ElementAt(0).Value));
                    this.nonLocusEnumerator = this.nonLocusObjects.GetEnumerator();

                    nolidCount = result.Elements("Tuple").Count();
                    this.nolidEnumerator = result.Elements("Tuple").GetEnumerator();
                }
                else
                {
                    //this.nonLocusObjects = result.Elements("Tuple").Select(obj => new NonLocusObject(obj.Elements("Answer").ElementAt(0).Value, obj.Elements("Answer").ElementAt(0).Value));

                    nolidCount = result.Elements("Answer").Count();
                    this.nolidEnumerator = result.Elements("Answer").GetEnumerator();
                }

                this.nolidEnumerator.MoveNext();

                if (nolidCount > 0)
                {
                    returnedData = true;

                    Console.WriteLine(nolidCount + "...DONE");
                }
                else
                {
                    Console.WriteLine("No data!");
                }
                
            }

            int n = 0;

            
            foreach (MapSchema.Property prop in this.map.Properties)
            {
                if (prop.Query != null)
                {
                    Console.Write("Getting property '" + prop.Name + "'...");

                    this.propPosition[n] = 0;

                    string q = this.ReplaceVariables(prop.Query);

                    /*
                    this.props[n] = (string[])this.connection.GetData("GetRelevanceResult", new object[] { relExp, "[$Username]", "[$Password]" });
                    */


                    string query = "query?relevance=";
                    query += System.Web.HttpUtility.UrlEncode(q);

                    System.IO.Stream stream = this.connection.GetRESTData(query);
                    this.propXmls[n] = System.Xml.Linq.XDocument.Load(stream);

                    XElement result = this.propXmls[n].Root.Element("Query").Element("Result");
                    //Console.WriteLine(result.ToString());

                    IEnumerable<XElement> tuples = result.Elements("Tuple");

                    this.propEnumumerators[n] = tuples.GetEnumerator();
                    this.propEnumumerators[n].MoveNext();

                    int tcount = tuples.Count();

                    if (tcount > 0)
                    {
                        returnedData = true;

                        //this.FillBuffer(n, this.props[n][0]);
                    }

                   Console.WriteLine(tcount + "...DONE");
                }
                else
                {
                    //If a value is set manually, then every object will necessarily have some data.
                    foreach( MapSchema.Value val in prop.Values )
                    {
                        if (val.Action == "set" || val.Action == "extract")
                            returnedData = true;
                    }                    
                }

                n++;
            }

            /*
            if (!returnedData)
            {
                this.idPosition += this.chunkSize;

                if (this.idPosition < this.ids.Length)
                {
                    Console.WriteLine( "No data received! Getting next chunk...");
                    System.Threading.Thread.Sleep(10000);
                    goto GetChunk;
                }
                else
                    this.eod = true;
            }
            */

            this.NextChunk();
        }

        private void FillBuffer(int n, string val)
        {
            //Reset the buffer just to keep a clean mind.
            this.buffer[n, 0] = null;
            this.buffer[n, 1] = null;

            //Now extract the new bes id + property value pair

            string[] str = SplitIDFromValue(val);

            buffer[n, 0] = str[0];
            buffer[n, 1] = str[1];
        }

        private string[] SplitIDFromValue(string val)
        {
            string[] str = new string[2];

            int ind = val.IndexOf(',');
            str[0] = val.Substring(0, ind);

            int offset = ind + 1;

            if (offset < val.Length)
            {
                str[1] = val.Substring(offset).Trim();
            }
            else
            {
                str[1] = null;
            }            

            return str;
        }

        private void NextChunk()
        {
            this.firstID = this.idEnumerator.Current.Value;

            int n = 0;

            while (n < this.chunkSize)
            {
                this.lastID = idEnumerator.Current.Value;

                if (!this.idEnumerator.MoveNext())
                    n = this.chunkSize;
                else
                    n++;
            }

            return;
        }
        private string ReplaceVariables(string relExp)
        {
            relExp = relExp.Replace("[$LocalID]", this.firstID);
            relExp = relExp.Replace("[$First]", this.firstID);

            relExp = relExp.Replace("[$Last]", this.lastID);

            return relExp;
        }

        private void ProcessValue(string rawval, Data.LocalObject obj)
        {
            //Console.WriteLine("Processing Value");

            foreach (MapSchema.Property prop in this.map.Properties)
            {
                foreach (MapSchema.Value val in prop.Values)
                {
                    Data.LocalObject.Property objProp = obj.Properties.FirstOrDefault(p => p.Name == prop.Name);
                    bool isNew = false;

                    if (objProp == null)
                    {
                        objProp = new Data.LocalObject.Property(prop.Name);
                        isNew = true;
                    }

                    if (val.Action == "set")
                    {
                        objProp.Values.Add(val.InnerValue);
                    }
                    else if (val.Action == "extract")
                    {
                        //Console.WriteLine("Extracting: " + rawval + "; " + val.InnerValue);

                        System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(rawval, val.InnerValue);

                        if (m.Success)
                        {
                            //Console.WriteLine("VALUE: " + m.Value.Trim());
                            objProp.Values.Add(m.Value.Trim());
                        }
                    }

                    if (isNew && objProp.Values.Count > 0)
                    {
                        obj.Properties.Add(objProp);
                    }
                }
            }    
        }

        private class NonLocusObject
        {
            public string LocusID;
            public string ID;

            public NonLocusObject( string id, string locusId)
            {
                this.ID = id;
                this.LocusID = locusId;
            }
        }
    }
}
