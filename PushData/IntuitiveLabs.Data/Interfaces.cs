using System;
using System.Data;

namespace IntuitiveLabs.Data
{
    public interface ISimpleDatabaseAdapter
    {
        DataSet ExecuteQuery(string Query, int CommandTimeout);
        DataSet ExecuteProcedure(System.Collections.Generic.List<Parameter> Parameters, string Query, int CommandTimeout);
    }
}
