using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data.SqlClient;
using AI.V2.BaseLibrary;
using System.Runtime.Serialization;

namespace API.Controllers
{
    public class NetworkController : ApiController
    {
        // GET: Network
        [HttpGet, Route("{domain}/{ObjectClass}/{objectGUID}/{position}/{count}/{orderBy}")]
        public IHttpActionResult GetDependencyInforamtion(string domain, string ObjectClass, Guid objectGUID, int position, int count, string orderBy)
        {
            AI.V2.BaseLibrary.Security.AssertRole(Request.Headers, Security.Role.User);

            AI.V2.BaseLibrary.Direction direction = Direction.Ascending;

            AI.V2.BaseLibrary.Object o = new AI.V2.BaseLibrary.Object();
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);

            //int domainID = GetDomainID(domain);

            //int objectClassID = GetObjectClassID(domainID, ObjectClass);
            
            var cmd = conn.CreateCommand();
            List<AI.V2.BaseLibrary.Dependency> Dependencies = new List<AI.V2.BaseLibrary.Dependency>();
            List<AI.V2.BaseLibrary.Dependant> Dependant = new List<AI.V2.BaseLibrary.Dependant>();

            Dependencies = this.GetDependencies(domain, objectGUID, position, count, orderBy, direction);
            Dependant = this.GetDependents(domain, objectGUID, position, count, orderBy, direction);

            AI.V2.BaseLibrary.DependencyLink DependencyInformation = new AI.V2.BaseLibrary.DependencyLink();

            DependencyInformation.Dependencies = Dependencies;
            DependencyInformation.Dependants = Dependant;
            DependencyInformation.Domain = domain;
            DependencyInformation.ObjectGUID = objectGUID;
            DependencyInformation.ObjectClass = ObjectClass;

            return Ok(DependencyInformation);
        }

        [HttpPost, Route("Domain/set/{tokenValue}/{domain}/{ObjectClass}/{objectGUID}/{position}/{count}/{orderBy}")]
        public IHttpActionResult SetDependency(Guid tokenValue, string domain, string dependencyClass, string subjectClass, Guid subjectGUID, string predicateClass, Guid predicateGUID, double weight, bool freeze)
        {
            int domainID = GetDomainID(domain);
            int dependencyClassID = GetDependencyTypeID(domainID, dependencyClass);
            int subjectClassID = GetObjectClassID(domainID, subjectClass);
            int predicateClassID = GetObjectClassID(domainID, predicateClass);
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "Update Dependency Set DependencyTypeID=" + dependencyClassID + "AND SubjectClassID=" + subjectClassID + "AND PredicateClassID=" + predicateClassID + "AND SubjectGUID=" + subjectGUID + "AND PredicateGUID=" + predicateGUID;

            try
            {
                cmd.ExecuteNonQuery();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    List<AI.V2.BaseLibrary.Dependency> dList = new List<AI.V2.BaseLibrary.Dependency>();
                    AI.V2.BaseLibrary.Dependency d = new AI.V2.BaseLibrary.Dependency();
                    //d.guid = guid.newguid();
                    //d.dependencytype = reader["dependencytype"].tostring();
                    //d.domain = reader["domain"].tostring();
                    //d.predicateclassid = (int)reader["predicateclassid"];
                    //d.subjectclassid = (int)reader["subjectclassid"];
                    //d.subjectguid = (guid)reader["subejectguid"];
                    //d.predictguid = (guid)reader["predictguid"];
                    //d.weight = (double)reader["weight"];
                    //d.isfrozen = (boolean)reader["isfrozen"];
                    //dlist.add(d);
                    if (dList.Count() == 0)
                    {
                        cmd.Parameters.Add(new SqlParameter("@Weight", weight));
                        cmd.Parameters.Add(new SqlParameter("@IsFrozen", "freeze"));
                        cmd.Parameters.Add(new SqlParameter("@Confirmed", System.DateTime.UtcNow));
                        cmd.CommandText = "Update Dependency Set Weight=" + weight + "IsFrozen=" + "freeze" + "Confirmed" + System.DateTime.UtcNow;
                        cmd.ExecuteNonQuery();

                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@DependencyTypeID", dependencyClassID));
                        cmd.Parameters.Add(new SqlParameter("@SubejectClassID", subjectClassID));
                        cmd.Parameters.Add(new SqlParameter("@Weight", weight));
                        cmd.Parameters.Add(new SqlParameter("@IsFrozen", "freeze"));
                        cmd.Parameters.Add(new SqlParameter("@SubjectGUID", subjectGUID));
                        cmd.Parameters.Add(new SqlParameter("@PredicateClassID", predicateClassID));
                        cmd.Parameters.Add(new SqlParameter("@PreicateGUID", predicateClassID));
                        cmd.Parameters.Add(new SqlParameter("@Created", System.DateTime.UtcNow));
                        cmd.Parameters.Add(new SqlParameter("@Confirmed", System.DateTime.UtcNow));
                        cmd.CommandText = "Insert into Dependency([DependencyTypeID],[SubjectClassID],[Weight],[IsFrozen],[SubjectGUID],[PredicateClassID],[PredicateGUID],[Created],[Confirmed])Values(@)";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                conn.Close();
            }
            return Ok();
        }

        [HttpDelete, Route("Domain/delete")]
        public IHttpActionResult DeleteDependency(Guid tokenValue, string domain, string dependencyType, string subjectClass, Guid subjectGUID, string predicateClass, Guid predicateGUID)
        {
            int domainID = GetDomainID(domain);
            int dependencyTypeID = GetDependencyTypeID(domainID, dependencyType);
            int subjectClassID = GetObjectClassID(domainID, subjectClass);
            int predicateClassID = GetObjectClassID(domainID, predicateClass);
            AI.V2.BaseLibrary.DependencyLink d = new AI.V2.BaseLibrary.DependencyLink();
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "Delete from Dependency Where DependencyTypeID=" + dependencyTypeID + "AND SubjectClassID=" + subjectClassID + "AND SubejectGUID=" + subjectGUID + "AND PredicateClassID=" + predicateClassID + "AND PredicateGUID=" + predicateGUID;
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
            return Ok();
        }

        [HttpGet, Route("Domain/get/{domain}/{ObjectClass}")]
        public IHttpActionResult GetDependencyType(string domain, string ObjectClass)
        {
            AI.V2.BaseLibrary.Object o = new AI.V2.BaseLibrary.Object();
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
            int domainID = GetDomainID(domain);
            int objectClassID = GetObjectClassID(domainID, ObjectClass);
            var cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "SELECT Name,DefaultWeight,Description FROM DependencyType WHERE ObjectClassID=" + objectClassID + "AND IsValid=true";
            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    AI.V2.BaseLibrary.DependencyLink result = new AI.V2.BaseLibrary.DependencyLink();
                    result.Name = reader["Name"].ToString();
                    result.DefaultWeight = (int)reader["DefaultWeight"];
                    result.Description = reader["Description"].ToString();

                }
            }
            finally
            {
                conn.Close();
            }
            return Ok();
        }


        private List<Dependant> GetDependents(string domain, Guid objectGUID, int position, int count, string orderBy, AI.V2.BaseLibrary.Direction direction)
        {
            AI.V2.BaseLibrary.Dependant d = new AI.V2.BaseLibrary.Dependant();
            List<AI.V2.BaseLibrary.Dependant> dList = new List<AI.V2.BaseLibrary.Dependant>();
            string strstr = "select * from Dependant where SubjectGUID=" + objectGUID;
            if (direction == AI.V2.BaseLibrary.Direction.Descending)
            {
                switch (orderBy)
                {
                    case "ObjectClass":
                        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
                        var cmd = conn.CreateCommand();
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = strstr + "OrderBy PredicateClassName,DependencyType,Weight DESC";
                        try
                        {
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                d.DependencyType = reader["DependencyType"].ToString();
                                d.Domain = reader["Domain"].ToString();
                                d.Weight = (double)reader["Weight"];
                                d.ObjectGUID = (Guid)reader["PredicateGUID"];
                                d.ObjectClass = reader["PredicateClass"].ToString();
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                        break;
                    default:
                        SqlConnection conn1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
                        var cmd1 = conn1.CreateCommand();
                        cmd1.CommandType = System.Data.CommandType.Text;
                        cmd1.CommandText = strstr + "OrderBy DependencyType,PredicateClassName,Weight DESC";
                        try
                        {
                            var reader = cmd1.ExecuteReader();
                            while (reader.Read())
                            {
                                d.DependencyType = reader["DependencyType"].ToString();
                                d.Domain = reader["Domain"].ToString();
                                d.Weight = (double)reader["Weight"];
                                d.ObjectGUID = (Guid)reader["PredicateGUID"];
                                d.ObjectClass = reader["PredicateClass"].ToString();
                            }
                        }
                        finally
                        {
                            conn1.Close();
                        }
                        break;

                }
            }
            else
            {
                switch (orderBy)
                {
                    case "ObjectClass":
                        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
                        var cmd = conn.CreateCommand();
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = strstr + "OrderBy PredicateClassName,DependencyType,Weight ASC";
                        try
                        {
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                d.DependencyType = reader["DependencyType"].ToString();
                                d.Domain = reader["Domain"].ToString();
                                d.Weight = (double)reader["Weight"];
                                d.ObjectGUID = (Guid)reader["PredicateGUID"];
                                d.ObjectClass = reader["PredicateClass"].ToString();
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                        break;
                    default:
                        SqlConnection conn1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
                        var cmd1 = conn1.CreateCommand();
                        cmd1.CommandType = System.Data.CommandType.Text;
                        cmd1.CommandText = strstr + "OrderBy DependencyType,PredicateClassName,Weight ASC";
                        try
                        {
                            var reader = cmd1.ExecuteReader();
                            while (reader.Read())
                            {
                                d.DependencyType = reader["DependencyType"].ToString();
                                d.Domain = reader["Domain"].ToString();
                                d.Weight = (double)reader["Weight"];
                                d.ObjectGUID = (Guid)reader["PredicateGUID"];
                                d.ObjectClass = reader["PredicateClass"].ToString();
                            }
                        }
                        finally
                        {
                            conn1.Close();
                        }
                        break;

                }
            }

            dList.Add(d);

            return dList;
        }

        private List<Dependency> GetDependencies(string domain, Guid objectGUID, int position, int count, string orderBy, AI.V2.BaseLibrary.Direction direction)
        {
            List<AI.V2.BaseLibrary.Dependency> dList = new List<AI.V2.BaseLibrary.Dependency>();
            AI.V2.BaseLibrary.Dependency d = new AI.V2.BaseLibrary.Dependency();
            string strstr = "select * from Dependency Where PredicateGUID=" + objectGUID;
            if (direction == AI.V2.BaseLibrary.Direction.Descending)
            {
                switch (orderBy)
                {
                    case "ObjectClass":
                        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
                        var cmd = conn.CreateCommand();
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = strstr + "OrderBy PredicateClassName,DependencyType,Weight DESC";
                        try
                        {
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                d.DependencyType = reader["DependencyType"].ToString();
                                d.Domain = reader["Domain"].ToString();
                                d.Weight = (double)reader["Weight"];
                                d.ObjectGUID = (Guid)reader["SubjectGUID"];
                                d.ObjectClass = reader["SubjectClass"].ToString();
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                        break;
                    default:
                        SqlConnection conn1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
                        var cmd1 = conn1.CreateCommand();
                        cmd1.CommandType = System.Data.CommandType.Text;
                        cmd1.CommandText = strstr + "OrderBy DependencyType,PredicateClassName,Weight DESC";
                        try
                        {
                            var reader = cmd1.ExecuteReader();
                            while (reader.Read())
                            {
                                d.DependencyType = reader["DependencyType"].ToString();
                                d.Domain = reader["Domain"].ToString();
                                d.Weight = (double)reader["Weight"];
                                d.ObjectGUID = (Guid)reader["SubjectGUID"];
                                d.ObjectClass = reader["SubjectClass"].ToString();
                            }
                        }
                        finally
                        {
                            conn1.Close();
                        }
                        break;

                }
            }
            else
            {
                switch (orderBy)
                {
                    case "ObjectClass":
                        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
                        var cmd = conn.CreateCommand();
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = strstr + "OrderBy PredicateClassName,DependencyType,Weight ASC";
                        try
                        {
                            var reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                d.DependencyType = reader["DependencyType"].ToString();
                                d.Domain = reader["Domain"].ToString();
                                d.Weight = (double)reader["Weight"];
                                d.ObjectGUID = (Guid)reader["SubjectGUID"];
                                d.ObjectClass = reader["SubjectClass"].ToString();
                            }
                        }
                        finally
                        {
                            conn.Close();
                        }
                        break;
                    default:
                        SqlConnection conn1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
                        var cmd1 = conn1.CreateCommand();
                        cmd1.CommandType = System.Data.CommandType.Text;
                        cmd1.CommandText = strstr + "OrderBy DependencyType,PredicateClassName,Weight ASC";
                        try
                        {
                            var reader = cmd1.ExecuteReader();
                            while (reader.Read())
                            {
                                d.DependencyType = reader["DependencyType"].ToString();
                                d.Domain = reader["Domain"].ToString();
                                d.Weight = (double)reader["Weight"];
                                d.ObjectGUID = (Guid)reader["SubjectGUID"];
                                d.ObjectClass = reader["SubjectClass"].ToString();
                            }
                        }
                        finally
                        {
                            conn1.Close();
                        }
                        break;

                }
            }
            dList.Add(d);
            return dList;
        }


        private int GetDependencyTypeID(int domainID, string dependencyClass)
        {
            AI.V2.BaseLibrary.DependencyLink d = new AI.V2.BaseLibrary.DependencyLink();
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "Select ID from DependencyType Where Name=" + dependencyClass + "And DomainID=" + domainID;
            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    d.DependencyTypeID = (int)reader["ID"];

                }
            }
            finally
            {
                conn.Close();
            }
            return d.DependencyTypeID;
        }

        private int GetObjectClassID(int domainID, string objectClass)
        {
            AI.V2.BaseLibrary.DependencyLink d = new AI.V2.BaseLibrary.DependencyLink();
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
            var cmd1 = conn.CreateCommand();
            cmd1.CommandType = System.Data.CommandType.Text;
            cmd1.CommandText = "IF NOTEXIST(Select * from ObjectClass Where domainID=" + domainID + "And ObjectCLass=" + objectClass + ")";

            conn.Open();

            try
            {
                cmd1.ExecuteNonQuery();
                throw new InvalidObjectClassException();
            }
            finally
            {
                conn.Close();
            }

            var cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "Select ID from ObjectClass Where Name=" + objectClass + "And DomainID=" + domainID;
            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    d.ObjectClassID = (int)reader["ID"];

                }
            }
            finally
            {
                conn.Close();
            }
            return d.ObjectClassID;
        }

        private int GetDomainID(string domain)
        {
            AI.V2.BaseLibrary.DependencyLink d = new AI.V2.BaseLibrary.DependencyLink();
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AIn.Registry"].ConnectionString);
            var cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "Select ID from Domain Where Name='" + domain + "'";

            conn.Open();

            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    d.DomainID = (int)reader["ID"];

                }
            }
            finally
            {
                conn.Close();
            }
            return d.DomainID;
        }

        [Serializable]
        private class InvalidObjectClassException : Exception
        {
            public InvalidObjectClassException()
            {
            }

            public InvalidObjectClassException(string message) : base(message)
            {
            }

            public InvalidObjectClassException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected InvalidObjectClassException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}