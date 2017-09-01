/*
Copyright 2009, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined) var IntuitiveLabs = {};
if (IntuitiveLabs.Collections == undefined) IntuitiveLabs.Collections = {};

String.prototype.trim = function() {
    return this.replace(/^\s*/, "").replace(/\s*$/, "");
}

IntuitiveLabs.Collections.Dictionary = function() {
    this.entries = new Array();    
}

IntuitiveLabs.Collections.Dictionary.prototype = {
    entries: null,

    add: function(key, value) {

        if (this.contains(key))
            throw Error("collection already contains the key " + key);

        var entry = new IntuitiveLabs.Collections.DictionaryEntry(key, value);

        this.entries.push(entry);
    },

    contains: function(key) {
        for (var i = 0; i < this.entries.length; i++) {
            if (this.entries[i].key == key)
                return true;
        }

        return false;
    },

    count: function() {
        return this.entries.length;
    },

    getEntry: function(key) {
        for (var i = 0; i < this.entries.length; i++) {
            if (this.entries[i].key == key)
                return this.entries[i];
        }

        return null;
    },

    getIndex: function(key) {
        for (var i = 0; i < this.entries.length; i++) {
            if (this.entries[i].key == key)
                return i;
        }

        return -1;        
    },

    clear: function() {
        this.entries = new Array();
    },

    remove: function(key) {
        for (var i = 0; i < this.entries.length; i++) {
            if (this.entries[i].key == key) {
                this.entries.splice(i, 1);
                break;
            }
        }

        return;
    },

    sortByValues: function(descending, comparer) {

        if (comparer == null) {
            comparer = function(subject, predicate) {
                if (subject.value < predicate.value) {
                    return (descending) ? 1 : -1;
                }
                else if (subject.value > predicate.value) {
                    return (descending) ? -1 : 1;
                }
                else {
                    return 0;
                }

            }
        }

        this.entries.sort(comparer);
    }
}

IntuitiveLabs.Collections.DictionaryEntry = function(key, value) {    
    
    this.value = value;
    this.key = key;
}

IntuitiveLabs.Collections.DictionaryEntry.prototype = {
    key: null,
    value: null
}

/*---------------------------List------------------------------*/

IntuitiveLabs.Collections.List = function() {
    this.items = new Array();
}

IntuitiveLabs.Collections.List.prototype = {
    items: null,

    add: function( item ) {
        this.items.push(item);
    },

    contains: function(item) {
        for (var i = 0; i < this.items.length; i++) {
            if (this.entries[i] == item)
                return true;
        }

        return false;
    },

    count: function() {
        return this.items.length;
    },

    getIndex: function(item) {
        for (var i = 0; i < this.items.length; i++) {
            if (this.items[i] == item)
                return i;
        }

        return -1;
    },

    clear: function() {
        this.items = new Array();
    },

    remove: function(item) {
        for (var i = 0; i < this.items.length; i++) {
            if (this.items[i] == item) {
                this.items.splice(i, 1);
                break;
            }
        }

        return;
    },

    sort: function(descending, comparer) {

        if (comparer == null) {
            comparer = function(subject, predicate) {
                if (subject < predicate) {
                    return (descending) ? 1 : -1;
                }
                else if (subject > predicate) {
                    return (descending) ? -1 : 1;
                }
                else {
                    return 0;
                }

            }
        }

        this.items.sort(comparer);
    }
}
