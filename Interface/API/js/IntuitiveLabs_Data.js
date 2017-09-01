/*
Copyright 2009, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined || IntuitiveLabs.Collections == undefined)
    throw new Error("The library IntuitiveLabs_Collections.js must be loaded");

if (IntuitiveLabs.Data == undefined) IntuitiveLabs.Data = {};

IntuitiveLabs.Data.Column = function( name, defaultValue ) {
    this.name = name;
    this.defaultValue = defaultValue
}

IntuitiveLabs.Data.Column.prototype = {
    name: null,
    table: null,
    dataObject: null
}

IntuitiveLabs.Data.Table = function() {
    this.columns = new IntuitiveLabs.Collections.Dictionary();        
    this.rows = new Array();
}

IntuitiveLabs.Data.Table.prototype = {
    columns: null,
    rows: null,
    sortIndex: -1,
    sortDescending: false,

    createColumn: function(name, defaultValue) {
        var column = new IntuitiveLabs.Data.Column(name, defaultValue);
        column.table = this;

        this.columns.add(name, column);

        for (var i = 0; i < this.rows.length; i++) {
            this.rows[i].values.push(defaultValue);
        }

        return column;
    },

    createRow: function() {
        var row = new IntuitiveLabs.Data.Row();

        for (var i = 0; i < this.columns.entries.length; i++) {
            var val = null;

            if (i < arguments.length)
                val = arguments[i];
            else
                val = this.columns.entries[i].value.defaultValue;

            row.values.push(val);
        }

        row.table = this;

        this.rows.push(row);

        return row;
    },


    count: function() {
        return this.rows.length;
    },

    /*
    getColumnIndex: function(columnName) {
    var n = 0;

        for (var i = 0; i < this.columns.length; i++) {
    if (this.columns[i] = columnName)
    return i;
    }

        throw Error("There is no column by the name " + columnName);
    }*/

    sort: function(columnName, descending) {

        if (descending != null)
            this.sortDescending = descending;

        if (!this.columns.contains(columnName))
            throw Error("Sort failed. No column named " + columnName);

        this.sortIndex = this.columns.getIndex(columnName);

        var comparer = function() { };
        var innerComparer = function() { };

        if (descending) {
            comparer = function(a, b) {
                if ((a.sortValue == undefined || a.sortValue == null || a.sortValue == "") && b.sortValue != undefined && b.sortValue != null && b.sortValue != "")
                    return 1;
                else if ((b.sortValue == undefined || b.sortValue == null || b.sortValue == "") && a.sortValue != undefined && a.sortValue != null && a.sortValue != "")
                    return -1;

                return (a.sortValue < b.sortValue) ? 1 : (a.sortValue > b.sortValue) ? -1 : 0;
            };

            innerComparer = function(a, b) {
                var x = a;

                if (a.nodeName && a.ownerDocument)
                    x = IntuitiveLabs.UI.DOM.getValue(a);

                var y = b;

                if (b.nodeName && b.ownerDocument)
                    y = IntuitiveLabs.UI.DOM.getValue(b);

                return (x < y) ? 1 : (x > y) ? -1 : 0;
            };
        }
        else {
            comparer = function(a, b) {
                if ((a.sortValue == undefined || a.sortValue == null || a.sortValue == "") && b.sortValue != undefined && b.sortValue != null && b.sortValue != "") 
                    return -1;                
                else if ((b.sortValue == undefined || b.sortValue == null || b.sortValue == "") && a.sortValue != undefined && a.sortValue != null && a.sortValue != "")
                    return 1;

                return (a.sortValue < b.sortValue) ? -1 : (a.sortValue > b.sortValue) ? 1 : 0;
            };

            innerComparer = function(a, b) {
                var x = a;

                if (a != undefined && a.nodeName && a.ownerDocument)
                    x = IntuitiveLabs.UI.DOM.getValue(a);

                var y = b;

                if (b != undefined && b.nodeName && b.ownerDocument)
                    y = IntuitiveLabs.UI.DOM.getValue(b);

                return (x < y) ? -1 : (x > y) ? 1 : 0;
            };
        }

        for (var i = 0; i < this.rows.length; i++) {
            var val = this.rows[i].values[this.sortIndex];

            if (val instanceof Array) {
                val.sort(innerComparer);

                this.rows[i].sortValue = val[0];

                var x = val[0];

                if (val[0] != undefined && val[0].nodeName && val[0].ownerDocument)
                    x = IntuitiveLabs.UI.DOM.getValue(val[0]);

                this.rows[i].sortValue = x;
            }
            else {
                var x = val;

                if (val != undefined && val.nodeName && val.ownerDocument)
                    x = IntuitiveLabs.UI.DOM.getValue(val);

                this.rows[i].sortValue = x;
            }
        }

        this.rows.sort(comparer);
    }
}

IntuitiveLabs.Data.Row = function(key, value) {
    this.values = new Array();    
    this.values.sort = function(){};
}

IntuitiveLabs.Data.Row.prototype = {
    values: null,
    table: null,
    sortValue: null,

    getValue: function(columnName) {
        if (this.table != null) {
            var colIndex = this.table.columns.getIndex(columnName);
            
            if( colIndex < 0 )
                throw Error("The table has no column named " + columnName);
                
            return this.values[colIndex];
        }

        return null;
    },
    
    setValue: function(columnName, value){
        if (this.table != null) {
            var colIndex = this.table.columns.getIndex(columnName);
            
            if( colIndex < 0 )
                throw Error("The table has no column named " + columnName);
                        
            this.values[colIndex] = value;
        }
    }
}
