using System;
using System.Collections.Generic;

using System.Data;
using System.Data.Common;

using System.Net;

namespace IntuitiveLabs.Data
{

    public class SimpleDatabaseAdapter : ISimpleDatabaseAdapter
    {
        private string m_ProviderName;
        private string m_ConnectionString;

        public SimpleDatabaseAdapter(string ProviderName, string ConnectionString, NetworkCredential NetworkCredential)
        {   
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            string UserID = NetworkCredential.UserName;
            if (NetworkCredential.Domain != "")
                UserID = NetworkCredential.Domain + "\\" + UserID;

            string Password = NetworkCredential.Password;
           
            builder.Remove("User ID");
            builder .Add("User ID", UserID);

            builder.Remove("Password");
            builder.Add("Password", Password);
            m_ConnectionString = builder.ConnectionString;

            m_ProviderName = ProviderName;

        }

        public DataSet ExecuteQuery(string Query, int CommandTimeout)
        {
            DataSet ds = new DataSet();
            DbProviderFactory factory = DbProviderFactories.GetFactory(m_ProviderName);
            using (DbConnection conn = factory.CreateConnection())
            using (DbCommand cmd = factory.CreateCommand())
            using (DbDataAdapter da = factory.CreateDataAdapter())
            {
                conn.ConnectionString = m_ConnectionString;
                cmd.CommandText = Query;
                cmd.CommandTimeout = CommandTimeout; // 
                cmd.Connection = conn;                
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            return ds;
        }

        public DataSet ExecuteProcedure( List<Parameter> Parameters, string Query, int CommandTimeout)
        {                        
            DataSet ds = new DataSet();
            
            DbProviderFactory factory = DbProviderFactories.GetFactory(m_ProviderName);           
            using (DbConnection conn = factory.CreateConnection())
            using (DbCommand cmd = factory.CreateCommand())
            using (DbDataAdapter da = factory.CreateDataAdapter())
            {
                foreach (Parameter param in Parameters)
                {
                    DbParameter pa = factory.CreateParameter();
                    pa.ParameterName = param.Name;
                    pa.DbType = param.DbType;
                    pa.Size = param.Size;
                    pa.Value = param.Value;

                    pa.Direction = ParameterDirection.Input;

                    cmd.Parameters.Add(pa);
                }
    
                conn.ConnectionString = m_ConnectionString;
                cmd.CommandText = Query;
                cmd.CommandTimeout = CommandTimeout; // 
                cmd.Connection = conn;
                da.SelectCommand = cmd;
                da.Fill(ds);               
            }
              
           
            return ds;
        }
    }

    public class Parameter
    {
        public DbType DbType;
        public string Name;
        public int Size = -1;
        public object Value;

        public Parameter(string Name, DbType Type, int Size, object Value)
        {
            this.Name = Name;
            this.DbType = Type;
            this.Size = Size;
            this.Value = Value;
        }

        public Parameter(string Name, DbType Type, object Value)
        {
            this.Name = Name;
            this.DbType = Type;
            this.Value = Value;
        }

    }
}
