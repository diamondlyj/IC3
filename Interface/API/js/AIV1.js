/*
Copyright 2010-15, AI^n 
All rights reserved
*/
if (AIV1 == undefined) var AIV1 = {};
if (AIV1.UI == undefined) AIV1.UI = {};

if (Xcellence == undefined || Xcellence.Meta == undefined || Xcellence.Meta.State == undefined)
    throw new Error("The library Xcellence.js must be loaded");

if (IntuitiveLabs == undefined || IntuitiveLabs.UI == undefined)
    throw new Error("The library IntuitiveLabs_UI.js must be loaded");

AIV1.UI.state = new Xcellence.Meta.State();
AIV1.storeUrl = "https://WIN-EJQAKUR5TTI:8451/";

AIV1.objectHasData = function(obj) {
    var node = IntuitiveLabs.Xml.Load(obj.AIObject);
    var sign = node.getElementsByTagName("Signature");

    if (sign.length == 0 || sign[0].childNodes.length == 0)
        return false;

    return true;
}

AIV1.FilterItem = function(friendlyPath, category, isDynamic) {
    this.friendlyPath = friendlyPath;
    this.category = category;
    this.isDynamic = isDynamic;        
}

AIV1.FilterItem.prototype.friendlyPath = null;
AIV1.FilterItem.prototype.category = null;
AIV1.FilterItem.prototype.isDynamic = false;

AIV1.createFilter = function(text, paths) {
    var item = new IntuitiveLabs.UI.MenuItem(text);

    var filter = new IntuitiveLabs.Collections.Dictionary();

    //alert(paths);

    for (var i = 0; i < paths.length; i++) {
        var cat = {Path:null};
        //new AIV1Portal.ServiceProxies.Data.Cube.Category();
        cat.Path = paths[i];

        var filterItem = new AIV1.FilterItem(paths[i].replace(/\./g, " &gt; "), cat, false);
        filter.add(filterItem.category.Path, filterItem)
    }

    item.onClick = function(sender) {
        var obj = sender;

        while (!(obj instanceof AIV1.UI.SearchWidget) && obj.parent != null) {

            if (obj.parent instanceof IntuitiveLabs.UI.TextObject) {
                obj.hide();
            }

            obj = obj.parent;
        }

        if (obj.loadFilter != undefined) {
            obj.clearFilter();
            obj.loadFilter(sender.dataObject);
        }


        //searchWidget.clearFilter();
        //searchWidget.loadFilter(sender.dataObject);
    }

    item.dataObject = filter;

    return item;
}

AIV1.CubeCache = function( domain, paths, vals, matchSubstrings, trees) {
    this.domain = domain;
    this.paths = paths;
    this.values = vals, 
    this.matchSubstrings = matchSubstrings;
    this.trees = trees;
}

AIV1.CubeCache.prototype.domain = null;
AIV1.CubeCache.prototype.paths = new Array();
AIV1.CubeCache.prototype.values = new Array();
AIV1.CubeCache.prototype.matchSubstrings = true;
AIV1.CubeCache.prototype.trees = null;

AIV1.CubeCacheCollection = function() {
    IntuitiveLabs.Collections.Dictionary.apply(this);
}

AIV1.CubeCacheCollection.prototype = new IntuitiveLabs.Collections.List();

AIV1.CubeCacheCollection.prototype.findCube = function(domain, paths, vals, matchSubstrings) {

    for (var i = 0; i < this.items.length; i++) {
        //alert(this.items[i].matchSubstrings + ":" + matchSubstrings);
        if (this.items[i].domain != domain
            || this.items[i].matchSubstrings != matchSubstrings
            || this.items[i].paths.length != paths.length
            || this.items[i].values.length != vals.length)
            continue;

        var found = true;

        for (var j = 0; j < this.items[i].paths.length; j++) {
            if (this.items[i].paths[j] != paths[j]) {
                found = false;
                break;
            }
        }

        if (!found)
            continue;

        for (var j = 0; j < this.items[i].values.length; j++) {
            if (this.items[i].values[j] != vals[j]) {
                found = false;
                break;
            }
        }

        if (!found)
            continue;

        return this.items[i];
    }

    return null;
}

AIV1.UI.CountCache = function() {
    this.dataObject = null;
    this.dataType = "";
}

AIV1.UI.CubeCaches = new AIV1.CubeCacheCollection();

AIV1.UI.onCountPropertyValues_Success = function(result, target) {

    hideObjectPanel();

    AIV1.UI.CountCache.dataObject = result;
    AIV1.UI.CountCache.dataType = "Property";

    var dataTable = new IntuitiveLabs.Data.Table();
    dataTable.createColumn("MIX2_PropertyValue");
    dataTable.createColumn("MIX2_ValueCount");

    for (var i = 0; i < result.Values.length; i++) {
        var row = dataTable.createRow(result.Values[i].Value, result.Values[i].Count);
    }

    var grid = new IntuitiveLabs.UI.Grid();
    grid.includeLineNumber = true;
    grid.dataTable = dataTable;
    grid.headerClass = "DataTable_Header";
    grid.tableClass = "DataTable";
    grid.rowClass = "DataTable_Row";

    grid.createColumn("MIX2_PropertyValue", "Value", "MIX2_PropertyValue");
    grid.createColumn("MIX2_ValueCount", "Object Count", "MIX2_ValueCount");

    var categoryInfoDiv = document.createElement("div");
    categoryInfoDiv.appendChild(grid.createElement());

    var container = element_CatalogPanel_Viewer;
    IntuitiveLabs.UI.DOM.removeChildren(container);
    container.appendChild(categoryInfoDiv);
    element_CatalogPanel_Load.style.display = "none";

    grid.sort("MIX2_ValueCount", true);

    var chart = document.getElementById("CatalogPanel_Chart");
    chart.src = "CatalogingChart.aspx";
}

AIV1.UI.onFindObjects_Success = function(result, target) {

    var selectList = target.getObject("ObjectList");
    selectList.clearItems();

    for (var i = 0; i < result.length; i++) {

        if (result[i].FriendlyName == null)
            result[i].FriendlyName = result[i].ObjectGUID;

        if (i == 0) {
            //selectList.pager.startText = result[i].FriendlyName.substring(0, 5) + " <span style='font-weight:normal;'>[" + (currentPosition + 1).toString() + "]</span>"
            target.pager.first = result[i].FriendlyName.substring(0, 5);
        }
        else if (i == (result.length - 1)) {
            var lastIndex = target.pager.currentPosition + target.pager.chunkSize;

            if (i < (chunkSize - 1))
                lastIndex = objectCount;

            target.pager.last = result[i].FriendlyName.substring(0, 5);

            //selectList.pager.endText = result[i].FriendlyName.substring(0, 5) + " <span style='font-weight:normal;'>[" + lastIndex.toString() + "]</span>";
        }

        var item = new IntuitiveLabs.UI.ListItem(result[i].ObjectGUID, result[i], "images/Torus-B.png");
        
        if (result[i].State < AIV1.UI.state.healthy) {
            if (result[i].State < AIV1.UI.state.unhealthy)
                item.imageUrl = "images/Torus-R.png";
            else
                item.imageUrl = "images/Torus-Y.png";
        }
        else
            item.imageUrl = "images/Torus-G.png";


        selectList.addItem(item);
    }

    selectList.refresh();
    //target.adjust();
}

AIV1.UI.onFindObjects_Count_Success = function(result, target) {
    //alert(result);
    //var selectList = target.getObject("ObjectList");
    target.pager.count = result;
    target.pager.refresh();

    var selectList = target.getObject("ObjectList");
    selectList.setInfo("OBJECTS (" + result + ")");
}

AIV1.UI.onFreezePropertyValue_success = function(result, target) {
    //alert(target.parent.dataObject.xml);
    target.parent.dataObject.setAttribute("IsFrozen", 1);
    //alert(target.parent.dataObject.xml);
    target.parent.refresh();

    currentObject.IsFrozen = true;
    objectToolbar.refresh();
}

AIV1.UI.onGetCategoryInfo_Success = function(result, target) {

    hideObjectPanel();

    AIV1.UI.CountCache.dataObject = result;
    AIV1.UI.CountCache.dataType = "Category";

    var dataTable = new IntuitiveLabs.Data.Table();
    dataTable.createColumn("MIX2_CategoryPath");
    dataTable.createColumn("MIX2_ObjectCount");

    for (var i = 0; i < result.Children.length; i++) {
        var row = dataTable.createRow(result.Children[i].Path.replace(/\./g, " &gt; "), result.Children[i].ObjectCount);
        row.dataObject = result.Children[i];
    }

    var grid = new IntuitiveLabs.UI.Grid();
    grid.includeLineNumber = true;
    grid.dataTable = dataTable;
    grid.headerClass = "DataTable_Header";
    grid.tableClass = "DataTable";
    grid.rowClass = "DataTable_Row";

    var catColumn = grid.createColumn("MIX2_CategoryPath", "Category", "MIX2_CategoryPath");

    catColumn.onSelectCell = function(sender) {
        var dataObject = sender.row.dataRow.dataObject;
        //var val = sender.value.replace(/ &gt; /g, ".");
        var filter = new IntuitiveLabs.Collections.Dictionary();
        var filterItem = new AIV1.FilterItem(dataObject.Path.replace(/\./g, " &gt; "), dataObject, false);
        filter.add(dataObject.Path, filterItem);
        searchWidget.loadFilter(filter);
    }

    grid.createColumn("MIX2_ObjectCount", "Object Count", "MIX2_ObjectCount");

    var categoryInfoDiv = document.createElement("div");
    categoryInfoDiv.appendChild(grid.createElement());

    var container = element_CatalogPanel_Viewer;
    IntuitiveLabs.UI.DOM.removeChildren(container);
    container.appendChild(categoryInfoDiv);

    element_CatalogPanel_Load.style.display = "none";

    grid.sort("MIX2_ObjectCount", true);

    var chart = document.getElementById("CatalogPanel_Chart");
    chart.src = "CatalogingChart.aspx";
}

AIV1.UI.onGetCube_Success = function(result, target) {

    if (result != undefined && result != null && result.Catalogs != undefined) {

        target.loadCube(result, target.catalogTrees, target.catalogsPanel);

    }
}

AIV1.UI.onGetDynacube_Success = function(result, sender) {

    if (result != undefined && result != null && result.Catalogs != undefined) {
        sender.target.loadCube(result, sender.target.dynacubeTrees, sender.target.dynacubePanel, sender.filter);
    }
}

AIV1.UI.onCalculateHealth_Success = function(result) {

    //alert("blurb");
    //var target = sender.target;
    var chart = document.getElementById("HealthPanel_FilterChart");
    chart.src = "HealthCylinder.aspx?t=CURRENT%20FILTER&d=" + result;
}


AIV1.UI.onSearch_Success = function(result, sender) {

    var target = sender.target;

    var selectList = target.getObject("ObjectList");
    selectList.clearItems();

    var objects = result.Objects;

    target.pager.count = result.Count;

    if (target.pager.count > 0)
        target.pager.show();

    selectList.setInfo("OBJECTS (" + result.Count.toString() + ")");

    for (var i = 0; i < objects.length; i++) {
        if (objects[i].FriendlyName == undefined || objects[i].FriendlyName == null) {
            objects[i].FriendlyName = objects[i].ObjectGUID;
        }

        if (i == 0) {
            target.pager.first = objects[i].FriendlyName.substring(0, 5);
        }
        else if (i == (objects.length - 1)) {
            var lastIndex = target.pager.position + target.pager.chunkSize;

            if (i < (chunkSize - 1))
                lastIndex = result.Count;

            target.pager.last = objects[i].FriendlyName.substring(0, 5);
        }

        var item = new IntuitiveLabs.UI.ListItem(objects[i].ObjectGUID, objects[i], "images/Torus-B.png");

        if (!AIV1.objectHasData(objects[i])) {
            item.imageUrl = "images/Torus-K.png";
        }
        else{

            if (objects[i].State < AIV1.UI.state.healthy) {
                if (objects[i].State < AIV1.UI.state.unhealthy)
                    item.imageUrl = "images/Torus-R.png";
                else
                    item.imageUrl = "images/Torus-Y.png";
            }
            else
                item.imageUrl = "images/Torus-G.png";
        }


        selectList.addItem(item);
    }

    selectList.refresh();

    //if(sender.refreshDynaCube)
    //target.getDynacube( sender.filter );

    //target.adjust();
}

AIV1.UI.onStoreFilter_Success = function(result) {
    //alert("Filter stored");
    window.location = AIV1.storeUrl + "ExportObjects.aspx?p=1&c=6&g=" + result;
}


AIV1.UI.onUnfreezePropertyValue_success = function(result, target) {
    //alert(target.parent.dataObject.xml);
    target.parent.dataObject.setAttribute("IsFrozen", 0);
    //alert(target.parent.dataObject.xml);
    target.parent.refresh();

    DataService.ObjectIsFrozen(currentObject.Domain, currentObject.ObjectClass, currentObject.ObjectGUID, onObjectIsFrozen_success, onFail, currentObject);    
}

// ------WidgetTemplate--------------
AIV1.UI.WidgetTemplate = function() {
    this.setProperty("border", "1px solid gray");
    this.setProperty("padding", "3px");
    //this.setProperty("margin", "0px");
    this.setProperty("backgroundImage", "url(images/DottedBG.gif)");
}

AIV1.UI.WidgetTemplate.prototype = new IntuitiveLabs.UI.StyleTemplate();


// ------LabelTemplate--------------
AIV1.UI.LabelTemplate = function() {
    this.setProperty("marginTop", "2px");
    this.setProperty("marginBottom", "0px");
    this.setProperty("color", "#888888");
    this.setProperty("fontWeight", "bold");
    this.setProperty("fontSize", "14px");
}

AIV1.UI.LabelTemplate.prototype = new IntuitiveLabs.UI.StyleTemplate();

// ------InputTemplate--------------
AIV1.UI.InputTemplate = function() {
    this.setProperty("width", "192px");
    this.setProperty("border", "1px solid gray");
    this.setProperty("margin", "2px");
}

AIV1.UI.InputTemplate.prototype = new IntuitiveLabs.UI.StyleTemplate();

// ------DropTemplate--------------
AIV1.UI.DropTemplate = function() {
    this.setProperty("width", "192px");
    this.setProperty("border", "1px solid gray");
    this.setProperty("margin", "2px");
    //this = new AIV1.UI.InputTemplate()
    this.setProperty("border", "inset 1px #999999");
    this.setProperty("width", "196px");
}

AIV1.UI.DropTemplate.prototype = new IntuitiveLabs.UI.StyleTemplate();


//--------NewDependencyWidget--------------

AIV1.UI.NewDependencyWidget = function() {
    IntuitiveLabs.UI.Widget.apply(this, arguments);

    this.isHidden = true;

    this.element = document.createElement("div");

    this.onSlideRight_stop = function(sender) {
        if (element_ObjectPanel != undefined && element_ObjectPanel != null) {
            if (actionIndicator != null)
                actionIndicator.style.display = "none";

            IntuitiveLabs.UI.DOM.alterRight(element_ObjectPanel, 0);

            if (uiWindow != undefined && uiWindow != null)
                uiWindow.adjust();
        }
    }

    this.onSlideLeft_stop = function(sender) {
        var catSelect2 = sender.searchWidget.getObject("Filter");
        DataService.GetParentClasses("IPNetworking", onGetParentClassesSuccess, onFail, catSelect2);
    }


    this.styleTemplate = new AIV1.UI.WidgetTemplate();

    var textTemplate = new IntuitiveLabs.UI.StyleTemplate();
    textTemplate.setProperty("marginTop", "2px");
    textTemplate.setProperty("marginBottom", "0px");
    textTemplate.setProperty("color", "#888888");
    textTemplate.setProperty("fontWeight", "bold");
    textTemplate.setProperty("fontSize", "14px");
    textTemplate.setProperty("paddingRight", "3px");

    //Clode window
    var closeIcon = new IntuitiveLabs.UI.Image("Image:");
    closeIcon.parent = this;
    closeIcon.url = "/images/IntuitiveLabs/icon16-SlideRight.png";
    closeIcon.onClick = function(sender) {
        sender.parent.hide();
    }

    this.addObject("CloseButton", closeIcon);

    //Strength
    var weight = new IntuitiveLabs.UI.Dropdown("Strength:");
    weight.textTemplate = textTemplate;
    weight.textPosition = "left";
    weight.addOption("strong", "strong", "1");
    weight.addOption("medium", "medium", "0.5");
    weight.addOption("weak", "weak", "0.25");

    weight.onChange = function(sender) {
        sender.parent.parent.weight = sender.value;
    };

    this.addObject("Strength", weight);

    this.searchWidget = new AIV1.UI.SearchWidget();
    this.searchWidget.expandsLeft = true;
    this.searchWidget.parent = this;
    this.searchWidget.styleTemplate = new IntuitiveLabs.UI.StyleTemplate();
    this.addObject("Search", this.searchWidget);


}


AIV1.UI.NewDependencyWidget.prototype = new IntuitiveLabs.UI.Widget();
AIV1.UI.NewDependencyWidget.constructor = AIV1.UI.NewDependencyWidget;
AIV1.UI.NewDependencyWidget.prototype.weight = 1;

AIV1.UI.NewDependencyWidget.prototype.hide = function() {
    if (!this.isHidden) {
        this.isHidden = true;
        newDependencyWidget.slideRight(252);
        element_ObjectPanel_Toolbar.style.paddingRight = "2px";
        
        //IntuitiveLabs.UI.Object.prototype.hide.call(this);
    }
};

AIV1.UI.NewDependencyWidget.prototype.show = function() {
    if (this.isHidden) {
        this.isHidden = false;
        //IntuitiveLabs.UI.Object.prototype.show.call(this);

        this.slideLeft(252);
        this.searchWidget.getCube();
        this.searchWidget.search();
        //newDependencyWidget.slideLeft(252);                
    }
};

//AIV1.UI.NewDependencyWidget.prototype.searchWidget = null;

//--------SearchWidget--------------
AIV1.UI.SearchWidget = function() {

    IntuitiveLabs.UI.Widget.apply(this, arguments);

    this.catalogTrees = new IntuitiveLabs.Collections.Dictionary();
    this.dynacubeTrees = new IntuitiveLabs.Collections.Dictionary();

    this.element = document.createElement("div");
    this.styleTemplate = new AIV1.UI.WidgetTemplate();

    this.styleTemplate.setProperty("width", "240px");

    var inputTemplate = new AIV1.UI.InputTemplate();
    var dropTemplate = new AIV1.UI.DropTemplate();

    //CatalogTrees
    var groupTemplate = new IntuitiveLabs.UI.StyleTemplate();
    groupTemplate.setProperty("height", "102px");
    groupTemplate.setProperty("border", "solid 1px #888888");
    groupTemplate.setProperty("backgroundColor", "#ffffff");
    groupTemplate.setProperty("marginBottom", "4px");

    this.catalogsPanel = new IntuitiveLabs.UI.PanelGroup();
    this.catalogsPanel.navigationType = "dropdown";
    this.catalogsPanel.styleTemplate = groupTemplate;

    this.dynacubePanel = new IntuitiveLabs.UI.PanelGroup();
    this.dynacubePanel.navigationType = "dropdown";
    this.dynacubePanel.styleTemplate = groupTemplate;

    var panelTemplate = new IntuitiveLabs.UI.StyleTemplate();
    panelTemplate.setProperty("overflow", "auto");
    panelTemplate.setProperty("height", "82px");
    //panelTemplate.setProperty("border", "none");
    this.catalogsPanel.panelTemplate = panelTemplate;
    this.dynacubePanel.panelTemplate = panelTemplate;

    var navTemplate = new IntuitiveLabs.UI.StyleTemplate();
    navTemplate.setProperty("width", "238px");
    navTemplate.setProperty("border", "solid 1px #888888");
    this.catalogsPanel.navigationTemplate = navTemplate;
    this.dynacubePanel.navigationTemplate = navTemplate;

    //this.addObject("CatalogPanels", this.catalogsPanel);
    //this.addObject("DynacubePanels", this.dynacubePanel);

    /*
    this.onClickCategory = function(sender) {
    var item = new IntuitiveLabs.UI.ListItem(sender.dataObject.Path, sender.dataObject, null);
    sender.tree.parent.pathList.addItem(item);
    sender.tree.parent//pathList.refresh();
    };
    */

    //Filtration Items       
    this.menuTemplate = new IntuitiveLabs.UI.StyleTemplate();
    this.menuTemplate.setProperty("fontSize", "10px");

    var textTemplate = new IntuitiveLabs.UI.StyleTemplate();
    textTemplate.setProperty("fontSize", "12px");
    textTemplate.setProperty("fontWeight", "bold");
    textTemplate.setProperty("padding", "4px");

    this.prefiltersText = new IntuitiveLabs.UI.TextObject("Prefilters");
    this.prefiltersText.styleTemplate = textTemplate.clone();

    this.prefiltersMenu = new IntuitiveLabs.UI.Menu();
    this.prefiltersMenu.parent = this.prefiltersText;
    this.prefiltersText.onMouseOver = function(sender) { sender.parent.prefiltersMenu.show(); };
    this.prefiltersText.onMouseOut = function(sender) { sender.parent.prefiltersMenu.hide(); };

    var invent = AIV1.createFilter("Inventory", ["Type.Node"])
    //var invMenu = new IntuitiveLabs.UI.Menu();

    //invMenu.addItem(AIV1.createFilter("Asia", ["Type.Node", "Location (geographic).Asia"]));
    //invMenu.addItem(AIV1.createFilter("Africa", ["Type.Node", "Location (geographic).Africa"]));

    //var eu = AIV1.createFilter("Europe", ["Type.Node", "Location (geographic).Europe"]);
    //var euMenu = new IntuitiveLabs.UI.Menu();
    //euMenu.addItem(AIV1.createFilter("France", ["Type.User", "Location (political).European Union.France"]));
    //euMenu.addItem(AIV1.createFilter("United Kingdom", ["Type.User", "Location (political).European Union.United Kingdom"]));
    //eu.setSubMenu(euMenu);
    //invMenu.addItem(eu);

    //invMenu.addItem(AIV1.createFilter("North America", ["Type.Node", "Location (geographic).North America"]));
    //invMenu.addItem(AIV1.createFilter("South America", ["Type.Node", "Location (political).South America"]));

    //invent.setSubMenu(invMenu);

    this.prefiltersMenu.addItem(invent);
    this.prefiltersMenu.addItem(AIV1.createFilter("Network", ["Type.Subnet"]));
    this.prefiltersMenu.addItem(AIV1.createFilter("Software", ["Software"]));
    this.prefiltersMenu.addItem(AIV1.createFilter("User", ["Type.User"]));

    /*
    var div = prefilterMenu.createElement();
    element_Prefilters.appendChild(div);

    prefilterMenu.adjust();

    prefilterMenu.hide();
    */

    this.prefiltersText.onCreated = function(sender) {
        //sender.parent.prefiltersMenu.expandsLeft = true;
        sender.element.appendChild(sender.parent.prefiltersMenu.createElement());
        //sender.parent.prefiltersMenu.adjust();
    }

    this.addObject("PrefiltersText", this.prefiltersText);

    textTemplate.setProperty("fontSize", "10px");

    this.catalogsText = new IntuitiveLabs.UI.TextObject("+Catalogs");
    this.catalogsText.styleTemplate = textTemplate;

    this.catalogsText.onCreated = function(sender) {
        sender.parent.catalogsMenu = AIV1.UI.CubeMenu.create(sender);
        sender.parent.catalogsMenu.expandsLeft = sender.parent.expandsLeft;
        sender.parent.catalogsMenu.styleTemplate = sender.parent.menuTemplate;
        sender.onMouseOver = function(sender) { sender.parent.catalogsMenu.show(); };
        sender.onMouseOut = function(sender) { sender.parent.catalogsMenu.hide(); };
    };
    this.addObject("CatalogsText", this.catalogsText);

    this.propertiesText = new IntuitiveLabs.UI.TextObject("+Properties");
    this.propertiesText.styleTemplate = textTemplate;

    this.propertiesText.onCreated = function(sender) {
        sender.parent.propertiesMenu = AIV1.UI.CubeMenu.createDynamically(sender);
        sender.parent.propertiesMenu.expandsLeft = sender.parent.expandsLeft;
        //alert(sender.parent.expandsLeft);
        sender.parent.propertiesMenu.styleTemplate = sender.parent.menuTemplate;
        sender.onMouseOver = function(sender) { sender.parent.propertiesMenu.show(); };
        sender.onMouseOut = function(sender) { sender.parent.propertiesMenu.hide(); };
    };
    this.addObject("PropertiesText", this.propertiesText);

    //PathList
    this.pathList = new IntuitiveLabs.UI.ObjectList();
    this.pathList.autoAdjust = false;
    this.pathList.styleTemplate.setProperty("height", "200px");
    this.pathList.styleTemplate.setProperty("marginTop", "4px");
    this.pathList.styleTemplate.setProperty("border", "solid 1px #888888");
    this.pathList.styleTemplate.setProperty("backgroundColor", "white");
    this.pathList.styleTemplate.setProperty("fontSize", "12px");
    //this.pathList.styleTemplate.setProperty("overflow", "auto");
    var plcol1 = new IntuitiveLabs.UI.ListColumn("data", "friendlyPath", "friendlyPath");

    var deleteFunction = function(sender) {

        var dotIndex = sender.item.dataObject.category.Path.indexOf(".");

        var catalogName = "";

        if (dotIndex > -1) {
            catalogName = sender.item.dataObject.category.Path.substring(0, dotIndex);
        }
        else {
            catalogName = sender.item.dataObject.category.Path;
        }

        var catalog = sender.item.parent.parent.catalogTrees.getEntry(catalogName);

        if (catalog != null) {
            var tree = catalog.value;
            tree.deselect();
        }

        var searchWidget = sender.item.parent.parent;

        var pathList = searchWidget.pathList;
        pathList.removeItem(sender.item.dataObject.category.Path);
        pathList.refresh();

        searchWidget.pager.position = 0;
        searchWidget.pager.first = "A";
        searchWidget.pager.last = "Z";
        searchWidget.pager.hide();
        searchWidget.objectList.setInfo("SEARCHING...");
        searchWidget.search();

        IntuitiveLabs.UI.DOM.removeChildren(element_CatalogPanel_Viewer);
        element_CatalogPanel_Load.style.display = "block";
        
    }

    plcol1.onClickItem = function(sender) {

        var path = sender.item.dataObject.category.Path;

        var searchWidget = sender.item.parent.parent;

        var filter = searchWidget.buildFilter();

        var target = function() { };
        target.target = searchWidget;
        target.filter = filter;

        target.refreshDynaCube = true;

        if (!sender.item.dataObject.isDynamic) {
            DataService.GetCategoryInfo("IPNetworking", path, filter.paths, filter.values, searchWidget.matchSubstrings, AIV1.UI.onGetCategoryInfo_Success, onFail, target);
        }
        else {
            var parts = path.split(".");
            var prop = "";

            if (parts.length > 1) {
                prop = parts[1];
            }

            DataService.CountPropertyValues("IPNetworking", parts[0], prop, filter.paths, filter.values, searchWidget.matchSubstrings, AIV1.UI.onCountPropertyValues_Success, onFail, target);
        }

        //var container = document.getElementById("CatalogPanel_Viewer");
        IntuitiveLabs.UI.DOM.removeChildren(element_CatalogPanel_Viewer);
        element_CatalogPanel_Load.style.display = "block";

        //BAM
    };

    this.pathList.onClickImage = deleteFunction;

    //var plcol2 = new IntuitiveLabs.UI.ListColumn("function", "remove", "remove");
    this.pathList.addColumn(plcol1);
    //this.pathList.addColumn(plcol2);
    this.pathList.info = "FILTER"
    this.pathList.pager.hide();


    this.addObject("PathList", this.pathList);

    //SearchValue (Filter)
    var searchValue = new IntuitiveLabs.UI.Textbox("Search:", null, false);
    searchValue.textPosition = "top";
    searchValue.inputTemplate = inputTemplate;
    searchValue.textTemplate = new AIV1.UI.LabelTemplate();
    searchValue.setValue("");

    this.searchValue = searchValue;

    searchValue.onEnter = function(sender) {
        sender.parent.pager.position = 0;
        //sender.parent.searchValue = this.value;
        //sender.parent.findObjects();
        //DataService.FindByPropertyValue("IPNetworking", "Node", sender.value, sender.parent.partialMatches, false, 1, 100, "FriendlyName", 0, onFind_Success_2, onFail, sender);
        sender.parent.search();
    };

    //searchValue.onEmpty = function(sender) { sender.parent.searchValue };

    this.addObject("SearchValue", searchValue);

    //PartialMatches
    var partialMatches = new IntuitiveLabs.UI.Checkbox("Partial Matches", "Partial");
    partialMatches.textPosition = "right";
    partialMatches.styleTemplate.setProperty("color", "#000000");
    partialMatches.styleTemplate.setProperty("fontWeight", "normal");
    partialMatches.styleTemplate.setProperty("fontSize", "10px");
    partialMatches.isChecked = true;

    partialMatches.onChange = function(sender) {
        sender.parent.matchSubstrings = sender.isChecked;
        sender.parent.search();
    };

    this.addObject("PartialMatches", partialMatches);


    //ObjectList
    this.objectList = new IntuitiveLabs.UI.ObjectList();
    this.objectList.pager.styleTemplate.setProperty("fontSize", "12px");
    this.objectList.pager.onPage = function(sender) { sender.parent.parent.search() };
    this.objectList.parent = this;
    this.pager = this.objectList.pager;

    var item = new IntuitiveLabs.UI.MenuItem("EXPORT");
    item.onClick = function(sender) {
        var obj = sender.parent.parent.parent.parent;
        var filter = obj.buildFilter();
        DataService.StoreFilter("IPNetworking", filter.paths, filter.values, obj.matchSubstrings, AIV1.UI.onStoreFilter_Success, onFail);
    };
    this.objectList.menu.addItem(item);

    var col1 = new IntuitiveLabs.UI.ListColumn("data", "FriendlyName", "FriendlyName");
    col1.itemClass = "List_Item";
    col1.onClickItem = function(sender) { if (sender.parent.parent.onClickItem != null) { sender.parent.parent.onClickItem(sender); } };
    this.objectList.addColumn(col1);

    var listTemplate = new IntuitiveLabs.UI.StyleTemplate();
    //listTemplate.setProperty("padding", "5px");
    listTemplate.setProperty("marginBottom", "4px");
    listTemplate.setProperty("border", "solid 1px black");
    listTemplate.setProperty("backgroundColor", "white");

    this.objectList.styleTemplate = listTemplate;

    this.addObject("ObjectList", this.objectList);

    //var test = new IntuitiveLabs.UI.Textbox("test", null, false);
    //this.addObject("Test", test);

    //DataService.GetCategoryInfo("IPNetworking", "", {}, {}, false, AIV1.UI.onGetCategoryInfo_Success, onFail);    
}

AIV1.UI.SearchWidget.prototype = new IntuitiveLabs.UI.Widget();
AIV1.UI.SearchWidget.constructor = AIV1.UI.SearchWidget;
AIV1.UI.SearchWidget.prototype.catalogSelector = null;
AIV1.UI.SearchWidget.prototype.catalogsPanel = null;
AIV1.UI.SearchWidget.prototype.dynacubeSelector = null;
AIV1.UI.SearchWidget.prototype.dybacubePanel = null;
AIV1.UI.SearchWidget.prototype.domain = "IPNetworking";
AIV1.UI.SearchWidget.prototype.expandsLeft = false;
AIV1.UI.SearchWidget.prototype.objectClass = "Node";
AIV1.UI.SearchWidget.prototype.matchSubstrings = true;
AIV1.UI.SearchWidget.prototype.searchValue = null;
AIV1.UI.SearchWidget.prototype.pager = null;
AIV1.UI.SearchWidget.prototype.onClickItem = null;
AIV1.UI.SearchWidget.prototype.onClickCategory = null;
AIV1.UI.SearchWidget.prototype.catalogTrees = null;
AIV1.UI.SearchWidget.prototype.dynacubeTrees = null;
AIV1.UI.SearchWidget.prototype.pathList = null;
AIV1.UI.SearchWidget.prototype.objectList = null;

AIV1.UI.SearchWidget.prototype.prefiltersText = null;
AIV1.UI.SearchWidget.prototype.prefiltersMenu = null;
AIV1.UI.SearchWidget.prototype.propertiesText = null;
AIV1.UI.SearchWidget.prototype.propertiesMenu = null;
AIV1.UI.SearchWidget.prototype.catalogsText = null;
AIV1.UI.SearchWidget.prototype.catalogsMenu = null;
AIV1.UI.SearchWidget.prototype.menuTemplate = null;


/*
AIV1.UI.SearchWidget.prototype.findObjects = function() {
    var val = this.searchValue.value;

    if (val != "") {
        DataService.FindByPropertyValue(this.domain, this.objectClass, val, this.matchSubstring, true, this.pager.position, this.pager.itemsPerPage, "FriendlyName", 0, AIV1.UI.onFindObjects_Success, onFail, this);
        DataService.FindByPropertyValue_Count(this.domain, this.objectClass, val, this.matchSubstrings, true, AIV1.UI.onFindObjects_Count_Success, onFail, this);
    }
    else {
        DataService.FindByObjectClass(this.domain, this.objectClass, true, this.pager.position, this.pager.itemsPerPage, "FriendlyName", 0, AIV1.UI.onFindObjects_Success, onFail, this);
        DataService.FindByObjectClass_Count(this.domain, this.objectClass, true, AIV1.UI.onFindObjects_Count_Success, onFail, this);
    }
}
*/

AIV1.UI.SearchWidget.prototype.addCategory = function(treeNode, category, isDynamic) {
    var node = new IntuitiveLabs.UI.TreeNode(category.Name, null);

    var filterItem = new AIV1.FilterItem(category.Path.replace(/\./g, " &gt; "), category, isDynamic);
    node.dataObject = filterItem;

    for (var i = 0; i < category.Children.length; i++) {
        this.addCategory(node, category.Children[i], isDynamic);
    }

    treeNode.addChild(node);
}

AIV1.UI.SearchWidget.prototype.getCube = function() {
    DataService.GetCube("IPNetworking", "FriendlyName", 0, AIV1.UI.onGetCube_Success, onFail, this);
}

AIV1.UI.SearchWidget.prototype.getDynacube = function(filter) {
    /*
    var paths = this.getPaths();

    var vals = new Array();

    var val = this.searchValue.value;

    if (val != null && val != "") {
    var p = new function() {
    this.Value = [val];
    }

        vals.push(p);
    }
    */

    var sender = function() { };
    sender.target = this;
    sender.filter = filter;

    DataService.GetDynamicCube("IPNetworking", filter.paths, filter.values, filter.matchSubstrings, "FriendlyName", 0, AIV1.UI.onGetDynacube_Success, onFail, sender);
}

AIV1.UI.SearchWidget.prototype.clearFilter = function() {
    this.pathList.clearItems();
}

AIV1.UI.SearchWidget.prototype.loadFilter = function(filter, clickedProperty) {
    this.pager.position = 0;
    this.pager.first = "A";
    this.pager.last = "Z";
    this.pager.hide();
    this.objectList.setInfo("SEARCHING...");

    for (var i = 0; i < filter.entries.length; i++) {

        if (!this.pathList.items.contains(filter.entries[i].value.category.Path)) {

            var item = new IntuitiveLabs.UI.ListItem(filter.entries[i].value.category.Path, filter.entries[i].value, "images/X.png");
            this.pathList.addItem(item);
            this.pathList.refresh();
        }
    }

    this.search(filter.entries[0].value.category.Path, clickedProperty);
}

AIV1.UI.SearchWidget.prototype.loadCube = function(cube, trees, panels, filter) {
    panels.panels.clear();
    trees.clear();

    var treeTemplate = new IntuitiveLabs.UI.StyleTemplate();

    var firstKey = null;

    for (var i = 0; i < cube.Catalogs.length; i++) {

        var tree = new IntuitiveLabs.UI.Tree();
        tree.styleTemplate = treeTemplate;
        tree.defaultIcon = "/images/Torus-B.png";
        tree.root.text = cube.Catalogs[i].Name;

        var filterItem = new AIV1.FilterItem(cube.Catalogs[i].Path.replace(/\./g, " &gt; "), cube.Catalogs[i], cube.IsDynamic);
        tree.root.dataObject = filterItem;

        tree.root.isExpanded = true;
        tree.parent = this;
        tree.allowDeselect = true;

        tree.onSelectNode = function(sender) {
            sender.tree.parent.pager.position = 0;
            sender.tree.parent.pager.first = "A";
            sender.tree.parent.pager.last = "Z";
            sender.tree.parent.pager.hide();
            sender.tree.parent.objectList.setInfo("SEARCHING...");

            if (sender.tree.previousNode != null)
                sender.tree.parent.pathList.removeItem(sender.tree.previousNode.dataObject.category.Path);

            if (!sender.tree.parent.pathList.items.contains(sender.dataObject.category.Path)) {

                /*Rewrite*/
                var item = new IntuitiveLabs.UI.ListItem(sender.dataObject.category.Path, sender.dataObject, "images/X.png");
                sender.tree.parent.pathList.addItem(item);
                sender.tree.parent.pathList.refresh();

                if (sender.tree.parent.onClickCategory != null)
                    sender.tree.parent.onClickCategory(sender);
            }

            sender.tree.parent.search(sender.dataObject.category.Path);
        }

        tree.onDeselectNode = function(sender) {
            sender.tree.parent.pager.position = 0;
            sender.tree.parent.pager.first = "A";
            sender.tree.parent.pager.last = "Z";
            sender.tree.parent.pager.hide();
            sender.tree.parent.objectList.setInfo("SEARCHING...");

            sender.tree.parent.pathList.removeItem(sender.dataObject.category.Path);
            sender.tree.parent.pathList.refresh();

            if (sender.tree.parent.onClickCategory != null)
                sender.tree.parent.onClickCategory(sender);

            sender.tree.parent.search();
        }

        tree.itemTemplate.setProperty("fontSize", "12px");

        for (var j = 0; j < cube.Catalogs[i].Children.length; j++) {
            this.addCategory(tree.root, cube.Catalogs[i].Children[j], cube.IsDynamic);
        }

        var key = cube.Catalogs[i].Path;

        trees.add(key, tree);

        var panel = new IntuitiveLabs.UI.Panel(key, cube.Catalogs[i].Name);
        panel.content = tree.createElement();

        panels.addPanel(panel);

        if (i == 0)
            firstKey = key;
    }

    /*Build this out*/
    if (filter != undefined && filter != null) {
        var cube = AIV1.UI.CubeCaches.findCube("IPNetworking", filter.paths, filter.values, filter.matchSubstrings, trees);

        if (cube == null) {
            var cubeCache = new AIV1.CubeCache("IPNetworking", filter.paths, filter.values, filter.matchSubstrings, trees);
            AIV1.UI.CubeCaches.add(cubeCache);
        }
    }

    panels.refresh();

    if (firstKey != null)
        panels.selectPanel(firstKey);

}

AIV1.UI.SearchWidget.prototype.buildFilter = function() {

    var vals = new Array();

    var val = this.searchValue.value;

    if (val != null && val != "") {
        var p = new function() {
            this.Value = [val];
            this.InChild = false;
        }

        vals.push(p);
    }

    var paths = new Array();

    /*
    var trees = this.catalogTrees;
    
    for (var i = 0; i < trees.entries.length; i++) {
    if (trees.entries[i].value.currentNode != null && trees.entries[i].value.currentNode.isSelected) {
    paths.push(trees.entries[i].value.currentNode.dataObject.category.Path);
    }
    }
    */

    for (var i = 0; i < this.pathList.items.entries.length; i++) {
        if (this.pathList.items.entries[i].value.dataObject.isDynamic) {
            var parts = this.pathList.items.entries[i].value.dataObject.category.Path.split(".");

            if (parts.length > 0) {

                var p = new function() {
                    this.Value = ["%"];
                    this.ObjectClass = parts[0];
                    this.InChild = true;
                }

                if (parts.length > 1) {
                    p.Property = parts[1];
                }

                vals.push(p);
            }
        }
        else {
            paths.push(this.pathList.items.entries[i].value.dataObject.category.Path);
        }
    }


    var filter = new function() { };
    filter.paths = paths;
    filter.values = vals;
    filter.matchSubstrings = this.matchSubstrings;


    return filter;
}

AIV1.UI.SearchWidget.prototype.search = function(infoPath, clickedProperty) {

    hideObjectPanel();

    var filter = this.buildFilter();

    var sender = function() { };
    sender.target = this;
    sender.filter = filter;

    sender.refreshDynaCube = true;

    var cube = AIV1.UI.CubeCaches.findCube("IPNetworking", filter.paths, filter.values, this.matchSubstrings);

    if (cube == null)
        sender.refreshDynaCube = true;
    else {
        /*Should be set to false when caching is fully implemented*/
        sender.refreshDynaCube = true;

        // Load view with trees
        this.dynacubeTrees = cube.trees;
    }

    //sender.filter.paths = paths;
    //sender.filter.values = vals;
    //sender.filter.matchSubstrings = this.matchSubstrings;

    var path = "";

    if (infoPath != undefined && infoPath != null) {
        path = infoPath;
    }

    DataService.Search(this.domain, filter.paths, filter.values, this.matchSubstrings, this.pager.position, this.pager.itemsPerPage, "FriendlyName", 0, AIV1.UI.onSearch_Success, onFail, sender);
    DataService.CalculateHealth(this.domain, filter.paths, filter.values, this.matchSubstrings, AIV1.UI.onCalculateHealth_Success, onFail, sender);

    if (!clickedProperty) {
        DataService.GetCategoryInfo(this.domain, path, filter.paths, filter.values, this.matchSubstrings, AIV1.UI.onGetCategoryInfo_Success, onFail, sender);
    }
    else {
        var parts = infoPath.split(".");
        var prop = "";

        if (parts.length > 1) {
            prop = parts[1];
        }

        //alert(filter.values);

        DataService.CountPropertyValues(this.domain, parts[0], prop, filter.paths, filter.values, this.matchSubstrings, AIV1.UI.onCountPropertyValues_Success, onFail, sender);
    }
}

//------StateTray------------

AIV1.UI.StateTray = function(dataObject) {
    IntuitiveLabs.UI.IconTray.apply(this);

    this.dataObject = dataObject;

    this.frozenIcon = new IntuitiveLabs.UI.Icon("/images/icon16-SnowFlake.png");
    this.unfrozenIcon = new IntuitiveLabs.UI.Icon("/images/icon16-Flash-Yellow.png");

    this.addIcon("Frozen", this.frozenIcon);
    this.addIcon("Unfrozen", this.unfrozenIcon);

    this.styleTemplate.setProperty("paddingLeft", "5px");
}

AIV1.UI.StateTray.prototype = new IntuitiveLabs.UI.IconTray();
AIV1.UI.StateTray.prototype.frozenIcon = null;
AIV1.UI.StateTray.prototype.unfrozenIcon = null;

//---PropertyStateTray------------------
AIV1.UI.PropertyStateTray = function(dataObject) {
    AIV1.UI.StateTray.apply(this, arguments);

    this.refresh = function() {
        if (this.dataObject != null) {
            var isFrozen = this.dataObject.getAttribute("IsFrozen");
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
    }

    this.unfrozenIcon.onClick = function(sender) {

        var domain = "IPNetworking";
        var objClass = sender.parent.dataObject.parentNode.parentNode.parentNode.getAttribute("ObjectClass");
        var objGUID = sender.parent.dataObject.parentNode.parentNode.parentNode.getAttribute("ObjectGUID");
        var propClass = sender.parent.dataObject.parentNode.getAttribute("Name");
        var propValue = IntuitiveLabs.UI.DOM.getValue(sender.parent.dataObject);

        DataService.FreezePropertyValue(domain, objClass, objGUID, propClass, propValue, AIV1.UI.onFreezePropertyValue_success, onFail, sender);
    };
    this.frozenIcon.onClick = function(sender) {

        var domain = "IPNetworking";
        var objClass = sender.parent.dataObject.parentNode.parentNode.parentNode.getAttribute("ObjectClass");
        var objGUID = sender.parent.dataObject.parentNode.parentNode.parentNode.getAttribute("ObjectGUID");
        var propClass = sender.parent.dataObject.parentNode.getAttribute("Name");
        var propValue = IntuitiveLabs.UI.DOM.getValue(sender.parent.dataObject);

        DataService.UnfreezePropertyValue(domain, objClass, objGUID, propClass, propValue, AIV1.UI.onUnfreezePropertyValue_success, onFail, sender);

    };
}

AIV1.UI.PropertyStateTray.prototype = new AIV1.UI.StateTray();

AIV1.UI.PropertyStateTray.prototype.refresh = function() {
    if (this.element != null) {
        if (this.dataObject.getAttribute("IsFrozen") == true) {
            this.hideIcon("Unfrozen");
        }
        else {
            this.hideIcon("Frozen");
        }
    }
}

//---ObjectStateTray------------------

AIV1.UI.ObjectStateTray = function(dataObject) {
    AIV1.UI.StateTray.apply(this, arguments);
    
    this.unfrozenIcon.onClick = function(sender) { alert("freeze") };
    this.frozenIcon.onClick = function(sender) { alert(sender.parent.dataObject.IsFrozen) };
}

AIV1.UI.ObjectStateTray.prototype = new AIV1.UI.StateTray();

AIV1.UI.ObjectStateTray.prototype.refresh = function() {
    if (this.element != null) {
        if (this.dataObject.IsFrozen == true) {
            this.hideIcon("Unfrozen");
        }
        else {
            this.hideIcon("Frozen");
        }
    }
}

//CubeMenu

AIV1.UI.CubeMenu = function(parent) {
    if (!(parent instanceof IntuitiveLabs.UI.Object)) {
        throw Error("Parent of AIV1.UI.CubeMenu must be of type IntuitiveLabs.UI.Object.");
    }

    IntuitiveLabs.UI.Menu.apply(this);
    this.parent = parent;
}

AIV1.UI.CubeMenu.onGetCube_Success = function(result, target) {
    if (result != undefined && result != null && result.Catalogs != undefined){
        target.generate(result, target);
    }
}

AIV1.UI.CubeMenu.prototype = new IntuitiveLabs.UI.Menu();
AIV1.UI.CubeMenu.prototype.isDynamic = false;

AIV1.UI.CubeMenu.create = function( parent ) {
    var menu = new AIV1.UI.CubeMenu( parent );
    DataService.GetCube("IPNetworking", "FriendlyName", 0, AIV1.UI.CubeMenu.onGetCube_Success, onFail, menu);
    return menu;
}

AIV1.UI.CubeMenu.createDynamically = function(parent) {
    var menu = new AIV1.UI.CubeMenu(parent);
    menu.isDynamic = true;
    //menu.container = parent;
    DataService.GetDynamicCube("IPNetworking", new Array(), new Array(), true, "FriendlyName", 0, AIV1.UI.CubeMenu.onGetCube_Success, onFail, menu);
    return menu;
}


AIV1.UI.CubeMenu.prototype.addChild = function(menu, category, isDynamic) {

    var item = new IntuitiveLabs.UI.MenuItem(category.Name);
    item.parent = this;

    var filter = new IntuitiveLabs.Collections.Dictionary();
    var filterItem = new AIV1.FilterItem(category.Path.replace(/\./g, " &gt; "), category, isDynamic);
    filter.add(filterItem.category.Path, filterItem)

    var holder = function() {};
    
    holder.filter = filter;
    holder.isDynamic = isDynamic;    

    item.dataObject = holder;

    item.onClick = function(sender) {
        var obj = sender;

        while (!(obj instanceof AIV1.UI.SearchWidget) && obj.parent != null) {
            if (obj.parent instanceof IntuitiveLabs.UI.TextObject) {
                obj.hide();
            }

            obj = obj.parent;
        }


        if (obj.loadFilter != undefined) {
            obj.loadFilter(sender.dataObject.filter, sender.dataObject.isDynamic);
        }

        IntuitiveLabs.UI.DOM.removeChildren(element_CatalogPanel_Viewer);
        element_CatalogPanel_Load.style.display = "block";        
    }

    //var subMenu = new IntuitiveLabs.UI.Menu();

    //menu.setSubMenu(subMenu);

    if (category.Children.length > 0) {
        var subMenu = new IntuitiveLabs.UI.Menu();
        subMenu.expandsLeft = this.expandsLeft;

        for (var i = 0; i < category.Children.length; i++) {
            this.addChild(subMenu, category.Children[i], isDynamic);
        }

        item.setSubMenu(subMenu);
    }

    menu.addItem(item);
}

AIV1.UI.CubeMenu.prototype.generate = function(cube, container) {

    var treeTemplate = new IntuitiveLabs.UI.StyleTemplate();

    for (var i = 0; i < cube.Catalogs.length; i++) {
        this.addChild(this, cube.Catalogs[i], cube.IsDynamic);
    }

    //alert();

    if (this.parent != null) {
        //alert(this.parent);
        if (this.parent instanceof IntuitiveLabs.UI.Object) {
            this.parent.element.appendChild(this.createElement());
        }
        else {
            this.parent.appendChild(this.createElement());
        }
    }

    this.adjust();
}

AIV1.UI.ChartMenu = new IntuitiveLabs.UI.Menu();

var chartItem = new IntuitiveLabs.UI.MenuItem("Test");
AIV1.UI.ChartMenu.addItem(chartItem);

var el = AIV1.UI.ChartMenu.createElement();

//document.getElementById("ChartMenu").appendChild(el);
