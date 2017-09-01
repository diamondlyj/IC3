<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Portal.aspx.cs" Inherits="AIV1Portal.Portal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Portal (AI Gen One)</title>
    
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
        font-size:12px;
        text-decoration: none;
        font-weight: normal;
        padding-left: 3px;
        color:#555555;
        height:20px;
        vertical-align:middle;
    }
    
    .PanelGroup_Toolbar
    {
    	padding: 5px;
    	text-decoration: underline;
    	text-align: right;
    }
    
    .DataTable
    {
    	/* border: solid 1px #AAAAAA;*/
    	/*background-color: #666666;*/
    }
    
    .DataTable_Header
    {
    	border: solid 1px #AAAAAA;
    	background-color: #EEEEEE;
    	font-weight: bold;
    	color: #666666;
    	padding: 2px;
    }
    
    .DataTable_Row
    {
    	border: dotted 1px #AAAAAA;
    	padding: 3px;
    }

    .Signature
    {
    	font-size: 14px;    	
    }

    .ChildSignature
    {
    	font-size: 11px;    	
    }
    
    </style>
    
    
    <script language="javascript" src = "js/Xcellence.js"type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_Collections.js"type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_Data.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI_MasterWidget.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI_Grid.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI_Menu.js" type="text/javascript"></script>
    <script language="javascript" src = "js/IntuitiveLabs_UI_ObjectList.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI_Popup.js" type="text/javascript"></script>
    <script language="javascript" src = "js/IntuitiveLabs_UI_Form.js" type="text/javascript"></script>
    <script language="javascript" src = "js/IntuitiveLabs_UI_Panel.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI_Tree.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_UI_Walk.js" type="text/javascript"></script>    
    <script language="javascript" src = "js/IntuitiveLabs_Xml.js"type="text/javascript"></script>    

    <script language="javascript" src = "js/AIV1.js"type="text/javascript"></script>    
    
    <script type="text/javascript">
 
        var uiWindow = null;
        
        var loginPopup = null;
        var form_login = null;
        //var form_addDependency = null;
        var isLoggedIn = false;

        var element_CatalogSelector;
        var element_ListDisplay = null;
        var element_List_Load = null;
        var element_ListSpecs = null
        var element_ListNext = null
        var element_ListPrev = null
        var element_ListSummary = null
        var element_LogOut = null
        var element_Management = null
        var element_Support=null

        var element_Object_Name = null;
        var element_Object_GUID = null;
        var element_ObjectPanel = null;
        var element_ObjectPanel_Expander = null;
        var element_ObjectPanel_Load = null;
        var element_ObjectPanel_Toolbar = null;
        var element_ObjectPanel_Viewer = null;
        var element_Object_Info = null;

        var element_CatalogPanel = null;
        var element_CatalogPanel_Viewer = null;
        var element_CatalogPanel_Chart = null;
        var element_CatalogPanel_Load = null;        //var element_Prefilters = null;
        var element_Catalogs = null;

        var element_HealthPanel = null;
        var element_HealthPanel_GlobalChart = null;

        var element_Selector = null;
        var element_Selector_CatalogSelector = null;
        var element_Selector_ListDisplay = null;
        var element_Selector_List_Load = null;
        var element_Selector_ListSpecs = null
        var element_Selector_ListNext = null
        var element_Selector_ListPrev = null
        var element_Selector_ListSummary = null
        
        var element_View_Container = null
        var element_View_Selector = null

        var element_SearchValue = null;
        var element_WalkDisplay = null;

        var about = null;
        var newDependencyWidget = null;
        var searchWidget = null;
        var dependencyTree = null;
        var actionIndicator = null;
        var panelGroup = null;
        var objectToolbar = null;
        var signatureElement = null;
        var objectToolbar = null;
        var prefilterMenu = null;
        var catalogsMenu = null;
        var propertiesMenu = null;
        var currentPanelName = "Details";

        var aspects = null;
        var childClassElements = null;

        var currentPosition = 0;        
        var chunkSize = 100;
        var objectClass = "Node";
        var objectCount = 0;
        var matchSubstrings = false;
        var pathList = null;
        var walk = null;

        var currentObject = null;

        var objectPanel_isExpanded = false;

        function pageLoad() {
            uiWindow = new IntuitiveLabs.UI.Window();
            
            childClassElements = new IntuitiveLabs.Collections.Dictionary();
            
            element_CatalogSelector = document.getElementById("CatalogSelector");
            element_ListDisplay = document.getElementById("ListDisplay");
            element_List_Load = document.getElementById("List_Load");
            element_ListSpecs = document.getElementById("ListSpecs");
            element_ListNext = document.getElementById("ListNext");
            element_ListPrev = document.getElementById("ListPrev");
            element_ListSummary = document.getElementById("ListSummary");
            element_LogOut = document.getElementById("LogOut");
            element_Management = document.getElementById("Management");
            element_Support=document.getElementById("Support");

            element_Object_Name = document.getElementById("Object_Name");
            element_Object_GUID = document.getElementById("Object_GUID");
            element_Object_Info = document.getElementById("Object_Info");
            element_ObjectPanel = document.getElementById("ObjectPanel");
            element_ObjectPanel_Expander = document.getElementById("ObjectPanel_Expander");
            element_ObjectPanel_Load = document.getElementById("ObjectPanel_Load");
            element_ObjectPanel_Toolbar = document.getElementById("ObjectPanel_Toolbar");
            element_ObjectPanel_Viewer = document.getElementById("ObjectPanel_Viewer");

            element_CatalogPanel = document.getElementById("CatalogPanel");
            element_CatalogPanel_Load = document.getElementById("CatalogPanel_Load");
            element_CatalogPanel_Viewer = document.getElementById("CatalogPanel_Viewer");
            element_CatalogPanel_Chart = document.getElementById("CatalogPanel_Chart");

            element_HealthPanel = document.getElementById("HealthPanel");
            element_HealthPanel_GlobalChart = document.getElementById("HealthPanel_GlobalChart");

            element_Catalogs = document.getElementById("Catalogs");
            //element_Prefilters = document.getElementById("Prefilters");
            element_Properties = document.getElementById("Properties");
            
            element_Selector = document.getElementById("Selector");
            element_Selector_CatalogSelector = document.getElementById("Selector_CatalogSelector");
            element_Selector_ListDisplay = document.getElementById("Selector_ListDisplay");
            element_Selector_List_Load = document.getElementById("Selector_List_Load");
            element_Selector_ListSpecs = document.getElementById("Selector_ListSpecs");
            element_Selector_ListNext = document.getElementById("Selector_ListNext");
            element_Selector_ListPrev = document.getElementById("Selector_ListPrev");
            element_Selector_ListSummary = document.getElementById("Selector_ListSummary");
            
            element_View_Container = document.getElementById("View_Container");
            element_View_Selector = document.getElementById("View_Selector");
            element_WalkDisplay = document.getElementById("WalkDisplay");
            
            element_SearchValue = document.getElementById("SearchValue");
            element_Selector_SearchValue = document.getElementById("Selector_SearchValue");

            var styleTemplate = new IntuitiveLabs.UI.StyleTemplate();
            styleTemplate.setProperty("width", "150px");
            styleTemplate.setProperty("border", "1px solid gray");
            styleTemplate.setProperty("paddingLeft", "2px");

            form_login = new IntuitiveLabs.UI.GenericForm();
            form_login.defaultStyleTemplate = styleTemplate;
            form_login.onEnter = function(sender) { onLogin(sender.parent) };

            var pwdBox = new IntuitiveLabs.UI.Textbox("Password", null, true);

            form_login.addInput("Username", new IntuitiveLabs.UI.Textbox("Username", null, false));
            form_login.addInput("Password", pwdBox);
            form_login.submitText = "Login";
            
            form_login.waitImage = "images/V1-Load.gif"
            form_login.onSubmit = onLogin;

            newDependencyWidget = new AIV1.UI.NewDependencyWidget();
            newDependencyWidget.searchWidget.onClickItem = addDependency;
            document.body.appendChild(newDependencyWidget.createElement());
            
            newDependencyWidget.affix("auto", 62, -250, 2);
            newDependencyWidget.adjust();
            
            searchWidget = new AIV1.UI.SearchWidget();
            document.body.appendChild(searchWidget.createElement());
            searchWidget.affix(2, 62, "auto", 2);
            searchWidget.adjust();

            actionIndicator = document.createElement("img");
            actionIndicator.src = "/images/ActionIndicator.png";
            actionIndicator.style.position = "fixed";
            actionIndicator.style.right = "250px";
            actionIndicator.style.top = "100px";
            actionIndicator.style.display = "none";
            actionIndicator.style.zIndex = "100";
            document.body.appendChild(actionIndicator);

            searchWidget.onClickItem = function(sender) { loadObject(sender.item.dataObject); };
                        
            setUIComponent("SearchWidget", searchWidget);

            var objectToolbar = new IntuitiveLabs.UI.Toolbar("Object Toolbar");

            about = new IntuitiveLabs.UI.Popup();
            var aboutContent = document.createElement("div");
            aboutContent.innerHTML = "<nobr><b>A.I. Version One (1.2)</b><br/>Copyright© AI N'TH,Inc. 2008-2015.<br/>All rights reserved.</nobr>";
            about.content = aboutContent;
         
            
            loginPopup = new IntuitiveLabs.UI.Popup();
            loginPopup.isClosable = false;

            walk = new IntuitiveLabs.UI.Walk();
            walk.onClickPoint = function(sender) { loadObject(sender.dataObject); }
            element_WalkDisplay.appendChild(walk.createElement());

            callGetDirectories();

            window.onresize = function() { uiWindow.adjust() };
        }                

        function addDependency(sender) {
        
            var x = currentObject;
            var y = sender.item.dataObject;
            
            var predicate = null;
            var subject = null;
            var treeNode = null;
            var isUpper = false;

            if (dependencyTree.currentTree != null) {

                if (!dependencyTree.core.isFocus && dependencyTree.currentTree.currentNode != null && dependencyTree.currentTree.currentNode.dataObject != null) {
                    x = dependencyTree.currentTree.currentNode.dataObject;
                    treeNode = dependencyTree.currentTree.currentNode;
                }

                if (dependencyTree.currentTree == dependencyTree.upperTree) {
                    isUpper = true;
                    subject = x;
                    predicate = y;

                    if (treeNode == null)
                        treeNode = dependencyTree.upperFork;
                }
                else {
                    subject = y;
                    predicate = x;
                    
                    if (treeNode == null)
                        treeNode = dependencyTree.lowerFork;
                }
            }
            else {                
                subject = y;
                predicate = x;
                treeNode = dependencyTree.lowerFork;
            }

            //Have to add a check on the DependencyType
            for (var i = 0; i < treeNode.children.length; i++) {
                if (treeNode.children[i].dataObject != null) {
                    if (isUpper) {
                        if (predicate.ObjectGUID == treeNode.children[i].dataObject.ObjectGUID) {
                            alert("already exists!");
                            return;
                        }
                    }
                    else {
                        if (subject.ObjectGUID == treeNode.children[i].dataObject.ObjectGUID) {
                            alert("already exists!");
                            return;
                        }
                    }
                }

            }

            if (subject.ObjectGUID == predicate.ObjectGUID)
                return;
                
            //Create dependency, send it to the method as target and add it to the dependency tree if successful

            var obj = function() { };
            obj.subject = subject;
            obj.predicate = predicate;
            obj.treeNode = treeNode;
            obj.isUpper = isUpper;
            obj.isDataEddy = false;

            var weight = newDependencyWidget.weight;

            var dataObject = null;

            if (isUpper)
                dataObject = predicate;
            else
                dataObject = subject;

            if ( dataObject.ObjectGUID == currentObject.ObjectGUID || treeNode.formsDataEddy(dataObject, dataComparer)) {
                var conf = confirm(" WARNING:\nYou are about to create a circular dependency.\nAre you ewant to proceed?");

                if (!conf)
                    return;

                obj.isDataEddy = true;
                               
                if (dataObject.ObjectGUID != currentObject.ObjectGUID) {
                    var nodeToCheck = treeNode.getConflux(dataObject, dataComparer);

                    if (nodeToCheck.parent != null && nodeToCheck.parent.dataObject != null && treeNode.dataObject != null) {
                        if(dataComparer(nodeToCheck.parent.dataObject, treeNode.dataObject) )
                            alert("The circular dependency alredy exists.");
                            return;                        
                    }                    
                }
            }

            var message = "\n\nYou are about to add a dependency to an object that can be\nAUTOMATICALLY DELETED AT ANY MOMENT!!!\n\nWould you like to \"freeze\" this object ( i.e. prevent this object from being deleted in the future)?";
            
            if (!subject.IsFrozen) {
                var name = subject.FriendlyName;

                if (name == null)
                    name = subject.ObjectGUID;

                if (confirm(name + message))
                    DataService.FreezeObject(subject.Domain, subject.ObjectClass, subject.ObjectGUID, subject.MIX2Object, onFreezeObject_success, onFail, subject);
            }

            if (!predicate.IsFrozen) {
                var name = predicate.FriendlyName;

                if (name == null)
                    name = predicate.ObjectGUID;

                //alert(predicate.MIX2Object);
                
                if( confirm(name + message) )
                    DataService.FreezeObject(predicate.Domain, predicate.ObjectClass, predicate.ObjectGUID, predicate.MIX2Object, onFreezeObject_success, onFail, predicate);
            }
            
            
            DataService.SetDependency( subject.Domain, "Generic", subject.ObjectClass, subject.ObjectGUID, predicate.ObjectClass, predicate.ObjectGUID, weight, true, onSetDependency_Success, onFail, obj);
        }
        

        function blankPassword() {
            var pwd = form_login.getInput("Password");
            pwd.nullifyValue();
        }                

        function callFindByObjectClass(domain, objectClass, parentlessOnly, position, count, orderBy, direction, target) {
            DataService.FindByObjectClass(domain, objectClass, parentlessOnly, position, count, orderBy, direction, onFind_Success, onFail, target);            
        }

        function callFindByObjectClass_Count(domain, objectClass, parentlessOnly) {
            DataService.FindByObjectClass_Count(domain, objectClass, parentlessOnly, onFind_Count_Success, onFail);
        }

        function callFindByPropertyValue(domain, objectClass, searchValue, parentlessOnly, matchSubstrings, position, count, orderBy, direction, target) {
            DataService.FindByPropertyValue(domain, objectClass, searchValue, matchSubstrings, parentlessOnly, position, count, orderBy, direction, onFind_Success, onFail, target);
        }

        function callFindByPropertyValue_Count(domain, objectClass, searchValue, matchSubstrings, parentlessOnly, target) {
            DataService.FindByPropertyValue_Count(domain, objectClass, searchValue, matchSubstrings, parentlessOnly, onFind_Count_Success, onFail, target);
        }

        
        
        function callGetDirectories() {
            GroupManagement.GetDirectories(onGetDirectoriesSuccess, onFail);
        }
        

        function callGetParentClasses(domain) {
            DataService.GetParentClasses(domain, onGetParentClassesSuccess, onFail);
        }
        
        /*

        function callGetSources(domain, objectClass, objectGUID, caller) {
            DataService.GetSources(domain, objectClass, objectGUID, onGetSourcesSuccess, onFail, caller);
        }
        */
        

        function changeView(viewName) {
            alert(viewName);
        }
        
        function createSignatureElement(node, className) {        
            var props = node.getElementsByTagName("Signature")[0].getElementsByTagName("Property");

            var signatureDiv = document.createElement("div");
            //signatureDiv.style.maxHeight = "20px";
            //signatureDiv.style.overflow = "auto";
            //signatureDiv.style.border = "solid 1px red";

            if (className != null) {
                signatureDiv.className = className;
            }
            //signatureDiv.style.borderBottom = "dashed 1px gray";

            var table = document.createElement("div");
            table.style.fontWeight = "bold";
            //table.style.paddingBottom = "10px";
            table.style.display = "table";

            var col1 = document.createElement("div");
            col1.style.display = "table-column";
            table.appendChild(col1);

            var col2 = document.createElement("div");
            col2.style.display = "table-column";
            table.appendChild(col2);

            for (var i = 0; i < props.length; i++) {

                var row = document.createElement("div");
                row.style.display = "table-row";

                var ncell = document.createElement("div");
                ncell.style.display = "table-cell";
                ncell.innerHTML = props[i].getAttribute("Name") + ": ";
                ncell.style.paddingRight = "10px";
                row.appendChild(ncell);

                var vcell = document.createElement("div");
                vcell.style.display = "table-cell";
                vcell.style.paddingBottom = "5px";

                var vals = props[i].getElementsByTagName("Value");

                for (var j = 0; j < vals.length; j++) {
                    //alert(vals[j].xml);

                    var valSection = document.createElement("div");

                    var valDiv = document.createElement("div");
                    valDiv.style.display = "inline";
                    valDiv.style.fontWeight = "normal";
                    valDiv.style.verticalAlign = "middle";
                    

                    valDiv.innerHTML = IntuitiveLabs.UI.DOM.getValue(vals[j]);
                    
                    /*
                    if (vals[j].text != undefined) {
                    valDiv.innerHTML = vals[j].text;
                    }
                    else {
                    valDiv.innerHTML = vals[j].childNodes[0].nodeValue;
                    }
                    */

                    valSection.appendChild(valDiv);
                    DataService
                    var iconTray = new AIV1.UI.PropertyStateTray(vals[j]);
                    valSection.appendChild(iconTray.createElement());
                    iconTray.refresh();

                    valSection.AIV1_icontray = iconTray;
                    
                    /*
                    var frozenIcon = document.createElement("img");
                    frozenIcon.style.paddingLeft = "10px";
                    frozenIcon.style.verticalAlign = "middle";

                    if (vals[j].getAttribute("IsFrozen") == true) {
                    frozenIcon.src = "/images/icon16-SnowFlake.png";
                    }
                    else {
                    frozenIcon.src = "/images/icon16-Flash-Yellow.png";
                    }
                    
                    valSection.appendChild(frozenIcon);
                    */

                    vcell.appendChild(valSection);
                    
                }

                row.appendChild(vcell);

                table.appendChild(row);

                //div.appendChild(table);
            }

            //signatureDiv.style.border = "solid 15px red";
            signatureDiv.appendChild(table);

            return signatureDiv;

        }

        function createSourceElement(objectClass, guid) {
            var table = document.createElement("div");
            table.style.display = "table";

            var row = document.createElement("div");
            row.style.display = "table-row";
            
            var sourceCell = document.createElement("div");
            sourceCell.style.display = "table-cell";
            sourceCell.style.fontSize = "10px";
            sourceCell.style.color = "#777777";

            sourceCell.innerHTML = "Source:"
            row.appendChild(sourceCell);

            var getCell = document.createElement("div");
            getCell.style.display = "table-cell";

            //Make getDIV from IntuitiveLabs UI component
            var getDiv = document.createElement("div");
            //getDiv.style.display = "table-cell";
            getDiv.innerHTML = "GET";
            getDiv.style.color = "#AAAAAA";
            getDiv.style.border = "solid 1px #AAAAAA";
            getDiv.style.verticalAlignment = "middle"
            getDiv.style.fontSize = "9px";
            //getDiv.style.paddingBottom = "2px";
            getDiv.style.marginLeft = "5px";
            //getDiv.style.width = "24px";
            //getDiv.style.height = "11px";
            getDiv.style.overflow = "hidden";
            getDiv.style.textAlign = "center";
            getDiv.style.verticalAlign = "middle";
            getDiv.intuitivelabs_parent = this;
            getDiv.style.cursor = "pointer";
            getDiv.mix2_objectClass = objectClass;
            getDiv.mix2_guid = guid;

            getDiv.onclick = function() {
                DataService.GetSources("IPNetworking", objectClass, guid, onGetSourcesForSingleChildSuccess, onFail, this);
            }

            getCell.appendChild(getDiv);

            row.appendChild(getCell);

            table.appendChild(row);

            return table;
        }
        
        function dataComparer(a, b) { 
            return (a.ObjectGUID == b.ObjectGUID);
        }

        function exportData() {
            window.location = "https://localhost:10043/Export/{csv}";
            
            /*
            if (AIV1.UI.CountCache.dataObject != null) {
                if( AIV1.UI.CountCache.dataType == "Property")
                    DataService.StoreValueCount(AIV1.UI.CountCache.dataObject, onExportData_Success);
                else if( AIV1.UI.CountCache.dataType == "Category")
                    DataService.StoreCategory(AIV1.UI.CountCache.dataObject, onExportData_Success);
            }
            */
        }
                

        function onSuccess(result) {        
            alert(result);
        }

        function callLogOff() {
            Authenticator.LoggOff(onLogOffSuccess, onFail);
        }

        function FindObjects(sender) {
            //alert("it's been called");            
            DataService.FindByPropertyValue("IPNetworking", "Node", sender.value, true, true, 1, 100, "FriendlyName", 0, onFind_Success_2, onFail, sender);
        }

        function loadDependencyInformation(sender) {
            currentPanelName = "Dependencies";
            DataService.GetDependencyInformation(currentObject.Domain, currentObject.ObjectClass, currentObject.ObjectGUID, 0, -1, null, 0, onGetDependencyInformation_Success, onFail, sender );
        }

        function loadObject(dataObject) {
            element_ObjectPanel_Load.style.display = "block";

            newDependencyWidget.hide();

            if (dataObject.FriendlyName != dataObject.ObjectGUID)
                element_Object_Name.innerHTML = dataObject.FriendlyName;
            else
                element_Object_Name.innerHTML = "";

            element_Object_GUID.innerHTML = "[" + dataObject.ObjectGUID + "]";
            element_Object_Info.innerHTML = "Confirmed " + dataObject.Confirmed;

            DataService.GetAspects(dataObject.Domain, dataObject.ObjectClass, onGetAspectsSuccess, onFail, dataObject);            
        }

        function onDeleteDependency_Success(result, target) {
            //alert(target.node.dataObject.ObjectGUID + " has been removed");
                           
            var parent = target.node.parent;
            //var tree = parent.getTree();

            parent.removeChild(target.node);
            parent.refresh();
        }

        function onFail(err, target) {
            var str = "";

            if (target != null) {
                for (var n in target.property) {
                    str += target.property[n].toString() + "\n";
                }
            }
            
            alert(err.get_message() + "\n" + str);
            //callLogOff();
        }

        function onCalculateHealthSuccess(result) {
            element_HealthPanel_GlobalChart.src = "HealthCylinder.aspx?t=GLOBAL&d=" + result;
        }

        function onExportData_Success(result) {
            //alert(result);
            window.location = "https://localhost:10043/Export" + result.toString();
        }
        
        function onFreezeObject_success(result, target) {
            target.IsFrozen = true;

            if (target.ObjectGUID == currentObject.ObjectGUID) {

                for (var i = 2; i < signatureElement.firstChild.childNodes.length; i++) {

                    for (var j = 0; j < signatureElement.firstChild.childNodes[i].childNodes[1].childNodes.length; j++) {
                        var obj = signatureElement.firstChild.childNodes[i].childNodes[1].childNodes[j].AIV1_icontray;
                        obj.dataObject.setAttribute("IsFrozen", 1);
                        obj.refresh();
                    }
                }
            }

            objectToolbar.refresh();                        
        }

        function onFreezePropertyValue_success(result, target) {
            var propClass = target.parent.dataObject.column;
            var node = target.parent.dataObject.value;

            node.setAttribute("IsFrozen", 1);
            target.parent.refresh();

            currentObject.IsFrozen = true;
            objectToolbar.refresh();            
        }

        function onGetSuccess(result) {
            currentObject = result;
            
            /*
            for (var name in result) {
                alert(name);
            }
            */

            //alert(result.AIObject);
            var node = IntuitiveLabs.Xml.Load(result.AIObject);
            //alert(node.xml);

            element_ObjectPanel_Load.style.display = "none";

            var signatureDiv = createSignatureElement(node, "Signature");
            signatureDiv.style.overflow = "auto";
            signatureDiv.style.maxHeight = "250px";
            signatureElement = signatureDiv;

            var viewerDiv = document.createElement("div");

            //alert(result.ObjectClass);
            var sourceDiv = createSourceElement(result.ObjectClass, result.ObjectGUID);
            sourceDiv.style.paddingBottom = "15px";
            
            //viewerDiv.style.height = "300px";
            
            panelGroup = new IntuitiveLabs.UI.PanelGroup();
            
            var navTemplate = new IntuitiveLabs.UI.StyleTemplate();            
    	    navTemplate.setProperty("backgroundColor", "#E9E9E9");
    	    navTemplate.setProperty("padding", "5px");
    	    navTemplate.setProperty("border", "solid 1px #AAAAAA");
    	    panelGroup.navigationTemplate = navTemplate;

            var selectedTemplate = new IntuitiveLabs.UI.StyleTemplate();
    	    selectedTemplate.setProperty("backgroundColor", "#FAFAFA");
    	    selectedTemplate.setProperty("padding", "5px");
    	    selectedTemplate.setProperty("border", "solid 1px #AAAAAA");
    	    selectedTemplate.setProperty("borderBottom", "none");
    	    panelGroup.selectedTemplate = selectedTemplate;

            var panelTemplate = new IntuitiveLabs.UI.StyleTemplate();
    	    panelTemplate.setProperty("backgroundColor","#FAFAFA");
    	    panelTemplate.setProperty("padding", "5px");
    	    panelTemplate.setProperty("border", "solid 1px #AAAAAA");
    	    panelTemplate.setProperty("borderTop", "none");
    	    panelGroup.panelTemplate = panelTemplate;

    	    var contentTemplate = new IntuitiveLabs.UI.StyleTemplate();
    	    contentTemplate.setProperty("backgroundColor", "#FFFFFF");
    	    contentTemplate.setProperty("padding", "5px");
    	    contentTemplate.setProperty("border", "solid 1px #AAAAAA");
    	    panelGroup.contentTemplate = contentTemplate;

    	    panelGroup.onSelect = function(sender) {
    	        //alert(sender);
    	        currentPanelName = sender.id;
    	    };

            //Details Panel
            var panel1 = new IntuitiveLabs.UI.Panel("Details", "Details");
            panelGroup.addPanel(panel1);
            
            var detailsToolbar = new IntuitiveLabs.UI.Toolbar("Details");
            var aspectSelector = new IntuitiveLabs.UI.Dropdown("Aspect");
            aspectSelector.id = "Aspect";
            aspectSelector.addOption("00_null", "All", null);

            aspectSelector.onChange = function(sender) {
                var aspect = null;

                if (sender.value != null) {
                    for (var i = 0; i < aspects.length; i++) {
                        if (aspects[i].Name == sender.value) {
                            aspect = aspects[i];
                            break;
                        }
                    }
                }

                for (var i = 0; i < childClassElements.count(); i++) {
                    var childClassElement = childClassElements.entries[i];

                    if (aspect != null) {
                        childClassElement.value.style.display = "none";

                        for (var j = 0; j < aspect.ObjectClasses.length; j++) {
                            if (childClassElement.key == aspect.ObjectClasses[j]) {
                                childClassElement.value.style.display = "block";
                            }
                        }
                    }
                    else {
                        childClassElement.value.style.display = "block";
                    }
                }

            };
            
            detailsToolbar.addItem("Aspect", aspectSelector);
            panel1.addToolbar("Details",detailsToolbar);
            
            
            //var panel3 = new IntuitiveLabs.UI.Panel("Something", "Something");
            //panelGroup.addPanel(panel3);
            
            var detailsDiv = document.createElement("div");

            var childrenNodes = node.getElementsByTagName("Children");

            //var dict = new IntuitiveLabs.Collections.Dictionary();

            childClassElements.clear();

            if (childrenNodes.length > 0) {

                var childClasses = node.getElementsByTagName("Children")[0].getElementsByTagName("ObjectClass");

                var aspectsDictionary = new IntuitiveLabs.Collections.Dictionary();

                if (aspects != null) {
                    for (var i = 0; i < aspects.length; i++) {

                        var aspect = aspects[i];

                        var exists = false;

                        for (var j = 0; j < aspect.ObjectClasses.length; j++) {

                            for (var k = 0; k < childClasses.length; k++) {

                                var childClassName = childClasses[k].getAttribute("Name");

                                if (aspect.ObjectClasses[j] == childClassName) {
                                    aspectsDictionary.add(aspect.Name, aspect);
                                    exists = true;
                                    break;
                                }
                            }

                            if (exists)
                                break;
                        }

                    }
                }                                

                for (var i = 0; i < childClasses.length; i++) {

                    //var childDomain = childClasses[i].getAttribute("Domain");
                    var childDomain = "IPNetworking";
                    var childClassName = childClasses[i].getAttribute("Name");
                    
                    var objClassDiv = document.createElement("div");
                    
                    var nameDiv = document.createElement("div");
                    nameDiv.style.display = "table";

                    var nameRow = document.createElement("div");
                    nameRow.style.display = "table-row";

                    var nameCell = document.createElement("div");
                    nameCell.style.display = "table-cell";   
                    nameCell.style.paddingRight = "10px";                                     
                    nameCell.innerHTML = childClassName;
                    nameCell.style.fontWeight = "bold";
                    nameCell.style.fontSize = "12px";
                    nameCell.style.color = "#666666";
                    nameCell.style.paddingBottom = "10px";

                    nameRow.appendChild(nameCell);
                    nameDiv.appendChild(nameRow);
                    
                    objClassDiv.style.paddingBottom = "20px";
                    objClassDiv.style.paddingTop = "20px";
                    objClassDiv.style.borderBottom = "dotted 1px #EFEFEF";
                    
                    objClassDiv.appendChild(nameDiv);                    
                    

                    var childObjects = childClasses[i].getElementsByTagName("Object");

                    if (childObjects.length == 1) {
                        var sourceCell = document.createElement("div");
                        sourceCell.style.display = "table-cell";
                        sourceCell.appendChild(createSourceElement(childClassName, childObjects[0].getAttribute("ObjectGUID")));
                        nameRow.appendChild(sourceCell);
                        
                        var childSignature = createSignatureElement(childObjects[0], "ChildSignature");
                        objClassDiv.appendChild(childSignature);
                    }
                    else {

                        var dataTable = new IntuitiveLabs.Data.Table();
                        //dataTable.createColumn("a", null);
                        //dataTable.createColumn("b", null);
                        //dataTable.createColumn("c", null);
                        //dataTable.createColumn("d", "HOOPLA");


                        var grid = new IntuitiveLabs.UI.Grid();
                        grid.dataTable = dataTable;
                        grid.headerClass = "DataTable_Header";
                        grid.tableClass = "DataTable";
                        grid.rowClass = "DataTable_Row";

                        dataTable.createColumn("MIX2_Domain");
                        dataTable.createColumn("MIX2_ObjectClass");
                        dataTable.createColumn("MIX2_ObjectGUID");
                        dataTable.createColumn("MIX2_IsFrozen");
                        
                        dataTable.createColumn("MIX2_FriendlyName");
                        grid.createColumn("MIX2_FriendlyName", "Object", "MIX2_FriendlyName");                        
                        
                        for (var j = 0; j < childObjects.length; j++) {
                            
                            var childGUID = childObjects[j].getAttribute("ObjectGUID");

                            var friendlyNameNode = childObjects[j].getElementsByTagName("FriendlyName");
                            var childFriendlyName;

                            if (friendlyNameNode.length > 0)
                                childFriendlyName = IntuitiveLabs.UI.DOM.getValue(friendlyNameNode[0]);
                            else
                                childFriendlyName = childGUID;
                            
                            var row = dataTable.createRow(childDomain, childClassName, childGUID, childObjects[j].getAttribute("IsFrozen"), childFriendlyName);
                            
                            var childProps = childObjects[j].getElementsByTagName("Property");

                            for (var k = 0; k < childProps.length; k++) {
                                var childProp = childProps[k];
                                var childPropName = childProp.getAttribute("Name");

                                if (!dataTable.columns.contains(childPropName)) {
                                    dataTable.createColumn(childPropName);
                                    var col = grid.createColumn(childPropName, childPropName, childPropName);

                                    col.getValue = function(obj) {
                                        if (obj.value != undefined)
                                            return IntuitiveLabs.UI.DOM.getValue(obj.value);
                                        else
                                            return undefined;
                                    }

                                    var iconTray = new IntuitiveLabs.UI.IconTray();

                                    iconTray.refresh = function() {

                                        if (this.dataObject.value != null) {
                                            var isFrozen = this.dataObject.value.getAttribute("IsFrozen");
                                            //alert(isFrozen);

                                            if (isFrozen == true) {
                                                this.hideIcon("Unfrozen");
                                                this.showIcon("Frozen");
                                            }
                                            else {
                                                this.showIcon("Unfrozen");
                                                this.hideIcon("Frozen");
                                            }
                                        }

                                        //alert(this.element.style.display);
                                    }
                                    
                                    var unfrozenIcon = new IntuitiveLabs.UI.Icon("/images/icon16-Flash-Yellow.png");
                                    
                                    unfrozenIcon.onClick = function(sender) {
                                        //alert(sender.parent.dataObject.row.getValue("MIX2_ObjectGUID") + "\n" + sender.parent.dataObject.column + ": " + sender.parent.parent.getValue());

                                        var domain = sender.parent.dataObject.row.getValue("MIX2_Domain");
                                        var objClass = sender.parent.dataObject.row.getValue("MIX2_ObjectClass");
                                        var objGUID = sender.parent.dataObject.row.getValue("MIX2_ObjectGUID");
                                        var propClass = sender.parent.dataObject.column;
                                        var propValue = sender.parent.parent.getValue();

                                        DataService.FreezePropertyValue(domain, objClass, objGUID, propClass, propValue, onFreezePropertyValue_success,onFail, sender);
                                    };
                                    
                                    iconTray.addIcon("Unfrozen", unfrozenIcon);

                                    var frozenIcon = new IntuitiveLabs.UI.Icon("/images/icon16-SnowFlake.png");
                                    
                                    frozenIcon.onClick = function(sender) {
                                        //alert(sender.parent.dataObject.row.getValue("MIX2_ObjectGUID") + "\n" + sender.parent.dataObject.column + ": " + sender.parent.parent.getValue());
                                        var domain = sender.parent.dataObject.row.getValue("MIX2_Domain");
                                        var objClass = sender.parent.dataObject.row.getValue("MIX2_ObjectClass");
                                        var objGUID = sender.parent.dataObject.row.getValue("MIX2_ObjectGUID");
                                        var propClass = sender.parent.dataObject.column;
                                        var propValue = sender.parent.parent.getValue();

                                        DataService.UnfreezePropertyValue(domain, objClass, objGUID, propClass, propValue, onUnfreezePropertyValue_success, onFail, sender);                                        
                                    };
                                    
                                    iconTray.addIcon("Frozen", frozenIcon);

                                    col.iconTray_cell = iconTray;
                                }

                                var childPropVals = childProp.getElementsByTagName("Value");

                                var val = new Array();

                                for (var m = 0; m < childPropVals.length; m++) {
                                    val.push(childPropVals[m]);
                                }

                                row.setValue(childPropName, val);
                            }
                        }

                      
                        dataTable.createColumn("MIX2_Source");
                        
                        var sourceCol = new IntuitiveLabs.UI.GridDataOnDemandColumn("MIX2_Source", "Source", "MIX2_Source", grid);
                            sourceCol.onclick = function(sender) {
                            var dataRow = sender.row.dataRow;

                            DataService.GetSources("IPNetworking", dataRow.getValue("MIX2_ObjectClass"), dataRow.getValue("MIX2_ObjectGUID"), onGetSourcesForChildSuccess, onFail, sender);
                            //sender.setValue("test");
                        };
                        grid.addColumn(sourceCol);
                                                
                        objClassDiv.appendChild(grid.createElement());
                        grid.sort("MIX2_FriendlyName");
                    }

                    childClassElements.add(childClassName, objClassDiv);

                    detailsDiv.appendChild(objClassDiv);
                }

                for (var i = 0; i < aspectsDictionary.count(); i++) {
                    var aspect = aspectsDictionary.entries[i].value;                    

                    aspectSelector.addOption(aspect.Name, aspect.Name, aspect.Name);
                }
            }

            panel1.content = detailsDiv;

            //Dependencies Panel
            var panel2 = new IntuitiveLabs.UI.Panel("Dependencies", "Dependencies");
            panel2.onSelect = loadDependencyInformation;
            panel2.onUnselect = function(sender) {
                newDependencyWidget.hide();
                element_ObjectPanel_Toolbar.style.paddingRight = "2px";
            }

            var toolbar1 = new IntuitiveLabs.UI.Toolbar("Dependencies");
            var item1 = new IntuitiveLabs.UI.ToolbarItem("Add");
    
            item1.onClick = function(sender) {
                var obj = null;

                if (!dependencyTree.core.isFocus && dependencyTree.currentTree != null) {
                    obj = dependencyTree.currentTree.currentNode;
                    //alert(obj.text);
                }

                IntuitiveLabs.UI.DOM.alterRight(element_ObjectPanel, 225);
                uiWindow.adjust();

                newDependencyWidget.show();

                actionIndicator.style.display = "block";
                actionIndicator.style.right = "249px";
                element_ObjectPanel_Toolbar.style.paddingRight = "18px";

                var rel = dependencyTree.core.element;

                if (dependencyTree.currentTree != null && dependencyTree.currentTree.currentNode != null) {
                    rel = dependencyTree.currentTree.currentNode.element;
                }

                var pos = IntuitiveLabs.UI.DOM.getCummulativeOffset(rel);

                var top = pos.top - panelGroup.selected.contentElement.scrollTop;

                actionIndicator.style.top = pos.top.toString() + "px";

            };
            
            var item2 = new IntuitiveLabs.UI.ToolbarItem("Remove");

            item2.onClick = function(sender) {
                if (dependencyTree != null && !dependencyTree.coreIsFocus && dependencyTree.currentTree != null && dependencyTree.currentTree.currentNode != null) {
                    

                    var node = dependencyTree.currentTree.currentNode;
                    var x = node.dataObject;
                    var y = (node.parent != null && node.parent.dataObject != null) ? node.parent.dataObject : currentObject;

                    if (dependencyTree.currentTree == dependencyTree.upperTree) {
                        predicate = x;
                        subject = y;
                    }
                    else {
                        subject = x;
                        predicate = y;
                    }

                    //alert(subject.ObjectClass + ";" + subject.ObjectGUID + ";" + predicate.ObjectClass + ";" + predicate.ObjectGUID);

                    var obj = function() { };
                    obj.tree = dependencyTree.currentTree;
                    obj.node = node;

                    //alert("Delete: " + subject.Domain + ";Generic;" + subject.ObjectClass + ";" + subject.ObjectGUID + ";" + predicate.ObjectClass + ";" + predicate.ObjectGUID);

                    DataService.DeleteDependency(subject.Domain, "Generic", subject.ObjectClass, subject.ObjectGUID, predicate.ObjectClass, predicate.ObjectGUID, onDeleteDependency_Success, onFail, obj);
                }
            }
            
            toolbar1.addItem("RemoveDependency", item2);
            toolbar1.addItem("AddDependency", item1);
            
            panel2.addToolbar("Dependencies",toolbar1);
            panelGroup.addPanel(panel2);
            
            IntuitiveLabs.UI.DOM.removeChildren(element_ObjectPanel_Viewer);
            panelGroup.render(viewerDiv);

            panelGroup.selectPanel(currentPanelName);

            IntuitiveLabs.UI.DOM.removeChildren(element_ObjectPanel_Viewer);
            IntuitiveLabs.UI.DOM.removeChildren(element_ObjectPanel_Toolbar);

            objectToolbar = new IntuitiveLabs.UI.Toolbar("Object Toolbar");
            objectToolbar.dataObject = result;

            objectToolbar.refresh = function() {
                if (this.dataObject.IsFrozen == true) {
                    this.hideItem("Unfrozen");
                    this.showItem("Frozen");
                }
                else {
                    this.showItem("Unfrozen");
                    this.hideItem("Frozen");
                }
            }
            
            var frozen = new IntuitiveLabs.UI.ToolbarItem("Frozen","/images/icon16-SnowFlake.png");

            frozen.onClick = function(sender) {
                if( confirm("Warning!!! Warning!!! Warning!!!\n\nYou are about to unfreeze an object that may have user-defined property values and dependencies. Unfreezing the object may eventually result in the complete loss of all user-defined information.\n\nAre you really sure you want to proceed?") )
                    DataService.UnfreezeObject(currentObject.Domain, currentObject.ObjectClass, currentObject.ObjectGUID, currentObject.MIX2Object, onUnfreezeObject_success, onFail, currentObject);
            }

            objectToolbar.addItem("Frozen", frozen);

            var unfrozen = new IntuitiveLabs.UI.ToolbarItem("Dynamic", "/images/icon16-Flash-Yellow.png");

            unfrozen.onClick = function(sender) {
                DataService.FreezeObject(currentObject.Domain, currentObject.ObjectClass, currentObject.ObjectGUID, currentObject.MIX2Object, onFreezeObject_success, onFail, currentObject);
            }

            objectToolbar.addItem("Unfrozen", unfrozen);
            objectToolbar.refresh();
            
            //objectToolbar.styleTemplate.setProperty("paddingBottom","15px");
            //objectToolbar.styleTemplate.setProperty("border", "solid 1px #AEAEAE");
            //objectToolbar.styleTemplate.setProperty("display", "block");
            
            element_ObjectPanel_Toolbar.appendChild(objectToolbar.createElement());
            element_ObjectPanel_Viewer.appendChild(sourceDiv);
            element_ObjectPanel_Viewer.appendChild(signatureDiv);
            element_ObjectPanel_Viewer.appendChild(viewerDiv);

            objectToolbar.refresh();

            panelGroup.adjust();

            setUIComponent("ObjectViewer", panelGroup);

            showObjectPanel();                                
        }

        function onGetAspectsSuccess(result, caller) {
            //alert(result);
            aspects = result;
            /*DataService.GetSources(caller.Domain, caller.ObjectClass, objectGUID, onGetSourcesSuccess, onFail, caller);*/
            DataService.Get(caller.Domain, caller.ObjectClass, caller.ObjectGUID, "<Object><Children/></Object>", onGetSuccess, onFail);
        }

        function onGetCube_Success(result) {
            alert(result.catalogs.length);
        }
        

        function onGetDependencyInformation_Success(result, caller) {

            var tree = new IntuitiveLabs.UI.BifurcatedTree();
            tree.dataComparer = dataComparer;

            tree.onInitiateNode = function(sender) {
                DataService.GetDependencyInformation(sender.dataObject.Domain, sender.dataObject.ObjectClass, sender.dataObject.ObjectGUID, 0, -1, null, 0, onGetDependencyInformation_ForNode_Success, onFail, sender);
            }

            tree.onSelectNode = function(sender) {
                var pos = IntuitiveLabs.UI.DOM.getCummulativeOffset(sender.element);

                var scroll = panelGroup.selected.contentElement.scrollTop;
                var top = pos.top - scroll;

                var height = panelGroup.selected.contentElement.offsetHeight;

                actionIndicator.style.top = top.toString() + "px";
            }

            tree.onDoubleClickNode = function(sender) {
                walk.addPoint(sender.dataObject.FriendlyName, sender.dataObject);
                loadObject( sender.dataObject );
            }                                               
            
            if( currentObject.FriendlyName != null )
                tree.core.text = currentObject.FriendlyName;
            else
                tree.core.text = currentObject.ObjectGUID;

            tree.core.dataObject = currentObject;                     

            //tree.onClick = function(node) { alert(node.text) };
            tree.upperTree.defaultIcon = "/images/Torus-B.png";
            tree.lowerTree.defaultIcon = "/images/Torus-B.png";
            

            if (result.Dependants.length == 1)
                tree.upperText = "depends on";
            else
                tree.upperText = "depend on";

            if (currentObject.ObjectClass == "User")
                tree.lowerText = "who depends on";
            else
                tree.lowerText = "which depends on";
            
            if (result.Dependants.length > 0) {
                for (var i = 0; i < result.Dependants.length; i++) {
                    var dep = result.Dependants[i];

                    var text = dep.ObjectGUID;

                    if (dep.FriendlyName != null)
                        text = dep.FriendlyName;


                    text = text;

                    var node = new IntuitiveLabs.UI.TreeNode(text, "[" + dep.ObjectClass +  "]");
                    node.dataObject = dep;
                    node.isInitiated = false;
                    
                    if (dep.State < 0.7854) {
                        if (dep.State < 0.2146)
                            node.icon = "images/Torus-R.png";
                        else
                            node.icon = "images/Torus-Y.png";
                    }
                    else
                        node.icon = "images/Torus-G.png";
                    
                    
                    tree.upperFork.addChild(node);
                }
            }
            else {
                var addNode = new IntuitiveLabs.UI.TreeNode("none (click to add)");
                //addNode.onClick = function(node){ alert("Add dependency:" + node.text) };
                tree.upperFork.addChild( addNode );
            }

            if (result.Dependencies.length > 0) {
                for (var i = 0; i < result.Dependencies.length; i++) {
                    var dep = result.Dependencies[i];

                    var text = dep.ObjectGUID;

                    if (dep.FriendlyName != null)
                        text = dep.FriendlyName;

                    text = text;

                    var node = new IntuitiveLabs.UI.TreeNode(text, "[" + dep.ObjectClass + "]");
                    node.dataObject = dep;
                    node.isInitiated = false;

                    if (dep.State < AIV1.UI.state.healthy ) {
                        if (dep.State < AIV1.UI.state.unhealthy)
                            node.icon = "images/Torus-R.png";
                        else
                            node.icon = "images/Torus-Y.png";
                    }
                    else
                        node.icon = "images/Torus-G.png";
                    
                    
                    tree.lowerFork.addChild(node);
                }
            }
            else {
                var addNode = new IntuitiveLabs.UI.TreeNode("none (click to add)");
                //addNode.onClick = function(node){ alert("Add dependant:" + node.text) };
                tree.lowerFork.addChild(addNode);
            }

            dependencyTree = tree;                        
            
            var centerElement = document.createElement("div");
            centerElement.align = "center";
            centerElement.style.marginTop = "30px";

            centerElement.appendChild(tree.createElement());

            caller.setContent(centerElement);
            tree.adjust();

            setUIComponent("DependencyTree", dependencyTree);

            dependencyTree.upperFork.expand();
            dependencyTree.lowerFork.expand();
        }

        function onGetDependencyInformation_ForNode_Success(result, node) {

            var tree = node.getTree();
            var children = null;

            if (tree == dependencyTree.upperTree) {
                //alert("Is upper");
                children = result.Dependants;
            }
            else {
                children = result.Dependencies;
            }

            for (var i = 0; i < children.length; i++) {
                var text = children[i].FriendlyName;
                
                if( text == null )
                    text = children[i].ObjectGUID;

                
                var childNode = new IntuitiveLabs.UI.TreeNode(text,"[" + children[i].ObjectClass + "]");
                childNode.dataObject = children[i];
                
                if (node.formsDataEddy(children[i], function(a, b) { return (a.ObjectGUID == b.ObjectGUID) ? true : false; ; }))
                    childNode.isInitiated = true;
                else
                    childNode.isInitiated = false;

                node.addChild(childNode);
            }

            node.isInitiated = true;
            node.refresh();
        }
        
        function onGetDirectoriesSuccess(result) {

            var drop = new IntuitiveLabs.UI.Dropdown("Directory");            

            //IntuitiveLabs.UI.DOM.removeChildren(element_DirectorySelector);

            for (var i = 0; i < result.length; i++) {
                //var option = document.createElement("OPTION");
                //option.value = result[i].guid;
                //option.innerHTML = result[i].identifier;
                //element_DirectorySelector.appendChild(option);

                drop.addOption(result[i].GUID, result[i].Identifier, result[i].Identifier);
            }

            drop.addOption("0", "[internal]", null);

            if (!isLoggedIn) {
                form_login.addInput("Directory", drop);

                loginPopup.content = form_login.createElement()
            }

            //element_DirectorySelector.options[0].selected = true;

            loginPopup.render();
            loginPopup.hide();
            about.render();
            about.hide();

            Authenticator.IsLoggedIn(onIsLoggedInSuccess, onFail);
        }


        function onGetParentClassesSuccess(result, caller) {

            if (caller != null) {
                //alert(caller.element.innerHTML);

                caller.options.clear();

                for (var i = 0; i < result.length; i++) {
                    caller.addOption(result[i], result[i], result[i]);
                }

                caller.refresh();

                /*
                if (caller.parent != null)
                    caller.parent.findObjects();
                */
            }

            else {
                //alert("is null");
            }

            /*
                IntuitiveLabs.UI.DOM.removeChildren(element_CatalogSelector);

                for (var i = 0; i < result.length; i++) {
                    var option = document.createElement("OPTION");
                    option.value = result[i];
                    option.innerHTML = result[i];
                    element_CatalogSelector.appendChild(option);
                }

                if (result.length > 0) {
                    objectClass = result[0];
                    refreshObjectList();

                }
            }
            */

        }
        

        function onGetSourcesSuccess(result, test) {
            //alert(test);
            for (var i = 0; i < result.Sources.length; i++) {                
                //alert(result.Sources[i].Nickname);
            }
        }

        function onGetSourcesForChildSuccess(result, caller) {
            var vals = new Array(result.Sources.length);

            for (var i = 0; i < result.Sources.length; i++) {
                vals[i] = result.Sources[i].Nickname;
            }

            caller.initializeValue(vals);
        }
        
        function onGetSourcesForSingleChildSuccess(result, caller) {

            IntuitiveLabs.UI.DOM.removeChildren(caller);

            caller.style.border = "none";
            caller.style.color = "#777777";

            for (var i = 0; i < result.Sources.length; i++) {
                caller.innerHTML += result.Sources[i].Nickname;

                if (i < (result.Sources.length - 1))
                    caller.innerHTML += ", ";
            }                                    
        }

        function onGetViewsSuccess(result) {
            IntuitiveLabs.UI.DOM.removeChildren(element_View_Selector);

            if (result.length > 1) {
                for (var i = 0; i < result.length; i++) {
                    //alert(result[i]);
                    var option = document.createElement("option");
                    option.value = result[i];
                    option.innerHTML = result[i];

                    element_View_Selector.appendChild(option);
                } 
                
                element_View_Container.style.display = "table";
            }
            else{
                element_View_Container.style.display = "none";
            }
        }
        
        function onIsLoggedInSuccess(result) {
            isLoggedIn = result;
            
            if (!result) {
                element_LogOut.style.visibility = "hidden";
                loginPopup.show();
            }
            else {
                loginPopup.hide();
                element_LogOut.style.visibility = "visible";

                
                //callGetParentClasses("IPNetworking");
                var catSelect = searchWidget.getObject("Filter");
                DataService.GetParentClasses("IPNetworking", onGetParentClassesSuccess, onFail, catSelect);
                DataService.CalculateHealth("IPNetworking", new Array(), new Array(), true, onCalculateHealthSuccess, onFail);

                addPrefilters();

                //cubeWidget.getCube();
                //searchWidget.getCube()
                searchWidget.search();

                //CUBIT
                //catalogsMenu = AIV1.UI.CubeMenu.create(element_Catalogs);
                //propertiesMenu = AIV1.UI.CubeMenu.createDynamically(element_Properties);                            
                
                Authenticator.IsAdmin(onIsAdminSuccess, onFail);
                DataService.GetViews(onGetViewsSuccess, onFail);
               
            }

        }

        function onIsAdminSuccess( isAdmin) {
            //alert(role);

            if (isAdmin) {
                element_Management.style.visibility = "visible";
                element_Support.style.visibility="visible";
            }
            else{
                element_Management.style.visibility = "hidden";
                element_Support.style.visibility="hidden";
            }

        }

        function createFilter(text, paths) {            
            var item = new IntuitiveLabs.UI.MenuItem(text);
            
            var filter = new IntuitiveLabs.Collections.Dictionary();

            for (var i = 0; i < paths.length; i++) {
                var cat = new MIX2.Data.Cube.Category();
                cat.Path = paths[i];
                
                var filterItem = new AIV1.FilterItem(paths[i].replace(/\./g, " &gt; "), cat, false);
                filter.add(filterItem.category.Path, filterItem)
            }

            item.onClick = function(sender) {
                searchWidget.clearFilter();
                searchWidget.loadFilter(sender.dataObject);
            }

            item.dataObject = filter;

            return item;            
        }

        function addPrefilters() {
            /*
            prefilterMenu = new IntuitiveLabs.UI.Menu();

            var invent = createFilter("Inventory", ["Type.Node"])
            var invMenu = new IntuitiveLabs.UI.Menu();
            
            invMenu.addItem(createFilter("Asia", ["Type.Node", "Location (geographic).Asia"]));
            invMenu.addItem(createFilter("Africa", ["Type.Node", "Location (geographic).Africa"]));

            var eu = createFilter("Europe", ["Type.Node", "Location (geographic).Europe"]);
            var euMenu = new IntuitiveLabs.UI.Menu();
            euMenu.addItem(createFilter("France", ["Type.User", "Location (political).European Union.France"]));
            euMenu.addItem(createFilter("United Kingdom", ["Type.User", "Location (political).European Union.United Kingdom"]));
            eu.setSubMenu(euMenu);
            invMenu.addItem(eu);
            
            invMenu.addItem(createFilter("North America", ["Type.Node", "Location (geographic).North America"]));
            invMenu.addItem(createFilter("South America", ["Type.Node", "Location (political).South America"]));
            
            invent.setSubMenu(invMenu);

            prefilterMenu.addItem(invent);
            prefilterMenu.addItem(createFilter("Software", ["Software"]));
            prefilterMenu.addItem(createFilter("User", ["Type.User"]));
            
            var div = prefilterMenu.createElement();
            element_Prefilters.appendChild(div);

            prefilterMenu.adjust();

            prefilterMenu.hide();
            */
        }

        function loadFilter(paths) {
            var filter = new IntuitiveLabs.Collections.Dictionary();

            for (var i = 0; i < paths.length; i++) {
                var cat = { Path: paths[i] };

                var filterItem = new AIV1.FilterItem(paths[i].replace(/\./g, " &gt; "), cat, false);
                filter.add(filterItem.category.Path, filterItem)
            }

            searchWidget.clearFilter();
            searchWidget.loadFilter(filter);
        }

        function onLogin(sender) {
            loginPopup.hideMessage();
            sender.showWaitImage();

            var dir = sender.getInput("Directory").current.value;
            var uname = sender.getInput("Username").value;
            var pwd = sender.getInput("Password");

            if (dir != null)
                Authenticator.LoginAgainstDirectory(dir, uname, pwd.value, onLoginSuccess, onFail);
            else
                Authenticator.Login(uname, pwd.value, onLoginSuccess, onFail);
        }

        function onLoginSuccess() {
            blankPassword();

            isLoggedIn = true;

            form_login.hideWaitImage();
            loginPopup.hide();

            element_LogOut.style.visibility = "visible";

            addPrefilters();
            
            var catSelector = searchWidget.getObject("Filter");
            DataService.GetParentClasses("IPNetworking", onGetParentClassesSuccess, onFail, catSelector)
            DataService.CalculateHealth("IPNetworking", new Array(), new Array(), true, onCalculateHealthSuccess, onFail);

            //searchWidget.getCube()
            searchWidget.search();

            //CUBIT
            //catalogsMenu = AIV1.UI.CubeMenu.create( element_Catalogs );
            //propertiesMenu = AIV1.UI.CubeMenu.createDynamically(element_Properties);            


            Authenticator.IsAdmin(onIsAdminSuccess, onFail);
            DataService.GetViews(onGetViewsSuccess, onFail);                        
        }

        function onLogOffSuccess() {
            //clearWindow();
            blankPassword();
            element_LogOut.style.visibility = "hidden";
            loginPopup.dropShadow.adjust();
            loginPopup.show();
            isLoggedIn = false;
            element_Management.style.visibility = "hidden";
            element_Support.style.visibility= "hidden";
        }

        function onObjectIsFrozen_success(result, target) {
            //alert(result);
            target.IsFrozen = result;
            objectToolbar.refresh();
        }
        
        function onSetDependency_Success(result, obj) {
            //alert("added\n" + result + " \n"  + obj.treeNode );

            var treeNode = obj.treeNode;
            var tree = treeNode.getTree();
            var dataObject = null;

            if (tree == dependencyTree.upperTree)
                dataObject = obj.predicate;
            else
                dataObject = obj.subject;            

            var node = new IntuitiveLabs.UI.TreeNode(dataObject.FriendlyName);
            node.id = obj.predicate.ObjectGUID;
            node.isInitiated = false;
            node.dataObject = dataObject;

            if (treeNode.children.length == 1 && treeNode.children[0].dataObject == null) {
                treeNode.removeChild(treeNode.children[0]);                
            }

            treeNode.addChild(node);            
            treeNode.refresh();
        }
        

        function onUnfreezeObject_success(result, target) {
            target.IsFrozen = false;
            
            if (target.ObjectGUID == currentObject.ObjectGUID) {

                for (var i = 2; i < signatureElement.firstChild.childNodes.length; i++) {
                    for (var j = 0; j < signatureElement.firstChild.childNodes[i].childNodes[1].childNodes.length; j++) {
                        var obj = signatureElement.firstChild.childNodes[i].childNodes[1].childNodes[j].AIV1_icontray;
                        obj.dataObject.setAttribute("IsFrozen", 0);
                        obj.refresh();
                    }
                }
            }
            
            objectToolbar.refresh();            
        }

        function onUnfreezePropertyValue_success(result, target) {
            var propClass = target.parent.dataObject.column;
            var node = target.parent.dataObject.value;

            node.setAttribute("IsFrozen", 0);
            target.parent.refresh();

            //alert(currentObject.Domain + ":" + currentObject.ObjectClass + ":" + currentObject.ObjectGUID);
            
            DataService.ObjectIsFrozen(currentObject.Domain, currentObject.ObjectClass, currentObject.ObjectGUID, onObjectIsFrozen_success, onFail, currentObject);
        }
        
        function setUIComponent(id, obj){
            if (uiWindow.children.contains(id)) {
                var uiObj = uiWindow.children.getEntry(id);
                uiObj.value = obj;
            }
            else {
                uiWindow.children.add(id, obj);
            }
        }

        function filterInventory() {
        }

        function hideObjectPanel() {
            element_CatalogPanel_Chart.style.visibility = "visible";
            element_HealthPanel.style.visibility = "visible";
            element_CatalogPanel_Chart.style.display = "block";
            element_HealthPanel.style.display = "block";
            
            element_CatalogPanel.style.height = "";
            element_CatalogPanel.style.bottom = "30px";
            
            
            element_ObjectPanel.style.height = "30px";
            element_ObjectPanel.style.top = "";
            element_ObjectPanel_Expander.src = "/images/PaneExpander.png";
            objectPanel_isExpanded = false;
        }        


        function showAbout() {
            about.show();
        }

        function showObjectPanel() {
            element_CatalogPanel_Chart.style.visibility = "hidden";
            element_HealthPanel.style.visibility = "hidden";
            element_CatalogPanel_Chart.style.display = "none";
            element_HealthPanel.style.display = "none";
            
            element_CatalogPanel.style.height = "40px";
            element_CatalogPanel.style.bottom = "";

            
            element_ObjectPanel.style.height = "";
            element_ObjectPanel.style.top = "99px";
            element_ObjectPanel_Expander.src = "/images/PaneCollapser.png";
            objectPanel_isExpanded = true;
        }

        function shiftObjectPanel() {
            if (objectPanel_isExpanded) {
                hideObjectPanel();
            }
            else {
                showObjectPanel();
            }
        }
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Authenticator.svc" />        
                <asp:ServiceReference Path="~/GroupManagement.svc" />
                <asp:ServiceReference Path="~/DataService.svc" />
            </Services>
        </asp:ScriptManager>
                    
        <div id="WalkPanel" style="position:fixed; left:0px; top:30px; height:32px; right: 0px;">
            <div class="FramePadder">
                <div class="TiledBackground">
                    <div id="WalkDisplay" style="display:block; background-color:white; position:absolute; left:2px; top:2px; bottom:2px; right: 2px; padding-top:2px; border: solid 1px gray; overflow:hidden;">
                    </div>
                </div>
            </div>
        </div>
        <!--height:260px;-->   
        <!--Has the Health cylinders, bar graph and object count table-->   
        <div id="CatalogPanel" style="position:fixed; left:250px; top:60px;  bottom:30px; right: 0px;">
            <div class="FramePadder">
                <div class="TiledBackground">
                    <div id="CatalogPanel_Display" style="display:block;">

                        <div style="position:absolute; top:2px; right:530px; left:4px; height:23px; border:solid 1px gray; border-bottom-style:dotted;  font-size:12px;  padding:2px; background-color:#f9f9f9; overflow:visible;" align="right" id="Div4">
                            <div style="float:left; padding-left:4px; text-decoration:none; font-weight:bold; cursor:pointer;" id="Div5">HEALTH</div>
                        </div>
                    
                        <!--The div with the Health Cylinders in it-->
                        <div align="left" style="position:absolute; padding:5px; padding-left:10px; top:31px; left:4px; right:530px; height:330px; display:block; background-color: #FFFFFF; border:solid 1px gray; border-top: none;" id="HealthPanel">
                            <iframe scrolling="no" src="HealthCylinder.aspx" style="display:inline; height:145px; width:220px; background-color: #FFFFFF; border:none; border-top: none;" frameborder="0" id="HealthPanel_FilterChart"></iframe>                          
                            <iframe scrolling="no" src="HealthCylinder.aspx" style="display:inline; height:145px; width:220px; background-color: #FFFFFF; border:none; border-top: none;" frameborder="0" id="HealthPanel_GlobalChart"></iframe>                          
                        </div>
                        
                        <div style="position:absolute; top:2px; width:516px; right:4px; height:23px; border:solid 1px gray; border-bottom-style:dotted;  font-size:12px;  padding:2px; background-color:#f9f9f9; overflow:visible;" align="right" id="Div1">
                            <div style="float:right; padding-right:4px; text-decoration:none; font-weight:bold;" id="Div2">CHART</div>
                        </div>

                        <!--The div with the catalog panel chart-->
                        <iframe scrolling="no" src="CatalogingChart.aspx" style="position:absolute; top:31px; right:4px; height:340px; width:520px; display:block; background-color: #FFFFFF; border:solid 1px gray; border-top: none;" frameborder="0" id="CatalogPanel_Chart">                            
                        </iframe>
                        
                        <div style="position:absolute; top:376px; left:4px; right:4px; height:23px; border:solid 1px gray; border-bottom-style:dotted;  font-size:12px;  padding:2px; background-color:#f9f9f9; overflow:visible;" align="right" id="Div3">
                            <div style="padding-left:10px; text-decoration:underline; cursor:pointer; float:right;">   
                                <form action="exportData();">
                                <select name="Formats">
                                <option value="csv">Csv</option>
                                <option value="xlsx">Xlsx</option>
                                <option value="pdf">Pdf</option>
                                </select>
                                <br>Export<br>
                                <input type="submit">
                                </form>
                              </div>                               
                        </div>
                        
                        <!--The div with the category panel table-->
                        <div style="position:absolute; top:399px; bottom:4px; left:4px; right:4px; border:solid 1px gray; border-top-style:dotted; font-size:12px;  padding:10px; background-color:#ffffff; overflow:auto;" id="CatalogPanel_Viewer">                            
                        </div> 
                                   
                        
                        <div style="position:absolute; top:50%; left:50%; display:none; " id="CatalogPanel_Load">
                            <img style="position:absolute; left:-32px; top:-32px;" src="images/V1-Load.gif" />                            
                        </div>     
                    </div>                                      
                </div>
            </div>
        </div>
        <!--top:318px-->
        <div id="ObjectPanel" style="position:fixed; left:250px; height:30px; bottom:0px; right: 0px;">        
            <div class="FramePadder">
                <div class="TiledBackground">
                    <div id="ObjectPanel_Display" style="display:block;">
                        <div style="cursor:pointer; position:absolute; text-decoration:none; top:4px; left:6px; font-weight:bold; font-size:12px;">
                            <span>OBJECT&nbsp;&nbsp;</span><span id="Object_Name"></span> <span id="Object_GUID" style="color:#666666; font-size:12px;"></span> <span id="Object_Info" style="color:#666666; font-size:12px; font-weight:normal;"></span><br />
                        </div>
                        <div style="cursor:pointer; position:absolute; text-decoration:none; color:#333333; right:4px; top: 4px; font-weight:bold;" onclick="shiftObjectPanel();">
                            <img id="ObjectPanel_Expander" src="/images/PaneExpander.png" />
                        </div>
                        <div style="position:absolute; top:26px; left:4px; right:4px; height:23px; border:solid 1px gray; border-bottom-style:dotted;  font-size:12px;  padding:2px; background-color:#f9f9f9; overflow:visible;" align="right" id="ObjectPanel_Toolbar"></div>
                        <div style="position:absolute; top:49px; bottom:4px; left:4px; right:4px; border:solid 1px gray; border-top-style:dotted; font-size:12px;  padding:10px; background-color:#ffffff; overflow:auto;" id="ObjectPanel_Viewer">                            
                        </div>
                        <div style="position:absolute; top:50%; left:50%; display:none; " id="ObjectPanel_Load">
                            <img style="position:absolute; left:-32px; top:-32px;" src="images/V1-Load.gif" />
                        </div>
                    </div>                   
                </div>
            </div>
        </div>
        
        <div id="ModuleSelector" style="position:fixed; left:0px; right:0px; top:2px; height:30px; overflow:visible; ">            
            <div class="FramePadder">
                <div class="TiledBackground">                
                    <div style="cursor:pointer; position:absolute; text-decoration:underline;  left:125px; top: 4px; font-weight:bold; font-size:12px;">
                        <div id="LogOut" style="visibility:hidden; cursor:pointer;" onclick="callLogOff();">Log Out</div>
                    </div>                    
                    <div style="cursor:pointer; position:absolute; text-decoration:underline;  left:190px; top: 4px; font-weight:bold; font-size:12px;">
                        <a id="Management" style="visibility:hidden; color:black; cursor:pointer;" href="/Management" target="_blank">Management</a>
                    </div>  
                    <div style="cursor:pointer; position:absolute; text-decoration:underline;  left:300px; top: 4px; font-weight:bold; font-size:12px;">
                        <a id="Support" style="visibility:hidden; color:black; cursor:pointer;" href="https://ain.support:4443" target="_blank">Support</a>
                    </div>   
                    <div style="cursor:pointer; position:absolute; text-decoration:underline;  left:75px; top: 4px; font-weight:bold; font-size:12px;">
                        <div id="About" style="visibility:visible; cursor:pointer;" onclick="showAbout();">About</div>
                    </div>    
                    <img src="images/FilterButton-Personnel.png" style="position:absolute; right:2px; top:2px;" onclick="loadFilter( new Array('Type.User') );" />
                    <img src="images/FilterButton-Software.png" style="position:absolute; right:104px; top:2px;" onclick="loadFilter( new Array('Software') );"   />
                    <img src="images/FilterButton-Security.png" style="position:absolute; right:206px; top:2px;" onclick="loadFilter( new Array('Alerts') );"  />
                    <img src="images/FilterButton-Inventory.png" style="position:absolute; right:308px; top:2px;" onclick="loadFilter( new Array('Type.Node') );"  />
                                    
                    <!--<div id="Prefilters" onmouseover="prefilterMenu.show();" onmouseout="prefilterMenu.hide();" style="visibility:visible; cursor:pointer; position:absolute; text-decoration:underline; right:330px; top: 4px; font-weight:bold; font-size:12px;">Prefilters</div>-->
                    
                    <!--
                    <div id="Catalogs" onmouseover="catalogsMenu.show();" onmouseout="catalogsMenu.hide();" style="visibility:visible; cursor:pointer; position:absolute; text-decoration:underline; right:255px; top: 5px; font-weight:bold; font-size:10px;">+Catalogs</div>
                    <div id="Properties" onmouseover="propertiesMenu.show();" onmouseout="propertiesMenu.hide();" style="visibility:visible; cursor:pointer; position:absolute; text-decoration:underline; right:175px; top: 5px; font-weight:bold; font-size:10px;">+Properties</div>
                    -->
                    
                    <div style="cursor:pointer; position:absolute; display: none; text-decoration:none;  right:306px; top: 2px; font-weight:bold; font-size:11px;">
                        <div style="display:none;" id="View_Container">
                            <div style="display:table-row;">
                                <div style="display:table-cell;padding-right:5px;">View:</div>
                                <div style="display:table-cell;">
                                    <select style="height:20px; font-size:12px; border:inset 1px #999999;" id="View_Selector" onchange="changeView(this.options[this.selectedIndex].value)"></select>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>            
        </div>        
        
        <div style="position:fixed; top:0px; z-index:1000;">
            <img src="images/V1.png" />
        </div>
        
    
    </form>
</body>
</html>

