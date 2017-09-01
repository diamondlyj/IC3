/*
Copyright 2009, IntuitiveLabs LLC 
intuitivelabs.net
All rights reserved
*/

if (IntuitiveLabs == undefined || IntuitiveLabs.UI == undefined)
    throw new Error("The library IntuitiveLabs_UI.js must be loaded");

if (IntuitiveLabs.Collections == undefined)
    throw new Error("The library IntuitiveLabs_Collections.js must be loaded");

/*
if (IntuitiveLabs == undefined) var IntuitiveLabs = {};
if (IntuitiveLabs.UI == undefined) IntuitiveLabs.UI = {};
*/

IntuitiveLabs.UI.Form = function() {
    this.inputs = new Array();
}

IntuitiveLabs.UI.Form.prototype = {

    inputs: null,
    textDistance: 5,
    parent: null,

    addTextbox: function(textbox) {

        if (textbox == undefined || textbox == null)
            throw new Error("a Textbox object must be passed to the addTextbox method.");
        
        var row = document.createElement("DIV");
        row.style.display = "table-row";

        var leftCell = document.createElement("DIV");
        leftCell.style.display = "table-cell";
        leftCell.style.whiteSpace = "nowrap";
        leftCell.style.paddingRight = this.textDistance.toString() + "px";

        if (textbox.text != undefined && textbox.text != null) leftCell.innerHTML = textbox.text;

        var rightCell = document.createElement("DIV");
        rightCell.style.display = "table-cell";

        //create input element
        textbox.parent = this;
        rightCell.appendChild(textbox.createElement());

        row.appendChild(leftCell);
        row.appendChild(rightCell);

        this.inputs.push(textbox);
        this.element.appendChild(row);

    }
}

IntuitiveLabs.UI.Form.prototype.createElement = function() {
    this.element = document.createElement("DIV");
    this.element.style.display = "table";
    this.element.IntuitiveLabs_parent = this;

    return this.element;
}

// GenericForm
IntuitiveLabs.UI.GenericForm = function() {
    IntuitiveLabs.UI.Object.apply(this, arguments);
    
    this.inputs = new IntuitiveLabs.Collections.Dictionary();
    this.defaultStyleTemplate = new IntuitiveLabs.UI.StyleTemplate();
}

IntuitiveLabs.UI.GenericForm.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.GenericForm.prototype.inputs = null;
IntuitiveLabs.UI.GenericForm.prototype.submitText = null;
IntuitiveLabs.UI.GenericForm.prototype.submitImage = null;
IntuitiveLabs.UI.GenericForm.prototype.onSubmit = null;
IntuitiveLabs.UI.GenericForm.prototype.onEnter = null;
IntuitiveLabs.UI.GenericForm.prototype.defaultStyleTemplate = null;

IntuitiveLabs.UI.GenericForm.prototype.addInput = function(id, input) {
    input.parent = this;
    this.inputs.add(id, input);
}

IntuitiveLabs.UI.GenericForm.prototype.createElement = function() {

    this.element = document.createElement("DIV");
    this.element.style.display = "table";
    this.element.IntuitiveLabs_parent = this;

    var row = document.createElement("DIV");
    row.style.display = "table-row";

    var leftCell = document.createElement("DIV");
    leftCell.style.display = "table-cell";

    var form = document.createElement("DIV");

    for (n in this.inputs.entries) {
        var entry = this.inputs.entries[n];
        var input = entry.value;

        if (input.styleTemplate != undefined && input.styleTemplate != null && input.styleTemplate.properties.entries.length == 0)
            input.styleTemplate = this.defaultStyleTemplate;

        var formRow = document.createElement("DIV");
        formRow.style.display = "table-row";

        var formLeftCell = document.createElement("DIV");
        formLeftCell.style.display = "table-cell";
        formLeftCell.style.whiteSpace = "nowrap";
        formLeftCell.style.paddingRight = "5px";

        if (input.text != undefined && input.text != null) formLeftCell.innerHTML = input.text;

        var formRightCell = document.createElement("DIV");
        formRightCell.style.display = "table-cell";

        //create input element
        formRightCell.appendChild(input.createElement());

        formRow.appendChild(formLeftCell);
        formRow.appendChild(formRightCell);

        form.appendChild(formRow);
    }

    leftCell.appendChild(form);

    this.middleCell = document.createElement("DIV");
    this.middleCell.style.display = "table-cell";
    this.middleCell.style.textDecoration = "underline";

    if (this.submitText != undefined && this.submitText != null)
        this.middleCell.innerHTML = this.submitText + " >><br/>";
    else
        this.middleCell.innerHTML = "Submit >>";

    this.middleCell.style.whiteSpace = "nowrap";
    this.middleCell.style.paddingLeft = "20px";
    this.middleCell.style.paddingRight = "20px";
    this.middleCell.style.verticalAlign = "middle";
    this.middleCell.style.cursor = "pointer";

    this.rightCell = document.createElement("DIV");
    this.rightCell.style.display = "none";
    this.rightCell.style.visibility = "hidden";

    this.image = document.createElement("IMG");
    this.rightCell.appendChild(this.image);
    this.rightCell.style.verticalAlign = "middle";
    this.rightCell.style.paddingRight = "20px";

    this.middleCell.onclick = function() {
        //var handler = this.parentNode.parentNode.IntuitiveLabs_parent.onSubmit;

        var path = this.parentNode.parentNode.IntuitiveLabs_parent.waitImage;
        var cell = this.parentNode.parentNode.IntuitiveLabs_parent.rightCell;
        var img = this.parentNode.parentNode.IntuitiveLabs_parent.image

        if (path != undefined && img != null) {
            img.src = path;
            cell.style.display = "table-cell";
            cell.style.visibility = "visible";
        }


        //if (handler != undefined && handler != null) {
        this.parentNode.parentNode.IntuitiveLabs_parent.submit();
        //}
    };


    row.appendChild(leftCell);
    row.appendChild(this.middleCell);
    row.appendChild(this.rightCell);

    this.element.appendChild(row);

    return this.element;
}

IntuitiveLabs.UI.GenericForm.prototype.getInput = function(id) {
    return this.inputs.getEntry(id).value;
}

IntuitiveLabs.UI.GenericForm.prototype.getInputs = function() {
    return this.inputs.entries;
}

IntuitiveLabs.UI.GenericForm.prototype.hideSubmit = function() {
    this.middleCell.style.display = "none";
    this.middleCell.style.visibility = "hidden";
}

IntuitiveLabs.UI.GenericForm.prototype.hideWaitImage = function() {
    this.rightCell.style.display = "none";
    this.rightCell.style.visibility = "hidden";
}

IntuitiveLabs.UI.GenericForm.prototype.showSubmit = function() {
    this.middleCell.style.display = "table-cell";
    this.middleCell.style.visibility = "visible";
}

IntuitiveLabs.UI.GenericForm.prototype.showWaitImage = function() {    
    this.image.src = this.waitImage;
    this.rightCell.style.display = "table-cell";
    this.rightCell.style.visibility = "visible";
}

IntuitiveLabs.UI.GenericForm.prototype.submit = function() {
    if (this.onSubmit != undefined && this.onSubmit != null)
        this.onSubmit(this);

    this.showWaitImage();
}


//---------------Input-----------------------------

IntuitiveLabs.UI.Input = function() {
    IntuitiveLabs.UI.Object.apply(this, arguments);
    this.textTemplate = new IntuitiveLabs.UI.StyleTemplate();
    this.inputTemplate = new IntuitiveLabs.UI.StyleTemplate();
}

IntuitiveLabs.UI.Input.prototype = new IntuitiveLabs.UI.Object();
IntuitiveLabs.UI.Input.prototype.text = null;
IntuitiveLabs.UI.Input.prototype.textPosition = "none";
IntuitiveLabs.UI.Input.prototype.id = null;
IntuitiveLabs.UI.Input.prototype.value = null;
IntuitiveLabs.UI.Input.prototype.onChange = null;
IntuitiveLabs.UI.Input.prototype.onEnter = null;
IntuitiveLabs.UI.Input.prototype.parent = null;
IntuitiveLabs.UI.Input.prototype.textElement = null;
IntuitiveLabs.UI.Input.prototype.inputElement = null;
IntuitiveLabs.UI.Input.prototype.textElement = null;
IntuitiveLabs.UI.Input.prototype.textTemplate = null;
IntuitiveLabs.UI.Input.prototype.inputTemplate = null;


IntuitiveLabs.UI.Input.prototype.createElement = function() {
    
    if (this.textPosition.toLowerCase() == "none") {
        IntuitiveLabs.UI.Object.prototype.createElement.call(this);

        this.inputElement = this.element;
        this.textElement = document.createElement("span");

    }
    else {
        //Clone the element
        var tagName = this.element.tagName;
        this.inputElement = document.createElement(tagName);

        if (tagName.toLowerCase() == "input") {
            this.inputElement.type = this.element.type;
        }

        //if (this.textPosition == "left" || this.textPosition == "right")
        //    this.textElement = document.createElement("span");
        //else
        this.textElement = document.createElement("div");

        this.textElement.innerHTML = this.text;

        this.element = document.createElement("div");

        //Call the base method to make sure styletemplates and such are applied to the top.
        //WARNING: Has to be done right after creating this.element since the base method removes all children forom this.element!!!
        IntuitiveLabs.UI.Object.prototype.createElement.call(this);

        if (this.textPosition.toLowerCase() == "top") {
            this.element.appendChild(this.textElement);

            //Input needs to be as wide as the element when its on top
            if (this.styleTemplate != null) {
                var wstyle = this.styleTemplate.properties.getEntry("width");

                if (wstyle != null) {
                    this.inputElement.style.width = wstyle.value;
                }
            }

            this.element.appendChild(this.inputElement);

        }
        else {
            this.element.style.display = "table";

            var row = document.createElement("div")
            row.style.display = "table-row";

            var textCell = document.createElement("div");
            textCell.style.display = "table-cell";

            if (this.textTemplate != null && !this.textTemplate.properties.contains("verticalAlign"))
                textCell.style.verticalAlign = "middle";

            textCell.appendChild(this.textElement);

            var inputCell = document.createElement("div");
            inputCell.style.display = "table-cell";

            if (this.inputTemplate != null && !this.inputTemplate.properties.contains("verticalAlign"))
                inputCell.style.verticalAlign = "middle";

            inputCell.appendChild(this.inputElement);

            if (this.textPosition == "right") {
                row.appendChild(inputCell);
                row.appendChild(textCell);
            }
            else if (this.textPosition == "left") {
                row.appendChild(textCell);
                row.appendChild(inputCell);
            }
            else
                throw new Error("Unrecognized position: " + this.textPosition);

            this.element.appendChild(row);
        }
    }


    if (this.textTemplate != null)
        this.textTemplate.apply(this.textElement);

    if (this.inputTemplate != null)
        this.inputTemplate.apply(this.inputElement);

    return this.element;
}

//--------Checkbox--------------------
IntuitiveLabs.UI.Checkbox = function(text, value, onChange ) {
    IntuitiveLabs.UI.Input.apply(this, arguments);

    this.text = text;        
    this.value = value;

    this.onChange = onChange;

    this.element = document.createElement("INPUT");
    this.element.type = "checkbox";    
}

IntuitiveLabs.UI.Checkbox.prototype = new IntuitiveLabs.UI.Input();

IntuitiveLabs.UI.Checkbox.prototype.isChecked = false;

IntuitiveLabs.UI.Checkbox.prototype.createElement = function() {

    IntuitiveLabs.UI.Input.prototype.createElement.call(this);

    //this.element.id = this.id;
    this.inputElement.IntuitiveLabs_parent = this;
    this.inputElement.checked = this.isChecked;

    this.inputElement.onclick = function() {
        this.IntuitiveLabs_parent.isChecked = this.checked;

        if (this.IntuitiveLabs_parent.onChange != undefined && this.IntuitiveLabs_parent.onChange != null) {
            this.IntuitiveLabs_parent.onChange(this.IntuitiveLabs_parent);
        }
    }


    return this.element;
    //this.element.value = this.value;
}

IntuitiveLabs.UI.Checkbox.prototype.uncheck = function() {
    this.element.checked = false;
    this.isChecked = false;
}

IntuitiveLabs.UI.Checkbox.prototype.check = function() {
    this.element.checked = true;
    this.isChecked = true;
}

//-----------Dropdown-------------
IntuitiveLabs.UI.Dropdown = function(text) {
    IntuitiveLabs.UI.Input.apply(this, arguments);
    this.text = text;
    this.options = new IntuitiveLabs.Collections.Dictionary();
}

IntuitiveLabs.UI.Dropdown.prototype = new IntuitiveLabs.UI.Input();
IntuitiveLabs.UI.Dropdown.prototype.options = null;
IntuitiveLabs.UI.Dropdown.prototype.current = null;

IntuitiveLabs.UI.Dropdown.prototype.addOption = function(id, text, value, onClick) {
    var option = new IntuitiveLabs.UI.Option(text, value);

    if (onClick != undefined)
        option.onClick = onClick;
        
    option.parent = this;

    var opt = this.options.add(id, option);
}

IntuitiveLabs.UI.Dropdown.prototype.createElement = function() {
    this.element = document.createElement("select");

    IntuitiveLabs.UI.Input.prototype.createElement.call(this);

    //this.element.id = this.id;
    
    this.inputElement.IntuitiveLabs_parent = this;

    //alert(this.inputElement.IntuitiveLabs_parent);

    this.createOptionElements();

    this.inputElement.onchange = function() {
        var index = this.selectedIndex;

        var option = this.options[index].IntuitiveLabs_parent;

        this.IntuitiveLabs_parent.value = option;
        this.IntuitiveLabs_parent.current = option;

        if (this.IntuitiveLabs_parent.onChange != undefined && this.IntuitiveLabs_parent.onChange != null) {
            this.IntuitiveLabs_parent.onChange(option);
        }
    }

    this.inputElement.onkeyup = function(evt) {
        var keyCode = IntuitiveLabs.UI.getKeyCode(evt);

        var obj = this.IntuitiveLabs_parent;

        if (keyCode == 13) {
            if (obj.onEnter != undefined && obj.onEnter != null) {
                obj.onEnter(obj);
            }
            else if (obj.parent != undefined && obj.parent != null && obj.parent.onEnter != undefined && obj.parent.onEnter != null) {
                obj.parent.onEnter(obj);
            }
        }
    };

    return this.element;
    //this.element.value = this.value;
}

IntuitiveLabs.UI.Dropdown.prototype.createOptionElements = function() {
    if (this.options.entries.length > 0) {
        this.current = this.options.entries[0].value;
        this.value = this.options.entries[0].value;

        for (var i = 0; i < this.options.entries.length; i++) {
            var option = this.options.entries[i].value;            
            this.inputElement.appendChild(option.createElement());
        }
    }
}

IntuitiveLabs.UI.Dropdown.prototype.refresh = function() {
    IntuitiveLabs.UI.DOM.removeChildren(this.inputElement);
    this.createOptionElements();
}


IntuitiveLabs.UI.Option = function(text, value) {
    this.text = text;
    this.value = value;
}

IntuitiveLabs.UI.Option.prototype = new IntuitiveLabs.UI.Input();

IntuitiveLabs.UI.Option.prototype.createElement = function() {
    this.element = document.createElement("OPTION");
    this.element.innerHTML = this.text;
    this.element.value = this.value;
    this.element.IntuitiveLabs_parent = this;

    this.element.onclick = function() {
        var obj = this.IntuitiveLabs_parent;

        if (obj.onClick != null)
            obj.onClick(obj);
    };

    return this.element;
}

//--------------Textbox-------------------------
IntuitiveLabs.UI.Textbox = function(text, value, isSecret, handler){

    IntuitiveLabs.UI.Input.apply(this, arguments);

    this.element = document.createElement("INPUT");

    /*
    if (id == undefined || id == null)
        throw new Error("Input must have an id defined");                   
    else
        this.id = id; 
     */
        
    if (text != undefined)
        this.text = text; 
        
    if (value != undefined)    
        this.value;
            
    this.isSecret = isSecret;
    
    this.handler = handler;
}

IntuitiveLabs.UI.Textbox.prototype = new IntuitiveLabs.UI.Input();

IntuitiveLabs.UI.Textbox.prototype.text = null;
IntuitiveLabs.UI.Textbox.prototype.value =  null;
IntuitiveLabs.UI.Textbox.prototype.isSecret = null;
IntuitiveLabs.UI.Textbox.prototype.handler = null;
IntuitiveLabs.UI.Textbox.prototype.onEnter = null;
IntuitiveLabs.UI.Textbox.prototype.onEmpty = null;

IntuitiveLabs.UI.Textbox.prototype.createElement = function() {

    IntuitiveLabs.UI.Input.prototype.createElement.call(this);

    //if (parent == undefined || parent == null)
    //    throw new Error("Parent must be set before creating the input's document element");


    if (this.isSecret)
        this.inputElement.type = "password";
    else
        this.inputElement.type = "text";

    //this.inputElement.id = this.id;
    //this.inputElement.style.width = "100px";
    this.inputElement.IntuitiveLabs_parent = this;

    if (this.value != undefined && this.value != null) 
        this.inputElement.value = this.value;

    /*
    this.inputElement.onchange = function() {
    alert("ch:" + this.value);
    this.IntuitiveLabs_parent.value = this.value;
    this.IntuitiveLabs_parent.changeValue();
    };        
    */

    /*
    this.inputElement.onblur = function() {
    this.IntuitiveLabs_parent.value = this.value;
    this.IntuitiveLabs_parent.changeValue();
    };
    */

    this.inputElement.onkeyup = function(evt) {
        var keyCode = IntuitiveLabs.UI.getKeyCode(evt);

        var obj = this.IntuitiveLabs_parent;

        if (keyCode == 13) {
            if (obj.onEnter != undefined && obj.onEnter != null) {
                obj.onEnter(obj);
            }
            else if (obj.parent != undefined && obj.parent != null && obj.parent.onEnter != undefined && obj.parent.onEnter != null) {
                obj.parent.onEnter(obj);
            }
        }
        else {
            obj.value = this.value;
        }

        if (obj.value == "" && obj.onEmpty != null) {
            obj.onEmpty(obj);
        }
    };


    return this.element;
};

IntuitiveLabs.UI.Textbox.prototype.changeValue= function() {
    if (this.handler != undefined && this.handler != null)
        this.handler(this)
};

IntuitiveLabs.UI.Textbox.prototype.nullifyValue = function() {
    this.value = null;
    
    if (this.inpuElement != null)
        this.inputElement.value = "";
};

IntuitiveLabs.UI.Textbox.prototype.setValue = function(val) {
    this.value = val;
    
    if( this.inpuElement != null )    
        this.inputElement.value = val;
};


// LoginForm
//Rewrite to inheret off GenericForm!!!!
IntuitiveLabs.UI.LoginForm = function() {

    this.baseForm = new IntuitiveLabs.UI.Form();
    this.baseForm.createElement();
    this.baseForm.parent = this;

    var usernameHandler = function(sender) {

        sender.parent.parent.username = sender.value;

        if (sender.parent.parent.onChangeUsername != undefined && sender.parent.parent.onChangeUsername != null)
            sender.parent.parent.onChangeUsername(sender);
    }

    var passwordHandler = function(sender) {

        sender.parent.parent.password = sender.value;

        if (sender.parent.parent.onChangePassword != undefined && sender.parent.parent.onChangePassword != null)
            sender.parent.parent.onChangePassword(sender);
    }


    this.baseForm.addTextbox(new IntuitiveLabs.UI.Textbox("Username:", null, false, usernameHandler));
    this.baseForm.addTextbox(new IntuitiveLabs.UI.Textbox("Password:", null, true, passwordHandler));
}

IntuitiveLabs.UI.LoginForm.prototype = new IntuitiveLabs.UI.Object();

IntuitiveLabs.UI.LoginForm.prototype.username = "";
IntuitiveLabs.UI.LoginForm.prototype.password = "";
IntuitiveLabs.UI.LoginForm.prototype.onLogin = null;
IntuitiveLabs.UI.LoginForm.prototype.onChangeUsername = null;
IntuitiveLabs.UI.LoginForm.prototype.onChangePassword = null;
IntuitiveLabs.UI.LoginForm.prototype.waitImage = null;

IntuitiveLabs.UI.LoginForm.prototype.createElement = function() {

    this.element = document.createElement("DIV");
    this.element.style.display = "table";
    this.element.IntuitiveLabs_parent = this;

    var row = document.createElement("DIV");
    row.style.display = "table-row";

    var leftCell = document.createElement("DIV");
    leftCell.style.display = "table-cell";

    leftCell.appendChild(this.baseForm.element);

    var middleCell = document.createElement("DIV");
    middleCell.style.display = "table-cell";
    middleCell.style.textDecoration = "underline";
    middleCell.innerHTML = "Log In >>";
    middleCell.style.whiteSpace = "nowrap";
    middleCell.style.paddingLeft = "20px";
    middleCell.style.paddingRight = "20px";
    middleCell.style.verticalAlign = "middle";
    middleCell.style.cursor = "pointer";

    /*
    if (this.element.addEventListener)
    rightCell.addEventListener("click", this.test);
    else {
    alert("suku");
    rightCell.attachEvent("click", this.test);
    }
    */


    this.rightCell = document.createElement("DIV");
    this.rightCell.style.display = "none";
    this.rightCell.style.visibility = "hidden";

    this.image = document.createElement("IMG");
    this.rightCell.appendChild(this.image);
    this.rightCell.style.verticalAlign = "middle";

    middleCell.onclick = function() {
        var handler = this.parentNode.parentNode.IntuitiveLabs_parent.onLogin;

        var path = this.parentNode.parentNode.IntuitiveLabs_parent.waitImage;
        var cell = this.parentNode.parentNode.IntuitiveLabs_parent.rightCell;
        var img = this.parentNode.parentNode.IntuitiveLabs_parent.image

        if (path != undefined && img != null) {
            img.src = path;
            cell.style.display = "table-cell";
            cell.style.visibility = "visible";
        }

        if (handler != undefined && handler != null)
            this.parentNode.parentNode.IntuitiveLabs_parent.onLogin(this.parentNode.parentNode.IntuitiveLabs_parent);
    };


    row.appendChild(leftCell);
    row.appendChild(middleCell);
    row.appendChild(this.rightCell);

    this.element.appendChild(row);

    return this.element;
}

IntuitiveLabs.UI.LoginForm.prototype.hideWaitImage = function() {
    this.rightCell.style.display = "none";
    this.rightCell.style.visibility = "hidden";
}

IntuitiveLabs.UI.LoginForm.prototype.blankPassword = function() {
    this.password = "";
    this.baseForm.inputs[1].nullifyValue();
}
 
