/*
Copyright 2011, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined || IntuitiveLabs.UI == undefined)
    throw new Error("The library IntuitiveLabs_UI.js must be loaded");

if (IntuitiveLabs.Collections == undefined)
    throw new Error("The library IntuitiveLabs_Collections.js must be loaded");

//-------------Menu--------------------

IntuitiveLabs.UI.Menu = function(parent) {
    IntuitiveLabs.UI.Object.apply(this);
    this.parent = parent;
    this.items = new Array();
}

IntuitiveLabs.UI.Menu.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.Menu.prototype.items = null;
IntuitiveLabs.UI.Menu.prototype.expandsLeft = false;

IntuitiveLabs.UI.Menu.prototype.addItem = function(menuItem) {
    if (!(menuItem instanceof IntuitiveLabs.UI.MenuItem)) {
        throw Error("A Menu object can only contain items that are of the type MenuItem.");
    }

    menuItem.parent = this;

    this.items.push(menuItem);
}

IntuitiveLabs.UI.Menu.prototype.adjust = function() {
    if (this.parent != null) {
        this.element.style.top = this.parent.element.offsetTop + "px";

        if (!this.expandsLeft) {
            this.element.style.left = this.parent.element.offsetWidth + "px";

        }
        else {
            this.element.style.left = "-" + this.element.offsetWidth + "px";
        }
    }

    for (var i = 0; i < this.items.length; i++) {
        this.items[i].adjust();
    }
}

IntuitiveLabs.UI.Menu.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);

    //this.element = document.createElement("div");
    this.element.style.cursor = "pointer";
    this.element.style.backgroundColor = "white";
    this.element.style.border = "solid 1px silver";
    //this.element.style.padding = "4px";
    this.element.style.position = "absolute";
    this.element.style.visibility = "hidden";

    this.element.style.top = "0px";
    this.element.style.left = "0px";

    this.element.style.zIndex = "10000";

    for (var i = 0; i < this.items.length; i++) {
        var el = this.items[i].createElement();
        this.element.appendChild(el);
    }

    return this.element;
}

IntuitiveLabs.UI.Menu.prototype.show = function() {
    if (this.element.parentNode != undefined && this.element.parentNode != null) {

        var top = this.element.parentNode.offsetTop + this.element.parentNode.offsetHeight - 1;
        this.element.style.top = top + "px";

        var left = this.element.parentNode.offsetLeft;
        this.element.style.left = left + "px";
    }

    IntuitiveLabs.UI.Object.prototype.show.call(this);

}

//-------------MenuItem--------------------

IntuitiveLabs.UI.MenuItem = function(text, description) {
    IntuitiveLabs.UI.Object.apply(this);
    this.text = text;
    this.items = new Array();
}

IntuitiveLabs.UI.MenuItem.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.MenuItem.prototype.dataObject = null;
IntuitiveLabs.UI.MenuItem.prototype.menu = null;
IntuitiveLabs.UI.MenuItem.prototype.subMenu = null;
IntuitiveLabs.UI.MenuItem.prototype.text = "item";
IntuitiveLabs.UI.MenuItem.prototype.onClick = null;

//65526
IntuitiveLabs.UI.MenuItem.prototype.adjust = function() {
    if (this.subMenu != null) {
        this.subMenu.adjust();
    }
}

IntuitiveLabs.UI.MenuItem.prototype.createElement = function() {
    this.element = document.createElement("div");
    this.element.style.cursor = "pointer";
    this.element.style.fontWeight = "normal";
    this.element.style.padding = "5px";
    this.element.style.whiteSpace = "nowrap";
    this.element.innerHTML = this.text;
    this.element.intuitivelabs_parent = this;

    this.element.onclick = function(e) {
        var obj = this.intuitivelabs_parent;

        if (obj.onClick != null) {
            if (e && e.stopPropagation) {
                e.stopPropagation();
            }
            else if (window.event) {
                window.event.cancelBubble = true;
            }

            obj.onClick(obj);
        }
    }

    this.element.onmouseover = function() {
        var obj = this.intuitivelabs_parent;

        this.style.backgroundColor = "#F1F1F1";

        if (obj.subMenu != null) {
            obj.subMenu.element.style.visibility = "visible";
        }
    }

    this.element.onmouseout = function() {
        var obj = this.intuitivelabs_parent;

        this.style.backgroundColor = "transparent";

        if (obj.subMenu != null) {
            obj.subMenu.element.style.visibility = "hidden";
        }

    }

    if (this.subMenu != null) {
        var sub = this.subMenu.createElement();
        this.element.appendChild(sub);
    }

    return this.element;
}

IntuitiveLabs.UI.MenuItem.prototype.setSubMenu = function(subMenu) {
    subMenu.parent = this;
    this.subMenu = subMenu;
}
