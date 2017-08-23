using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIX2.Acquisition.IBM.Tivoli
{
    public class EndpointFactory: IMIXFactory
    {
        string sourceName;
        DataSourceSchema.ObjectClass currentClass;
        MIX2.Acquisition.MapSchema map;
        ServiceConnection connection;
        int chunkCount;

        string[] ids;
        int idPosition;
        string[,] nonLocusIDs;
        int nonLocusPosition;
        bool hasParent;
        bool parentIsLocus;
        string[] eprops;
        string[] eids;

        string[][] props;
        int[] propPosition;
        //The multidimensional array will be filled by bes computer id + property value.
        string[,] buffer;
        
        int position;
        int chunkSize;
        bool eod;

        public EndpointFactory(string source, object connection)
        {
            this.connection = (ServiceConnection)connection;
            this.sourceName = source;
            this.ids = new string[] { };
            this.eprops = new string[] { };
            this.eids = new string[] { };
            this.idPosition = -1;
            this.nonLocusPosition = -1;
            this.chunkSize = -1;
            this.hasParent = false;
            this.eod = true;
            this.parentIsLocus = false;
        }

        public bool IsAlive(out string mes)
        {
            mes = "OK";

            try
            {
                string[] results = (string[])this.connection.GetData("GetRelevanceResult", new object[] { "number of bes computers", "[$Username]", "[$Password]" });

                int x = int.Parse( results[0] );

                if (x > 0)
                {
                    mes += ": " + x + " computer";

                    if (x > 1)
                        mes += "s";

                    mes += ".";

                    return true;
                }

                mes = this.sourceName + " contains no computer data.";
            }
            catch(Exception exc) 
            {
                mes = exc.Message;
            }

            return false;
        }

        public void Open(DataSourceSchema.ObjectClass objectClass)
        {
            //this.ids = (string[])this.connection.GetData("GetRelevanceResult", new object[] { "ids of bes computers whose (exists ((property results of it) whose (((name of it) of property of it) = \"DNS Servers - Windows\")))", "[$Username]", "[$Password]" });

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


            if (!parentIsLocus)
            {
                this.ids = (string[])this.connection.GetData("GetRelevanceResult", new object[] { map.Query, "[$Username]", "[$Password]" });
            }
            else
            {
                if (map.Parent == null && map.Parent.Query == null)
                    throw new Exception("The locus is parent but no query was defined for parent objects.");
                
                this.ids = (string[])this.connection.GetData("GetRelevanceResult", new object[] { map.Parent.Query, "[$Username]", "[$Password]" });
            }

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


            if (this.ids.Length == 0)
            {
                this.idPosition = -1;
                this.nonLocusPosition = -1;
                this.eod = true;

                Console.WriteLine("...NO OBJECTS RETURNED");

                return;
            }

            Console.WriteLine(this.ids.Length + "...DONE");

            int len = map.Properties.Count();
            
            this.props = new string[len][];
            this.propPosition = new int[len];
            this.buffer = new string[len,2];

            if ( this.chunkSize < 1)
            {
                this.chunkSize = this.ids.Length;
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
                this.GetNextChunk();
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
            //obj.ObjectClass = this.currentClass.Name;            

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
                    obj.LocalID = this.ids[this.idPosition];

                    string[] vals = this.ids[this.idPosition].Split(',');

                    if( vals.Length > 1 )
                        rawval = vals[1].Trim();

                    if (hasParent && vals.Length > 0)
                    {
                        obj.ParentClass = this.map.Parent.ObjectClass;

                        if (this.map.Parent.Query == null)
                        {
                            obj.ParentID = vals[0];
                        }
                        else
                        {
                            while (this.nonLocusPosition < this.nonLocusIDs.GetLength(0) && this.nonLocusIDs[this.nonLocusPosition, 0] != this.ids[this.idPosition])
                                this.nonLocusPosition++;

                            if (this.nonLocusPosition != this.nonLocusIDs.GetLength(0))
                                obj.ParentID = this.nonLocusIDs[this.nonLocusPosition, 1];
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
                    while (this.propPosition[n] < this.props[n].Length && buffer[n, 0] == this.ids[this.idPosition])
                    {
                        //Console.WriteLine("NEXT:" + this.props[n][this.propPosition[n]]);

                        objProp.Values.Add(buffer[n, 1]);

                        FillBuffer(n, this.props[n][this.propPosition[n]]);

                        this.propPosition[n]++;
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

            if (this.idPosition == this.ids.Length)
                this.eod = true;
            else
            {
                if (!this.parentIsLocus)
                {

                    if (this.nonLocusPosition == this.chunkSize)
                    {
                        this.nonLocusPosition = 0;
                        this.GetNextChunk();
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

                        this.GetNextChunk();
                    }
                }
            }

            return obj;
        }

        private void GetNextChunk()
        {
        GetChunk:

            bool returnedData = false;

            this.chunkCount++;

            //Console.WriteLine("CHUNK " + this.chunkCount);

            this.nonLocusPosition = 0;

            //Get the object based on the locus (the type of objects in this.ids).
            if ( (!parentIsLocus && this.map.Parent != null && this.map.Parent.Query != null) || (parentIsLocus && this.map.Query != null))
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
                             
                string[] nlids = (string[])this.connection.GetData("GetRelevanceResult", new object[] { objectQuery, "[$Username]", "[$Password]" });

                if (nlids.Length > 0)
                {
                    returnedData = true;

                    this.nonLocusIDs = new string[nlids.Length, 2];

                    for (int i = 0; i < nlids.Length; i++)
                    {
                        string[] str = this.SplitIDFromValue(nlids[i]);
                        this.nonLocusIDs[i, 0] = str[0];
                        this.nonLocusIDs[i, 1] = str[1];
                    }

                    Console.WriteLine(this.nonLocusIDs.GetLength(0) + "...DONE");
                }
            }

            int n = 0;

            foreach (MapSchema.Property prop in this.map.Properties)
            {
                if (prop.Query != null)
                {
                    Console.Write("Getting property '" + prop.Name + "'...");

                    this.propPosition[n] = 0;

                    string relExp = this.ReplaceVariables(prop.Query);

                    this.props[n] = (string[])this.connection.GetData("GetRelevanceResult", new object[] { relExp, "[$Username]", "[$Password]" });

                    //Move this to end
                    if (this.props[n].Length > 0)
                    {
                        returnedData = true;

                        this.FillBuffer(n, this.props[n][0]);
                    }

                    Console.WriteLine(this.props[n].Length + "...DONE");
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

        private string ReplaceVariables(string relExp)
        {
            relExp = relExp.Replace("[$LocalID]", ids[this.idPosition]);
            relExp = relExp.Replace("[$First]", ids[this.idPosition]);

            int last = this.idPosition + this.chunkSize;

            bool adjust = false;

            if (last >= this.ids.Length)
            {
                last = this.ids.Length - 1;
                adjust = true;
            }

            string lastID = ids[last];

            if ( adjust )
            {
                lastID = (int.Parse(lastID) + 1).ToString();
            }

            relExp = relExp.Replace("[$Last]", lastID);

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
    }
}
