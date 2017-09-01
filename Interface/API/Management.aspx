<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Management.aspx.cs" Inherits="AIV1Portal.Management" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Management (AI Gen One)</title>
    <style type="text/css">    
    body
    {
        font-family: Verdana;
    }
    
    .TiledBackground
    {
        position:absolute;
        left: 0px;
        right:0px;
        top:0px;
        bottom:0px;
        background-image:url(images/DottedBG.gif); 
        background-repeat:repeat;        
    }
    
    .FramePadder
    {
        border:solid 1px gray;
        position: absolute;
        left: 2px;
        right: 2px;
        top: 2px;
        bottom: 2px;
    }
    
    .ID_Header
    {
        font-size: 14px;
        color: white;
        background-color:Blue;
        border: 1px solid black;
    }
    
    .GUID_Column
    {
        font-size: 18px;
        color:Red;
    }
    
    .Lists
    {
        padding: 10px;
    }
    
    .List_Item
    {
        font-size:14px;
        text-decoration: none;
        font-weight: bold;
        padding-left: 3px;
        color:#555555;
    }
    </style>

    <script language="javascript" src = "js/IntuitiveLabs_Collections.js"type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI_Menu.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI_ObjectList.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI_Popup.js" type="text/javascript"></script>
    <script language="javascript" src = "js/IntuitiveLabs_UI_Form.js" type="text/javascript"></script>

    
    <script type="text/javascript" language="javascript">

        /*
        document.onkeypress = function(evt) {
            if (!evt) {
                evt = window.event;
                evt.which = evt.keyCode;
            }

            if (evt.which == 13 && !isLoggedIn && form_login != null) {
                var pwd = form_login.getInput("Password");
                alert(pwd.value);
                //setTimeout(submitLogin,500);
            }
        };
        */        
        

        var activeGroup = null;
        var popup_addGroup = null;
        var loginPopup = null;
        var form_login = null;
        var form_addGroup = null;
        var form_roles = null;
        var element_LogOut = null
        var element_DirectorySelector = null;
        var element_ListDisplay = null;
        var element_ObjectPanel_Display = null;
        var element_ObjectPanel_Viewer = null;
        var element_ObjectPanel_Load = null;
        var element_Object_Name = null;
        var element_Object_GUID = null;
        var isLoggedIn = false;

        var uiWindow = null;
        
        function adjust() {
            uiWindow.adjust();
            //alert("da");
        }

        function submitLogin() {
            form_login.submit();
        }

        function init() {
            uiWindow = new IntuitiveLabs.UI.Window();

            window.onresize = adjust;

            element_LogOut = document.getElementById("LogOut");            
            element_DirectorySelector = document.getElementById("DirectorySelector");
            element_ListDisplay = document.getElementById("ListDisplay");
            element_ObjectPanel_Display = document.getElementById("ObjectPanel_Display");
            element_ObjectPanel_Viewer = document.getElementById("ObjectPanel_Viewer");
            element_ObjectPanel_Load = document.getElementById("ObjectPanel_Load");
            element_Object_Name = document.getElementById("Object_Name");
            element_Object_GUID = document.getElementById("Object_GUID");

            //form_login = new IntuitiveLabs.UI.LoginForm();
            //form_login.onLogin = onLogin;
            //form_login.waitImage = "images/LoadCircle2.gif";
            form_login = new IntuitiveLabs.UI.GenericForm();
            form_login.onEnter = function(sender) { onLogin(sender.parent) };

            var pwdBox = new IntuitiveLabs.UI.Textbox("Password", null, true);
            //pwdBox.onEnter = function(sender) { alert(sender.value); };
            
            form_login.addInput("Username", new IntuitiveLabs.UI.Textbox("Username", null, false));
            form_login.addInput("Password", pwdBox);
            form_login.submitText = "Login"
            form_login.waitImage = "images/V1-Load.gif"
            form_login.onSubmit = onLogin;
            
            loginPopup = new IntuitiveLabs.UI.Popup();            
            loginPopup.isClosable = false;

            //form.onChangeUsername = function(sender) { alert(sender.value) }
            //form.onChangePassword = function(sender) { alert(sender.value) }

            //Create a form to add groups
            form_addGroup = new IntuitiveLabs.UI.GenericForm();
            form_addGroup.addInput("Directory", new IntuitiveLabs.UI.Textbox("Directory", null, false));
            form_addGroup.addInput("Identifier", new IntuitiveLabs.UI.Textbox("Identifier", null, false));
            form_addGroup.submitText = "Add"
            form_addGroup.waitImage = "images/V1-Load.gif"
            form_addGroup.onSubmit = onAddGroup;

            var div_roles = document.createElement("DIV");
            var div_roles_header = document.createElement("DIV");
            div_roles_header.style.fontWeight = "bold";
            div_roles_header.style.color = "#666666";
            div_roles_header.innerHTML = "Roles";
            div_roles.appendChild(div_roles_header);
                                   
            form_roles = new IntuitiveLabs.UI.GenericForm();
            form_roles.addInput("User", new IntuitiveLabs.UI.Checkbox("User", 4, onChangeRole));
            form_roles.addInput("Administrator", new IntuitiveLabs.UI.Checkbox("Administrator", 2, onChangeRole));

            div_roles.appendChild(form_roles.createElement());
            
            element_ObjectPanel_Viewer.appendChild(div_roles);
            
            form_roles.hideSubmit();
            form_roles.hide();

            popup_addGroup = new IntuitiveLabs.UI.Popup();
            popup_addGroup.content = form_addGroup.createElement()
            popup_addGroup.render();
            popup_addGroup.hide();

            callGetDirectories();
        }

        function blankPassword() {
            var pwd = form_login.getInput("Password");
            pwd.nullifyValue();        
        }

        function callAddGroup(directory, identifier) {
            GroupManagement.AddGroup(directory, identifier, onAddGroupSuccess, onFail);
        }

        function callAssignRole(groupGUID, role) {
            GroupManagement.AssignRole(groupGUID, role, onChangeRoleSuccess, onFail);
        }

        function callGetDirectories() {
            GroupManagement.GetDirectories( onGetDirectoriesSuccess, onFail);
        }

        function callGetGroup(groupGUID) {
            GroupManagement.GetGroup(groupGUID, onGetGroupSuccess, onFail);
        }

        function callGetGroups( directoryGUID, position, chunkSize ) {
            GroupManagement.GetGroups(directoryGUID, position, chunkSize, onGetGroupsSuccess, onFail);
        }        

        function callLogOff() {
            Authenticator.LoggOff(onLogOffSuccess, onFail);
        }
        
        function callRevokeRole(groupGUID, role) {
            GroupManagement.RevokeRole(groupGUID, role, onChangeRoleSuccess, onFail);
        }

        function changeDirectory() {
            var option = element_DirectorySelector.options[element_DirectorySelector.selectedIndex];
            var guid = option.value;

            if (guid != "") {
                callGetGroups(guid, 0, -1);
            }
        }

        function clearWindow() {
            element_Object_Name.innerHTML = "";
            element_Object_GUID.innerHTML = "";
            element_ListDisplay.style.display = "none";
        }

        function hideViewerLoad() {
            element_ObjectPanel_Load.style.display = "none";
        }
        
        function loadObject(sender){
            //alert("EDIT: " + sender.item.dataObject.guid);
            element_ObjectPanel_Load
            element_Object_Name.innerHTML = "";
            element_Object_GUID.innerHTML = "";
            element_ObjectPanel_Load.style.display = "block";
            form_roles.hide();
            callGetGroup(sender.item.dataObject.guid);
        }

        function onAddGroup(sender) {
            popup_addGroup.hideMessage();

            var dir = sender.getInput("Directory").value;
            var id = sender.getInput("Identifier").value

            if (dir != null && id != null && dir != "" && id != "") {
                callAddGroup(dir,id)

            }
            else {
                popup_addGroup.showMessage("Please enter a directory and id.");
                form_addGroup.hideWaitImage();
                popup_addGroup.dropShadow.adjust();
            }
        }

        function onAddGroupSuccess() {
            form_addGroup.hideWaitImage();
            popup_addGroup.hide();
            callGetDirectories();
        }

        function onChangeRole(sender) {
            form_roles.hide();
            showViewerLoad();

            if (sender.isChecked)
                callAssignRole(activeGroup.guid, sender.value);
            else
                callRevokeRole(activeGroup.guid, sender.value);
                
            //alert(sender.value + ":" + sender.isChecked);
        }

        function onChangeRoleSuccess() {
            form_roles.show();
            hideViewerLoad();
        }

        function onIsLoggedInSuccess(result) {
            isLoggedIn = result;
        
            if (!result) {                        
                element_LogOut.style.visibility = "hidden";
                loginPopup.show();
                form_roles.hide();                
            }
            else {
                changeDirectory();            
                loginPopup.hide();
                element_LogOut.style.visibility = "visible";
            }                       
        }

        function onGetDirectoriesSuccess(result) {

            var drop = new IntuitiveLabs.UI.Dropdown("Directory");


            IntuitiveLabs.UI.DOM.removeChildren(element_DirectorySelector);

            for (var i = 0; i < result.length; i++) {
                var option = document.createElement("OPTION");
                option.value = result[i].GUID;
                option.innerHTML = result[i].Identifier;
                element_DirectorySelector.appendChild(option);

                drop.addOption(result[i].GUID, result[i].Identifier, result[i].Identifier);
            }

                        
            drop.addOption("0", "[internal]", null);

            if (!isLoggedIn) {
                form_login.addInput("Directory", drop);

                loginPopup.content = form_login.createElement()
            }
            
            /*
            var option = document.createElement("OPTION");
            option.value = "";
            option.innerHTML = "[internal]";
            element_DirectorySelector.appendChild(option);
            */

            element_DirectorySelector.options[0].selected = true;

            loginPopup.render();
            loginPopup.hide();
            
            Authenticator.IsLoggedIn( onIsLoggedInSuccess, onFail); 
        }

        function onGetGroupSuccess(result) {
            activeGroup = result;
            
            element_ObjectPanel_Display.style.display = "block";
            
            hideViewerLoad();

            element_Object_Name.innerHTML = result.identifier;
            element_Object_GUID.innerHTML = result.guid;

            var inputs = form_roles.getInputs();

            for (var i = 0; i < inputs.length; i++) {
                if ((result.role & inputs[i].value.value) != 0)
                    inputs[i].value.check();
                else
                    inputs[i].value.uncheck();                
            }
            
            

            form_roles.show();                     
            
            //alert(IntuitiveLabs.Security.Role.Administrator & result.role);
        }

        function onGetGroupsSuccess(result) {
            activeDirectory = result;
            

            element_ListDisplay.style.display = "block";

            IntuitiveLabs.UI.DOM.removeChildren(element_ListDisplay);

            var objList = new IntuitiveLabs.UI.ObjectList();
            objList.listClass = "Lists";
            objList.displayHeader = false;
            //objList.headerClass = "ID_Header";

            var col1 = new IntuitiveLabs.UI.ListColumn("data", "identifier", "ID", loadObject);
            col1.itemClass = "List_Item";            
            objList.addColumn(col1);

            /*
            var col2 = new IntuitiveLabs.UI.ListColumn("function", "edit", "EDIT", loadObject );
            col2.itemClass = "List_Column";
            objList.addColumn(col2);
            
            var col3 = new IntuitiveLabs.UI.ListColumn("function", "delete", "DELETE", function(sender) { alert("DELETE: " + sender.item.dataObject.guid) });
            col3.itemClass = "List_Column";
            objList.addColumn(col3);
            */
            
            objList.itemClass = "ItemClass";

            for (var i = 0; i < result.Groups.length; i++) {
                var item = new IntuitiveLabs.UI.ListItem( result.Groups[i].guid, result.Groups[i],"images/Group-Green.png" );
                item.onClick = function(){ alert("ba"); }
                objList.addItem(  item );
            }

            var el = objList.createElement();

            objList.render(element_ListDisplay);

            uiWindow.children.remove("ObjectList");
            uiWindow.children.add("ObjectList", objList);
        }

        function onLogin(sender) {
            //alert("baba");
            loginPopup.hideMessage();
            sender.showWaitImage();
            
            var dir = sender.getInput("Directory").current.value;
            var uname = sender.getInput("Username").value;                                   
            var pwd = sender.getInput("Password");
            
            if(dir != null)
                Authenticator.LoginAgainstDirectory(dir, uname, pwd.value, onLoginSuccess, onFail);                        
            else
                Authenticator.Login(uname, pwd.value, onLoginSuccess, onFail);
        }

        function onLoginSuccess( role ) {
            //form_login.blankPassword();
            blankPassword();
            
            isLoggedIn = true;
            
            form_login.hideWaitImage();
            loginPopup.hide();

            element_LogOut.style.visibility = "visible";

            changeDirectory();                       
        }

        function onLogOffSuccess() {
            clearWindow();
            blankPassword();
            element_LogOut.style.visibility = "hidden";            
            loginPopup.dropShadow.adjust();
            loginPopup.show();
            form_roles.hide();
            isLoggedIn = false;    
        }
                        

        function onFail(err) {
            alert(err.get_message());

            blankPassword();
            //loginPopup.showMessage("Failed to log in.");
            form_login.hideWaitImage();
            
            loginPopup.dropShadow.adjust();
        }

        function showAddGroup() {
            var sel = document.getElementById("DirectorySelector");
            var dir = sel.options[sel.selectedIndex].text;
            var inp = form_addGroup.getInput("Directory");
            inp .setValue(dir);
            popup_addGroup.show();
        }

        function showViewerLoad() {
            element_ObjectPanel_Load.style.display = "block";
        }
          
        
    </script>
</head>
<body onload="init();">
    <form id="form1" runat="server" onsubmit="alert('hello'); return false">        
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Authenticator.svc" />
                <asp:ServiceReference Path="~/GroupManagement.svc" />
            </Services>
        </asp:ScriptManager>
        <div id="ModuleSelector" style="position:fixed; left:0px; right:0px; top:2px; height:30px; overflow:hidden; ">            
            <div class="FramePadder">
                <div class="TiledBackground">                
                    <div style="cursor:pointer; position:absolute; text-decoration:underline;  left:75px; top: 4px; font-weight:bold; font-size:12px;">
                        <div id="LogOut" style="visibility:hidden; cursor:pointer;" onclick="callLogOff();">Log Out</div>
                    </div>
                    <div style="cursor:pointer; position:absolute; text-decoration:none;  right:10px; top: 4px; font-weight:bold; font-size:12px;">
                        <div>Access Rights</div>
                    </div>
                </div>
            </div>            
        </div>
        <div id="Navigator" style="position:fixed; left:0px; width:250px; top:30px; bottom:0px; overflow:hidden;">
            <div class="FramePadder">
                <div class="TiledBackground">
                    <div style="cursor:pointer; position:absolute; text-decoration:none; color:#888888; font-size: 14px; left:2px; top: 10px; font-weight:bold;">
                        Directory:<br />
                        <select style="width:240px;" id="DirectorySelector" onchange="changeDirectory();"></select>
                    </div>
                    <div style="position:absolute; top:50px; bottom:4px; left:4px; right:4px; border:solid 1px gray; background-color:#ffffff; overflow:hidden;">
                        <div style="cursor:pointer; color:#000000; font-size: 12px; font-weight:bold; border-bottom:1px dotted gray;">
                            <div style="display:table; width:228px; border:none; right:0px; padding:2px;">
                                <div style="display:table-row;">
                                    <div style="display:table-cell; text-align:left; color:#666666">GROUPS</div>
                                    <div style="display:table-cell; text-align:right; text-decoration:underline;" onclick="showAddGroup();">ADD</div>
                                <div>
                            </div>
                        </div>                    
                        <div id="ListDisplay" style="border:none; position:absolute; top:20px; bottom:0px; left:0px; right:0px;">
                        </div>
                    </div>
                </div>            
            </div>
        </div>
        <div id="ObjectPanel" style="position:fixed; left:250px; top:30px; bottom:0px; right: 0px;">
            <div class="FramePadder">
                <div class="TiledBackground">
                    <div id="ObjectPanel_Display" style="display:block;">
                        <div style="cursor:pointer; position:absolute; text-decoration:none; color:#333333; left:2px; top: 6px; font-weight:bold;">
                            <span id="Object_Name"></span> <span id="Object_GUID" style="color:#666666; font-size:12px;"></span><br />
                        </div>
                        <div style="position:absolute; top:26px; bottom:4px; left:4px; right:4px; border:solid 1px gray; padding:10px; background-color:#ffffff; overflow:visible;" id="ObjectPanel_Viewer">
                        </div>
                        <div style="position:absolute; top:50%; left:50%; display:none; " id="ObjectPanel_Load">
                            <img style="position:absolute; left:-32px; top:-32px;" src="imagesV1-Load.gif" />
                        </div>
                    </div>                   
                </div>
            </div>
        </div>
        <div style="position:fixed; top:0px;">
            <img src="images/V1.png" />
        </div>
        
    </form>
</body>
</html>

