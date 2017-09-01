if (IntuitiveLabs == undefined || IntuitiveLabs.UI == undefined)
    throw new Error("The library IntuitiveLabs_UI.js must be loaded");

//----------Widget------------------------
IntuitiveLabs.UI.Widget = function() {
    IntuitiveLabs.UI.Object.apply(this.arguments);
    
    this.objects = new IntuitiveLabs.Collections.Dictionary();
    this.element = document.createElement("div");
}

IntuitiveLabs.UI.Widget.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.Widget.constructor = IntuitiveLabs.UI.Widget;
IntuitiveLabs.UI.Widget.prototype.objects = null;
IntuitiveLabs.UI.Widget.prototype.isFixed = false;

IntuitiveLabs.UI.Widget.prototype.affix = function(left, top, right, bottom) {
    this.isFixed = true;
    this.setLeft(left);
    this.setRight(right);
    this.setBottom(bottom);
    this.setTop(top);
    this.element.style.position = "fixed";
}

IntuitiveLabs.UI.Widget.prototype.addObject = function(id, obj) {
    //IntuitiveLabs.UI.Object.prototype.test.call(this, 2, 3);
    obj.parent = this;
    this.objects.add(id, obj);
}

IntuitiveLabs.UI.Widget.prototype.adjust = function() {
    IntuitiveLabs.UI.DOM.adjustHeightToParent(this.element);

    for (var n in this.objects.entries) {
        var obj = this.objects.entries[n].value;
        
        if( obj.autoAdjust == true )
            obj.adjust();
    }
}

IntuitiveLabs.UI.Widget.prototype.createElement = function() {

    IntuitiveLabs.UI.Object.prototype.createElement.call(this);

    for (var i = 0; i < this.objects.entries.length; i++) {
        this.element.appendChild(this.objects.entries[i].value.createElement());
    }

    return this.element;
}

IntuitiveLabs.UI.Widget.prototype.getObject = function(id) {
    var obj = this.objects.getEntry(id);

    if (obj != null)
        return obj.value;
    else
        return null;
}

IntuitiveLabs.UI.Widget.prototype.removeObject = function(id) {
    this.objects.remove(id);
}

IntuitiveLabs.UI.Widget.prototype.refresh = function() {

    IntuitiveLabs.UI.DOM.removeChildren(this.element);

    for (var i = 0; i < this.objects.entries.length; i++) {
        this.element.appendChild(this.objects.entries[i].value.createElement());
    }

}

IntuitiveLabs.UI.Widget.prototype.setHeight = function(height) {
    if (isNaN(height))
        this.element.style.height = height;
    else
        this.element.style.height = height.toString() + "px";
}

IntuitiveLabs.UI.Widget.prototype.setWidth = function( width ) {
    if (isNaN(width))
        this.element.style.width = width;
    else
        this.element.style.width = width.toString() + "px";
}

IntuitiveLabs.UI.Widget.prototype.setBottom = function(x) {
    if (x != null) {        
        if (isNaN(x))
            this.element.style.bottom = x;
        else
            this.element.style.bottom = x.toString() + "px";
    }
}

IntuitiveLabs.UI.Widget.prototype.setLeft = function(x) {
    if (x != null) {        
        if (isNaN(x))
            this.element.style.left = x;
        else
            this.element.style.left = x.toString() + "px";
    }
}

IntuitiveLabs.UI.Widget.prototype.setRight = function(x) {    
    if (x != null) {
        if (isNaN(x))
            this.element.style.right = x;
        else
            this.element.style.right = x.toString() + "px";
    }
}

IntuitiveLabs.UI.Widget.prototype.setTop = function(x) {
    if (top != null) {
        if (isNaN(x))
            this.element.style.top = x;
        else
            this.element.style.top = x.toString() + "px";
    }
}

IntuitiveLabs.UI.Widget.prototype.slideLeft = function(x) {
    IntuitiveLabs.UI.Object.prototype.slideLeft.call(this, x, 1, 100);
}

IntuitiveLabs.UI.Widget.prototype.slideRight = function(x) {
    IntuitiveLabs.UI.Object.prototype.slideRight.call(this, x, 1, 100);
}

