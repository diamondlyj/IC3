using System;
using System.Collections.Generic;
using System.Text;

namespace MIX.Data
{
    public class DataPointValue: ICloneable
    {
        public DateTime Time;
        public string Object;
        public string Instance;
        public string Value;

        public DataPointValue() { }

        public DataPointValue(string Object, string Instance, string Value)
        {
            this.Assign(Object, Instance, Value);
        }

        public DataPointValue(DataPointValue dpValue)
        {
            if( dpValue != null )
                this.Assign(dpValue.Object, dpValue.Instance, dpValue.Value);
        }

        protected void Assign(string Object, string Instance, string Value)
        {
            this.Object = Object;
            this.Instance = Instance;
            this.Value = Value;
        }

        public Object Clone()
        {
            return new DataPointValue(this);
        }

        override public string ToString()
        {
            return Object + "." + Instance + "." + Value + " (" + Time.ToString() + ")";
        }
    }

    public class DataPointIndex : ICloneable
    {
        public string ObjectClass;
        public string DataClass;
        public string Attribute;

        public DataPointIndex() { }

        public DataPointIndex(string ObjectClass, string DataClass, string Attribute)
        {
            this.ObjectClass = ObjectClass;
            this.DataClass = DataClass;
            this.Attribute = Attribute;
        }


        public DataPointIndex(DataPointIndex dpIndex)
        {
            this.ObjectClass = dpIndex.ObjectClass;
            this.DataClass = dpIndex.DataClass;
            this.Attribute = dpIndex.Attribute;
        }

        protected void Assign(string ObjectClass, string DataClass, string Attribute)
        {
            this.ObjectClass = ObjectClass;
            this.DataClass = DataClass;
            this.Attribute = Attribute;
        }

        public Object Clone()
        {
            return new DataPointIndex(this);
        }
        
        override public string ToString()
        {
            return ObjectClass + "." + DataClass + "." + Attribute;
        }
    }

    public class DataPoint : ICloneable
    {
        public string         DataSource;
        public DataPointIndex Index;
        public DataPointValue Value;


        public DataPoint(string DataSource, DataPointIndex dpIndex, DataPointValue dpValue)
        {
            this.Assign(DataSource, dpIndex, dpValue);
        }


        public DataPoint(DataPoint dp)
        {
            if( dp != null )
                this.Assign(dp.DataSource, dp.Index, dp.Value);
        }

        protected void Assign(string DataSource, DataPointIndex dpIndex, DataPointValue dpValue)
        {
            this.DataSource = DataSource;
            if (dpIndex != null)
                this.Index = (DataPointIndex)dpIndex.Clone();
            if (dpValue != null)
                this.Value = (DataPointValue)dpValue.Clone();
        }


        public object Clone()
        {
            return new DataPoint(this);
        }


        override public string ToString()
        {
            return DataSource + ":" + Index.ToString() + ":" + Value.ToString();
        }
    }


    public class ObjectRange : ICloneable
    {
        public string Start;
        public string End;

        public ObjectRange() { }

        public ObjectRange(string Start, string End)
        {
            this.Start = Start;
            this.End = End;
        }

        public ObjectRange(ObjectRange Range)
            : this(Range.Start, Range.End)
        {
        }

        public object Clone()
        {
            return new ObjectRange(this);
        }

        override public string ToString()
        {
            return "[" + Start + "," + End + "]";
        }
    }

    
    





    }







