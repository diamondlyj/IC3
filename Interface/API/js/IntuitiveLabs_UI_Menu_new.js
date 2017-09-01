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
IntuitiveLabs.UI.Menu.prototype.itemsElement = null;
IntuitiveLabs.UI.Menu.prototype.expandsLeft = false;
IntuitiveLabs.UI.Menu.prototype.current = null;

IntuitiveLabs.UI.Menu.prototype.addItem = function(menuItem) {
    if (!(menuItem instanceof IntuitiveLabs.UI.MenuItem)) {
        throw Error("A Menu object can only contain items that are of the type MenuItem.");
    }

    menuItem.parent = this;

    this.items.push(menuItem);
}

IntuitiveLabs.UI.Menu.prototype.adjust = function() {
    /*
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
    */
}

IntuitiveLabs.UI.Menu.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);

    //this.element = document.createElement("div");
    this.element.style.cursor = "pointer";
    this.element.style.backgroundColor = "white";
    this.element.style.border = "solid 1px silver";
    this.element.style.position = "absolute";
    this.element.style.visibility = "hidden";

    this.element.style.top = "0px";
    this.element.style.left = "0px";

    this.element.style.zIndex = "10000";

    this.itemsElement = document.createElement("div");
    this.itemsElement.style.border = "solid red 1px";
    //this.itemsElement.style.maxHeight = "35px";
    this.itemsElement.style.overflow = "hidden";

    for (var i = 0; i < this.items.length; i++) {
        var el = this.items[i].createElement();
        this.itemsElement.appendChild(el);

        if (this.items[i].subMenu != null) {
            this.element.appendChild(this.items[i].subMenu.createElement());
        }
    }

    this.element.appendChild(this.itemsElement);

    //this.element.onmouseover = function() {
    //    console.log("suma");
    //}


    return this.element;
}

IntuitiveLabs.UI.Menu.prototype.setCurrent = function(menuItem) {
    //console.log("S");

    if (this.current != null) {
        if (this.current != menuItem) {
            if (this.current.subMenu != null) {
                //this.current.subMenu.hideItems();
                //this.current.subMenu.hide();
                this.current.subMenu.element.style.visibility = "hidden";                                
            }
            if (menuItem.subMenu != null) {
                menuItem.subMenu.show();
            }
        }
        else {
            if (menuItem.subMenu != null) {
                menuItem.subMenu.hideItems();
                menuItem.subMenu.current = null;
            }
        }
    }
    else if (menuItem.subMenu != null) {
        menuItem.subMenu.show();
    }

    this.current = menuItem;
}


IntuitiveLabs.UI.Menu.prototype.hideItems = function() {
    for (var i = 0; i < this.items.length; i++) {
        this.items[i].hide();
    }
}

/*
IntuitiveLabs.UI.Menu.prototype.checkCurrent = function(menuItem) {
    console.log(menuItem.text);
    
    if (this.current != null && this.current != menuItem && this.current.subMenu != null) {
        this.current.subMenu.hide();

    }
}
*/

IntuitiveLabs.UI.Menu.prototype.hide = function() {
    this.element.style.visibility = "hidden";
    //this.hideItems();


    //for (var i = 0; i < this.items.length; i++) {
    //    this.items[i].hide();
    //}
}

IntuitiveLabs.UI.Menu.prototype.show = function() {

    this.element.style.visibility = "visible";

    if (this.parent != null) {

        var top = this.parent.element.offsetTop - 1;

        var left = this.parent.element.offsetLeft;

        if (this.parent instanceof IntuitiveLabs.UI.MenuItem) {
            left += this.parent.element.offsetWidth;
            top += 1;
        }
        else {
            top += this.parent.element.offsetHeight;
        }

        this.element.style.top = top + "px";
        this.element.style.left = left + "px";
    }
}

//-------------MenuItem--------------------

IntuitiveLabs.UI.MenuItem = function(text, description) {
    IntuitiveLabs.UI.Object.apply(this);
    this.text = text;
    this.items = new Array();
}

IntuitiveLabs.UI.MenuItem.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.MenuItem.prototype.dataObject = null;
IntuitiveLabs.UI.MenuItem.prototype.subMenu = null;
IntuitiveLabs.UI.MenuItem.prototype.text = "item";
IntuitiveLabs.UI.MenuItem.prototype.onClick = null;

//65526
IntuitiveLabs.UI.MenuItem.prototype.adjust = function() {
    //if (this.subMenu != null) {
    //    this.subMenu.adjust();
    //}
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

        if (obj.parent != null) {
            obj.parent.setCurrent(obj);
            //obj.subMenu.show();
            //obj.subMenu.element.style.visibility = "visible";
        }
    }


    this.element.onmouseout = function() {
        var obj = this.intuitivelabs_parent;

        this.style.backgroundColor = "transparent";

        if (obj.parent != null) {
            //obj.parent.checkCurrent(obj);
            //obj.subMenu.hide();
            //obj.subMenu.element.style.visibility = "hidden";
        }

    }


    /*
    if (this.subMenu != null) {
    var sub = this.subMenu.createElement();
    this.element.appendChild(sub);
    }
    */

    return this.element;
}

IntuitiveLabs.UI.MenuItem.prototype.show = function() {
    this.element.visibility = "visible";
}

IntuitiveLabs.UI.MenuItem.prototype.hide = function() {
    this.element.visibility = "hidden";

    if (this.subMenu != null) {
        this.subMenu.hide();
    }
}

IntuitiveLabs.UI.MenuItem.prototype.setSubMenu = function(subMenu) {
    subMenu.parent = this;
    this.subMenu = subMenu;
}
