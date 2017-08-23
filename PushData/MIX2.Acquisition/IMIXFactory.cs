using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

using MIX2.Data;

namespace MIX2.Acquisition
{
    public interface IMIXFactory
    {
        //DataSourceSchema.ObjectClass ObjectClass { get; }        

        void Open( DataSourceSchema.ObjectClass  ObjectClass );
        void Close();
        bool IsAlive( out string message );

        LocalObject GetNext();

        bool EOD { get; }
    }
}
