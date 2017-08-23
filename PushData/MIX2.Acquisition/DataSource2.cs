using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Reflection;
//using System.Data.SqlClient;
using System.Xml.XPath;

using IntuitiveLabs.Data;
using MIX2.Data;

using System.Diagnostics;

namespace MIX2.Acquisition
{
    //public delegate void DataChunkAcquiredDelegat(string DataSourceName, DataPointIndex Index, int Count);
    //public delegate void DataPointDroppedDelegat(string DataSourceName, DataPointIndex Index);

    public delegate void IdentifiersAcquiredDelegat(string DataSourceName, DataSourceSchema.ObjectClass ObjectClass, int Count);


    public class DataSource: BaseSource
    {
        //private Assembly assembly;
        //private object connection;
        //private string m_Name;
        //private NetworkCredential m_NetworkCredential;

        private DataSourceSchema m_Schema;
        private string rawSourceMapping;

        protected DataSet m_Data = null;
        protected int m_idPos = -1;
        protected int m_idBadDataCount;
        protected DataSourceSchema.ObjectClass m_dpObjectClass;       

        private IMIXFactory objFactory;

        public DataSource(string Name, NetworkCredential NetworkCredential, MIX2.Acquisition.Configuration.Connection Connection, DataSourceSchema Schema, string AssemblyName):
            base( Name, NetworkCredential, Connection, AssemblyName )
        {
            this.m_Schema = Schema;
        }

        public DataSource(string Name, NetworkCredential NetworkCredential, MIX2.Acquisition.Configuration.Connection Connection, DataSourceSchema Schema, string AssemblyName, ITokenProvider TokenProvider):
            base(Name, NetworkCredential, Connection, AssemblyName, TokenProvider)
        {
            this.m_Schema = Schema;
        }

        public DataSource(string Name, NetworkCredential NetworkCredential, MIX2.Acquisition.Configuration.Connection Connection, string SourceMapping, string AssemblyName) :
            base(Name, NetworkCredential, Connection, AssemblyName)
        {
            this.rawSourceMapping = SourceMapping;
        }

        public DataSource(string Name, NetworkCredential NetworkCredential, MIX2.Acquisition.Configuration.Connection Connection, string SourceMapping, string AssemblyName, ITokenProvider TokenProvider) :
            base(Name, NetworkCredential, Connection, AssemblyName, TokenProvider)
        {
            this.rawSourceMapping = SourceMapping;
        }


        public event IdentifiersAcquiredDelegat IdentifiersAcquired;

        public void Close()
        {
            lock (this)
            {
                if (this.MIXFactory != null)
                    this.MIXFactory.Close();
                else
                    throw new Exception("Datasource is not open.");
            }
        }

        public IMIXFactory MIXFactory
        {
            get{ return this.objFactory; }
        }
  

        public DataSourceSchema Schema
        {
            get { return m_Schema; }
        }

        public string RawSourceMapping
        {
            get { return this.rawSourceMapping; }
        }

        public void Open(DataSourceSchema.ObjectClass ObjectClass)
        {
            lock (this)
            {
                MIX2.Acquisition.MapSchema map = new MapSchema(ObjectClass.Map);

                InitializeFactory(ObjectClass.FactoryType);

                this.objFactory.Open(ObjectClass);
            }
        }

        public void OpenForTest(DataSourceSchema.ObjectClass ObjectClass)
        {
            lock (this)
            {
                this.objFactory.Open(ObjectClass);
            }
        }


        public void InitializeFactory( string factoryType )
        {
            Type t;

            try
            {
                t = this.Assembly.GetType(factoryType);
            }
            catch
            {
                throw new Exception("The referenced MIXLibrary does not contain the type " + factoryType  + ".");
            }

            if (t == null)
                throw new Exception("The referenced MIXLibrary does not contain the type " + factoryType + ".");

            if (t.GetInterface(typeof(IMIXFactory).FullName) == null)
            {
                Console.WriteLine("Throwing error");
                throw new Exception(factoryType + " does not implement the interface " + typeof(IMIXFactory).FullName);
            }

            object obj;

            try
            {
                //Console.WriteLine("F");

                object[] cArgs = { this.Name, this.Connection };
                obj = t.InvokeMember(null, BindingFlags.CreateInstance, null, this.Assembly, cArgs);
                this.objFactory = (IMIXFactory)obj;

                //Console.WriteLine("G");

            }
            catch (Exception exc)
            {
                throw new Exception("Failed to instantiate the MIXFactory of type. Make sure the library " + this.Assembly.FullName + " contains the following constructor: " + factoryType + "( string Source, object Connection). " + exc.Message);
            }
        
        }
  
    }
}
