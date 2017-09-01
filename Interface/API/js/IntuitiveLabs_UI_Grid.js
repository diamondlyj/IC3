/*
Copyright 2009, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined || IntuitiveLabs.Data == undefined)
    throw new Error("The library IntuitiveLabs_Data.js must be loaded");

if (IntuitiveLabs.Data == undefined) IntuitiveLabs.Data = {};

IntuitiveLabs.UI.Grid = function() {
    this.rows = new Array();
    this.columns = new IntuitiveLabs.Collections.Dictionary();
}

IntuitiveLabs.UI.Grid.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.Grid.prototype.columns = null;
IntuitiveLabs.UI.Grid.prototype.dataTable = null;
IntuitiveLabs.UI.Grid.prototype.tableClass = null;
IntuitiveLabs.UI.Grid.prototype.includeLineNumber = false;
IntuitiveLabs.UI.Grid.prototype.headerClass = null;
IntuitiveLabs.UI.Grid.prototype.rowClass = null;
IntuitiveLabs.UI.Grid.prototype.rows = null;
IntuitiveLabs.UI.Grid.prototype.sortHeader = null;
IntuitiveLabs.UI.Grid.prototype.isDescending = false;

IntuitiveLabs.UI.Grid.prototype.addColumn = function(column) {
    column.grid = this;
    this.columns.add(column.id, column);
}

IntuitiveLabs.UI.Grid.prototype.createColumn = function(id, headerText, sourceColumn) {
    var column = new IntuitiveLabs.UI.GridColumn(id, headerText, sourceColumn, this);
    this.columns.add(id, column);

    return column;
}

IntuitiveLabs.UI.Grid.prototype.createElement = function() {
    this.element = document.createElement("div");

    if (this.tableClass != null) {
        this.element.className = this.tableClass;
    }

    this.element.style.display = "table";

    var headDiv = document.createElement("div");

    headDiv.style.display = "table-row";

    var cols = this.columns.entries;

    //this.dataTable.columns.entries;

    var lineCol = document.createElement("div");
    lineCol.className = this.headerClass;
    lineCol.style.borderRight = "none";
    
    if (!this.includeLineNumber) {
        lineCol.style.display = "none";
    }
    else {
        lineCol.style.display = "table-cell";
    }

    headDiv.appendChild(lineCol);

    for (var i = 0; i < cols.length; i++) {

        var col = cols[i].value;

        //var header = new IntuitiveLabs.UI.GridColumn(col.name, col.name, this);

        var colDiv = document.createElement("div");
        colDiv.appendChild(col.createElement());

        //this.headers.add(col.name, header);

        if (this.headerClass != null) {
            //alert(this.headerClass);
            colDiv.className = this.headerClass;
        }

        if (i != 0)
            colDiv.style.borderLeft = "none";

        colDiv.style.display = "table-cell";
        //colDiv.innerHTML = ;

        headDiv.appendChild(colDiv);
    }

    this.element.appendChild(headDiv);

    var rows = this.dataTable.rows;

    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];

        var gridRow = this.createRow(row);
        gridRow.lineCell.innerHTML = i + 1;

        this.element.appendChild(gridRow.createElement());
    }

    return this.element;
}

IntuitiveLabs.UI.Grid.prototype.createRow = function(dataRow) {
    var row = new IntuitiveLabs.UI.GridRow(dataRow, this);
    row.parent = this;
    this.rows.push(row);

    return row;
}

IntuitiveLabs.UI.Grid.prototype.sort = function(columnName, sortDescending) {

    if (this.element == null)
        throw Error("The grid can only be sorted after it has been fully created (by calling createElement).");

    if (sortDescending == null)
        sortDescending = false;

    this.isDescending = sortDescending;

    this.dataTable.sort(columnName, sortDescending);

    var cols = this.columns.entries;

    if (this.sortHeader != null)
        this.sortHeader.sorterElement.style.display = "none";

    this.sortHeader = this.columns.getEntry(columnName).value;
    this.sortHeader.sorterElement.style.display = "table-cell";

    if (sortDescending)
        this.sortHeader.sorterElement.childNodes[0].innerHTML = "DESC";
    else
        this.sortHeader.sorterElement.childNodes[0].innerHTML = "ASC";


    for (var i = 0; i < this.dataTable.rows.length; i++) {
        var row = this.dataTable.rows[i];

        for (var j = 0; j < cols.length; j++) {
            var column = cols[j].value;
            var columnID = this.dataTable.columns.getIndex(column.sourceColumn);

            if (column instanceof IntuitiveLabs.UI.GridDataOnDemandColumn) {
                if (row.values[columnID] == undefined)
                    this.rows[i].cells[j].getElement.style.display = "block";
                else
                    this.rows[i].cells[j].getElement.style.display = "none";
            }

            this.rows[i].cells[j].isExpanded = false;
            this.rows[i].cells[j].setValue(row.values[columnID]);

            this.rows[i].dataRow = row;
        }

        //alert(this.rows[i].cells[0].value);
    }
}

//------------GridCell----------------

IntuitiveLabs.UI.GridCell = function(val, isExpanded, row, column) {

this.value = val;    
    this.isExpanded = isExpanded;
    this.row = row;
    this.column = column;
}

IntuitiveLabs.UI.GridCell.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.GridCell.prototype.value = null;
IntuitiveLabs.UI.GridCell.prototype.row = null;
IntuitiveLabs.UI.GridCell.prototype.column = null;
IntuitiveLabs.UI.GridCell.prototype.valueElement = null;
IntuitiveLabs.UI.GridCell.prototype.expanderElement = null;
IntuitiveLabs.UI.GridCell.prototype.isExpanded = false;
//IntuitiveLabs.UI.GridCell.prototype.iconTray = null;

IntuitiveLabs.UI.GridCell.prototype.createElement = function() {
    this.element = document.createElement("div");
    //this.iconTray = new IntuitiveLabs.UI.IconTray();

    var val = this.value;

    this.element.style.cursor = "pointer";

    var valTable = document.createElement("div");
    valTable.style.display = "table";

    var valTable_row = document.createElement("div");
    valTable_row.style.display = "table-row";

    var valTable_leftCell = document.createElement("div");
    valTable_leftCell.style.display = "table-cell";

    if (val instanceof Array) {
        for (var i = 0; i < val.length; i++) {
            if (val[i] != undefined && val[i] != null) {
                var gridVal = new IntuitiveLabs.UI.GridValue(val[i], this);
                valTable_leftCell.appendChild(gridVal.createElement());
            }
        }
        //valTable_leftCell.innerHTML = val[0].toString();
    }
    else {
        if (val != undefined && val != null) {
            var gridVal = new IntuitiveLabs.UI.GridValue(val, this);
            valTable_leftCell.appendChild(gridVal.createElement());
            //valTable_leftCell.innerHTML = val.toString(); ;
        }
    }

    this.valueElement = valTable_leftCell;

    var valTable_rightCell = document.createElement("div");
    valTable_rightCell.style.display = "table-cell";


    var moreDiv = document.createElement("div");
    moreDiv.innerHTML = "+";
    moreDiv.style.color = "#AAAAAA";
    moreDiv.style.border = "solid 1px #AAAAAA";
    moreDiv.style.verticalAlignment = "middle"
    moreDiv.style.fontSize = "9px";
    //moreDiv.style.paddingBottom = "2px";
    moreDiv.style.marginRight = "5px";
    moreDiv.style.width = "11px";
    moreDiv.style.height = "11px";
    moreDiv.style.overflow = "hidden";
    moreDiv.style.textAlign = "center";
    moreDiv.style.verticalAlign = "middle";

    valTable_rightCell.appendChild(moreDiv);

    valTable_rightCell.onclick = function() {
        var obj = this.intuitivelabs_parent;

        if (obj.isExpanded) {
            obj.isExpanded = false;
            this.childNodes[0].innerHTML = "+";
        }
        else {
            obj.isExpanded = true;
            this.childNodes[0].innerHTML = "-";
        }
    }

    valTable_rightCell.onmouseover = function() {
        var obj = this.intuitivelabs_parent;

        if (!obj.isExpanded) {
            //var val = obj.value;
            obj.showAll();

        }
    };

    valTable_rightCell.onmouseout = function() {
        var obj = this.intuitivelabs_parent;

        if (!obj.isExpanded) {
            obj.showSingle();
            /*
            var val = obj.value;

            if (val instanceof Array && val.length > 0 && val[0] != undefined & val[0] != null)
            obj.valueElement.innerHTML = val[0];
            */
        }
    };


    valTable_rightCell.intuitivelabs_parent = this;

    this.expanderElement = valTable_rightCell;

    if (!(val instanceof Array) || val.length < 2)
        this.expanderElement.style.display = "none";

    valTable_row.appendChild(valTable_rightCell);
    valTable_row.appendChild(valTable_leftCell);
    valTable.appendChild(valTable_row);

    this.element.appendChild(valTable);

    return this.element;

}

IntuitiveLabs.UI.GridCell.prototype.showSingle = function() {
    //var val = this.value;
    
    if (this.valueElement.childNodes.length > 0) {
        this.valueElement.childNodes[0].style.display = "block";
        this.valueElement.childNodes[0].style.visibility = "visible";

        for (var i = 1; i < this.valueElement.childNodes.length; i++) {
            this.valueElement.childNodes[i].style.display = "none";
            this.valueElement.childNodes[i].style.visibility = "hidden";
        }
    }

    /*
    if (val instanceof Array) {        
    if (val.length > 0 && val[0] != undefined & val[0] != null) {
    this.valueElement.innerHTML = val[0].toString();
    }
    else
    this.valueElement.innerHTML = "";
    }
    else {
    if (val != undefined && val != null)
    this.valueElement.innerHTML = val.toString();
    else
    this.valueElement.innerHTML = "";
    }
    */
}

IntuitiveLabs.UI.GridCell.prototype.showAll = function() {
    //var val = this.value;


    for (var i = 0; i < this.valueElement.childNodes.length; i++) {
        this.valueElement.childNodes[i].style.display = "block";
        this.valueElement.childNodes[i].style.visibility = "visible";
    }

    /*
    if (val instanceof Array) {
        
    var str = "";

        for (var i = 0; i < val.length; i++) {
    str += val[i] + "<br>";
    }

        this.valueElement.innerHTML = str;
    }
    else {
    if (val != undefined && val != null)
    this.valueElement.innerHTML = val.toString();
    else
    this.valueElement.innerHTML = "";
    }
    */
}

IntuitiveLabs.UI.GridCell.prototype.setValue = function(val) {
    this.value = val;

    if (this.valueElement != null) {
        if (val instanceof Array) {
            for (var i = 0; i < val.length; i++) {
                if (i >= this.valueElement.childNodes.length) {
                    var valNode = new IntuitiveLabs.UI.GridValue(val[i], this);
                    this.valueElement.appendChild(valNode.createElement());
                    //alert("creating:" + val[i]);
                }
                else {
                    //alert("setting");
                    var obj = this.valueElement.childNodes[i].intuitivelabs_parent;
                    obj.setValue(val[i]);
                }
            }

            while (val.length < this.valueElement.childNodes.length) {
                //alert("removing");
                this.valueElement.removeChild(this.valueElement.childNodes[val.length]);
            }
        }
        else {
            if (val != undefined && val != null) {
                if (this.valueElement.childNodes.length == 0) {
                    var valNode = new IntuitiveLabs.UI.GridValue(val, this);
                    this.valueElement.appendChild(valNode.createElement());
                }
                else {
                    var obj = this.valueElement.childNodes[0].intuitivelabs_parent;
                    obj.setValue(val);
                }

                while (this.valueElement.childNodes.length > 1) {
                    this.valueElement.removeChild(this.valueElement.childNodes[1]);
                }
            }
            else {
                IntuitiveLabs.UI.DOM.removeChildren(this.valueElement);
            }
        }

        if (!(val instanceof Array) || val.length < 2)
            this.expanderElement.style.display = "none";
        else {
            this.expanderElement.style.display = "table-cell";
            this.expanderElement.childNodes[0].innerHTML = "+";
            //this.isExpanded = false;
        }



        if (this.isExpanded)
            this.showAll();
        else
            this.showSingle();
    }
}

//------------GridDataOnDemandCell-------------

IntuitiveLabs.UI.GridDataOnDemandCell = function(row,column) {
    this.isExpanded = false;
    this.value = null;
    this.row = row;
    this.column = column;
}

IntuitiveLabs.UI.GridDataOnDemandCell.prototype = new IntuitiveLabs.UI.GridCell();
IntuitiveLabs.UI.GridDataOnDemandCell.prototype.getElement = null;
IntuitiveLabs.UI.GridDataOnDemandCell.prototype.isInitialized = false;

IntuitiveLabs.UI.GridDataOnDemandCell.prototype.createElement = function() {
    var innerElement = IntuitiveLabs.UI.GridCell.prototype.createElement.call(this);
    var div = document.createElement("div");

    var getDiv = document.createElement("div");
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

    getDiv.onclick = function() {
        var obj = this.intuitivelabs_parent;

        obj.isSet = true;

        if (obj.column.onclick != null) {
            obj.column.onclick(obj);
        }
    }

    this.getElement = getDiv;

    div.appendChild(this.getElement);
    div.appendChild(innerElement);

    this.element = div;

    return this.element;
}

IntuitiveLabs.UI.GridDataOnDemandCell.prototype.initializeValue = function(val) {
    //if( this.isSet )
    this.isInitialized = true;
    this.getElement.style.display = "none";
       
    this.row.dataRow.setValue(this.column.sourceColumn, val);

    IntuitiveLabs.UI.GridCell.prototype.setValue.call(this, val);
}

//------------GridColumn-------------------

IntuitiveLabs.UI.GridColumn = function(id, text, sourceColumn, grid) {
    this.id = id;
    this.grid = grid;
    this.sourceColumn = sourceColumn,
    this.text = text;
}

IntuitiveLabs.UI.GridColumn.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.GridColumn.prototype.grid = null;
IntuitiveLabs.UI.GridColumn.prototype.text = null;
IntuitiveLabs.UI.GridColumn.prototype.iconTray_cell = null;
IntuitiveLabs.UI.GridColumn.prototype.id = null;
IntuitiveLabs.UI.GridColumn.prototype.isSortable = true;
IntuitiveLabs.UI.GridColumn.prototype.sorterElement = null;
IntuitiveLabs.UI.GridColumn.prototype.sourceColumn = null;
IntuitiveLabs.UI.GridColumn.prototype.onclick = null;
IntuitiveLabs.UI.GridColumn.prototype.onSelectCell = null;

IntuitiveLabs.UI.GridColumn.prototype.getValue = function( dataObject ) {
    return dataObject.value;
}

IntuitiveLabs.UI.GridColumn.prototype.createElement = function() {
    this.element = document.createElement("div");

    var table = document.createElement("div");
    table.style.display = "table";

    var row = document.createElement("div");
    row.style.display = "table-row";

    var leftCell = document.createElement("div");
    leftCell.style.display = "table-cell";
    leftCell.innerHTML = this.text;
    row.appendChild(leftCell);

    this.sorterElement = document.createElement("div");
    this.sorterElement.style.display = "none";
    var orderDiv = document.createElement("div");
    orderDiv.innerHTML = "ASC";
    orderDiv.style.border = "solid 1px #AAAAAA";
    orderDiv.style.color = "#AAAAAA";
    orderDiv.fontSize = "10px";
    orderDiv.style.marginLeft = "5px";

    this.sorterElement.appendChild(orderDiv);

    row.appendChild(this.sorterElement);

    table.appendChild(row);

    this.element.appendChild(table);

    this.element.style.cursor = "pointer";
    this.element.intuitivelabs_parent = this;

    if (this.isSortable) {
        this.element.onclick = function() {
            var obj = this.intuitivelabs_parent;

            if (obj != obj.grid.sortHeader || obj.grid.isDescending) {
                obj.grid.isDescending = false;
            }
            else {
                obj.grid.isDescending = true;
            }

            obj.grid.sort(obj.sourceColumn, obj.grid.isDescending);
        }
    }

    return this.element;
}


//--GridDataOnDemandColumn
IntuitiveLabs.UI.GridDataOnDemandColumn = function(id, text, sourceColumn, grid) {
    IntuitiveLabs.UI.GridColumn.apply(this, arguments);
    //this.id = id;
    //this.grid = grid;
    //this.text = text;
    //this.sourceColumn = sourceColumn;
}

IntuitiveLabs.UI.GridDataOnDemandColumn.prototype = new IntuitiveLabs.UI.GridColumn();


//---------------GridRow---------------------

IntuitiveLabs.UI.GridRow = function(dataRow, grid) {
    this.grid = grid;
    this.dataRow = dataRow;
    this.lineCell = null;
    this.cells = new Array();

    this.lineCell = document.createElement("div");
    this.lineCell.style.display = "table-cell";
    this.lineCell.className = this.grid.rowClass;
    this.lineCell.style.borderTop = "none";
    this.lineCell.style.borderRight = "none";
}

IntuitiveLabs.UI.GridRow.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.GridRow.prototype.grid = null;

IntuitiveLabs.UI.GridRow.prototype.createCell = function(val, isExpanded, column) {
    var cell = new IntuitiveLabs.UI.GridCell(val, isExpanded, this, column);
    this.cells.push(cell);
    return cell;
}

IntuitiveLabs.UI.GridRow.prototype.createElement = function() {
    this.element = document.createElement("div");
    this.element.style.display = "table-row";

    this.element.appendChild(this.lineCell);

    
    if (this.parent != null && this.parent.includeLineNumber) {
        this.lineCell.style.display = "table-cell";
    }
    else {
        this.lineCell.style.display = "none";
    }


    var cols = this.grid.columns.entries;

    for (var j = 0; j < cols.length; j++) {

        var column = cols[j].value;

        var cellDiv = document.createElement("div");

        if (this.grid.rowClass != null)
            cellDiv.className = this.grid.rowClass;

        cellDiv.style.display = "table-cell";

        if (j == 0)
            cellDiv.style.backgroundColor = "#F5F5F5";
        else {
            //alert(IntuitiveLabs.UI.DOM.getProperty(cellDiv,"borderLeft"));
            cellDiv.style.borderLeft = "none";
        }

        //if (i != 0 )
        cellDiv.style.borderTop = "none";

        var columnID = this.grid.dataTable.columns.getIndex(column.sourceColumn);


        if (column instanceof IntuitiveLabs.UI.GridDataOnDemandColumn) {
            //var val = "on demand";

            //var gridCell = this.createCell(val, false);
            var gridCell = new IntuitiveLabs.UI.GridDataOnDemandCell(this, column);
            this.cells.push(gridCell);

            //var getIcon = document.createElement("div");
            //getIcon.innerHTML = "Get";

            //cellDiv.appendChild(getIcon);
            cellDiv.appendChild(gridCell.createElement());
        }
        else {

            var val = this.dataRow.values[columnID];

            var gridCell = this.createCell(val, false, column);

            cellDiv.appendChild(gridCell.createElement());
        }

        this.element.appendChild(cellDiv);
    }

    return this.element;
}

//------GridValue-----------------
IntuitiveLabs.UI.GridValue = function(val, parent) {
    IntuitiveLabs.UI.Object.apply(this);
    this.parent = parent;
    
    if (this.parent != null && this.parent.column != null && this.parent.column.iconTray_cell != null) {
        this.iconTray = this.parent.column.iconTray_cell.clone();
    }
    else {
        this.iconTray = new IntuitiveLabs.UI.IconTray();
    }
    
    this.iconTray.parent = this;

    this.setValue(val);
}

IntuitiveLabs.UI.GridValue.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.GridValue.prototype.value = null;
IntuitiveLabs.UI.GridValue.prototype.textElement = null;
IntuitiveLabs.UI.GridValue.prototype.iconTray = null;

IntuitiveLabs.UI.GridValue.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);

    this.element.intuitivelabs_parent = this;

    this.textElement = document.createElement("div");
    this.textElement.style.display = "inline";
    this.textElement.style.verticalAlign = "middle";

    this.textElement.innerHTML = this.getValue();

    this.element.appendChild(this.textElement);

    if (this.iconTray != null) {
        this.element.appendChild(this.iconTray.createElement());
    }

    if (this.parent.column != null && this.parent.column.onSelectCell != null) {
        this.element.onclick = function() {
            var obj = this.intuitivelabs_parent;
            obj.parent.column.onSelectCell(obj.parent);
        }
    }

    return this.element;
}

IntuitiveLabs.UI.GridValue.prototype.setValue = function(val) {
    this.value = val;

    var dataCell = function() { };
    dataCell.value = this.value;
    dataCell.row = null;
    dataCell.column = null;

    if (this.parent != null && this.parent.column != null && this.parent.column.iconTray_cell != null) {

        if (this.parent.row != null && this.parent.row.dataRow)
            dataCell.row = this.parent.row.dataRow;

        if (this.parent.column != null && this.parent.column.sourceColumn != null)
            dataCell.column = this.parent.column.sourceColumn;
    }

    this.iconTray.dataObject = dataCell;

    this.refresh();
}

IntuitiveLabs.UI.GridValue.prototype.getValue = function() {
    if (this.parent.column != null) {
        return this.parent.column.getValue(this);
    }

    return this.value;
}

IntuitiveLabs.UI.GridValue.prototype.refresh = function() {
    if (this.textElement != null) {
        this.textElement.innerHTML = this.getValue();
    }

    this.iconTray.refresh();
}
