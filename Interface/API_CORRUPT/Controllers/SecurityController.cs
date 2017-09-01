using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class SecurityController : ApiController
    {
        [HttpGet, Route("AssertRole")]
        public IHttpActionResult AssertRole()
        {
            var str = "";
            // TODO: Get token from Header and put it in GUID
            SqlConnection conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["AIn.Userbase"].ConnectionString);
            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select [GUID] from Token";
            cmd.Connection = conn;

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    str = reader["GUID"].ToString();
                }
            }
            finally
            {
                conn.Close();
            }
            


            return Ok(str);
        }
    }
}

/*
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using AIn.Userbase;

namespace AIn.Security.Authentication
{

    public class Authenticator : IAuthenticator
    {
        private DataClassesDataContext dataContext;

        public Authenticator()
        {
            this.dataContext = new DataClassesDataContext(ConfigurationManager.ConnectionStrings["AIn.Userbase"].ConnectionString);
        }

        public DataClassesDataContext Context
        {
            get { return this.dataContext; }
        }

        public void AssertRole(Guid tokenValue, AIn.Security.Role role)
        {
            role |= AIn.Security.Role.Super;

            AIn.Userbase.Token token = this.GetToken(tokenValue);

            AIn.Security.Role arole = (AIn.Security.Role)token.Role;

            if ((arole & role) == 0)
                throw new PermissionDeniedException();
        }

        private AIn.Userbase.Token GetToken(Guid tokenValue)
        {
            Token token = this.dataContext.Tokens.Single(t => t.GUID == tokenValue);
            this.ValidateToken(token);

            return token;
        }

        public bool IsValid(Guid tokenValue)
        {
            if (!this.dataContext.Tokens.Any(t => t.GUID == tokenValue))
                return false;

            Token token = this.dataContext.Tokens.Single(t => t.GUID == tokenValue);

            return this.IsValid(token);
        }

        private bool IsValid(Token token)
        {
            DateTime updated = (DateTime)token.Validated;

            //TimeSpan t = System.Xml.XmlConvert.ToTimeSpan(ConfigurationSettings.AppSettings["SessionLength"]);
            TimeSpan t = new TimeSpan(0, 0, token.LifeSpan);
            TimeSpan t2 = DateTime.UtcNow.Subtract(updated);

            if (t2 > t)
                return false;
            else
                return true;
        }

        public AIn.Userbase.User GetUser(Guid tokenValue)
        {
            Token token = this.GetToken(tokenValue);
            return token.User;
        }

        public Guid Login(AIn.Security.IDType idType, string identifier, string pwd)
        {
            byte[] userPwd = System.Text.Encoding.Default.GetBytes(pwd);

            switch (idType)
            {
                case AIn.Security.IDType.Username:

                    if (!this.dataContext.InternalUsers.Any(u => u.Username == identifier))
                        throw new AuthenticationException();

                    InternalUser user = this.dataContext.InternalUsers.Single(u => u.Username == identifier);

                    AIn.Userbase.Salt salt = user.Salts.Single(s => s.Type == 0);
                    byte[] hashWithSalt = AIn.Security.Salt.CreateHash(userPwd, salt.Value.ToArray());

                    if (!AIn.Security.Salt.AreIdentical(hashWithSalt, user.Pwd.ToArray()))
                        throw new AuthenticationException();

                    //Make this a function
                    Token token = new Token();
                    token.Validated = DateTime.UtcNow;
                    token.LifeSpan = Convert.ToInt32(System.Xml.XmlConvert.ToTimeSpan(System.Configuration.ConfigurationManager.AppSettings["SessionLength"]).TotalSeconds);

                    try
                    {
                        AssignedRole arole = this.dataContext.AssignedRoles.Single(a => a.ObjectGUID == user.GUID);
                        token.Role = arole.Role;
                    }
                    catch
                    {
                        token.Role = (int)AIn.Security.Role.User;
                    }

                    token.UserGUID = user.GUID;

                    this.dataContext.Tokens.InsertOnSubmit(token);
                    this.dataContext.SubmitChanges();

                    return token.GUID;

                default:
                    throw new AuthenticationException();
            }
        }

        public Guid LoginAgainstDirectory(string directory, string identifier, string pwd)
        {
            //System.DirectoryServices.AccountManagement.PrincipalContext context = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain, directory);

            //if (!context.ValidateCredentials(identifier, pwd) || !this.dataContext.ExternalDirectories.Any(d => d.Identifier == directory))
            //    throw new AuthenticationException();
            //else
            //{
            ExternalDirectory dir = this.dataContext.ExternalDirectories.Single(d => d.Identifier == directory);

            Guid guid = Guid.Empty;

            //Make this a function
            Token token = new Token();
            token.Validated = DateTime.UtcNow;
            token.LifeSpan = Convert.ToInt32(System.Xml.XmlConvert.ToTimeSpan(System.Configuration.ConfigurationManager.AppSettings["SessionLength"]).TotalSeconds);

            ExternalUser extUser = new ExternalUser();

            bool isnew = false;

            if (!this.dataContext.ExternalUsers.Any(u => u.Identifier == identifier))
            {
                isnew = true;

                guid = Guid.NewGuid();

                User user = new User();
                user.GUID = guid;
                user.IsExternal = true;

                extUser.GUID = guid;
                extUser.Identifier = identifier;
                extUser.DirectoryGUID = dir.GUID;


                user.ExternalUser = extUser;

                this.dataContext.Users.InsertOnSubmit(user);

            }
            else
            {
                extUser = this.dataContext.ExternalUsers.Single(u => u.DirectoryGUID == dir.GUID && u.Identifier == identifier);
            }

            //Now check if the person has authority to access system.
            try
            {   //Bind to the native AdsObject to force authentication.		
                string path = System.Configuration.ConfigurationManager.AppSettings["LDAPPath"];

                String domainAndUsername = directory + @"\" + identifier;
                System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(path, domainAndUsername, pwd);

                Object obj = entry.NativeObject;

                System.DirectoryServices.DirectorySearcher search = new System.DirectoryServices.DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + identifier + ")";
                //search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("memberOf");
                System.DirectoryServices.SearchResult result = search.FindOne();

                int role = 0;

                if (null == result)
                {
                    Console.WriteLine("no user");
                }
                else
                {
                    int propertyCount = result.Properties["memberOf"].Count;

                    String dn;

                    IQueryable<ExternalGroup> groups = this.dataContext.ExternalGroups.Where(g => g.DirectoryGUID == dir.GUID);

                    int equalsIndex, commaIndex;

                    for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                    {

                        dn = (String)result.Properties["memberOf"][propertyCounter];

                        equalsIndex = dn.IndexOf("=", 1);
                        commaIndex = dn.IndexOf(",", 1);


                        string str = dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1);
                        Console.WriteLine(str);

                        foreach (ExternalGroup group in groups)
                        {
                            if (group.Identifier == str)
                            {
                                AssignedRole arole = dataContext.AssignedRoles.Single(a => a.ObjectGUID == group.GUID);
                                role |= arole.Role;
                            }
                        }

                    }
                }

                token.Role = role;
            }
            catch (Exception exc)
            {
                throw exc;
            }

            token.UserGUID = extUser.GUID;

            if (isnew)
                this.dataContext.ExternalUsers.InsertOnSubmit(extUser);

            this.dataContext.Tokens.InsertOnSubmit(token);

            this.dataContext.SubmitChanges();

            return token.GUID;
            //}
        }

        public void LogOff(Guid tokenValue)
        {
            if (!this.dataContext.Tokens.Any(t => t.GUID == tokenValue))
                return;

            Token token = this.dataContext.Tokens.Single(t => t.GUID == tokenValue);
            token.LifeSpan = 0;
            //token.Validated = DateTime.UtcNow.Subtract(new TimeSpan(0, 1, 0));

            this.dataContext.SubmitChanges();
        }

        public bool UserHasRole(User user, AIn.Security.Role role)
        {
            //User user = this.GetUser(tokenValue);

            return this.dataContext.AssignedRoles.Any(a => a.ObjectGUID == user.GUID && ((AIn.Security.Role)a.Role & role) != 0);
        }


        public AIn.Security.Role GetUserRoles(User user)
        {
            if (this.dataContext.AssignedRoles.Any(a => a.ObjectGUID == user.GUID))
            {
                AssignedRole assignedRole = this.dataContext.AssignedRoles.Single(a => a.ObjectGUID == user.GUID);

                return (AIn.Security.Role)assignedRole.Role;
            }
            else
                return AIn.Security.Role.Unknown;
        }


        public AIn.Security.Role GetRoles(Guid tokenValue)
        {
            AIn.Userbase.User user = GetUser(tokenValue);
            return GetUserRoles(user);
        }

        public void ValidateToken(AIn.Userbase.Token token)
        {
            if (!this.IsValid(token))
                throw new SessionExpiredException();

            token.Validated = DateTime.UtcNow;
            this.dataContext.SubmitChanges();
        }

        public string TestConnection(AIn.Security.IDType idType, string identifier, string pwd)
        {
            byte[] userPwd = System.Text.Encoding.Default.GetBytes(pwd);

            switch (idType)
            {
                case AIn.Security.IDType.Username:

                    if (!this.dataContext.InternalUsers.Any(u => u.Username == identifier))
                        return "no user";
                    //throw new AuthenticationException();

                    InternalUser user = this.dataContext.InternalUsers.Single(u => u.Username == identifier);

                    AIn.Userbase.Salt salt = user.Salts.Single(s => s.Type == 0);
                    byte[] hashWithSalt = AIn.Security.Salt.CreateHash(userPwd, salt.Value.ToArray());

                    if (!AIn.Security.Salt.AreIdentical(hashWithSalt, user.Pwd.ToArray()))
                        return "wrong";
                    //throw new AuthenticationException();

                    //Make this a function
                    Token token = new Token();
                    token.Validated = DateTime.UtcNow;
                    token.LifeSpan = Convert.ToInt32(System.Xml.XmlConvert.ToTimeSpan(System.Configuration.ConfigurationManager.AppSettings["SessionLength"]).TotalSeconds);

                    try
                    {
                        AssignedRole arole = this.dataContext.AssignedRoles.Single(a => a.ObjectGUID == user.GUID);
                        token.Role = arole.Role;
                    }
                    catch
                    {
                        token.Role = (int)AIn.Security.Role.User;
                    }

                    token.UserGUID = user.GUID;

                    this.dataContext.Tokens.InsertOnSubmit(token);
                    this.dataContext.SubmitChanges();

                    return token.GUID.ToString();

                default:
                    return "wrong ID Type";
                    //    throw new AuthenticationException();
            }
        }

        public string Echo(string str)
        {
            return str;
        }


    }

}
*/

