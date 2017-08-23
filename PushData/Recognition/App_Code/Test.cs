using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;


/// <summary>
/// Summary description for Test
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Test : System.Web.Services.WebService {

    public Test () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public relevanceAnswer GetStringArray( string a, string b) 
    {
        return new relevanceAnswer();
    }


    public class relevanceAnswer
    {
        private string[] sample;

        public relevanceAnswer()
        {            
        }

        public string[] a
        {
            get { return this.sample; }
            set { this.sample = value; }
        }
    }
    
}

