/*
Copyright 2009, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/
IntuitiveLabs.Clone = function() { };

IntuitiveLabs.clone = function(obj) {
    IntuitiveLabs.Clone.prototype = new obj();
    return new IntuitiveLabs.Clone();
}

if (IntuitiveLabs == undefined || IntuitiveLabs.Collections == undefined)
    throw new Error("The library IntuitiveLabs_Collections.js must be loaded");

if (IntuitiveLabs.UI == undefined) IntuitiveLabs.UI = {};
if (IntuitiveLabs.UI.DOM == undefined) IntuitiveLabs.UI.DOM = {};

IntuitiveLabs.UI.getKeyCode = function(evt) {
    if (!evt)
        return event.keyCode;
    else
        return evt.which;
}

IntuitiveLabs.UI.DOM.removeChildren = function(node) {

    while (node.firstChild) {
        IntuitiveLabs.UI.DOM.removeChildren(node.firstChild);
        node.removeChild( node.firstChild );
    }
}

IntuitiveLabs.UI.DOM.adjustHeightToParent = function(element, ignoreSyblings) {
    if (element.parentNode != null) {

        //Due to differences in the css box model and javascript offsetHeight we have to do some recalculations
        //offsetHeight = css height + css border + css padding (margin is not included);
        //alert("parentNode.offsetHeight:" + element.parentNode.offsetHeight);

        var parentTopHeight = IntuitiveLabs.UI.DOM.getMeasurement(element.parentNode, "borderTopWidth") + IntuitiveLabs.UI.DOM.getMeasurement(element.parentNode, "paddingTop");
        var parentBottomHeight = IntuitiveLabs.UI.DOM.getMeasurement(element.parentNode, "borderBottomWidth") + IntuitiveLabs.UI.DOM.getMeasurement(element.parentNode, "paddingBottom");

        //For parent we subtract the padding and the border (but not the margin), since this defines the area where conent can be legitimately placed.

        var parentHeight = element.parentNode.offsetHeight - parentTopHeight - parentBottomHeight;

        //alert("parentHeight:" + parentHeight);

        //Since the CSS height excludes the margin, padding and border, these have to be subtracted later
        var t = IntuitiveLabs.UI.DOM.getMeasurement(element, "borderTopWidth") + IntuitiveLabs.UI.DOM.getMeasurement(element, "paddingTop") + IntuitiveLabs.UI.DOM.getMeasurement(element, "marginTop");
        var b = IntuitiveLabs.UI.DOM.getMeasurement(element, "borderBottomWidth") + IntuitiveLabs.UI.DOM.getMeasurement(element, "paddingBottom") + IntuitiveLabs.UI.DOM.getMeasurement(element, "marginBottom");
        var extrasHeight = t + b;


        var siblingsHeight = 0;

        if (!ignoreSyblings) {
            for (var i = 0; i < element.parentNode.childNodes.length; i++) {

                if (element.parentNode.childNodes[i] != element) {

                    //alert(element.parentNode.childNodes[i].innerHTML);

                    //These calculations dop not take into account that syblings can be inline! Add at some point.

                    //For syblings, we have to measure the offsetHeight + the margin, since that defines the entrie realestate occupied.
                    //This heigh cannot be filled by the element
                    var child = element.parentNode.childNodes[i];

                    var disp = IntuitiveLabs.UI.DOM.getProperty(child, "display");

                    if (disp == undefined || disp == null || disp != "bug") {
                        var childTopMargin = IntuitiveLabs.UI.DOM.getMeasurement(child, "marginTop");
                        var childBottomMargin = IntuitiveLabs.UI.DOM.getMeasurement(child, "marginBottom");


                        //alert("child.offsetHeight:" + child.offsetHeight);

                        var childHeight = childTopMargin + childBottomMargin;

                        if (child.offsetHeight != undefined) {
                            childHeight += child.offsetHeight;
                        }

                        //alert(childHeight);

                        siblingsHeight += childHeight;
                    }
                }
            }
        }

        //var newHeight = parentHeight - siblingsHeight;
        var newHeight = parentHeight - siblingsHeight - extrasHeight;

        //alert("extrasHeight:" + extrasHeight);
        //alert("newHeight:" + newHeight);

        if (newHeight > 0)
            element.style.height = newHeight.toString() + "px";
    }
}

IntuitiveLabs.UI.DOM.alterRight = function(element, value) {
    element.style.right = value + "px";
}


IntuitiveLabs.UI.DOM.getValue = function(node) {
    if (node.text != undefined) {
        return node.text;
    }
    else {
        return node.childNodes[0].nodeValue;
    }
}


IntuitiveLabs.UI.DOM.getMeasurement = function(node, property) {

    var n = parseInt(IntuitiveLabs.UI.DOM.getProperty(node, property));        

    if (isNaN(n))
        return 0;
    else
        return n;
}

IntuitiveLabs.UI.DOM.getCummulativeOffset = function(element) {
    //Code must be added to take scrolling into account
    var position = function() { };

    var node = element;
    var posx = 0;
    var posy = 0;

    while (node != null) {

        var adjustment = 0;

        //alert(node.offsetHeight);


        if (node.offsetTop != undefined)
            posy += node.offsetTop;

        if (node.offsetLeft != undefined)
            posx += node.offsetLeft;

        //if (node.offsetTop != undefined);

        node = node.offsetParent;
    }

    position.left = posx;
    position.top = posy;

    return position;
}

IntuitiveLabs.UI.DOM.getCummulativeScroll = function(element) {
    //Code must be added to take scrolling into account
    var position = function() { };

    var node = element;
    var posx = 0;
    var posy = 0;

    while (node != null) {

        var adjustment = 0;

        if (node.scrollTop != undefined)
            posy += node.scrollTop;

        if (node.scrollLeft != undefined)
            posx += node.scrollLeft;


        node = node.offsetParent;
    }

    position.left = posx;
    position.top = posy;

    return position;
}


IntuitiveLabs.UI.DOM.getProperty = function(node, property) {

    if (node.currentStyle) {

        var prop = "";

        for (var i = 0; i < property.length; i++) {

            if (property.charAt(i) == '-') {
                i++;
                prop += property.charAt(i).toUpperCase();
            }
            else
                prop += property.charAt(i);
        }

        return node.currentStyle[prop];
    }
    else {
        var prop = "";

        for (var i = 0; i < property.length; i++) {
            var code = property.charCodeAt(i);

            if (code > 64 && code < 90 && i > 0 && property.charAt(i - 1) != '-')
                prop += "-";

            prop += String.fromCharCode(code);
        }

        prop = prop.toLowerCase();


        try {
            return document.defaultView.getComputedStyle(node, null).getPropertyValue(prop);
        }
        catch (err) {
            return "";
            //throw new Error(prop + " failed to compute;");
        }
    }
}


IntuitiveLabs.UI.DOM.getWidthOfEdges = function(node) {
    var borderLeft = IntuitiveLabs.UI.DOM.getMeasurement(node, "borderLeftWidth");
    var borderRight = IntuitiveLabs.UI.DOM.getMeasurement(node, "borderRightWidth");

    var paddingLeft = IntuitiveLabs.UI.DOM.getMeasurement(node, "paddingLeft");
    var paddingRight = IntuitiveLabs.UI.DOM.getMeasurement(node, "paddingRight");

    return borderLeft + borderRight + paddingLeft + paddingRight;
}

IntuitiveLabs.UI.DOM.getHeightOfEdges = function(node) {
    var borderTop = IntuitiveLabs.UI.DOM.getMeasurement(node, "borderTopWidth");
    var borderBottom = IntuitiveLabs.UI.DOM.getMeasurement(node, "borderBottomWidth");

    var paddingTop = IntuitiveLabs.UI.DOM.getMeasurement(node, "paddingTop");
    var paddingBottom = IntuitiveLabs.UI.DOM.getMeasurement(node, "paddingBottom");

    return borderTop + borderBottom + paddingTop + paddingBottom;
}


IntuitiveLabs.UI.DOM.sizeWidthToParent = function(node) {
    if (node.parentNode.offsetWidth != undefined) {

        var width = node.parentNode.offsetWidth;

        var withOfEdges = IntuitiveLabs.UI.DOM.getWidthOfEdges(node);
        var pWithOfEdges = IntuitiveLabs.UI.DOM.getWidthOfEdges(node.parentNode);

        var newWidth = width - withOfEdges - pWithOfEdges;

        if (newWidth > 0) {
            node.style.width = newWidth.toString() + "px";
        }
    }
}
//This has been deprecated. Use adjustHeightToParent
IntuitiveLabs.UI.DOM.sizeHeightToParent = function(node, deductSyblings) {
    if (node.parentNode.offsetHeight != undefined) {

        var height = node.parentNode.offsetHeight;

        var heightOfEdges = IntuitiveLabs.UI.DOM.getHeightOfEdges(node);
        var pHeightOfEdges = IntuitiveLabs.UI.DOM.getHeightOfEdges(node);

        var newHeight = height - heightOfEdges - pHeightOfEdges;

        if (deductSyblings) {

            for (var i = 0; i < node.parentNode.childNodes.length; i++) {
                var childNode = node.parentNode.childNodes[i];

                if (childNode != node) {
                    newHeight -= childNode.offsetHeight;
                }
            }
        }

        if (newHeight > 0) {
            node.style.height = newHeight.toString() + "px";
        }
    }
}


IntuitiveLabs.UI.DOM.sizeToParent = function(node,deductSyblings) {
    IntuitiveLabs.UI.DOM.sizeWidthToParent(node);
    IntuitiveLabs.UI.DOM.sizeHeightToParent(node,deductSyblings);
}

/*
IntuitiveLabs.UI.DOM.slideLeft = function(element, x, speed) {

    var width = IntuitiveLabs.UI.DOM.getMeasurement(element, "marginLeft")
            + element.offsetWidth
            + IntuitiveLabs.UI.DOM.getMeasurement(element, "marginRight");

    IntuitiveLabs.UI.DOM.slideLeft_oneStep(element, x, speed);
}
*/


//------------Object---------------
IntuitiveLabs.UI.Object = function() {
    this.element = document.createElement("div");
    this.styleTemplate = new IntuitiveLabs.UI.StyleTemplate();
}


IntuitiveLabs.UI.Object.prototype = {
    autoAdjust: true,
    element: null,
    id: null,
    isHidden: false,
    parent: null,
    container: null,
    styleTemplate: null,
    defaultDisplay: "block",
    onCreated: null,
    onMouseOver: null,
    onMouseOut: null,
    onSlide_stop: null,
    onSlideLeft_stop: null,
    onSlideRight_stop: null,
    onHide: null,

    adjust: function() {
    },

    adjustToParent: function() {
        this.adjustHeightToParent();
    },


    adjustHeightToParent: function() {
        IntuitiveLabs.UI.DOM.adjustHeightToParent(this.element);
    },


    hide: function() {
        this.isHidden = true;

        //alert("hide " + this.id + ":" + this.defaultDisplay);

        if (this.element != null) {
            this.element.style.display = "none";
            this.element.style.visibility = "hidden";
        }

        if (this.onHide != null)
            this.onHide(this);

    },

    render: function(container) {
        this.container = container;
        this.container.appendChild(this.createElement());
        this.adjust();
    },

    refresh: function() {
    },

    show: function() {
        this.isHidden = false;

        if (this.element != null) {
            this.element.style.visibility = "visible";
            this.element.style.display = this.defaultDisplay;
        }
    },

    test: function(x, y) {
        alert(x + y);
    },

    createElement: function() {
        if (this.element != null)
            IntuitiveLabs.UI.DOM.removeChildren(this.element);
        else
            throw new Error("An element must be created in the constructor using document.createElement(tagName).");

        this.styleTemplate.apply(this.element);

        /*
        var disp = IntuitiveLabs.UI.DOM.getProperty(this.element, "display");
        
        if (disp != undefined && disp != null && disp.toLowerCase != "none")
        this.defaultDisplay = disp;
            
        */

        this.element.intuitivelabs_parent = this;

        //if (this.onCreated != null) {
        //    this.onCreated(this);
        //}

        return this.element;
    },

    clone: function() {
        //Clone.prototype = new this;
        return IntuitiveLabs.clone(this);
    },

    slide_oneStep: function(self, x, direction, delay, stepSize) {

        var offset = stepSize;

        if (offset > x)
            offset = x;

        //alert("x:" + x);

        if (this.element.style.left != "auto") {
            this.element.style.left = (this.element.offsetLeft + (offset * direction)).toString() + "px";
        }

        //var wsize = document.body.clientWidth;

        //Dangerous! This is not an assured value if it hasn't been set!!!
        if (element.style.right != "auto") {
            var right = IntuitiveLabs.UI.DOM.getMeasurement(this.element, "right");
            this.element.style.right = (right - (offset * direction)).toString() + "px";
            //alert(element.style.right);
        }

        x -= offset;

        if (x > 0) {
            //alert("Repeat:" + x);
            setTimeout(function() { self.slide_oneStep(self, x, direction, delay, stepSize) }, delay);
        }
        else {
            if (self.onSlide_stop != null)
                self.onSlide_stop(self);

            if (direction == -1) {
                if (this.onSlideLeft_stop != null)
                    this.onSlideLeft_stop(this);
            }
            else if (this.onSlideRight_stop != null)
                this.onSlideRight_stop(this);

            //alert("stopd at:" + x);
        }

        //IntuitiveLabs.UI.DOM.slideLeft(this.element, x, speed);
    },

    slideLeft: function(x, delay, stepSize) {

        this.slide_oneStep(this, x, -1, delay, stepSize);

        //IntuitiveLabs.UI.DOM.slideLeft(this.element, x, speed);
    },

    slideRight: function(x, delay, stepSize) {

        this.slide_oneStep(this, x, 1, delay, stepSize);

        //IntuitiveLabs.UI.DOM.slideLeft(this.element, x, speed);
    }
}


//----------StyleTemplate--------------------
IntuitiveLabs.UI.StyleTemplate = function() {
    this.properties = new IntuitiveLabs.Collections.Dictionary();
}

IntuitiveLabs.UI.StyleTemplate.prototype = {

    properties: null,

    apply: function(element) {
        for (var i = 0; i < this.properties.entries.length; i++) {

            var key = this.properties.entries[i].key;
            var val = this.properties.entries[i].value;

            try {
                var str = 'element.style.' + key + '="' + val + '"';
                eval(str);
            }
            catch (err) {
                throw new Error('Cannot apply the style "' + key + '" with value "' + val + '"');
            }
        }
    },

    clone: function() {
        var template = new IntuitiveLabs.UI.StyleTemplate();

        for (var n in this.properties.entries) {
            key = this.properties.entries[n].key;
            val = this.properties.entries[n].value;

            template.properties.add(key, val);
        }

        return template;
    },

    getProperty: function(propertyName) {
        var prop = this.properties.getEntry(propertyName);

        if (prop != null)
            return prop.value;

        return null;
    },

    isEmpty: function() {
        if (this.properties.entries.length == 0)
            return true;
        else
            return false;
    },

    setProperty: function(propertyName, value) {
        var prop = this.properties.getEntry(propertyName);

        if (prop != null)
            prop.value = value;
        else
            this.properties.add(propertyName, value);
    }
}

//-----------Image-------------------


IntuitiveLabs.UI.Image = function(url) {
    IntuitiveLabs.UI.Object.apply(this);
    this.element = document.createElement("img");
    this.url = url;
    //this.element.style.display = "inline";
}


IntuitiveLabs.UI.Image.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.Image.prototype.url = null;
IntuitiveLabs.UI.Image.prototype.onClick = null;

IntuitiveLabs.UI.Image.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);
    this.refresh();

    this.element.intuitivelabs_parent = this;

    this.element.onclick = function() {
        var obj = this.intuitivelabs_parent;

        if (obj.onClick != null) {
            obj.onClick(obj);
        }
    }

    return this.element;
}

IntuitiveLabs.UI.Image.prototype.refresh = function() {
    if (this.url != null)
        this.element.src = this.url;
}




//-----------TextObject-------------------
IntuitiveLabs.UI.TextObject = function(text) {
    IntuitiveLabs.UI.Object.apply(this, arguments);

    this.text = text;
    //this.element = document.createElement("div");
    //this.element.intuitivelabs_parent = this;
    //this.element.style.display = "inline";
}

IntuitiveLabs.UI.TextObject.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.TextObject.prototype.text = "";

IntuitiveLabs.UI.TextObject.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this);
    this.element.style.display = "inline";

    this.refresh();

    if (this.onCreated != null) {
        this.onCreated(this);
    }

    if (this.onMouseOver != null) {
        this.element.style.cursor = "pointer";
        this.element.onmouseover = function() {
            this.intuitivelabs_parent.onMouseOver(this.intuitivelabs_parent);
        }
    }

    if (this.onMouseOut != null) {
        this.element.style.cursor = "pointer";
        this.element.onmouseout = function() {
            this.intuitivelabs_parent.onMouseOut(this.intuitivelabs_parent);
        }
    }

    if (this.onClick != null) {
        this.element.style.cursor = "pointer";
        this.element.onclick = function() {
            this.intuitivelabs_parent.onClick(this.intuitivelabs_parent);
        }
    }

    return this.element;
}

IntuitiveLabs.UI.TextObject.prototype.refresh = function() {
    this.element.innerHTML = this.text;
}

//------------Window---------------------------
IntuitiveLabs.UI.Window = function() {
    this.children = new IntuitiveLabs.Collections.Dictionary();
}

IntuitiveLabs.UI.Window.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.Window.prototype.adjust = function() {

    for (var i in this.children.entries) {
        //alert(this.children.entries[i].value)
        if( this.children.entries[i].value.autoAdjust == true )
            this.children.entries[i].value.adjust();
    }
}


