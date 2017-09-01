using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.V2.BaseLibrary
{    
    [Serializable]
    [DataContract]
    public class Object 
    {
        private List<string> objClass;
        private Guid guid;
        private PropertyBag meta;
        private PropertyBag signature;
        //private Queue<Relation> addRelationOnUpdate;
        //private Queue<Relation> updateRelationOnUpdate;

        public Object()
        {
            Init();
            this.guid = Guid.NewGuid();
        }

        public Object(string guid)
        {
            Init();
            this.guid = new Guid(guid);
        }

        private void Init()
        {
            this.signature = new BaseLibrary.PropertyBag(this);
            this.meta = new BaseLibrary.PropertyBag(this);
        }

        /*
        public Queue<Relation> AddRelationOnUpdate
        {
            get { return this.addRelationOnUpdate; }
        }
        
        public Queue<Relation> UpdateRelationOnUpdate
        {
            get { return this.updateRelationOnUpdate; }
        }
         * */

        [DataMember]
        public PropertyBag Meta
        {
            get
            {
                return this.meta;
            }
            set
            {
                this.meta = value;
            }
        }

        public PropertyBag Signature
        {
            get
            {
                return this.signature;
            }
            set
            {
                this.signature = value;
            }
        }


        public List<string> Class
        {
            get
            {
                return this.objClass;
            }
        }

        [DataMember]
        public Guid GUID
        {
            get { return this.guid; }
            set { this.guid = value; }
        }



    }

    //[Serializable]
    //[DataContract]
    //public class Property 
    //{
    //    private List<Value> vals;
    //    private string name;
    //    private Object obj;

    //    public Property(Object obj, string name)
    //    {
    //        this.obj = obj;
    //        this.name = name;
    //        this.vals = new List<Value>();
    //    }

    //    [DataMember]
    //    public string Name
    //    {
    //        get
    //        {
    //            return this.name;
    //        }

    //        set
    //        {
    //            this.name = value;
    //        }
    //    }

    //    public Object Object
    //    {
    //        get
    //        {
    //            return this.obj;
    //        }
    //    }

    //    [DataMember]
    //    public List<Value> Values
    //    {
    //        get
    //        {
    //            return this.vals;
    //        }
    //    }


    //}

    [DataContract]
    [Serializable]
    public class Value 
    {
        private object val;
        private Property prop;
        public object newVal;

        public Value(Property prop, object val)
        {
            this.val = val;
            this.prop = prop;
        }



        public Property Property
        {
            get
            {
                return this.prop;
            }
        }

        public object Get()
        {
            return this.val;
        }

        public object GetFreshest()
        {
            if (this.newVal != null)
                return this.newVal;
            else
                return this.val;
        }

        public void Set(object val)
        {
            this.newVal = val;
        }

        [DataMember]
        public object Val
        {
            set { this.Set(value); }
            get { return this.Get(); }
        }

        public void Update()
        {
            if (this.newVal != null)
            {
                this.val = this.newVal;
                this.newVal = null;
            }
        }

        public void Delete()
        {
        }

        public void Create()
        {
        }
    }

    [DataContract]
    public class PropertyBag 
    {
        private List<Property> prop;
        private Object obj;

        public PropertyBag(Object obj)
        {
            this.obj = obj;
            this.prop = new List<Property>();
        }

        public Object Object
        {
            get { return this.obj; }
        }

        [DataMember]
        public List<Property> Property
        {
            get { return this.prop; }
            set { this.prop = value; }
        }

        public Property GetProperty(string name)
        {
            foreach (Property p in this.prop)
            {
                if (p.PropertyClass == name)
                    return p;
            }

            return null;
        }

    }
}
