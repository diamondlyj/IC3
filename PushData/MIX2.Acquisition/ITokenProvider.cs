using System;
using System.Collections.Generic;
using System.Text;

namespace MIX2.Acquisition
{
    public interface ITokenProvider
    {
        object GetToken( object[] Parameters);
    }
}
