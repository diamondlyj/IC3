/*
Copyright 2009, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined || IntuitiveLabs.UI == undefined)
    throw new Error("The library IntuitiveLabs_UI.js must be loaded");

if (IntuitiveLabs.Collections == undefined)
    throw new Error("The library IntuitiveLabs_Collections.js must be loaded");

//--------Panel--------------
IntuitiveLabs.UI.Panel = function(id, name) {
    this.id = id;
    this.name = name;
    this.toolbars = new IntuitiveLabs.Collections.Dictionary();
}

IntuitiveLabs.UI.Panel.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.Panel.prototype.id = null;
IntuitiveLabs.UI.Panel.prototype.name = null;
IntuitiveLabs.UI.Panel.prototype.onSelect = null;
IntuitiveLabs.UI.Panel.prototype.onUnselect = null;
IntuitiveLabs.UI.Panel.prototype.toolbars = null;
IntuitiveLabs.UI.Panel.prototype.content = null;
IntuitiveLabs.UI.Panel.prototype.contentElement = null;
IntuitiveLabs.UI.Panel.prototype.contentTemplate = null;
IntuitiveLabs.UI.Panel.prototype.toolbarTemplate = null;

IntuitiveLabs.UI.Panel.prototype.addToolbar = function(id, toolbar) {
    toolbar.parent = this;
    toolbar.id = id;
    this.toolbars.add(toolbar.id, toolbar);
}

IntuitiveLabs.UI.Panel.prototype.createElement = function() {
    this.element = document.createElement("div");
    //this.element.style.border = "solid 1px red";

    for (var i in this.toolbars.entries) {
        var toolbar = this.toolbars.entries[i].value;
        var div = toolbar.createElement();
        //div.innerHTML = toolbar.id;

        this.element.appendChild(div);
    }

    //this.contentElement.style.border = "solid 1px blue";
    this.contentElement = document.createElement("div");

    if (this.content != null)
        this.contentElement.appendChild(this.content);

    if (this.contentTemplate != null)
        this.contentTemplate.apply(this.contentElement);
    else if (this.parent != null && this.parent.contentTemplate != undefined && this.parent.contentTemplate != null)
        this.parent.contentTemplate.apply(this.contentElement);

    this.contentElement.style.overflow = "auto";

    //this.contentElement.innerHTML = "Content";    

    this.element.appendChild(this.contentElement);

    //this.element.innerHTML = this.id;

    return this.element;
}

IntuitiveLabs.UI.Panel.prototype.adjust = function() {
    //this.adjustHeightToParent();

    IntuitiveLabs.UI.DOM.sizeToParent(this.element);
    IntuitiveLabs.UI.DOM.sizeToParent(this.contentElement, true);    
}

IntuitiveLabs.UI.Panel.prototype.setContent = function(element) {
    this.content = element;
    IntuitiveLabs.UI.DOM.removeChildren(this.contentElement);
    this.contentElement.appendChild(this.content);
}


//--------PanelGroup--------------
IntuitiveLabs.UI.PanelGroup = function() {
    this.panels = new IntuitiveLabs.Collections.Dictionary();
    this.navElements = new IntuitiveLabs.Collections.Dictionary();
}

IntuitiveLabs.UI.PanelGroup.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.PanelGroup.prototype.navigationTemplate = null;
IntuitiveLabs.UI.PanelGroup.prototype.selectedTemplate = null;
IntuitiveLabs.UI.PanelGroup.prototype.separatorTemplate = null;
IntuitiveLabs.UI.PanelGroup.prototype.panelTemplate = null;
IntuitiveLabs.UI.PanelGroup.prototype.contentTemplate = null;
IntuitiveLabs.UI.PanelGroup.prototype.toolbarTemplate = null;
IntuitiveLabs.UI.PanelGroup.prototype.navigationType = "tabs";

IntuitiveLabs.UI.PanelGroup.prototype.selected = null;
IntuitiveLabs.UI.PanelGroup.prototype.panels = null;
IntuitiveLabs.UI.PanelGroup.prototype.table = null;
IntuitiveLabs.UI.PanelGroup.prototype.filler = null;
IntuitiveLabs.UI.PanelGroup.prototype.panelElement = null;
IntuitiveLabs.UI.PanelGroup.prototype.toolbar = null;
IntuitiveLabs.UI.PanelGroup.prototype.onSelect = null;
IntuitiveLabs.UI.PanelGroup.prototype.onUnselect = null;
IntuitiveLabs.UI.PanelGroup.prototype.navElements = null;

IntuitiveLabs.UI.PanelGroup.prototype.addPanel = function(panel) {    
    panel.parent = this;
    this.panels.add(panel.id, panel);        
}

IntuitiveLabs.UI.PanelGroup.prototype.selectPanel = function(id) {

    if (this.selected != null)
        this.selected.element.style.display = "none";

    if (this.navigationType == null || this.navigationType == "tabs") {
        if (this.navigationTemplate != null && this.selected != null) {
            this.navigationTemplate.apply(this.navElements.getEntry(this.selected.id).value);
        }

        if (this.selectedTemplate != null) {
            this.selectedTemplate.apply(this.navElements.getEntry(id).value);
        }
    }

    this.selected = this.panels.getEntry(id).value;
    this.selected.element.style.display = "block";

    if (this.selected.onSelect != null) {
        this.selected.onSelect(this.selected);
    }

    this.selected.adjust();
}

IntuitiveLabs.UI.PanelGroup.prototype.createElement = function() {
    this.element = document.createElement("div");
    this.createPanelElements();

    return this.element;
}

IntuitiveLabs.UI.PanelGroup.prototype.createPanelElements = function() {

    if (this.styleTemplate != null)
        this.styleTemplate.apply(this.element);

    //IntuitiveLabs.UI.Object.prototype.createElement.call(this);

    this.navElements.clear();

    this.table = document.createElement("div");
    this.table.style.display = "table";

    row = document.createElement("div");
    row.style.display = "table-row";

    if (this.navigationType.toLowerCase() == "dropdown") {
        var dropDown = new IntuitiveLabs.UI.Dropdown();

        dropDown.styleTemplate = this.navigationTemplate;
        dropDown.parent = this;

        /*
        dropDown.onChange = function(sender) {
        alert(sender.parent);
        };
        */

        //alert("here");

        var dhandler = function(sender) {
            var obj = sender.value;

            if (obj.parent.selected != null && obj.parent.selected != obj) {
                if (obj.parent.selected.onUnselect != null)
                    obj.parent.selected.onUnselect(obj.parent.selected);
                else if (obj.parent.onUnselect != null)
                    obj.parent.onUnselect(obj.parent.selected);
            }

            obj.parent.selectPanel(obj.id);

            if (obj.onSelect != null)
                obj.onSelect(obj);
            else if (obj.parent.onSelect != null)
                obj.parent.onSelect(obj);
        }

        for (var i = 0; i < this.panels.count(); i++) {
            var panel = this.panels.entries[i].value;
            dropDown.addOption("Panel" + i, panel.name, panel, dhandler);
            this.navElements.add(this.panels.entries[i].key, cell);
        }

        dropDown.onChange = dhandler;        

        this.element.appendChild(dropDown.createElement());
    }
    else {
        this.table.style.marginTop = "5px";

        for (var i = 0; i < this.panels.count(); i++) {

            var panel = this.panels.entries[i].value;

            var cell = document.createElement("div");
            cell.style.display = "table-cell";

            if (this.navigationTemplate != null)
                this.navigationTemplate.apply(cell);

            //cell.style.borderBottom = "none";
            cell.style.cursor = "pointer";

            cell.innerHTML = panel.name;
            cell.IntuitiveLabs_parent = panel;

            cell.onclick = function() {
                var obj = this.IntuitiveLabs_parent;

                if (obj.parent.selected != null && obj.parent.selected != obj) {
                    if (obj.parent.selected.onUnselect != null)
                        obj.parent.selected.onUnselect(obj.parent.selected);
                    else if (obj.parent.onUnselect != null)
                        obj.parent.onUnselect(obj.parent.selected);
                }

                obj.parent.selectPanel(obj.id);

                if (obj.onSelect != null)
                    obj.onSelect(obj);
                else if (obj.parent.onSelect != null)
                    obj.parent.onSelect(obj);

            };

            this.navElements.add(this.panels.entries[i].key, cell);

            row.appendChild(cell);

            var separator = document.createElement("div");
            separator.style.display = "table-cell";

            if (this.separatorTemplate != null)
                this.separatorTemplate.apply(separator);
            else {
                if (this.selectedTemplate != null) {
                    var fborder = this.selectedTemplate.getProperty("borderRight");

                    if (fborder != null)
                        separator.style.borderBottom = fborder;

                    else {
                        fborder = this.selectedTemplate.getProperty("border");

                        if (fborder != null)
                            separator.style.borderBottom = fborder;
                    }
                }

                separator.style.width = "5px";
            }

            row.appendChild(separator);

            //this.element.appendChild(panel.createElement());
        }

        this.filler = document.createElement("div");
        this.filler.style.display = "table-cell";

        if (this.separatorTemplate != null) {
            var fborder = this.selectedTemplate.getProperty("borderBottom");

            if (fborder != null)
                this.filler.style.borderBottom = fborder;
        }
        else {

            if (this.selectedTemplate != null) {
                var fborder = this.selectedTemplate.getProperty("borderRight");

                if (fborder != null)
                    this.filler.style.borderBottom = fborder;

                else {
                    fborder = this.selectedTemplate.getProperty("border");

                    if (fborder != null)
                        this.filler.style.borderBottom = fborder;
                }
            }
        }

        row.appendChild(this.filler);
    }

    this.table.appendChild(row);
    this.element.appendChild(this.table);

    this.panelElement = document.createElement("div");

    /*
    if (this.panelClass != null)
    this.panelElement.className = this.panelClass;
    */

    if (this.panelTemplate != null) {
        this.panelTemplate.apply(this.panelElement);
    }

    for (var i = 0; i < this.panels.count(); i++) {

        var panel = this.panels.entries[i].value;
        var pElement = panel.createElement();
        pElement.style.display = "none";
        this.panelElement.appendChild(pElement);
    }


    this.element.appendChild(this.panelElement);

    return this.element;
}


IntuitiveLabs.UI.PanelGroup.prototype.adjust = function() {
    var tableWidth = this.table.offsetWidth;
    var width = this.element.offsetWidth;
    var extraWidth = width - tableWidth;

    if (this.navigationType == null || this.navigationType.toLowerCase() == "tabs") {
        
        var fillerWidth = this.filler.offsetWidth;
        fillerWidth += extraWidth;

        if (fillerWidth > 0) {
            this.filler.style.width = fillerWidth.toString() + "px";
        }
        else {
            this.filler.style.width = "0px";
        }
    }

    var nullHeight = true;

    if (this.styleTemplate != null && this.styleTemplate.getProperty("height") != null)
        nullHeight = false;

    if (nullHeight && this.container != null && this.container.parentNode != null) {
        var totalHeight = 0;
        var tHeight = 0;

        for (var i = 0; i < this.container.parentNode.childNodes.length; i++) {
            totalHeight += this.container.parentNode.childNodes[i].offsetHeight;
        }

        //alert("totalHeight=" + totalHeight);

        if (this.container.parentNode.offsetHeight != undefined) {
            var height = this.container.parentNode.offsetHeight;

            var padding = IntuitiveLabs.UI.DOM.getMeasurement(this.container.parentNode, "paddingTop");

            var tableHeight = this.table.offsetHeight;
            var extraHeight = height - totalHeight - tableHeight - padding;

            var panelHeight = this.panelElement.offsetHeight;
            panelHeight += extraHeight;

            //alert(totalHeight);
            //alert(tHeight);        

            if (panelHeight > 0) {
                this.panelElement.style.height = panelHeight.toString() + "px";
            }
        }
    }

    for (var i = 0; i < this.panels.count(); i++) {
        var panel = this.panels.entries[i].value;
        panel.adjust();
    }

}


IntuitiveLabs.UI.PanelGroup.prototype.refresh = function() {
    IntuitiveLabs.UI.DOM.removeChildren(this.element);
    this.createPanelElements();
}



//----------Toolbar---------------  -----

IntuitiveLabs.UI.Toolbar = function(name) {
    IntuitiveLabs.UI.Object.apply(this, arguments);

    this.element = document.createElement("div");
    this.styleTemplate.setProperty("textAlign","right");

    this.name = name;
    this.items = new IntuitiveLabs.Collections.Dictionary();
}

IntuitiveLabs.UI.Toolbar.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.Toolbar.prototype.id = null;
IntuitiveLabs.UI.Toolbar.prototype.name = null;
IntuitiveLabs.UI.Toolbar.prototype.barClass = null;
IntuitiveLabs.UI.Toolbar.prototype.items = null;
IntuitiveLabs.UI.Toolbar.prototype.dataObject = null;

IntuitiveLabs.UI.Toolbar.prototype.addItem = function(id, toolbarItem) {
toolbarItem.parent = this;
    
    toolbarItem.id = id;
    this.items.add(toolbarItem.id, toolbarItem);
}

IntuitiveLabs.UI.Toolbar.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);
    //this.element.style.border = "solid 1px green";

    /*
    if (this.barClass != null) {
        this.element.className = this.barClass;
    }
    else if (this.parent != null) {
        if (this.parent.toolbarClass != undefined && this.parent.toolbarClass != null) {
            this.element.className = this.parent.toolbarClass;
        }
        else if (this.parent.parent != null && this.parent.parent.toolbarClass != undefined && this.parent.parent.toolbarClass != null) {
            this.element.className = this.parent.parent.toolbarClass;
        }
    }
    */

    for (var i = 0; i < this.items.entries.length; i++) {
        this.element.appendChild(this.items.entries[i].value.createElement());
    }

    return this.element;
}

IntuitiveLabs.UI.Toolbar.prototype.hideItem = function(id) {

    var item = this.items.getEntry(id);

    if (item != null) {
        item.value.hide();
    }

}

IntuitiveLabs.UI.Toolbar.prototype.showItem = function(id) {
    var item = this.items.getEntry(id);

    if (item != null) {
        item.value.show();
    }
}

//--------ToolbarItem--------------------
IntuitiveLabs.UI.ToolbarItem = function(text, imageUrl) {
    IntuitiveLabs.UI.Object.apply(this, arguments);

    this.element = document.createElement("div");
    this.element.style.display = "inline";
    this.element.style.cursor = "pointer";
    this.element.style.marginLeft = "5px";
    this.element.style.whiteSpace = "nowrap";

    this.text = text;

    if (imageUrl != undefined)
        this.imageUrl = imageUrl;
}

IntuitiveLabs.UI.ToolbarItem.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.ToolbarItem.prototype.id = null;
IntuitiveLabs.UI.ToolbarItem.prototype.text = null;
IntuitiveLabs.UI.ToolbarItem.prototype.textElement = null;
IntuitiveLabs.UI.ToolbarItem.prototype.image = null;
IntuitiveLabs.UI.ToolbarItem.prototype.imageUrl = null;
IntuitiveLabs.UI.ToolbarItem.prototype.onClick = null;

IntuitiveLabs.UI.ToolbarItem.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);

    this.element.intuitivelabs_parent = this;

    this.element.onclick = function() {

        var obj = this.intuitivelabs_parent;

        if (obj.onClick != null) {
            obj.onClick(obj);
        }
    }

    this.textElement = document.createElement("div");
    this.textElement.style.display = "inline";
    this.textElement.style.verticalAlign = "middle";
    this.textElement.style.paddingRight = "3px";
    this.textElement.style.paddingLeft = "6px";
    this.textElement.innerHTML = this.text;
    this.element.appendChild(this.textElement);

    if (this.imageUrl != null) {
        this.image = new IntuitiveLabs.UI.Image(this.imageUrl);
        this.image.styleTemplate.setProperty("verticalAlign", "middle");
        this.element.appendChild(this.image.createElement());
    }

    return this.element;
}

//----------IconTray--------------------

IntuitiveLabs.UI.IconTray = function() {
    IntuitiveLabs.UI.Object.apply(this, arguments);

    this.element = document.createElement("div");
    this.element.style.display = "inline";
    this.defaultDisplay = "inline";
    this.element.style.verticalAlign = "middle";

    this.icons = new IntuitiveLabs.Collections.Dictionary();
}

IntuitiveLabs.UI.IconTray.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.IconTray.prototype.dataObject = null;

IntuitiveLabs.UI.IconTray.prototype.addIcon = function(id, icon) {
    icon.parent = this;
    icon.id = id;
    this.icons.add(id, icon);
}

IntuitiveLabs.UI.IconTray.prototype.clone = function() {
    var newObj = new IntuitiveLabs.UI.IconTray();
    newObj.parent = this.parent;
    newObj.refresh = this.refresh;
    newObj.defaultDisplay = this.defaultDisplay;

    for (var i = 0; i < this.icons.entries.length; i++) {
        var icon = this.icons.entries[i].value.clone();
        icon.parent = newObj;
        newObj.addIcon(this.icons.entries[i].key, icon);
    }
    //newObj.parent = this.parent;
    //newObj.dataObject = this.dataObject;

    return newObj;
}

IntuitiveLabs.UI.IconTray.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);

    this.element.intuitivelabs_parent = this;

    for (var i = 0; i < this.icons.entries.length; i++) {
        this.element.appendChild(this.icons.entries[i].value.createElement());
    }

    this.refresh();

    return this.element;
}

IntuitiveLabs.UI.IconTray.prototype.hideIcon = function(id) {    
    
    var icon = this.icons.getEntry(id);

    if (icon != null) {
        icon.value.hide();
    }

}

IntuitiveLabs.UI.IconTray.prototype.showIcon = function(id) {
    var icon = this.icons.getEntry(id);

    if (icon != null) {
        icon.value.show();
    }
}

//----------Icon--------------------

IntuitiveLabs.UI.Icon = function(url) {
    IntuitiveLabs.UI.Image.apply(this);
    this.element.style.verticalAlign = "middle";
    this.element.style.display = "inline";
    this.defaultDisplay = "inline";
    this.url = url;
}

IntuitiveLabs.UI.Icon.prototype = new IntuitiveLabs.UI.Image();
IntuitiveLabs.UI.Icon.prototype.isActive = true;
IntuitiveLabs.UI.Icon.prototype.dataObject = null;

IntuitiveLabs.UI.Icon.prototype.clone = function() {
    var newObj = new IntuitiveLabs.UI.Icon(this.url);
    newObj.isActive = this.isActive;
    newObj.dataObject = this.dataObject;
    newObj.parent = this.parent
    newObj.onClick = this.onClick;
    newObj.defaultDisplay = this.defaultDisplay;

    return newObj;
}

IntuitiveLabs.UI.Icon.prototype.createElement = function() {
    IntuitiveLabs.UI.Image.prototype.createElement.call(this);

    this.element.onclick = function() {
        var obj = this.intuitivelabs_parent;

        if (obj.isActive && obj.onClick != null) {
            obj.onClick(obj);
        }
    }
        
    return this.element;
}
