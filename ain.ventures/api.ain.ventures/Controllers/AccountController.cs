using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;

using AIn.Ventures.BaseLibrary;
using Newtonsoft.Json;
using System.Web;

namespace api.ain.ventures.Controllers
{
    public class AccountController : ApiController
    {
        int iterationCount = 16384;

        /*
        ---------------------------------------
        ---------------------------------------
        */
        /// <summary>
        /// Route for creating a user. Users must be associated with a valid email address (used for login).
        /// An email is sent to the address contained in the POST sent to the route.
        /// The user will not be activated until the email is confirmed. See method <b>ConfirmUser(Guid GUID)</b>
        /// associated with route /Confirmation/{GUID}.
        /// </summary>
        /// <param name="user">Object containing the email address used for logging in (along with other user details) and password. Password that will be salted, encrypted and stored.</param>
        /// <returns>GUID generated for the new user.</returns>
        [HttpPost, Route("Users")]
        [AllowAnonymous]
        public IHttpActionResult CreateUser(AIn.Ventures.Models.User user)
        {
            RandomNumberGenerator gen = RNGCryptoServiceProvider.Create();

            byte[] salt = new byte[32];

            gen.GetBytes(salt);
            
            Rfc2898DeriveBytes pbdkdf2 = new Rfc2898DeriveBytes(user.Pwd, salt, iterationCount);
            user.Pwd = null;

            byte[] hash = pbdkdf2.GetBytes(20);

            string pwd = Convert.ToBase64String(salt) + ":" + iterationCount.ToString() + ":" + Convert.ToBase64String(hash);
            
            user.GUID = Guid.NewGuid();
            //user.Pwd = hash;

            
            AInVentureEntities entities = new AInVentureEntities();
            entities.CreateUser(user.GUID, user.EmailAddress, user.GivenNames, user.Surname, pwd);
  
            SendToken(user.GUID,user.EmailAddress);

          
           // SetUser(user,url);
          
            return Ok(user);
        }
        
      
        private void SetUser([FromBody]AIn.Ventures.Models.User user,[FromUri]string url)
        {
            Guid guid = Guid.NewGuid();
            Guid ObjectGuid = Guid.NewGuid();
            AInVentureEntities entities = new AInVentureEntities();
            Uri myUri = new Uri("Url");
            string project = HttpUtility.ParseQueryString(myUri.Query).Get("project");
            string product = HttpUtility.ParseQueryString(myUri.Query).Get("product");
            entities.CreateProject(guid, project, "defaultDescription", 100, user.GUID);
            //entities.CreateComponent(ObjectGuid, product, product, "defaultDescription", 0, null, null, null, guid);
          
        }
        /*
      ---------------------------------------
      Test function. Should be commented out in production!
      ---------------------------------------
      */
        [HttpGet, Route("Users/Create")]
        public IHttpActionResult CreateUser()
        {
            Random rand = new Random();
            int n = rand.Next(999);

            
            AIn.Ventures.Models.User u = new AIn.Ventures.Models.User();
            u.Surname = "Doe" + n.ToString();
            u.GivenNames = "John";
            u.EmailAddress = "andreas.olsson@ain.email";
            
            /*
            String[] nameFirstA = { "Jo", "Do", "Da", "Je", "Le", "Pe" };
            String[] nameFirstB = { "n", "e", "rry", "ve", "nny" };
            String[] nameLastA = { "Wat", "Rus", "Scot", "Do" };
            String[] nameLastB = { "son", "sel", "t", "e" };
            String[] domainA = { "ain.email", "yahoo.com", "gmail.com" };
            String[] passwordA = { "", "green", "pwd", "my", "letmein", "strong", "qwerty", "123", "654", "1q2w3e", "home" };
            String[] passwordB = { "456", "123", "uiop", "321", "bad", "4r5t6y", "please", "star" };
            
            String nameFirst = nameFirstA[rand.Next(nameFirstA.Length)] + nameFirstB[rand.Next(nameFirstB.Length)];
            String nameLast = nameLastA[rand.Next(nameLastA.Length)] + nameLastB[rand.Next(nameLastB.Length)];
            String domain = domainA[rand.Next(domainA.Length)];
            String password = passwordA[rand.Next(passwordA.Length)] + passwordB[rand.Next(passwordB.Length)];
            */
        
            var form = new Dictionary<string, string>
            {
                {"Surname", "Doe" + n.ToString()},
                {"GivenNames", "John"},
                {"EmailAddress", "john.doe" + n.ToString() + "@ain.ventures"},
                {"Pwd", "pwdPwd321!" }
            };
            
            
            
            /*
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(JsonConvert.SerializeObject(u)),"User");
            formData.Add(new StringContent("pwdPwd321!"), "Password");
            */

            var client = new HttpClient();
            

            var result = client.PostAsync("https://localhost:4243/Users", new FormUrlEncodedContent(form)).Result;
            
            return Ok(result.Content.ReadAsStringAsync());

            //return Ok(formData);
        }

        /// <summary>
        /// Route used to confirm receipt of an email and that the user is the owner of an email address being used
        /// for authentication purposes.
        /// 
        /// User will not be able to log in until they have confirmed and must therefore use a valid existing email.
        /// </summary>
        /// <param name="GUID"></param>
        /// Idenifier for the token that was created along with the user.
        /// <returns>empty response</returns>
        [HttpGet, Route("Confirmation/{GUID}")]
        public IHttpActionResult ConfirmUser(Guid GUID)
        {
            AIn.Ventures.BaseLibrary.AInVentureEntities ent = new AIn.Ventures.BaseLibrary.AInVentureEntities();
            ent.ConfirmEmailAddress(GUID);
            ent.SaveChanges();

            return Ok();
        }

        public void SendToken(Guid userGUID, string emailAddress)
        {
            TimeSpan span = new TimeSpan(0,30,0);
            Guid g = Guid.NewGuid();
            DateTime expires = DateTime.UtcNow.Add(span);

            AIn.Ventures.BaseLibrary.AInVentureEntities entities = new AIn.Ventures.BaseLibrary.AInVentureEntities();
            entities.CreateToken(g,userGUID,expires);

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Credentials = new NetworkCredential("ain.ventures@intuitivelabs.pvt", "sabA%324");
            client.Port = 25;
            client.Host = "mail.intuitivelabs.net";

            System.Net.Mail.MailMessage mes = new System.Net.Mail.MailMessage("ain.ventures@ain.email", emailAddress);

            mes.Subject = "A New Experience";
            mes.Body = "Thank you for with AiN Ventures. In order to use toyr you need to confirm "
                + " receipt of this . Once you have comfirmed you will be able to receive updates at this address"
                + " This address will also be used for recovery purposes. You can add additional addresses"
                + " using the AIn Ventures.\n\n The thing about this message is that body can do bo "
                + " aligator border and so something was late to somewhere\n\nYes this is tedious tamper with tomp"
                + " making crocodiles hungry in winter\n\nCheerio!!!\n\n" + g.ToString();

            
            try
            {
                client.Send(mes);

            }
            catch (Exception exc)
            {
                //return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, exc.Message);
            }
        }

        /// <summary>
        /// To alter a password the old password has to be known.
        /// </summary>
        /// <param name="GUID">Identifier of user to change password for.</param>
        /// <param name="reset">Complex type containing old and new password.</param>
        /// <returns></returns>

        [HttpPost, Route("Users/{GUID}/Pwd")]
        [Authorize]
        public IHttpActionResult SetPassword(Guid GUID, AIn.Ventures.Models.PasswordReset reset)
        {

            AIn.Ventures.BaseLibrary.AInVentureEntities entities = new AIn.Ventures.BaseLibrary.AInVentureEntities();
            IEnumerator<GetUser_ByGUID_Result> users = entities.GetUser_ByGUID(GUID).GetEnumerator();


            if (users.MoveNext() && users.Current.Pwd != null)
            {


                char[] lim = { ':' };
                string[] split = users.Current.Pwd.Split(lim);

                if (split.Length >= 3)
                {


                    byte[] salt = Convert.FromBase64String(split[0]);
                    int iter = int.Parse(split[1]);
                    byte[] hash = Convert.FromBase64String(split[2]);

                    Rfc2898DeriveBytes pbdkdf2 = new Rfc2898DeriveBytes(reset.OldPwd, salt, iter);
                    byte[] b = pbdkdf2.GetBytes(20);

                    int n = 0;

                    while (n < 20)
                    {
                        if (b[n] != hash[n])
                        {
                            break;
                        }
                        n++;
                    }

                    if (n == 20)
                    {
                        //reset pwd.
                        byte[] salt_2 = new byte[32];

                        RandomNumberGenerator gen = RNGCryptoServiceProvider.Create();
                        gen.GetBytes(salt_2);

                        Rfc2898DeriveBytes pbdkdf2_2 = new Rfc2898DeriveBytes(reset.NewPwd, salt_2, iterationCount);

                        byte[] hash_2 = pbdkdf2_2.GetBytes(20);

                        string pwd = Convert.ToBase64String(salt_2) + ":" + iterationCount.ToString() + ":" + Convert.ToBase64String(hash_2);


                        entities = new AIn.Ventures.BaseLibrary.AInVentureEntities();
                        entities.User_ChangePassword(GUID, pwd);

                        return Ok(true);

                        //users.Current.Pwd = pwd;
                        // entities.SaveChanges();
                    }
                }
            }

            return Ok(false);
        }
    }
}

