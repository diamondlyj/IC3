using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AIn.Ventures.Models
{
    [DataContract]
    public class QPoint
    {
        public QPoint()
        {
            this.Properties = new List<KeyValuePair<string, string>>();
            this.Categories = new List<String>();
            this.Sources = new List<Models.Source>();
            this.QpointCategories = new List<Models.QpointCategory>();
        }

        [DataMember]
        public Guid GUID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public String Description { get; set; }


        [DataMember]
        public List<Source> Sources { get; set; } //where this sort of thing can be ordered from
        [DataMember]
        public List<KeyValuePair<string, string>> Properties; //requirments
        [DataMember]
        public List<String> Categories;
        [DataMember]
        public List<QpointCategory> QpointCategories { get; set; }

    }

    [DataContract]
    public class Source : IDataSource
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid GUID { get; set; }

        [DataMember]
        public Guid QpointGUID { get; set; }

        [DataMember]
        public string Query { get; set; } //what to search to get the right products

        [DataMember]
        public Uri Api { get; set; }

        [DataMember]
        public object Token { get; set; }

        public object GetData(string query)
        {
            return null;
        }

        public List<AvailableProduct> GetProduct(QPoint qp)
        {
            throw new NotImplementedException();
        }

        public AvailableProduct GetDetails(string ItemKey)
        {
            throw new NotImplementedException();
        }

        public bool AddToBasket(string ItemKey)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFromBasket()
        {
            throw new NotImplementedException();
        }

        public bool Checkout()
        {
            throw new NotImplementedException();
        }

        public bool buyNow()
        {
            throw new NotImplementedException();
        }

        public bool DeleteFromBasket(string ItemKey)
        {
            throw new NotImplementedException();
        }

        public bool Checkout(string ItemKey)
        {
            throw new NotImplementedException();
        }

        public bool BuyNow(string ItemKey)
        {
            throw new NotImplementedException();
        }

        public bool ChangeAmount(string ItemKey)
        {
            throw new NotImplementedException();
        }

        public AvailableProduct FindAlternative(string ItemKey)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDataSource
    {
        object GetData(string query);
        List<AvailableProduct> GetProduct(QPoint qp);
        AvailableProduct GetDetails(string ItemKey);
        bool AddToBasket(string ItemKey);
        bool DeleteFromBasket(string ItemKey);
        bool Checkout(string ItemKey);
        bool BuyNow(string ItemKey);
        bool ChangeAmount(string ItemKey);
        AvailableProduct FindAlternative(string ItemKey);
        string Name { set; get; }
        Guid GUID { set; get; }
        object Token { set; get; }
    }

    [DataContract]
    public class OrderParameters
    {
        [DataMember]
        public QPoint Qpoint { get; set; }
        [DataMember]
        public int Amount { get; set; }
        [DataMember]
        public double PriceMin { get; set; }
        [DataMember]
        public double PriceMax { get; set; }
        [DataMember]
        public string SourceName { get; set; }

    }

    [DataContract]
    public class AvailableProduct
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string SKU { get; set; }
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public string OrderID { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public Source Source { get; set; }

    }

    [DataContract]
    public class SearchParameters
    {
        public SearchParameters()
        {
            this.Categories = new List<string>();
            this.Properties = new Dictionary<string, string>();
        }

        [DataMember]
        public string Keywords { get; set; }

        [DataMember]
        public List<string> Categories { get; set; }

        [DataMember]
        public Dictionary<string,string> Properties { get; set; }
    }

    [DataContract]
    public class SourceProduct
    {
        public SourceProduct()
        {
            this.Source = new Models.Source();
        }

        [DataMember]
        public string ItemKey { get; set; }
        [DataMember]
        public DateTime ItemDate { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public IDataSource Source { get; set; }
        [DataMember]
        public Guid ComponentGUID { get; set; }
        [DataMember]
        public Guid ParentGUID { get; set; }
    }
    [DataContract]
    public class QpointCategory
    {
        public QpointCategory()
        {
   
            this.QpointCategories = new List<Models.QpointCategory>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid GUID { get; set; }

        [DataMember]
        public string SourceName { get; set; }

        [DataMember]
        public string LabelName { get; set; }

        [DataMember]
        public List<QpointCategory> QpointCategories { get; set; }

    }
    [DataContract]
    public class Label
    {
        public Label()
        {
            this.labels = new List<Models.Label>();
        }
        [DataMember]
        public List<Label> labels { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid CategoryGUID { get; set; }
        [DataMember]
        public Guid QpointGUID { get; set; }
        [DataMember]
        public string Value { get; set; }
    }


}
