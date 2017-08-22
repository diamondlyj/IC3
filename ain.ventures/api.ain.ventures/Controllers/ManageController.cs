using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using AIn.Ventures.BaseLibrary;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace api.ain.ventures.Controllers
{
    public class ManageController : ApiController
    {
        // GET: Manage
        [HttpGet, Route("Data")]
        [Authorize]
        public IHttpActionResult Index()
        {
            return Ok("Manage Controller Login! Welcome!");
        }


        [HttpDelete, Route("Data/query")]
        [Authorize]
        public IHttpActionResult RemoveQuery(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "DeleteQuery_ByQpointGUID(GUID)";
            cmd.Parameters.Add(new SqlParameter("@QpointGUID", GUID));
            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok();
        }

        [HttpPost, Route("Data/query")]
        [Authorize]
        public IHttpActionResult AddQuery(Guid GUID, [FromBody]AIn.Ventures.Models.Source source)
        {
            //return Ok();
            AIn.Ventures.Models.QPoint NewQpoint = new AIn.Ventures.Models.QPoint();
            AInVentureEntities ent = new AInVentureEntities();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "AddNewQuery";
            cmd.Parameters.Add(new SqlParameter("@QPointGUID", GUID));
            cmd.Parameters.Add(new SqlParameter("@Name", source.Name));
            cmd.Parameters.Add(new SqlParameter("@query", source.Query));
            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok(NewQpoint);

        }
        [HttpPost, Route("Data/qpoint/new")]
        [Authorize]
        public IHttpActionResult AddQpoint(Guid GUID, [FromBody]AIn.Ventures.Models.QPoint qp)
        {
            // return Ok();
            AIn.Ventures.Models.QPoint NewQpoint = new AIn.Ventures.Models.QPoint();
            AInVentureEntities ent = new AInVentureEntities();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CreateNewQpoints";
            cmd.Parameters.Add(new SqlParameter("@QPointGUID", GUID));
            cmd.Parameters.Add(new SqlParameter("@Name", qp.Name));
            cmd.Parameters.Add(new SqlParameter("Description", qp.Description));

            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok(NewQpoint);
        }

        [HttpDelete, Route("Data/{GUID}")]
        [Authorize]

        public IHttpActionResult DeleteQpoint(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "DeleteQpoint_ByQpointGUID(GUID)";
            cmd.Parameters.Add(new SqlParameter("@QpointGUID", GUID));
            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok();
        }


        [Authorize]
        [ActionRightsFilter(SecuredEntity = Entity.Project, RequiredRight = Right.View)]
        [HttpPut, Route("Data/{GUID}")]
        public IHttpActionResult UpdateQpoint(Guid GUID, AIn.Ventures.Models.QPoint qp)
        {
            AInVentureEntities ent = new AInVentureEntities();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "UpdateQpoint_ByQpointGUID(GUID)";
            cmd.Parameters.Add(new SqlParameter("@QpointGUID", GUID));
            cmd.Parameters.Add(new SqlParameter("@Name", qp.Name));
            cmd.Parameters.Add(new SqlParameter("@Description", qp.Description));
            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok("OK");
        }

        [Authorize]
        [HttpPost, Route("Data/query")]
        public IHttpActionResult AddQueryForQpoints(Guid GUID, AIn.Ventures.Models.Source source)
        {
            AInVentureEntities ent = new AInVentureEntities();

            Guid SourceGUID = new Guid();
            if (source.Name.ToLower() == "ebay")
            {
                SourceGUID = GUID;
            }



            ent.UpdateQuery_ByQpointGUID(GUID, SourceGUID, source.Query);
            ent.SaveChanges();

            return Ok("OK");
        }

        [HttpGet, Route("Data/Source")]
        public IHttpActionResult GetLabel(Guid CategoryGUID)
        {

            AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();

            var cmd = ent.Database.Connection.CreateCommand();
            cmd.Parameters.Add(new SqlParameter("@CategoryGUID", CategoryGUID));

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetAllLabels";

            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery(); ;
            }
            finally
            {

                ent.Database.Connection.Close();

            }

            return Ok();
        }

        [HttpPost, Route("Data/label/new")]
        [Authorize]
        public IHttpActionResult CreateNewLabel(Guid GUID, [FromBody]AIn.Ventures.Models.Label label)
        {
            // return Ok();
            AInVentureEntities ent = new AInVentureEntities();
            Guid CategoryGUID = new Guid();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CreateNewLabel";
            cmd.Parameters.Add(new SqlParameter("@QPointGUID", GUID));
            cmd.Parameters.Add(new SqlParameter("@CategoryGUID", CategoryGUID));
            cmd.Parameters.Add(new SqlParameter("@Label", label.Name));

            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok(label);
        }

        [HttpDelete, Route("Data/{GUID}")]
        [Authorize]

        public IHttpActionResult DeleteLabel(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "DeleteLabel_ByCategoryGUID";
            cmd.Parameters.Add(new SqlParameter("@CategoryGUID", GUID));
            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok();

        }

        [HttpPost, Route("Data/source/new")]
        [Authorize]
        public IHttpActionResult CreateNewSource(Guid GUID, [FromBody]AIn.Ventures.Models.Source source)
        {
            // return Ok();
            AInVentureEntities ent = new AInVentureEntities();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CreateNewSource";
            cmd.Parameters.Add(new SqlParameter("@SourceGUID", GUID));
            cmd.Parameters.Add(new SqlParameter("@Name", source.Name));

            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok(source);
        }

        [HttpDelete, Route("Data/Source/{GUID}")]
        [Authorize]

        public IHttpActionResult RemoveSource(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "RemoveSource_ByQpointGUID";
            cmd.Parameters.Add(new SqlParameter("@QpointGUID", GUID));
            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }

            return Ok();
        }


        [Authorize]
        [HttpPut, Route("Data/source")]
        public IHttpActionResult AddSource(Guid QpointGUID, Guid SourceGUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "AddSource_ByQpointGUID";
            cmd.Parameters.Add(new SqlParameter("@QpointGUID", QpointGUID));
            cmd.Parameters.Add(new SqlParameter("@SourceGUID", SourceGUID));
            try
            {
                ent.Database.Connection.Open();
                cmd.ExecuteNonQuery();

            }
            finally
            {
                ent.Database.Connection.Close();
            }
            return Ok("OK");
        }
        [HttpGet,Route("Data/qpoint1/{GUID}")]
        [AllowAnonymous]
        public IHttpActionResult GetOneQpoint(Guid GUID)
        {
            AInVentureEntities ent = new AInVentureEntities();
            AIn.Ventures.Models.QPoint point = new AIn.Ventures.Models.QPoint();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetQpoint_ByQpointGUID";
            cmd.Parameters.Add(new SqlParameter("@QpointGUID",GUID));
            try
            {
                ent.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                
               
                
                if (reader.Read())
                {
                    point.Name = (String)reader["Name"];
                    point.GUID = (Guid)reader["GUID"];
                    if (reader["Description"] != System.DBNull.Value)
                    {
                        point.Description = (String)reader["Description"];
                    }
                   
                }
            }
            finally
            {
                ent.Database.Connection.Close();
            }
            return Ok(point);
        }


        [HttpGet, Route("Data/qpoint")]
        // [Authorize]
        [AllowAnonymous]
        public IHttpActionResult GetQpoint()
        {

            List<AIn.Ventures.Models.QPoint> points = new List<AIn.Ventures.Models.QPoint>();
            AInVentureEntities ent = new AInVentureEntities();
            AIn.Ventures.Models.QPoint menu = new AIn.Ventures.Models.QPoint();
            AIn.Ventures.Models.QpointCategory c = new AIn.Ventures.Models.QpointCategory();
            var cmd = ent.Database.Connection.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "GetQpoints";


            try
            {
                ent.Database.Connection.Open();
                var reader = cmd.ExecuteReader();

                List<AIn.Ventures.Models.QpointCategory> cats = new List<AIn.Ventures.Models.QpointCategory>();


                while (reader.Read())
                {
                    AIn.Ventures.Models.QpointCategory cat = new AIn.Ventures.Models.QpointCategory();
                    cat.Name = reader["Name"].ToString();
                    cat.GUID = (Guid)reader["QPointGUID"];
                    cat.SourceName = (String)reader["SourceName"];
                    c.QpointCategories.Add(cat);
                }
            }
            finally
            {
                ent.Database.Connection.Close();
            }


            return Ok(c);
            //return Ok(points);
        }
    }
}


