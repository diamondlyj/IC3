using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntuitiveLabs.Security
{
    [Flags]
    public enum Role
    {
        Unknown = 0, Super = 1, Administrator = 2, User = 4, Viewer = 4, Creator = 8, Editor = 16, Publisher = 32 
    }

    [Flags]
    public enum ObjectType
    {
        Unknown = 0, User = 1, ExternalUser = 2, Group = 4, ExternalGroup = 8 
    }

    [Flags]
    public enum IDType
    {
        Unknown = 0, Username = 1, EmailAddress = 2, MobileAddress = 4, Nickname = 8
    }
}
