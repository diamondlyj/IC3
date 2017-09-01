/*
Copyright 2011, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined || IntuitiveLabs.UI == undefined)
    throw new Error("The library IntuitiveLabs_UI.js must be loaded");

if (IntuitiveLabs.Collections == undefined)
    throw new Error("The library IntuitiveLabs_Collections.js must be loaded");

//-------------Walk--------------------

IntuitiveLabs.UI.Walk = function() {
    IntuitiveLabs.UI.Object.apply(this);
    this.styleTemplate.setProperty("fontSize", "12px");
    this.styleTemplate.setProperty("color", "#444444");
    this.styleTemplate.setProperty("overflow", "hidden");
    //this.styleTemplate.setProperty("border", "solid 1px green");
    this.styleTemplate.setProperty("position", "absolute");
    this.styleTemplate.setProperty("right", "0px");
    //this.styleTemplate.setProperty("textOverflow", "clip");
    this.points = new Array();
}

IntuitiveLabs.UI.Walk.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.Walk.prototype.points = null;
IntuitiveLabs.UI.Walk.prototype.onClickPoint = null;

IntuitiveLabs.UI.Walk.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);
    //this.element.align = "right";
    return this.element;
}

IntuitiveLabs.UI.Walk.prototype.addPoint = function(text, dataObject) {
    var point = new IntuitiveLabs.UI.WalkPoint(text, dataObject);
    point.parent = this;

    if (this.element != null) {
        if (this.points.length > 0) {
            var sep = new IntuitiveLabs.UI.WalkSeparator(this.points[this.points.length - 1], point);
            this.element.appendChild(sep.createElement());
        }

        this.element.appendChild(point.createElement());
    }

    this.points.push(point);
}

//-------------WalkPoint--------------------

IntuitiveLabs.UI.WalkPoint = function(text, dataObject) {
    IntuitiveLabs.UI.Object.apply(this);
    this.styleTemplate.setProperty("paddingLeft","5px");
    this.styleTemplate.setProperty("paddingRight", "5px");
    this.styleTemplate.setProperty("cursor", "pointer");
    this.text = text;
    this.dataObject = dataObject;
}

IntuitiveLabs.UI.WalkPoint.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.WalkPoint.prototype.text = "";
IntuitiveLabs.UI.WalkPoint.prototype.onClick = null;
IntuitiveLabs.UI.WalkPoint.prototype.dataObject = null;

IntuitiveLabs.UI.WalkPoint.prototype.createElement = function() {
    this.styleTemplate.setProperty("display", "inline");
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);
    this.element.innerHTML = this.text;
    this.element.intuitivelabs_parent = this;

    this.element.onclick = function() {
        var obj = this.intuitivelabs_parent;

        if (obj.onClick != null) {
            obj.onClick(obj);
        }
        else if (obj.parent != null && obj.parent.onClickPoint != null) {
            obj.parent.onClickPoint(obj);
        }

    }

    return this.element;
}

//-------------WalkSeparator--------------------

IntuitiveLabs.UI.WalkSeparator = function(antecedent, precedent) {
    IntuitiveLabs.UI.Object.apply(this);
    this.styleTemplate.setProperty("paddingLeft", "5px");
    this.styleTemplate.setProperty("paddingRight", "5px");
    this.antecedent = antecedent;
    this.precedent = precedent;
}

IntuitiveLabs.UI.WalkSeparator.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.WalkSeparator.prototype.precedent = null;
IntuitiveLabs.UI.WalkSeparator.prototype.antecedent = null;

IntuitiveLabs.UI.WalkSeparator.prototype.createElement = function() {
    this.styleTemplate.setProperty("display", "inline");
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);
    this.element.innerHTML = "&gt";

    return this.element;
}
