/*
Copyright 2009-2011, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined || IntuitiveLabs.UI == undefined)
    throw new Error("The library IntuitiveLabs_UI.js must be loaded");

if (IntuitiveLabs.Collections == undefined)
    throw new Error("The library IntuitiveLabs_Collections.js must be loaded");

if (IntuitiveLabs.UI.Menu == undefined)
    throw new Error("The library IntuitiveLabs_Menu.js must be loaded");

IntuitiveLabs.UI.ObjectList = function() {
    this.items = new IntuitiveLabs.Collections.Dictionary();
    this.columns = new IntuitiveLabs.Collections.Dictionary();
    this.displayHeader = false;

    this.pager = new IntuitiveLabs.UI.Pager();
    this.pager.parent = this;

    this.menu = new IntuitiveLabs.UI.Menu();

    this.element = document.createElement("div");
}

IntuitiveLabs.UI.ObjectList.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.ObjectList.prototype.headerClass = null;
IntuitiveLabs.UI.ObjectList.prototype.listClass = null;
IntuitiveLabs.UI.ObjectList.prototype.columns = null;
IntuitiveLabs.UI.ObjectList.prototype.displayHeader = null;
IntuitiveLabs.UI.ObjectList.prototype.pager = null;
IntuitiveLabs.UI.ObjectList.prototype.infoElement = null;
IntuitiveLabs.UI.ObjectList.prototype.info = "OBJECTS";
IntuitiveLabs.UI.ObjectList.prototype.menu = null;
IntuitiveLabs.UI.ObjectList.prototype.menuText = "Menu";
IntuitiveLabs.UI.ObjectList.prototype.onClickImage = null;

IntuitiveLabs.UI.ObjectList.prototype.addItem = function(item) {
    item.parent = this;
    this.items.add(item.id, item);
}

IntuitiveLabs.UI.ObjectList.prototype.addColumn = function(column) {
    column.parent = this;
    this.columns.add(column.propertyName, column);
}

IntuitiveLabs.UI.ObjectList.prototype.adjust = function() {    
    this.adjustHeightToParent();
    IntuitiveLabs.UI.DOM.adjustHeightToParent( this.listElement );

    return;


    if (this.element != undefined && this.element != null) {

        try {
            var width = this.element.parentNode.offsetWidth;
            var height = this.element.parentNode.offsetHeight;


            /*
            //Measure the height of the borders
            var topBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.element.parentNode, "borderTopWidth")
            + IntuitiveLabs.UI.DOM.getMeasurement(this.element.parentNode, "paddingTop");

            var bottomBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.element.parentNode, "borderBottomWidth");
            +IntuitiveLabs.UI.DOM.getMeasurement(this.element.parentNode, "paddingBottom");

            var listTopBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.listElement, "borderTopWidth");
            var listBottomBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.listElement, "borderBottomWidth");

            var heightBorder = topBorder + bottomBorder + listTopBorder + listBottomBorder;

            //Measure the width of the borders
            var leftBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.element.parentNode, "borderLeftWidth")
            + IntuitiveLabs.UI.DOM.getMeasurement(this.element.parentNode, "paddingLeft");

            var rightBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.element.parentNode, "borderRightWidth");
            +IntuitiveLabs.UI.DOM.getMeasurement(this.element.parentNode, "paddingRight");

            var listLeftBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.listElement, "borderLeftWidth");
            var listRightBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.listElement, "borderRightWidth");

            var widthBorder = leftBorder + rightBorder + listLeftBorder + listRightBorder;

            var tableLeftBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.listElement.firstChild, "borderLeftWidth");
            var tableRightBorder = IntuitiveLabs.UI.DOM.getMeasurement(this.listElement.firstChild, "borderRightWidth");
            var tableBorder = tableLeftBorder + tableRightBorder;

            //Set the widths an heights
            var listHeight = height - heightBorder;

            this.headerElement.style.borderRightWidth = tableRightBorder + "px";
            this.headerElement.style.borderLeftWidth = tableRightBorder + "px";

            //var listWidth = this.headerElement.offsetHeight;

            listHeight = listHeight - this.headerElement.offsetHeight;

            this.listElement.style.height = listHeight + "px";

            var listWidth = width - widthBorder;
            this.listElement.style.width = listWidth + "px";


            //Extend the lasst cell to the edge of the container element
            var lastCellIndex = this.listElement.firstChild.firstChild.childNodes.length - 1;

            var lastCellWidth = this.listElement.firstChild.firstChild.childNodes[lastCellIndex].offsetWidth;
            var tableWidth = this.listElement.firstChild.offsetWidth
            var tableMinusLastWidth = tableWidth - lastCellWidth;

            this.listElement.firstChild.firstChild.childNodes[lastCellIndex].style.width = (width - tableMinusLastWidth).toString() + "px";

            //Adjust size of columns in header to match table below            
            for (var i = 0; i <= this.columns.count(); i++) {
            this.headerElement.firstChild.childNodes[i].style.width = this.listElement.firstChild.firstChild.childNodes[i].offsetWidth + "px";
            }
            */
        }
        catch (exc) {
        }
    }

}

IntuitiveLabs.UI.ObjectList.prototype.clearItems = function() {
    this.items.clear();
}

IntuitiveLabs.UI.ObjectList.prototype.countItems = function() {
    return this.items.count();
}

IntuitiveLabs.UI.ObjectList.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);
    //this.element.style.border = "solid 1px red";

    //this.element.style.height = "100%";
    //this.element.style.border = "none";

    //Above evreything a small area to describe what the list is about, its current content etc.
    var toolbar = document.createElement("div");
    toolbar.style.borderBottom = "dotted 1px gray";
    toolbar.style.padding = "4px";
    toolbar.style.fontSize = "12px";

    this.infoElement = document.createElement("div");
    this.infoElement.innerHTML = this.info;
    toolbar.appendChild(this.infoElement);

    var exp = new IntuitiveLabs.UI.TextObject(this.menuText);
    exp.styleTemplate.setProperty("textDecoration", "underline")
    exp.parent = this;
    exp.onMouseOver = function(sender) { sender.parent.menu.show(); };
    exp.onMouseOut = function(sender) { sender.parent.menu.hide(); };
    this.menu.parent = exp;
    //exp.styleTemplate.setProperty("cursor", "pointer")
    //exp.onClick = function() { alert("export"); };
    //exp.appendChild(this.menu.createElement());

    var el = exp.createElement();

    el.appendChild(this.menu.createElement());

    toolbar.appendChild(el);

    if (this.menu.items.length == 0)
        exp.hide();

    this.element.appendChild(toolbar);

    //ADD Pager
    this.element.appendChild(this.pager.createElement());

    /*
    this.headerElement = document.createElement("DIV");
    this.headerElement.style.whiteSpace = "nowrap";
    this.headerElement.style.display = "table";
    this.headerElement.style.cursor = "pointer";
    this.headerElement.style.border = "solid 1px green";

    var headerRow = document.createElement("DIV");
    headerRow.style.display = "table-row";

    var defCls = this.headerClass;

    //Add blank cell to match up with image cell below
    var blankCell = document.createElement("DIV")
    blankCell.innerHTML = '<span style="font-size:1px;">&nbsp;</span>';

    if (defCls != undefined && defCls != null)
    blankCell.className = defCls;

    headerRow.appendChild(blankCell);    

    //Now add the actual headers
    for (var j = 0; j < this.columns.count(); j++) {
    var cls = this.columns.entries[j].value.headerClass;

        var cell = document.createElement("DIV")
    cell.style.verticalAlign = "middle";
    cell.innerHTML = this.columns.entries[j].value.headerText;

        if (cls != undefined && cls != null)
    cell.className = cls;
        
    else if (defCls != undefined && defCls != null)
    cell.className = defCls;

        cell.style.display = "table-cell";

        cell.IntuitiveLabs_parent = this.columns.entries[j].value;

        headerRow.appendChild(cell);
    }

    this.headerElement.appendChild(headerRow);    

    if (!this.displayHeader)
    this.headerElement.style.display = "none";

    this.element.appendChild(this.headerElement);
    */

    //The element that lists the objects
    this.listElement = document.createElement("div");
    this.listElement.style.overflow = "auto";
    this.listElement.style.padding = "5px";
    //this.listElement.style.border = "solid 1px blue";

    this.listElement.appendChild(this.createTableOfItems());

    //this.element = this.listElement;
    //return this.element;

    this.element.appendChild(this.listElement);

    return this.element;
}

IntuitiveLabs.UI.ObjectList.prototype.createTableOfItems = function() {

    var table = document.createElement("DIV");
    table.style.display = "table";

    //if a class has been defined for the list (listClass), apply it to the table. This allows, for exanple,
    //to add a padding without affecting the display of scrolling (which occrs in the parent (this.listElement))

    if (this.listClass != undefined && this.listClass != null)
        table.className = this.listClass;

    ///List of items
    for (var i = 0; i < this.countItems(); i++) {
        var row = document.createElement("DIV");

        row.style.display = "table-row";
        row.style.whiteSpace = "nowrap";
        row.style.cursor = "pointer";
        row.IntuitiveLabs_parent = this.items.entries[i].value;

        var rowClass = this.items.entries[i].value.className;

        if (rowClass != undefined && rowClass != null)
            row.className = rowClass;

        var imgCell = document.createElement("DIV");
        imgCell.style.display = "table-cell";
        imgCell.style.verticalAlign = "middle";

        var imgUrl = this.items.entries[i].value.imageUrl;

        if (imgUrl != undefined && imgUrl != null) {
            var img = document.createElement("IMG");
            img.src = imgUrl;

            imgCell.appendChild(img);

            imgCell.onclick = function() {
            if (this.parentNode.IntuitiveLabs_parent.parent.onClickImage != null) {
                    var holder = new IntuitiveLabs.UI.ListCell(this.parentNode.IntuitiveLabs_parent, null);                
                    this.parentNode.IntuitiveLabs_parent.parent.onClickImage(holder);
                }
            }
        }

        row.appendChild(imgCell);

        for (var j = 0; j < this.columns.count(); j++) {
            var cell = document.createElement("DIV")
            cell.style.display = "table-cell";
            cell.style.verticalAlign = "middle";

            if (this.columns.entries[j].value.columnType == "data")
                cell.innerHTML = this.items.entries[i].value.dataObject[this.columns.entries[j].value.propertyName];
            else
                cell.innerHTML = this.columns.entries[j].value.headerText;

            //If a class has been defined for the columns, overwrite the class defined for the row (should this be reversed?).
            var colClass = this.columns.entries[j].value.itemClass;

            if (colClass != undefined && colClass != null)
                cell.className = colClass;

            //cell.IntuitiveLabs_columnIndex = j + 1;
            var listCell = new IntuitiveLabs.UI.ListCell(this.items.entries[i].value, this.columns.entries[j].value);
            cell.intuitivelabs_parent = listCell;
            listCell.parent = this;

            var handler = this.items.entries[i].value.onClick;
            var colHandler = this.columns.entries[j].value.onClickItem;

            cell.onclick = function() {
                var sender = this.intuitivelabs_parent;

                if (sender.column.onClickItem != undefined && sender.column.onClickItem != null)
                    sender.column.onClickItem(sender);
            }

            row.appendChild(cell);
            //obj.innerHTML = this.properties.entries[i].key + "<br/>";
        }

        table.appendChild(row);
    }

    return table;
}

IntuitiveLabs.UI.ObjectList.prototype.refresh = function() {
    IntuitiveLabs.UI.DOM.removeChildren(this.listElement);
    this.listElement.appendChild(this.createTableOfItems());
    this.infoElement.innerHTML = this.info;
    IntuitiveLabs.UI.DOM.adjustHeightToParent(this.listElement);
    this.pager.refresh();
}

IntuitiveLabs.UI.ObjectList.prototype.removeItem = function(key) {
    this.items.remove(key);
}

IntuitiveLabs.UI.ObjectList.prototype.setInfo = function(text) {
    this.info = text;
    
    if (this.infoElement != null)
        this.infoElement.innerHTML = this.info;
}

/*
IntuitiveLabs.UI.ObjectList.prototype.render = function(container) {
    container.appendChild(this.createElement());
    this.adjust();
}
*/

//--------ListCell----------------------
IntuitiveLabs.UI.ListCell = function(item, column) {
    this.item = item;
    this.column = column;
}

IntuitiveLabs.UI.ListCell.prototype = {
    item: null,
    column: null,
    parent: null
}

//--------LisColumn-------------

IntuitiveLabs.UI.ListColumn = function(columnType, propertyName, headerText, onClickItem) {
    if( columnType == undefined || columnType == null )
        throw new Error("The type of the column (function, data) must be define.");
    
    if( columnType != "data" && columnType != "function")
        throw new Error("Unrecognised column type (" + columnType + "). The type of the column can only be the following: function, data");
        
    
    this.columnType = columnType.toLowerCase();
    
    this.propertyName = propertyName;
    this.headerText = headerText;
    this.onClickItem = onClickItem;
}

IntuitiveLabs.UI.ListColumn.prototype = {
    columType: null,
    headerText: null,
    propertyName: null,
    itemClass: null,    
    headerClass: null,
    onClickItem: null,
    parent: null
}

//Item

IntuitiveLabs.UI.ListItem = function(id, dataObject, imageUrl, className) {
    this.dataObject = dataObject;
    this.imageUrl = imageUrl;
    this.className = className;
    this.id = id;
}

IntuitiveLabs.UI.ListItem.prototype = {
    id: null,
    dataObject: null,
    imageUrl: null,
    className: null,
    onClick: null
}

//----Pager------

IntuitiveLabs.UI.Pager = function() {
    this.element = document.createElement("div");
    this.element.align = "center";
    this.element.style.cursor = "pointer";
}

IntuitiveLabs.UI.Pager.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.Pager.prototype.first = "A";
IntuitiveLabs.UI.Pager.prototype.last = "Z";
IntuitiveLabs.UI.Pager.prototype.position = 0;
IntuitiveLabs.UI.Pager.prototype.count = 0;
IntuitiveLabs.UI.Pager.prototype.itemsPerPage = 100;
IntuitiveLabs.UI.Pager.prototype.previousElement = null;
IntuitiveLabs.UI.Pager.prototype.nextElement = null;
IntuitiveLabs.UI.Pager.prototype.firstElement = null;
IntuitiveLabs.UI.Pager.prototype.middleElement = null;
IntuitiveLabs.UI.Pager.prototype.lastElement = null;
IntuitiveLabs.UI.Pager.prototype.onNext= null;
IntuitiveLabs.UI.Pager.prototype.onPrevious = null;
IntuitiveLabs.UI.Pager.prototype.onPage = null;

IntuitiveLabs.UI.Pager.prototype.createElement = function() {

    IntuitiveLabs.UI.Object.prototype.createElement.call(this);

    this.element.intuitivelabs_parent = this;

    this.previousElement = document.createElement("div");
    this.previousElement.style.display = "inline";
    this.previousElement.style.paddingRight = "5px";
    this.previousElement.style.verticalAlign = "middle";
    this.previousElement.innerHTML = "&lt;&lt",

    this.previousElement.onclick = function() {
        var obj = this.parentNode.intuitivelabs_parent;

        obj.position -= obj.itemsPerPage;

        if (obj.position < 0)
            obj.position = 0;

        if (obj.onPage != null)
            obj.onPage(obj);

        if (obj.onPrevious != null)
            obj.onPrevious(obj);
    }

    this.element.appendChild(this.previousElement);

    this.firstElement = document.createElement("div");
    this.firstElement.style.display = "inline";
    this.firstElement.style.verticalAlign = "middle";
    this.firstElement.innerHTML = this.first;
    this.element.appendChild(this.firstElement);

    this.middleElement = document.createElement("div");
    this.middleElement.style.display = "inline";
    this.middleElement.style.verticalAlign = "middle";
    this.middleElement.innerHTML = "-";
    this.element.appendChild(this.middleElement);

    this.lastElement = document.createElement("div");
    this.lastElement.style.display = "inline";
    this.lastElement.style.verticalAlign = "middle";
    this.lastElement.innerHTML = this.last;
    this.element.appendChild(this.lastElement);

    this.nextElement = document.createElement("div");
    this.nextElement.style.display = "inline";
    this.nextElement.style.paddingLeft = "5px";
    this.nextElement.style.verticalAlign = "middle";
    this.nextElement.innerHTML = "&gt;&gt",

    this.nextElement.onclick = function() {
        var obj = this.parentNode.intuitivelabs_parent;

        obj.position += obj.itemsPerPage;

        if (obj.position > (obj.count - 1)) {
            if (obj.count > 0)
                obj.position = obj.count - 1;
            else
                obj.position = 0;
        }

        if (obj.onPage != null)
            obj.onPage(obj);

        if (obj.onNext != null)
            obj.onNext(obj);
    }

    this.element.appendChild(this.nextElement);

    this.refresh();


    return this.element;
}

IntuitiveLabs.UI.Pager.prototype.refresh = function() {
    this.firstElement.innerHTML = this.first;
    this.lastElement.innerHTML = this.last;

    if (this.position < (this.itemsPerPage - 1))
        this.previousElement.style.visibility = "hidden";
    else
        this.previousElement.style.visibility = "visible";

    if ((this.position + this.itemsPerPage - 1) < this.count)
        this.nextElement.style.visibility = "visible";
    else
        this.nextElement.style.visibility = "hidden";

    if ((this.count - this.position) == 1) {
        this.middleElement.style.visibility = "hidden";
        this.lastElement.style.visibility = "hidden";
    }
    else{
        this.middleElement.style.visibility = "visible";
        this.lastElement.style.visibility = "visible";
    }
}
