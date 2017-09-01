/*
Copyright 2009, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined || IntuitiveLabs.UI == undefined)
    throw new Error("The library IntuitiveLabs_UI.js must be loaded");

if (IntuitiveLabs.Collections == undefined)
    throw new Error("The library IntuitiveLabs_Collections.js must be loaded");

//------Node-----------------
IntuitiveLabs.UI.Node = function(text, description) {
    IntuitiveLabs.UI.Object.apply(this);

    
    this.text = text;
    this.description = description;

    this.element = document.createElement("div");
    this.element.style.cursor = "default";

    this.styleTemplate_dataEddy = new IntuitiveLabs.UI.StyleTemplate();    
    this.styleTemplate_selected = new IntuitiveLabs.UI.StyleTemplate();
    this.styleTemplate_focus = new IntuitiveLabs.UI.StyleTemplate();
}


IntuitiveLabs.UI.Node.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.Node.prototype.clickTime = new Date();
IntuitiveLabs.UI.Node.prototype.dataObject = null;
IntuitiveLabs.UI.Node.prototype.description = null;
IntuitiveLabs.UI.Node.prototype.isSelected = false;
IntuitiveLabs.UI.Node.prototype.isFocus = false;
IntuitiveLabs.UI.Node.prototype.markDataEddy = false;
IntuitiveLabs.UI.Node.prototype.onClick = null;
IntuitiveLabs.UI.Node.prototype.onDoubleClick = null;
IntuitiveLabs.UI.Node.prototype.styleTemplate_dataEddy = null;
IntuitiveLabs.UI.Node.prototype.styleTemplate_selected = null;
IntuitiveLabs.UI.Node.prototype.styleTemplate_focus = null;
IntuitiveLabs.UI.Node.prototype.text = null;
IntuitiveLabs.UI.Node.prototype.textElement = null;

IntuitiveLabs.UI.Node.prototype.createElement = function() {
    IntuitiveLabs.UI.Object.prototype.createElement.call(this)

    this.element.intuitivelabs_parent = this;

    this.textElement = document.createElement("div");
    this.textElement.style.cursor = "default";

    if (this.textElement != null)
        this.textElement.innerHTML = this.text;

    this.element.appendChild(this.textElement);

    this.textElement.onclick = function() {
        var obj = this.parentNode.intuitivelabs_parent;

        obj.isSelected = true;
        obj.isFocus = true;

        obj.updateStyle();


        //obj.unselectOther();
        //this.style.border = "solid 1px gray";

        if (obj.onClick != null) {
            obj.onClick(obj);
        }
        else if (obj.parent != null && obj.parent.onSelectNode != null) {
            obj.parent.onSelectNode(obj);
        }

        if (obj.parent != null && obj.parent.unselectOther != undefined) {
            obj.parent.unselectOther(obj);
        }
    }

    this.updateStyle();

    return this.element;
}

IntuitiveLabs.UI.Node.prototype.updateStyle = function() {
    if (this.textElement != null) {
        //alert(this.dataObject.ObjectGUID + ":" + this.isSelected);

        if (this.isSelected) {
            if( this.isFocus && !this.styleTemplate_focus.isEmpty() ){
                this.styleTemplate_focus.apply(this.textElement);                    
            }
            else if (!this.styleTemplate_selected.isEmpty())
                this.styleTemplate_selected.apply(this.textElement);

            else if (this.tree != null && !this.tree.itemTemplate_selected.isEmpty()) {
                this.tree.itemTemplate_selected.apply(this.textElement);
            }

            //If data eddies (circular references) are being highlighted, we copy the color of the borders around the element forming the data eddy
            if (this.markDataEddy) {

                if (!this.styleTemplate_dataEddy.isEmpty()) {
                    var borderColor = this.styleTemplate_dataEddy.getProperty("borderColor");

                    if (borderColor != null)
                        this.textElement.style.borderColor = borderColor;
                }
            }
        }
        else if (this.markDataEddy) {
            if (!this.styleTemplate_dataEddy.isEmpty())
                this.styleTemplate_dataEddy.apply(this.textElement);
        }
        else {

            if (!this.styleTemplate.isEmpty())
                this.styleTemplate.apply(this.textElement);
        }
    }
}


//--------Tree--------------
IntuitiveLabs.UI.Tree = function() {
    IntuitiveLabs.UI.Object.apply(this, arguments);
    this.root = new IntuitiveLabs.UI.TreeNode();
    this.root.parent = this;

    this.itemTemplate = new IntuitiveLabs.UI.StyleTemplate();
    this.itemTemplate.setProperty("border","dotted 1px transparent");
    
    this.itemTemplate_dataEddy = new IntuitiveLabs.UI.StyleTemplate();
    this.itemTemplate_dataEddy.setProperty("border","dotted 1px red");
    
    this.itemTemplate_selected = new IntuitiveLabs.UI.StyleTemplate();
    this.itemTemplate_selected.setProperty("border","dotted 1px gray");

    this.itemTemplate_focus = new IntuitiveLabs.UI.StyleTemplate();
    this.itemTemplate_focus.setProperty("border", "solid 1px gray");
}

IntuitiveLabs.UI.Tree.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.Tree.prototype.allowDeselect = false;
IntuitiveLabs.UI.Tree.prototype.itemTemplate = null;
IntuitiveLabs.UI.Tree.prototype.itemTemplate_dataEddy = null;
IntuitiveLabs.UI.Tree.prototype.itemTemplate_selected = null;
IntuitiveLabs.UI.Tree.prototype.currentNode = null; 
IntuitiveLabs.UI.Tree.prototype.dataComparer = null;
IntuitiveLabs.UI.Tree.prototype.defaultIcon = null;
IntuitiveLabs.UI.Tree.prototype.id = null;
IntuitiveLabs.UI.Tree.prototype.isInverted = false;
IntuitiveLabs.UI.Tree.prototype.onInitiateNode = null;
IntuitiveLabs.UI.Tree.prototype.onSelectNode = null;
IntuitiveLabs.UI.Tree.prototype.onDoubleClickNode = null;
IntuitiveLabs.UI.Tree.prototype.onDeselectNode = null;
IntuitiveLabs.UI.Tree.prototype.previousNode = null;
IntuitiveLabs.UI.Tree.prototype.root = null;
IntuitiveLabs.UI.Tree.prototype.treeElement = null;

IntuitiveLabs.UI.Tree.prototype.createElement = function() {
    //this.element = document.createElement("div");

    IntuitiveLabs.UI.Object.prototype.createElement.call(this);

    this.treeElement = document.createElement("div");
    this.treeElement.style.display = "table";
    //this.element.style.border = "solid 1px blue";

    var row = document.createElement("div");
    row.style.display = "table-row";

    var cell = document.createElement("div");
    cell.style.display = "table-cell";

    var rootElement = this.root.createElement();

    cell.appendChild(rootElement);

    row.appendChild(cell);

    this.treeElement.appendChild(row);
    this.element.appendChild(this.treeElement);

    //alert(this.element.innerHTML);

    return this.element;
}

IntuitiveLabs.UI.Tree.prototype.deselect = function() {
    if (this.currentNode != null) {
        this.currentNode.isSelected = false;
        this.currentNode.updateStyle();
        this.currentNode = null;
    }
}

IntuitiveLabs.UI.Tree.prototype.setSelection = function( node ) {
}

//--------Tree Node--------------
IntuitiveLabs.UI.TreeNode = function(text, description) {
    IntuitiveLabs.UI.Node.apply(this, arguments);

    this.children = new Array();
    this.isExpanded = false;
    //this.isSelected = false;
    //this.isFocus = false;
}

IntuitiveLabs.UI.TreeNode.prototype = new IntuitiveLabs.UI.Node();

IntuitiveLabs.UI.TreeNode.prototype.bypass = false;
IntuitiveLabs.UI.TreeNode.prototype.children = null;
IntuitiveLabs.UI.TreeNode.prototype.childClass = null;
IntuitiveLabs.UI.TreeNode.prototype.icon = null;
IntuitiveLabs.UI.TreeNode.prototype.id = null;
IntuitiveLabs.UI.TreeNode.prototype.index = -1;
IntuitiveLabs.UI.TreeNode.prototype.isInitiated = true;
IntuitiveLabs.UI.TreeNode.prototype.isExpanded = false;
IntuitiveLabs.UI.TreeNode.prototype.childrenElement = null;
IntuitiveLabs.UI.TreeNode.prototype.onInitiateNode = null;
IntuitiveLabs.UI.TreeNode.prototype.stateElement = null;
IntuitiveLabs.UI.TreeNode.prototype.tree = null;

IntuitiveLabs.UI.TreeNode.prototype.addChild = function(node) {
    node.parent = this;
    node.index = this.children.length;
    //this.isExpanded = true;
    this.children.push(node);
}

IntuitiveLabs.UI.TreeNode.prototype.appendChildren = function() {

    this.getTree();

    //this.element.style.border = "solid 1px green";

    if (this.tree.isInverted)
        this.element.align = "right";
    else
        this.element.align = "left";

    var itemElement = document.createElement("div");
    itemElement.style.display = "table";
    //itemElement.style.border = "solid 1px red";

    if (this.tree.element == null)
        throw new Error("The element of a TreeNode cannot be created independently of the Tree it belongs to.");

    var row = document.createElement("div");
    row.style.display = "table-row";
    row.intuitivelabs_parent = this;

    var stateCell = document.createElement("div");
    stateCell.style.display = "table-cell";
    stateCell.style.verticalAlign = "middle";
    //stateCell.style.border = "solid 1px red";

    this.stateElement = document.createElement("img");

    var stateFunction = function() {

        var obj = this.parentNode.intuitivelabs_parent;

        if (obj.isInitiated)
            obj.shiftState();
        else {
            if (obj.onInitiateNode != null)
                obj.onInitiateNode(obj);
            else if (obj.parent != null) {
                if (obj.parent.onInitiateNode != null)
                    obj.parent.onInitiateNode(obj);
                else {
                    var tree = obj.getTree();

                    if (tree.onInitiateNode != null)
                        tree.onInitiateNode(obj);
                    else if (tree.parent != null && tree.parent.onInitiateNode != null)
                        tree.parent.onInitiateNode(obj);
                }
            }
        }

        return;
        //alert("clicked");
    }

    if (!(this.parent instanceof IntuitiveLabs.UI.Tree) && this.index == this.parent.children.length - 1) {
        if (this.children.length == 0 && this.isInitiated) {
            if (!this.tree.isInverted)
                this.stateElement.src = "/images/IntuitiveLabs/Tree/L.gif";
            else
                this.stateElement.src = "/images/IntuitiveLabs/Tree/ReverseL.gif";

        }
        else {
            if (!this.isiniaited && this.isExpanded) {
                if (!this.tree.isInverted)
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/Lminus.gif";
                else
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/ReverseLminus.gif";
            }
            else {
                if (!this.tree.isInverted)
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/Lplus.gif";
                else
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/ReverseLplus.gif";
            }

            stateCell.onclick = stateFunction;
        }
    }
    else {
        if (this.parent instanceof IntuitiveLabs.UI.Tree) {
            if (this.isExpanded) {
                if (!this.tree.isInverted)
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/Rminus.gif";
                else
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/ReverseRminus.gif";
            }
            else {
                if (!this.tree.isInverted)
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/Rplus.gif";
                else
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/ReverseRplus.gif";
            }

        }
        else {
            if (!this.isInitiated || this.children.length > 0) {
                if (this.isExpanded && this.isInitiated) {
                    if (!this.tree.isInverted)
                        this.stateElement.src = "/images/IntuitiveLabs/Tree/Tminus.gif";
                    else
                        this.stateElement.src = "/images/IntuitiveLabs/Tree/ReverseTminus.gif";
                }
                else {
                    if (!this.tree.isInverted)
                        this.stateElement.src = "/images/IntuitiveLabs/Tree/Tplus.gif";
                    else
                        this.stateElement.src = "/images/IntuitiveLabs/Tree/ReverseTplus.gif";
                }
            }
            else {
                if (!this.tree.isInverted)
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/T.gif";
                else
                    this.stateElement.src = "/images/IntuitiveLabs/Tree/ReverseT.gif";
            }
        }

        stateCell.onclick = stateFunction;
    }

    stateCell.appendChild(this.stateElement);
    row.appendChild(stateCell);

    var node = this.parent;
    var cell = stateCell;

    while (node != null && !(node instanceof IntuitiveLabs.UI.Tree)) {
        var lineCell = document.createElement("div");
        lineCell.style.display = "table-cell";
        lineCell.style.verticalAlign = "middle";
        //lineCell.style.border = "solid 1px red";

        var lineImg = document.createElement("img");

        if (!(node.parent instanceof IntuitiveLabs.UI.Tree)) {
            if (node.index == node.parent.children.length - 1) {
                lineImg.src = "/images/IntuitiveLabs/Tree/white.gif";
            }
            else {
                lineImg.src = "/images/IntuitiveLabs/Tree/i.gif";
            }
        }
        else {
            lineImg.src = "/images/IntuitiveLabs/Tree/white.gif";
        }

        lineCell.appendChild(lineImg);

        if (cell == null || this.tree.isInverted)
            row.appendChild(lineCell);
        else {
            row.insertBefore(lineCell, cell);
        }

        cell = lineCell;
        node = node.parent;
    }

    var iconCell = document.createElement("div");

    iconCell.onclick = function() {
        var obj = this.parentNode.intuitivelabs_parent;

        var now = new Date();
        var diff = now.valueOf() - obj.clickTime.valueOf();
        obj.clickTime = now;

        var isDoubleClick = false;

        if (diff < 1000)
            isDoubleClick = true;

        var tree = obj.getTree();

        if (isDoubleClick) {
            if (obj.onDoubleClick != null)
                obj.onDoubleClick(obj);
            else {

                if (tree != null) {
                        if (tree.onDoubleClickNode != null)
                            tree.onDoubleClickNode(obj);
                        else if (tree.parent != null && tree.parent.onDoubleClickNode != null)
                            tree.parent.onDoubleClickNode(obj);
                    }
                }

        }
        else {
            if (tree.currentNode != null && tree.currentNode != obj) {
                tree.currentNode.isSelected = false;
                tree.currentNode.updateStyle();
            }

            tree.previousNode = tree.currentNode;
            tree.currentNode = obj;

            if (tree.allowDeselect && obj.isSelected) {
                obj.isSelected = false;
                obj.isFocus = false;
            }
            else {
                obj.isSelected = true;
                obj.isFocus = true;
            }

            obj.updateStyle();

            //if (tree.currentNode.textElement != null)
            //    tree.currentNode.textElement.style.border = "solid 1px gray";

            if (obj.onClick != null)
                obj.onClick(obj);
            else {

                if (tree != null) {
                    if (obj.isSelected) {
                        if (tree.onSelectNode != null)
                            tree.onSelectNode(obj);
                        else if (tree.parent != null && tree.parent.onSelectNode != null)
                            tree.parent.onSelectNode(obj);
                    }
                    else if (tree.allowDeselect) {
                        if (tree.onDeselectNode != null)
                            tree.onDeselectNode(obj);
                        else if (tree.parent != null && tree.parent.onDeselectNode != null)
                            tree.parent.onDeselectNode(obj);
                    }
                }
            }


            if (tree != null && tree.parent != null && tree.parent.setSelection != undefined && tree.parent.setSelection != null) {
                tree.parent.setSelection(tree);
            }
        }
    }

    if (!this.bypass && (this.icon != null || this.tree.defaultIcon != null)) {
        iconCell.style.display = "table-cell";
        iconCell.style.verticalAlign = "middle";
        iconCell.style.paddingLeft = "4px";
        //iconCell.style.border = "solid 1px orange";

        var img = document.createElement("img");

        if (this.icon != null)
            img.src = this.icon;
        else
            img.src = this.tree.defaultIcon;

        iconCell.appendChild(img);

    }
    else {
        var bypassDiv = document.createElement("div");

        bypassDiv.style.display = "table-cell";
        var bypassImg = document.createElement("img");

        if (!this.tree.isInverted)
            bypassImg.src = "/images/IntuitiveLabs/Tree/ReverseL.gif";
        else
            bypassImg.src = "/images/IntuitiveLabs/Tree/L.gif";

        bypassDiv.appendChild(bypassImg);

        if (!this.tree.isInverted)
            row.appendChild(bypassDiv);
        else
            row.insertBefore(bypassDiv, row.childNodes[0]);
    }

    if (!this.tree.isInverted) {
        row.appendChild(iconCell);
    }
    else {
        row.insertBefore(iconCell, row.childNodes[0]);
    }

    //alert("Step1:" + (this.parent != null && this.dataObject != null));
    //alert("Step2:" + (this.parent.dataComparer != null || (this.parent.parent != null && this.parent.parent.dataComparer != null)));

    //Ceck and mark the node if its a forms a data eddy ( a circular reference )
    if (this.parent != null && this.tree != null && this.dataObject != null && (this.tree.dataComparer != null || (this.tree.parent != null && this.tree.parent.dataComparer != null))) {

        var comparer = null;

        if (this.tree.dataComparer != null)
            comparer = this.tree.dataComparer;
        else
            comparer = this.tree.parent.dataComparer;

        if (this.parent.formsDataEddy(this.dataObject, comparer)) {

            var eddyDiv = document.createElement("div");

            eddyDiv.onclick = function() {

                var obj = this.parentNode.intuitivelabs_parent;
                obj.markDataEddy = !obj.markDataEddy;
                obj.updateStyle();

                var tree = obj.getTree();

                if (tree != null && obj.parent != null) {

                    var comparer = null;

                    if (tree.dataComparer != null)
                        comparer = tree.dataComparer;
                    else
                        comparer = tree.parent.dataComparer;

                    var conflux = obj.parent.getConflux(obj.dataObject, comparer)

                    conflux.markDataEddy = obj.markDataEddy;
                    conflux.updateStyle();

                    /*
                    if (conflux.textElement != null) {
                    conflux.textElement.style.border = "dotted 1px red";
                    }
                    */
                }
            }

            eddyDiv.style.display = "table-cell";
            var eddyImg = document.createElement("img");

            if (!this.tree.isInverted)
                eddyImg.src = "/images/IntuitiveLabs/Tree/icon16-DataEddy.png";
            else
                eddyImg.src = "/images/IntuitiveLabs/Tree/icon16Reverse-DataEddy.png";

            eddyDiv.appendChild(eddyImg);

            if (!this.tree.isInverted)
                row.appendChild(eddyDiv);
            else
                row.insertBefore(eddyDiv, row.childNodes[0]);
        }
    }

    if (!this.bypass) {
        var textCell = document.createElement("div");
        textCell.style.display = "table-cell";
        textCell.style.verticalAlign = "middle";
        textCell.style.whiteSpace = "nowrap";
        textCell.style.border = "dotted 1px transparent";
        textCell.innerHTML = this.text;

        if (this.description != null)
            textCell.innerHTML += '<span style="color:#999999;font-size:10px;">' + this.description + '</span>';

        //textCell.style.border = "solid 1px green";

        textCell.onclick = iconCell.onclick;

        if (!this.tree.isInverted) {
            row.appendChild(textCell);
        }
        else {
            row.insertBefore(textCell, row.childNodes[0]);
        }

        this.textElement = textCell;

    }


    itemElement.appendChild(row);

    this.element.appendChild(itemElement);
    this.childrenElement = document.createElement("div");

    for (var i = 0; i < this.children.length; i++) {
        if (!this.tree.isInverted || i == 0)
            this.childrenElement.appendChild(this.children[i].createElement());
        else {
            this.childrenElement.insertBefore(this.children[i].createElement(), this.children[i - 1].element);
        }
    }

    if (!this.tree.isInverted)
        this.element.appendChild(this.childrenElement);
    else {
        this.element.insertBefore(this.childrenElement, itemElement);
    }

    this.updateStyle();

    if (!this.isExpanded) {
        this.collapse();
    }
}


IntuitiveLabs.UI.TreeNode.prototype.collapse = function() {
    this.isExpanded = false;
    this.stateElement.src = this.stateElement.src.replace("minus", "plus");
    this.childrenElement.style.display = "none";
}

IntuitiveLabs.UI.TreeNode.prototype.createElement = function() {
    IntuitiveLabs.UI.Node.prototype.createElement.call(this);

    IntuitiveLabs.UI.DOM.removeChildren(this.element);
    this.appendChildren();

    return this.element;
}

IntuitiveLabs.UI.TreeNode.prototype.expand = function() {
    this.isExpanded = true;
    this.stateElement.src = this.stateElement.src.replace("plus","minus");
    this.childrenElement.style.display = "block";
}

IntuitiveLabs.UI.TreeNode.prototype.formsDataEddy = function(dataObject, comparer) {

    if (comparer == undefined || comparer == null)
        throw new Error("A function to compare the data objects must be provided");

    var pnode = this;

    while (pnode != null && !(pnode instanceof IntuitiveLabs.UI.Tree)) {

        if (pnode.dataObject != null) {

            //alert(dataObject.ObjectGUID + ";" + pnode.dataObject.ObjectGUID + ": " + (dataObject == pnode.dataObject))

            if (comparer(dataObject, pnode.dataObject))
                return true;
        }

        pnode = pnode.parent;
    }

    if ((pnode instanceof IntuitiveLabs.UI.Tree) && pnode.parent != null && pnode.parent.core != null && pnode.parent.core.dataObject != null && comparer(dataObject, pnode.parent.core.dataObject))
        return true;

    return false;
}

IntuitiveLabs.UI.TreeNode.prototype.getConflux = function(dataObject, comparer) {

    if (comparer == undefined || comparer == null)
        throw new Error("A function to compare the data objects must be provided");

    var pnode = this;

    while (pnode != null && !(pnode instanceof IntuitiveLabs.UI.Tree)) {

        if (pnode.dataObject != null) {

            //alert(dataObject.ObjectGUID + ";" + pnode.dataObject.ObjectGUID + ": " + (dataObject == pnode.dataObject))

            if (comparer(dataObject, pnode.dataObject))
                return pnode;
        }

        pnode = pnode.parent;
    }

    if ((pnode instanceof IntuitiveLabs.UI.Tree) && pnode.parent != null && pnode.parent.core != null && pnode.parent.core.dataObject != null && comparer(dataObject, pnode.parent.core.dataObject))
        return pnode.parent.core;

    return null;
}

IntuitiveLabs.UI.TreeNode.prototype.getTree = function( forceEvaluation ) {
    if( forceEvaluation  || this.tree == null ){
        var node = this;

        while (node.parent != null && !(node.parent instanceof IntuitiveLabs.UI.Tree)) {
            node = node.parent
        }
        
        if( (node.parent instanceof IntuitiveLabs.UI.Tree) ){
            this.tree = node.parent;
        }
    }
    
    return this.tree;    
}

IntuitiveLabs.UI.TreeNode.prototype.hasDataChild = function(dataObject, comparer) {
    if (comparer == undefined || comparer == null)
        throw new Error("A function to compare the data objects must be provided");
    
    for (var i = 0; i < this.children.length; i++) {
        return comparer(dataObject, this.children[i]);
    }
}

IntuitiveLabs.UI.TreeNode.prototype.refresh = function() {
    IntuitiveLabs.UI.DOM.removeChildren(this.element);
    this.appendChildren();
}

IntuitiveLabs.UI.TreeNode.prototype.removeChild = function(node) {
    var index = -1;

    var n = 0;
    var found = false;

    for (var i = 0; i < this.children.length; i++) {

        this.children[i].index = n;

        if (!found && this.children[i] == node) {
            index = i;
            found = true;
        }
        else {
            n++;
        }
    }

    if (index != -1) {
        this.children.splice(index, 1);
    }
}

IntuitiveLabs.UI.TreeNode.prototype.select = function() {
    var tree = node.getTree();
    tree.setSelection(this);
}

IntuitiveLabs.UI.TreeNode.prototype.shiftState = function() {
    if (this.isExpanded) {
        this.collapse();
    }
    else {
        this.expand();
    }

    var tree = this.getTree();
    tree.adjust();
}

IntuitiveLabs.UI.TreeNode.prototype.updateStyle = function() {
    if (this.textElement != null) {

        //alert(this.isSelected);

        if (this.isSelected) {
            if (this.isFocus && (!this.styleTemplate_focus.isEmpty() || (this.tree != null && !this.tree.itemTemplate_focus.isEmpty()))) {
                if (!this.styleTemplate_focus.isEmpty())
                    this.styleTemplate_focus.apply(this.textElement);
                else
                    this.tree.itemTemplate_focus.apply(this.textElement);
            }
            else if (!this.styleTemplate_selected.isEmpty())
                this.styleTemplate_selected.apply(this.textElement);

            else if (this.tree != null && !this.tree.itemTemplate_selected.isEmpty()) {
                this.tree.itemTemplate_selected.apply(this.textElement);
            }

            //If data eddies (circular references) are being highlighted, we copy the color of the borders around the element forming the data eddy
            if (this.markDataEddy) {
                var borderColor = null;

                if (!this.styleTemplate_dataEddy.isEmpty())
                    borderColor = this.styleTemplate_dataEddy.getProperty("borderColor");

                else if (this.tree != null && !this.tree.itemTemplate_dataEddy.isEmpty())
                    borderColor = this.tree.itemTemplate_dataEddy.getProperty("borderColor");

                if (borderColor != null)
                    this.textElement.style.borderColor = borderColor;
            }
        }
        else if (this.markDataEddy) {
            if (!this.styleTemplate_dataEddy.isEmpty())
                this.styleTemplate_dataEddy.apply(this.textElement);

            else if (this.tree != null && !this.tree.itemTemplate_dataEddy.isEmpty())
                this.tree.itemTemplate_dataEddy.apply(this.textElement);
        }
        else {
            //alert(this.dataObject.ObjectGUID + " is not selected apply default style.");
            //alert(this.styleTemplate);

            if (!this.styleTemplate.isEmpty())
                this.styleTemplate.apply(this.textElement);

            else if (this.tree != null && !this.tree.itemTemplate.isEmpty()) {
                this.tree.itemTemplate.apply(this.textElement);
            }
        }
    }
}

//--------BifurcatedTree--------------
IntuitiveLabs.UI.BifurcatedTree = function() {
    this.upperTree = new IntuitiveLabs.UI.Tree();
    this.upperTree.isInverted = true;
    this.upperTree.parent = this;
    this.upperFork = this.upperTree.root;
    this.upperFork.bypass = true;
    this.upperTree.adjust = function() { this.parent.adjust(); };
    this.upperTree.id = "upper";

    this.lowerTree = new IntuitiveLabs.UI.Tree();
    this.lowerTree.parent = this;
    this.lowerTree.isInverted = false;
    this.lowerFork = this.lowerTree.root;
    this.lowerFork.bypass = true;
    this.lowerTree.adjust = function() { this.parent.adjust(); };
    this.lowerTree.id = "lower";

    this.core = new IntuitiveLabs.UI.Node("core");
    this.core.parent = this;

    this.core.styleTemplate_dataEddy.setProperty("border", "dotted 1px red");
    this.core.styleTemplate.setProperty("border", "dotted 1px transparent");
    this.core.styleTemplate_focus.setProperty("border", "solid 1px gray");
    this.core.styleTemplate_selected.setProperty("border", "dotted 1px gray");
}


IntuitiveLabs.UI.BifurcatedTree.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.BifurcatedTree.prototype.core = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.coreIsSelected = false;
IntuitiveLabs.UI.BifurcatedTree.prototype.coreIsFocus = true;
IntuitiveLabs.UI.BifurcatedTree.prototype.coreLeftElement = false;
IntuitiveLabs.UI.BifurcatedTree.prototype.coreElement = false;
IntuitiveLabs.UI.BifurcatedTree.prototype.coreRightElement = false;
IntuitiveLabs.UI.BifurcatedTree.prototype.dataComparer = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.lowerFork = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.upperTree = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.lowerTree = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.upperText = "";
IntuitiveLabs.UI.BifurcatedTree.prototype.lowerText = "";
IntuitiveLabs.UI.BifurcatedTree.prototype.upperElement = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.lowerElement = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.currentTree = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.onInitiateNode = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.onSelectNode = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.onDoubleClickNode = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.onDeselectNode = null;
IntuitiveLabs.UI.BifurcatedTree.prototype.upperFork = null;

IntuitiveLabs.UI.BifurcatedTree.prototype.adjust = function() {
    var upperWidth = this.upperTree.element.offsetWidth;
    var lowerWidth = this.lowerTree.element.offsetWidth;

    if (upperWidth > lowerWidth) {
        this.lowerElement.style.paddingRight = (upperWidth - lowerWidth).toString() + "px";
        this.upperElement.style.paddingLeft = "0px";
    }
    else {
        this.upperElement.style.paddingLeft = (lowerWidth - upperWidth).toString() + "px";
        this.lowerElement.style.paddingRight = "0px";
    }

    //var width = this.element.scrollWidth;

    //this.element.scrollLeft = 200;
    //alert(width);

}

IntuitiveLabs.UI.BifurcatedTree.prototype.createElement = function() {
    this.element = document.createElement("div");
    this.element.style.display = "table";
    //this.element.style.border = "solid 1px blue";

    //Upper
    var upperRow = document.createElement("div");
    upperRow.style.display = "table-row";

    var upperCell = document.createElement("div");
    upperCell.style.display = "table-cell";
    upperCell.appendChild(this.upperTree.createElement());
    this.upperElement = upperCell;

    //upperCell.style.border = "solid 1px red";
    upperRow.appendChild(upperCell);

    this.element.appendChild(upperRow);

    //Core    
    var coreRow = document.createElement("div");
    coreRow.style.display = "table-row";
    coreRow.intuitivelabs_parent = this;

    var coreLeft = document.createElement("div");
    coreLeft.style.display = "table-cell";
    coreLeft.align = "right";
    coreLeft.style.paddingRight = "5px";
    coreLeft.style.color = "silver";
    coreLeft.style.cursor = "default";
    coreLeft.innerHTML = this.upperText;

    coreLeft.onclick = function() {
        var obj = this.parentNode.intuitivelabs_parent;

        obj.core.isSelected = true;
        obj.core.isFocus = true;

        obj.coreRightElement.style.border = "dotted 1px transparent";

        obj.unselectOther(obj.core);

        obj.currentTree = obj.upperTree;

        if (obj.onSelectNode != null) {
            obj.onSelectNode(obj.core);
        }

        this.style.border = "solid 1px gray";
    }

    coreRow.appendChild(coreLeft);

    this.coreLeftElement = coreLeft;


    var coreCell = document.createElement("div");
    coreCell.style.display = "table-cell";
    coreCell.style.border = "dotted 1px transparent";
    coreCell.style.whiteSpace = "nowrap";
    //coreCell.innerHTML = this.coreText;
    coreCell.style.cursor = "default";

    coreCell.appendChild( this.core.createElement() );

    //this.coreElement = coreCell;

    /*
    coreCell.onclick = function() {
        var obj = this.parentNode.intuitivelabs_parent;

        obj.coreIsSelected = true;
        obj.coreIsFocus = true;

        obj.unselectOther();

        this.style.border = "solid 1px gray";

        if (obj.onClick != null) {
            obj.onClick(obj);
        }
    }
    */

    //coreCell.style.border = "solid 1px green";

    coreRow.appendChild(coreCell);

    var coreRight = document.createElement("div");
    coreRight.align = "left";
    coreRight.style.paddingLeft = "5px";
    coreRight.style.color = "silver";
    coreRight.style.display = "table-cell";
    coreRight.style.cursor = "default";
    coreRight.style.border = "dotted 1px transparent";
    coreRight.innerHTML = this.lowerText;

    coreRight.onclick = function() {
        var obj = this.parentNode.intuitivelabs_parent;

        obj.core.isSelected = true;
        obj.core.isFocus = true;

        obj.unselectOther(obj.core);

        obj.coreLeftElement.style.border = "dotted 1px transparent";

        obj.currentTree = obj.lowerTree;

        this.style.border = "solid 1px gray";

        if (obj.onSelectNode != null) {
            obj.onSelectNode(obj.core);
        }
    }

    coreRow.appendChild(coreRight);

    this.coreRightElement = coreRight;

    this.element.appendChild(coreRow);

    //Lower
    var lowerRow = document.createElement("div");
    lowerRow.style.display = "table-row";

    for (var i = 0; i < 2; i++) {
        var lowerFill = document.createElement("div");
        lowerFill.style.display = "table-cell";
        lowerRow.appendChild(lowerFill);
    }

    var lowerCell = document.createElement("div");
    lowerCell.style.display = "table-cell";
    lowerCell.appendChild(this.lowerTree.createElement());
    this.lowerElement = lowerCell;
    //lowerCell.style.border = "solid 1px orange";

    lowerRow.appendChild(lowerCell);

    this.element.appendChild(lowerRow);
    //alert(this.element.innerHTML);

    return this.element;
}

IntuitiveLabs.UI.BifurcatedTree.prototype.unselectCore = function() {
    /*
    if (this.coreIsSelected) {
    this.coreElement.style.border = "dotted 1px silver";
    }

    this.coreLeftElement.style.border = "dotted 1px transparent";
    this.coreRightElement.style.border = "dotted 1px transparent";
    */

    this.core.isSelected = false;
    this.core.isFocus = false;
    this.core.updateStyle();
}

IntuitiveLabs.UI.BifurcatedTree.prototype.unselectOther = function( node ) {

    if (this.upperTree.currentNode != null) {
        this.upperTree.currentNode.isFocus = false;
        this.upperTree.currentNode.updateStyle();
    }

    if (this.lowerTree.currentNode != null) {
        this.lowerTree.currentNode.isFocus = false; ;
        this.lowerTree.currentNode.updateStyle();
    }

    this.currentTree = null;
}

IntuitiveLabs.UI.BifurcatedTree.prototype.setSelection = function(tree) {
    this.currentTree = tree;

    this.unselectCore();

    if (tree.id == "upper") {
        if (this.lowerTree.currentNode != null) {
            this.lowerTree.currentNode.isFocus = false;
            this.lowerTree.currentNode.updateStyle();
        }
    }
    else {
        if (this.upperTree.currentNode != null) {
            this.upperTree.currentNode.isFocus = false;
            this.upperTree.currentNode.updateStyle();
        }
    }

    /*
    if (this.coreIsSelected) {
    this.coreElement.style.border = "dotted 1px gray";
    }
    */

    this.coreIsFocus = false;

}


//------CoreNode-----------------
IntuitiveLabs.UI.CoreNode = function() {
    IntuitiveLabs.UI.Node.apply(this);
}


IntuitiveLabs.UI.CoreNode.prototype = new IntuitiveLabs.UI.Node();
