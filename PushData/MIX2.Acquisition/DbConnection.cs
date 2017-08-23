using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;

namespace MIX2.Acquisition
{
    public class DbConnection: IDisposable, ITokenProvider
    {
        private string connectionString;        
        private System.Data.Common.DbProviderFactory dbFactory;
        private System.Data.Common.DbConnection dbConnection;
        private string providerName;
        private int timeOut;
        private ITokenProvider tokenProvider;
        private Guid guid;

        public DbConnection( string ProviderName, string ConnectionString, int TimeOut )
        {
            Init(ProviderName, ConnectionString, TimeOut );
        }

        public DbConnection(string ProviderName, string ConnectionString, int TimeOut, ITokenProvider TokenProvider)
        {
            Init(ProviderName, ConnectionString, TimeOut);
            this.tokenProvider = TokenProvider;
        }

        private void Init(string ProviderName, string ConnectionString, int TimeOut)
        {
            this.connectionString = ConnectionString;
            this.providerName = ProviderName;
            this.timeOut = TimeOut;

            this.dbFactory = DbProviderFactories.GetFactory(this.providerName);
            this.dbConnection = this.dbFactory.CreateConnection();
            this.dbConnection.ConnectionString = this.connectionString;

            this.guid = Guid.NewGuid();
        }

        public void Close()
        {
            if (this.dbConnection.State == System.Data.ConnectionState.Open)
                this.dbConnection.Close();
        }

        public DbDataAdapter CreateDataAdapter( string Query )
        {
            System.Data.Common.DbCommand cmd = this.dbFactory.CreateCommand();

            cmd.CommandText = Query ;
            cmd.CommandTimeout = this.timeOut;
            cmd.Connection = this.dbConnection; ;

            return new DbDataAdapter(this.dbFactory, cmd);
        }

        public void Open()
        {
            if( this.dbConnection.State == System.Data.ConnectionState.Closed )
                this.dbConnection.Open();
        }


        public void Dispose()
        {
        }

        public object GetToken(object[] Parameters)
        {
            if (this.tokenProvider != null)
                return this.tokenProvider.GetToken(Parameters);
            else
                return null;
        }

        public override string ToString()
        {
            return this.dbConnection.ConnectionString ;
        }
    }


    public class DbDataAdapter
    {
        private System.Data.Common.DbDataAdapter adapter;

        public DbDataAdapter( System.Data.Common.DbProviderFactory Factory, System.Data.Common.DbCommand SelectCommand)
        {
            this.adapter = Factory.CreateDataAdapter();
            this.adapter.SelectCommand = SelectCommand;
        }

        public void Fill( System.Data.DataSet DataSet )
        {
            this.adapter.Fill(DataSet);
        }

        public void AddTableMapping( string SourceTable, string DataSetTableName)
        {
            this.adapter.TableMappings.Add(SourceTable, DataSetTableName);
        }
    }


}
