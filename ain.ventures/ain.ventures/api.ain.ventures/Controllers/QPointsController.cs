using AIn.Ventures.BaseLibrary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.ain.ventures.Controllers
{
    public class QPointsController : ApiController
    {
        [HttpPost, Route("QPoint")]
        //[Authorize]
        public IHttpActionResult FindQPoints([FromBody]AIn.Ventures.Models.SearchParameters p)
        {
            List<AIn.Ventures.Models.QPoint> list = new List<AIn.Ventures.Models.QPoint>();

            AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();

            var cmd = ent.Database.Connection.CreateCommand();

            //cmd.Parameters.Add(new SqlParameter("@ObjectGUID", cp.GUID));

            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "create table #qpoint( guid uniqueidentifier )"
                + " insert into #qpoint"

                + " select [QPointGUID]"
                + " from [QPointToCategory] qp";

            if(p.Categories.Count >0)
                cmd.CommandText += " where qp.[Value] like '" + p.Categories[0] + "%'";

            for (int i = 1; i < p.Categories.Count(); i++)
            {
                cmd.CommandText += " delete q from #qpoint q inner join [QPointToCategory] qp"
                    + " on q.[guid] <> qp.[QPointGUID] and qp.[Value] like '" + p.Categories[i] + "%'";
            }

            foreach(KeyValuePair<string,string> pair in p.Properties)
            {
                cmd.CommandText += " delete q from #qpoint q inner join [QPointToProperty] qp"
                    + " on q.[guid] <> qp.[QPointGUID]"
                    + " and qp.[Name] = '" + pair.Key + "'"
                    + " and qp.[Value] = '" + pair.Value + "'";
            }

            cmd.CommandText += " select q.[GUID],"
                + " q.[Name],"
                + " s.[GUID] SourceGUID,"
                + " s.[Name] SourceName"
                + " from QPoint q"
                + " inner join #qpoint q2 on q2.[guid] = q.[GUID]"
                + " left outer join [QPointToSource] qs on qs.[QPointGUID] = q.[GUID]"
                + " left outer join [Source] s on s.[GUID] = qs.[SourceGUID]";

            
            cmd.CommandText += " order by q.[Name], q.[GUID], s.[Name]";

            try
            {
                ent.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                string name = string.Empty;
                Guid guid = new Guid();

                AIn.Ventures.Models.QPoint current = new AIn.Ventures.Models.QPoint();

                
                while (reader.Read())
                {
                    guid = (Guid)reader["GUID"];
                    name = reader["Name"].ToString();
                    Guid? sguid = (Guid?)reader["GUID"];

                    if (guid != current.GUID)
                    {
                        AIn.Ventures.Models.QPoint q = new AIn.Ventures.Models.QPoint()
                        {
                            GUID = guid,
                            Name = name
                        };

                        list.Add(q);

                        current = q;
                    }

                    if(sguid != null)
                    {
                        AIn.Ventures.Models.Source s = new AIn.Ventures.Models.Source();
                        s.Name = reader["SourceName"].ToString();
                        current.Sources.Add(s);
                    }
            
                }
                
            }
            finally
            {
                ent.Database.Connection.Close();
            }
            

            //return Ok(cmd.CommandText);
            return Ok(list);
        }
    

    [HttpPost, Route("QPoint/new")]
    //[Authorize]
    public IHttpActionResult CreatQPoints([FromBody]AIn.Ventures.Models.SearchParameters p,String Name,String Description)
    {
        List<AIn.Ventures.Models.QPoint> list = new List<AIn.Ventures.Models.QPoint>();

        AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();

        var cmd = ent.Database.Connection.CreateCommand();
        
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "CreateQpoints";
        cmd.Parameters.Add(new SqlParameter("@Name", Name));
        cmd.Parameters.Add(new SqlParameter("@Description", Description));
            try
        {
            ent.Database.Connection.Open();
            cmd.ExecuteNonQuery();

            }
        finally
        {
            ent.Database.Connection.Close();
        }


        //return Ok(cmd.CommandText);
        return Ok();
    }

        [HttpPost, Route("QPoint/query")]
        //[Authorize]
        public IHttpActionResult CreatQueryForQPoints([FromBody]AIn.Ventures.Models.SearchParameters p, String query)
        {
            List<AIn.Ventures.Models.QPoint> list = new List<AIn.Ventures.Models.QPoint>();

            AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();

            var cmd = ent.Database.Connection.CreateCommand();

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CreateQuery";
            cmd.Parameters.Add(new SqlParameter("@Query", query));
            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }


            //return Ok(cmd.CommandText);
            return Ok();
        }

        [HttpGet, Route("QPoint/search/{Query}")]
        //[Authorize]
        [AllowAnonymous]
        public IHttpActionResult GetQPointsBySearch(String Query) //http://www.webreference.com/programming/asp_net/Employee_Directory/index.html
        {
            List<AIn.Ventures.Models.QPoint> qpoints = new List<AIn.Ventures.Models.QPoint>();
            //List<AIn.Ventures.Models.QpointCategory> qpointcatergories = new List<AIn.Ventures.Models.QpointCategory>();
            AInVentureEntities ent = new AInVentureEntities();
            AIn.Ventures.Models.QPoint menu = new AIn.Ventures.Models.QPoint();
            AIn.Ventures.Models.QpointCategory c = new AIn.Ventures.Models.QpointCategory();
            //var cmd = ent.Database.Connection.CreateCommand();
            System.Data.Common.DbCommand cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetQPoint_ByCategoryLike";
            //cmd.CommandText = "GetQPoint_ByCategoryLike";
            cmd.Parameters.Add(new SqlParameter("@Search", "c"));

            try
            {
                ent.Database.Connection.Open();
                System.Data.Common.DbDataReader reader = cmd.ExecuteReader();
                //SqlDataReader reader = cmd.ExecuteReader();

                //List<AIn.Ventures.Models.QpointCategory> cats = new List<AIn.Ventures.Models.QpointCategory>();

                while (reader.Read())
                {
                    //AIn.Ventures.Models.QpointCategory cat = new AIn.Ventures.Models.QpointCategory();
                    AIn.Ventures.Models.QPoint qpo = new AIn.Ventures.Models.QPoint();
                    qpo.GUID = new Guid(reader["GUID"].ToString());
                    qpo.Name = reader["Name"].ToString();
                    qpo.Description = reader["Description"].ToString();
                    qpo.Categories = reader["Value"].ToString().Split('.').ToList<string>();
                    //cat.Name = (String)reader["c.Name"];
                    //cat.GUID = (Guid)reader["c.GUID"];
                    qpoints.Add(qpo);
                    //qpointcatergories.Add(cat);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                //cmd.Dispose();
                ent.Database.Connection.Close();
            }
            return Ok(qpoints);
        }
    }
}
