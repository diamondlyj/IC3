using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for MIX1Instance
/// </summary>
/// 
public class MIX1Instance
{
    private string objClass;
    private string dataClass;
    private string objName;
    private string instName;
    private string[] instProperties;

    public MIX1Instance( string ObjectClass, string DataClass )
    {
        string appKey = "MIX1InstanceProperties." + DataClass;

        if (System.Configuration.ConfigurationManager.AppSettings[appKey] == null)
            this.instProperties = new string[0];
        else
        {
            this.instProperties = System.Configuration.ConfigurationManager.AppSettings[appKey].Split(';');
        }

        this.dataClass = DataClass;
        this.objClass = ObjectClass;
    }

    public string ObjectClass
    {
        get { return this.objClass; }
    }

    public string DataClass
    {
        get { return this.dataClass; }
    }


    public string ObjectName
    {
        get { return this.objName; }
        set { this.objName = value; }
    }

    public string InstanceName
    {
        get { return this.instName; }
        set { this.instName = value; }
    }

    public bool IsKeyProperty( string PropertyName )
    {
        if (this.instProperties.Length == 0)
            return true;

        for (int i = 0; i < this.instProperties.Length; i++)
        {
            if (this.instProperties[i] == PropertyName)
                return true;
        }

        return false;
    }
}

public class MIX1DataPoint
{
    private string attr;
    private string val;

    public MIX1DataPoint()
    {
    }

    public string Attribute
    {
        get { return this.attr; }
        set { this.attr = value; }
    }

    public string Value
    {
        get { return this.val; }
        set { this.val = value; }
    }
}
