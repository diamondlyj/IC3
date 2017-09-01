<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral,  PublicKeyToken=31BF3856AD364E35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp4" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HealthCylinder.aspx.cs" Inherits="AIV1Portal.HealthCylinder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="font-family:Verdana; font-size:14px">
    <form id="form1" runat="server">
    <div align="center">
        <div style="margin-left:10px; border:none;">
        <asp4:Chart ID="Chart1" runat="server" Height="100" Width="160" TextAntiAliasingQuality="High" BackColor="#FFFFFF" BackSecondaryColor="White" BackGradientStyle="TopBottom">          
            <ChartAreas>
                <asp4:ChartArea Name="ChartArea1">              
                </asp4:ChartArea>
            </ChartAreas>
        </asp4:Chart>
        </div>
        <div>
            <asp:Label runat="server" ID="TextLabel"></asp:Label>
        </div>
        <div style="font-size:10px;">
            ESTIMATE<div style="display:inline; width:25px;"> <asp:Label runat="server" ID="EstimateLabel"></asp:Label></div>
        </div>
    </div>
    </form>
</body>
</html>

