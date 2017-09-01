<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral,  PublicKeyToken=31BF3856AD364E35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatalogingChart.aspx.cs" Inherits="AIV1Portal.CatalogingChart" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="padding: 0px; margin: 0px;">
    <form id="form1" runat="server">
    <div>
        <asp:Chart ID="Chart1" runat="server" Height="340" Width="520" TextAntiAliasingQuality="High" BackColor="#F1F1F1" BackSecondaryColor="White" BackGradientStyle="TopBottom">          
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1">              
                </asp:ChartArea>
            </ChartAreas>
        </asp:Chart>
    </div>
    </form>
</body>
</html>
