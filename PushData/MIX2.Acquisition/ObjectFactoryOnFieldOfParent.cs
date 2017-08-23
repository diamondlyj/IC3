using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

using MIX2.Data;

namespace MIX2.Acquisition
{
    public class ObjectFactoryOnFieldOfParent : IMIXFactory 
    {
        private int position = -1;
        private int badDataCount;
        private DataSet data = null;
        private DataSourceSchema.ObjectClass objectClass;
        private string source;
        private DbConnection connection;
        private MapForFOPSchema schema;


        public ObjectFactoryOnFieldOfParent(string Source, object Connection)
        {
            this.objectClass = ObjectClass;
            this.source = Source;
            this.connection = (DbConnection)Connection;
        }

        public DataSourceSchema.ObjectClass ObjectClass
        {
            get { return this.objectClass; }
            set { this.objectClass = value; }
        }

        public bool IsAlive(out string message)
        {
            message = "OK";
            return true;
        }

        public void Open(DataSourceSchema.ObjectClass ObjectClass)
        {
            this.objectClass = ObjectClass;
            this.schema = new MapForFOPSchema(ObjectClass.Map);

            if (this.data == null)
            {
                // Get all the values and reset the next element
                this.position = 0;
                this.badDataCount = 0;
                this.data = new DataSet();
               
                using (DbConnection conn = this.connection )
                {
                    System.Diagnostics.Trace.WriteLine(this.objectClass.Name + ": Get data: start at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));
                    System.Diagnostics.Trace.Indent();

                    //Get Basetable
                    DbDataAdapter da = conn.CreateDataAdapter(schema.Query);
                    da.AddTableMapping("Table", "LocalID");

                    conn.Open();
                    da.Fill(this.data);
                    conn.Close();

                    System.Diagnostics.Trace.WriteLine("--------   end at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));
                    System.Diagnostics.Trace.WriteLine("Total records retrieved from DataSource:" + this.data.Tables[0].Rows.Count.ToString());

                    System.Diagnostics.Trace.WriteLine(this.objectClass.Name + " Normalize data: start at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));

                    DataTable dt = new DataTable("Instance");
                    
                    DataColumn idColumn = new DataColumn("ParentID");
                    idColumn.DataType = this.data.Tables[0].Rows[0][0].GetType();
                    dt.Columns.Add(idColumn);

                    DataColumn propColumn = new DataColumn("Properties");
                    propColumn.DataType = this.data.Tables[0].Rows[0][1].GetType();
                    dt.Columns.Add(propColumn);

                    foreach (DataRow row in this.data.Tables[0].Rows)
                    {
                        string str = row[1].ToString();

                        string[] inst = str.Split(this.schema.Delimiters.Instance);

                        for (int i = 0; i < inst.Length; i++)
                        {
                            object[] vals = new object[2];
                            vals[0] = row[0];
                            vals[1] = inst[i];

                            dt.Rows.Add(vals);
                        }
                    }
                    this.data.Merge(dt);

                    this.data.Relations.Add("InstanceToParent", this.data.Tables[0].Columns[0], this.data.Tables["Instance"].Columns[0]);

                    System.Diagnostics.Trace.WriteLine("--------   end at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));
                    System.Diagnostics.Trace.Unindent();
                }


                //  Count performance
                //this.IdentifiersAcquired(m_Name, m_dpObjectClass, m_Data.Tables["LocalID"].Rows.Count);
            }
            else
            {
                throw new Exception("ObjectClass already opened");
            }
        }

        public LocalObject GetNext()
        {
            DataRow row = this.data.Tables["Instance"].Rows[this.position++];
            
            LocalObject obj = new LocalObject(this.source, this.objectClass.Domain, this.objectClass.Name);
            obj.ParentID = row[0];
            obj.ParentClass = this.schema.Parent.ObjectClass;

            //System.Diagnostics.Trace.WriteLine(row[1].ToString());
            
            string[] props = row[1].ToString().Split(this.schema.Delimiters.Property);
            int n = 0;

            string id = String.Empty;

            foreach (MapSchema.Property prop in this.schema.Properties)
            {
                if (n > props.Length-1)
                    break;

                string val = props[n].Trim();

                if (val != string.Empty)
                {
                    LocalObject.Property newProp = new LocalObject.Property(prop.Name);

                    if (prop.ValueDelimiter != null)
                    {
                        string valsText = props[n];
                        string[] vals = valsText.Split(prop.ValueDelimiter);

                        for (int j = 0; j < vals.Length; j++)
                        {
                            string theval = Trim(vals[j], prop);

                            if (theval != string.Empty)
                                newProp.Values.Add(theval);
                        }
                    }
                    else
                    {
                        string theval = Trim(props[n], prop);

                        if (theval != string.Empty)
                            newProp.Values.Add(theval);
                    }
                    
                    obj.Properties.Add(newProp);

                    if( id != String.Empty )
                        id += ".";

                    id += props[n];
                }
                n++;
            }

            if (id != string.Empty)
            {
                obj.LocalID = id;

                Console.WriteLine(obj.GetXml());

                return obj;
            }
            else
                return null;
        }

        public bool EOD
        {
            get
            {
                if (this.data == null)
                {
                    throw new Exception("DataSource is not opened");
                }

                return this.position >= this.data.Tables["Instance"].Rows.Count;
            }
        }

        public void Close()
        {
            this.data = null;
            this.position = -1;

            if (badDataCount > 0)
            {
                //Do something
            }
        }

        private string Trim(string val, MapSchema.Property prop)
        {
            val = val.Trim();

            if (prop.Trim.Start != null)
                val = val.TrimStart(prop.Trim.Start);

            if (prop.Trim.End != null)
                val = val.TrimStart(prop.Trim.End);

            return val;
        }

    }
}
