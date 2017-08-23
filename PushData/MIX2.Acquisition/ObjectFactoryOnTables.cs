using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.Data;
using System.Data.Common;

using MIX2.Data;

namespace MIX2.Acquisition
{
    public class ObjectFactoryOnTables: IMIXFactory 
    {
        private int position = -1;
        private int badDataCount;
        private DbConnection connection;
        private DataSet data = null;
        private DataSourceSchema.ObjectClass objClass;
        private MapSchema schema;
        private string source;        

        public ObjectFactoryOnTables( string Source, object Connection )
        {
            this.source = Source;
            this.connection = (DbConnection)Connection;
        }

        public bool IsAlive(out string message)
        {
            message = "OK";
            return true;
        }

        public void Open(DataSourceSchema.ObjectClass ObjectClass)
        {
            try
            {
                this.objClass = ObjectClass;
                this.schema = new MapSchema(ObjectClass.Map);

                if (this.data == null)
                {
                    // Get all the values and reset the next element
                    this.position = 0;
                    this.badDataCount = 0;
                    this.data = new DataSet();

                    using (DbConnection conn = this.connection)
                    {
                        System.Diagnostics.Trace.WriteLine(this.objClass.Name + ": Get LocalIDs: start at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));
                        System.Diagnostics.Trace.Indent();

                        DbDataAdapter da = conn.CreateDataAdapter(schema.Query);
                        da.AddTableMapping("Table", "LocalID");

                        conn.Open();
                        da.Fill(this.data);

                        System.Diagnostics.Trace.WriteLine("--------   end at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));
                        System.Diagnostics.Trace.WriteLine("Total records retrieved from DataSource:" + this.data.Tables["LocalID"].Rows.Count.ToString());

                        //If parents are defined get the parents table and merge it with datset containg the LocalIDs
                        if (schema.Parent != null)
                        {
                            System.Diagnostics.Trace.Indent();
                            System.Diagnostics.Trace.WriteLine(this.objClass.Name + ": Get Parents: start at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));

                            DataSet parentsDs = new DataSet();

                            da = conn.CreateDataAdapter(schema.Parent.Query);
                            //System.Diagnostics.Trace.WriteLine(schema.Parent.Query );

                            da.AddTableMapping("Table", "ParentID");

                            da.Fill(parentsDs);

                            this.data.Merge(parentsDs);

                            this.data.Relations.Add("ParentID", this.data.Tables["LocalID"].Columns[0], this.data.Tables["ParentID"].Columns[0]);

                            System.Diagnostics.Trace.WriteLine("--------   end at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));
                            System.Diagnostics.Trace.WriteLine("Total records retrieved from DataSource:" + this.data.Tables["ParentID"].Rows.Count.ToString());
                            System.Diagnostics.Trace.Unindent();
                        }


                        //Get All Properties and merge them into the main DataSet containing the LocalIDs

                        System.Diagnostics.Trace.Indent();

                        foreach (MapSchema.Property prop in schema.Properties)
                        {
                            System.Diagnostics.Trace.WriteLine("Get " + prop.Name + ": start at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));

                            DataSet ds = new DataSet();
                            da = conn.CreateDataAdapter(prop.Query);

                            //Console.WriteLine(prop.Query);

                            da.AddTableMapping("Table", "PROPERTY_" + prop.Name);
                            da.Fill(ds);

                            this.data.Merge(ds);

                            //This should be a Column Array sinc ParentID can be several key columns
                            this.data.Relations.Add("PROPERTY_" + prop.Name, this.data.Tables["LocalID"].Columns[0], this.data.Tables["PROPERTY_" + prop.Name].Columns[0]);

                            System.Diagnostics.Trace.WriteLine("--------   end at " + DateTime.Now.ToString("MM-dd hh:mm:ss.f"));
                            System.Diagnostics.Trace.WriteLine("Total records retrieved from DataSource:" + this.data.Tables["PROPERTY_" + prop.Name].Rows.Count.ToString());
                        }

                        System.Diagnostics.Trace.Unindent();

                        conn.Close();
                    }

                    //  Count performance
                    //this.IdentifiersAcquired(m_Name, m_dpObjectClass, m_Data.Tables["LocalID"].Rows.Count);
                }
                else
                {
                    throw new Exception("ObjectClass already opened");
                }
            }
            catch (Exception exc)
            {
                throw new Exception( "Could not open factory for " + ObjectClass.Name +": " + exc.Message );
            }
        }

        public LocalObject GetNext()
        {
            DataRow row = this.data.Tables["LocalID"].Rows[this.position++];
            
            //System.Threading.Thread.CurrentThread.Join(5000);
            
            if (!row.IsNull(0))
            {
                string localID = ConvertToString( row[0] );

                if (localID != null)
                {
                    LocalObject obj = new LocalObject(this.source, this.objClass.Domain, this.objClass.Name);

                    if (this.data.Tables.Contains("ParentID"))
                    {
                        DataRow[] parentRows = row.GetChildRows("ParentID");

                        if (parentRows.Length > 0 && !parentRows[0].IsNull(1))
                        {
                            obj.ParentID = ConvertToString(parentRows[0][1]);
                            obj.ParentClass = schema.Parent.ObjectClass;
                        }
                    }

                    obj.LocalID = localID;

                    foreach (MapSchema.Property prop in this.schema.Properties)
                    {
                        DataRow[] children = row.GetChildRows("PROPERTY_" + prop.Name);

                        if (children.Length > 0)
                        {
                            LocalObject.Property aprop = new LocalObject.Property(prop.Name);

                            for (int i = 0; i < children.Length; i++)
                            {
                                if (prop.ValueDelimiter != null)
                                {
                                    string valsText = ConvertToString(children[i][1]);
                                    string[] vals = valsText.Split(prop.ValueDelimiter);

                                    for (int j = 0; j < vals.Length; j++)
                                    {
                                        string val = Trim(vals[j], prop);

                                        if (val != string.Empty)
                                            aprop.Values.Add(val);
                                    }
                                }
                                else
                                {
                                    string val = Trim( ConvertToString(children[i][1]), prop);

                                    if (val != string.Empty)
                                        aprop.Values.Add(val);
                                }
                            }

                            obj.Properties.Add(aprop);
                        }
                    }

                    return obj;
                }
                else
                    return null;
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

                return this.position >= this.data.Tables["LocalID"].Rows.Count;
            }
        }

        public void Close()
        {
            this.data = null;
            this.position = -1;
            
            if (badDataCount  > 0)
            {
                //Do something
            }
        }

        private string ConvertToString( object Value )
        {
            Type t = Value.GetType();
            
            switch( t.FullName )
            {
                case "System.Byte[]":
                    return System.BitConverter.ToString((byte[])Value);                    

                default:
                    return Value.ToString();
            }

            return null;
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
