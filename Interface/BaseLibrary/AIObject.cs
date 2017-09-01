using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AI.V2.BaseLibrary
{
    public class AIObject
    {
        public AIObject()
        {
            this.Meta = new Meta();
            this.Signature = new Signature();
            this.Network = new Network();
        }

        public AIObject(string GUID)
        {
            this.GUID = new Guid(GUID);
            this.Meta = new Meta();
            this.Signature = new Signature();
            this.Network = new Network();
        }


        public string ObjectClass
        {
            get; set;
        }

        public Guid GUID
        {
            get; set;
        }

        public Meta Meta
        {
            set; get;
        }
        public Signature Signature
        {
            set; get;
        }
        public Network Network
        {
            set; get;
        }
    }


    public class Meta
    {
        public Meta()
        {
            this.Properties = new List<Property>();
        }
        public List<Property> Properties
        {
            set; get;
        }

    }
    public class Signature
    {
        public Signature()
        {
            this.Properties = new List<Property>();
        }
        public List<Property> Properties
        {
            set; get;
        }

    }
    public class Property
    {
        public Property()
        {
            this.Value = new List<string>();
        }

        public Property(string FriendlyName)
        {
            this.PropertyClass = FriendlyName;
            this.Value = new List<string>();
        }

        public string PropertyClass
        {
            get; set;
        }

        public List<string> Value
        {
            set; get;
        }
    }

    public class Network
    {
        public Network()
        {
            this.Links = new List<Link>();
        }

        public List<Link> Links
        {
            set;get;
        }
    }

    public class Link
    {
        public Link(string ObjectClass)
        {
            this.ObjectClass = ObjectClass;
            this.Weight = 1;
        }

        public Link(string ObjectClass, double Weight)
        {
            this.ObjectClass = ObjectClass;
            this.Weight = Weight;
        }


        public string ObjectClass
        {
            get; set;
        }

        public double Weight {
            get; set;
        }

        public Link()
        {

        }

        public List<Guid> GUIDs
        {
            get;set;
        } 
    }

}