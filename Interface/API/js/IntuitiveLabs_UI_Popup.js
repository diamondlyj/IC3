/*
Copyright 2009, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined || IntuitiveLabs.UI == undefined)
    throw new Error("The library IntuitiveLabs_UI.js must be loaded.");

IntuitiveLabs.UI.Popup = function() {
}

IntuitiveLabs.UI.Popup.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.Popup.prototype.content = null;
IntuitiveLabs.UI.Popup.prototype.element = null;
IntuitiveLabs.UI.Popup.prototype.border= "1px solid gray";
IntuitiveLabs.UI.Popup.prototype.backgroundColor = "white";
IntuitiveLabs.UI.Popup.prototype.padding = 5;
IntuitiveLabs.UI.Popup.prototype.height = -1;
IntuitiveLabs.UI.Popup.prototype.width = -1;
IntuitiveLabs.UI.Popup.prototype.dropShadow = null;
IntuitiveLabs.UI.Popup.prototype.isClosable = true;

/*
IntuitiveLabs.UI.Popup.prototype.hide = function() {
    this.element.style.visibility = "hidden";
    this.content.style.visibility = "hidden";
}
*/

IntuitiveLabs.UI.Popup.prototype.createElement = function() {
    this.element = document.createElement("DIV");
    this.element.style.position = "fixed";
    this.element.style.left = "50%";
    this.element.style.top = "45%";

    this.panel = document.createElement("DIV");
    this.panel.style.position = "absolute";
    this.panel.IntuitiveLabs_parent = this;

    if( this.content != undefined && this.content != null )
        this.panel.appendChild(this.content);


    if (this.isClosable) {
        var closeButton = document.createElement("DIV");
        closeButton.style.position = "absolute";
        closeButton.style.right = "0px";
        closeButton.style.top = "0px";
        closeButton.style.width = "12px";
        closeButton.style.height = "12px";
        closeButton.style.border = "1px dotted black";
        closeButton.style.fontSize = "10px";
        closeButton.style.fontWeight = "bold";
        closeButton.style.padding = "0px";
        closeButton.style.textAlign = "center";
        closeButton.style.verticalAlign = "middle";
        closeButton.style.margin = "2px";
        closeButton.style.cursor = "pointer";
        closeButton.innerHTML = "X";

        closeButton.onclick = function() { this.parentNode.IntuitiveLabs_parent.hide() }

        this.panel.appendChild(closeButton);
    }

    this.messageDisplay = document.createElement("DIV");
    this.messageDisplay.style.border = "1px dotted #999999";
    this.messageDisplay.style.marginTop = "5px";
    this.messageDisplay.style.paddingLeft = "2px";
    this.messageDisplay.style.color = "red";
    this.messageDisplay.style.visibility = "hidden";
    this.messageDisplay.style.display = "none";

    this.panel.appendChild(this.messageDisplay);

    this.element.appendChild(this.panel);

    this.dropShadow = new IntuitiveLabs.UI.DropShadow(this);
}


IntuitiveLabs.UI.Popup.prototype.hide = function() {

    //alert("backshee");
    this.hideMessage();
    
    if (this.element != null) {
        this.element.style.display = "none";
        this.element.style.visibility = "hidden";
    }


    //if (this.dropShadow != null) {
    //}
}

IntuitiveLabs.UI.Popup.prototype.render = function() {

    this.createElement();
    
    this.panel.style.backgroundColor = this.backgroundColor;
    this.panel.style.border = this.border;
    this.panel.style.padding = this.padding + "px";

    document.body.appendChild(this.element);

    this.panel.style.zIndex = "2";
    this.zIndex = 2;
    this.width = parseInt(this.panel.offsetWidth);
    this.height = parseInt(this.panel.offsetHeight);

    var lpos = -this.width / 2;
    var tpos = -this.height / 2;
    this.panel.style.left = lpos.toString() + "px";
    this.panel.style.top = tpos.toString() + "px";

    this.dropShadow.render();

}

IntuitiveLabs.UI.Popup.prototype.showMessage = function(message) {
    this.messageDisplay.innerHTML = message;
    this.messageDisplay.style.display = "block";
    this.messageDisplay.style.visibility = "visible";

    this.dropShadow.adjust();
}

IntuitiveLabs.UI.Popup.prototype.hideMessage = function() {
    this.messageDisplay.style.display = "none";
    this.messageDisplay.style.visibility = "hidden";

    this.readDimensions();
    this.dropShadow.adjust();
}

IntuitiveLabs.UI.Popup.prototype.readDimensions = function() {
    this.width = parseInt(this.panel.offsetWidth);
    this.height = parseInt(this.panel.offsetHeight);
}

IntuitiveLabs.UI.Popup.prototype.show = function() {

    if (this.element != null) {
        this.element.style.visibility = "visible";
        this.element.style.display = "block";
    }

    if (this.dropShadow != null) {
        this.dropShadow.element.style.visibility = "visible";
        this.dropShadow.element.style.display = "block";
        this.dropShadow.adjust();
    }
}

//DropShadow

IntuitiveLabs.UI.DropShadow = function( parent ) {
    if (parent == undefined)
        throw new Error( "DropShadow must have a parent." );
        
    this.parent= parent;
    this.element = document.createElement("DIV");
    this.element.style.position = "absolute";
}

IntuitiveLabs.UI.DropShadow.prototype = {
    width: -1,
    height: -1,
    offset: 2,
    color: "gray",

    render: function() {
        this.element.style.width = this.parent.width.toString() + "px";
        this.element.style.height = this.parent.height.toString() + "px";

        var lpos = -this.parent.width / 2;
        var tpos = -this.parent.height / 2;

        this.element.style.left = (lpos + this.offset) + "px";
        this.element.style.top = (tpos + this.offset) + "px";

        this.element.style.backgroundColor = this.color;

        this.parent.element.appendChild(this.element);
        this.element.style.zIndex = "1";
    },

    adjust: function() {
        this.parent.readDimensions();
        this.element.style.height = this.parent.height.toString() + "px";
        this.element.style.width = this.parent.width.toString() + "px";
    }
}