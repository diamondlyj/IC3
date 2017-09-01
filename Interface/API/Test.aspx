<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="AIV1Portal.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Username: <input id="username" runat="server" type="text" />
        </div>
        <div>
            Password: <input id="pwd" runat="server" type="password" />
        </div>
        <div>
            Password: <input id="submit" runat="server" type="submit" />
        </div>
        <div>
            <asp:label ID="myLabel" runat="server" text="Label"></asp:label>
        </div>
      
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/DataService.svc" />
            </Services>
        </asp:ScriptManager>
    </form>
</body>
</html>

