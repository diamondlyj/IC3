/*********************************************************************************************
general scripts
**********************************************************************************************/


$(document).ready(function () {
    $(document).on("click", "[data-action]", function () {
        // pass params as either single param or key-value data object
        // as in {"foo": 1, "bar": "qux"}
        var params = {};
        if($(this).data("params")){
            params = $(this).data("params");
        }
        window[$(this).data("action")].call(this, params);
});
$(document).on("change", "[data-change]", function () {
    var params = {};
    if ($(this).data("params")) {
        params = $(this).data("params");
    }
    window[$(this).data("change")].call(this, params);
});
$(document).on("mouseenter", "[data-mouseover]", function () {
    var params = {};
    if ($(this).data("params")) {
        params = $(this).data("params");
    }
    window[$(this).data("mouseover")].call(this, params);
});

$(document).on("mouseleave", "[data-mouseout]", function () {
    var params = {};
    if ($(this).data("params")) {
        params = $(this).data("params");
    }
    window[$(this).data("mouseout")].call(this, params);

})

});


var  submitForm = function (formId){
    //console.log("running submitForm method");
    var form = $("form#" + formId);
    if (form) {
        form.submit();
    }
}

/**
* This is only for revealing the top level category of
* the Category page. It should hide all the others. 
*/
var revealTopLevelCategory = function () {
    var categoryDiv = $("#" + $("#CategorySelect").val()).first();
    var defaultDisplay = $(this).data("display") ? $(this).data("display") : "block";
    $(".TopCategoryList").first().children("li").each(function () {
        $(this).css("display", $(this).is(categoryDiv.first()) ? defaultDisplay : "none");
    });
}

/**
* @param {object} params either a string which is a parent id or object that has keys ParentId and ChildTag 
*
*/
var revealChildren = function (params) {
    // parentId is the parent of what you want to reveal
    // childTag is the tag or class of the child you want to reveal
    // default childTag is 'ul' if params is just a string
    if($.type(params)==='string'){
        parentId = params;
        childTag = 'ul';
    } else {
        parentId = params.ParentId;
        childTag = params.ChildTag;
    }
    if (parentId.length > 0) {
        var parent = $("#" + parentId);
        if (parent) {
            var children = parent.children(childTag);
            children.each(function () {
                //console.log("revealing child " + $(this).clone().children().remove().end().text()); //debug
                // to revert to a different display than block use data-display attribute as in
                // <ul data-display = 'inline-block'>
                var defaultDisplay = $(this).data("display") ? $(this).data("display") : "block";
                $(this).css("display", defaultDisplay);
            });
        }
    }
}

/**
* @param {string} string the string to santize
* @return {string} the string with underscores for spaces and everything else weird removed
*/
var sanitizeString = function (string) {
    string = string.replace(/ /g, "_");
    string = string.replace(/\W/g, '');
    return string;
}

/*********************************************************************************************/


/*********************************************************************************************
scripts for Modules page
**********************************************************************************************/

/**
* @param {string} categoryName the category that will be added
*/


var addCategory = function (categoryName) {
    var s = "";
    var par = $("#" + categoryName);
    var child = par.children("a.CategoryName").first();
    s = child.clone().remove().end().text().trim();
    while (par.parent().parent().is("li")) {
        par = par.parent().parent();
        child = par.children("a.CategoryName").first();
        liText = child.clone().remove().end().text().trim();
        //console.log("liText = " + liText)
        s = liText + "." + s;
    }
    var filter = $("#CategoryListTextArea");
    var qPointForm = $("#EnterQPointForm");
    if (filter) {
        var currentVal = filter.val();
        //console.log("s = " + s + ", currentVal = " + currentVal);
        if (currentVal.indexOf(s + "\n") === -1) {
            currentVal += s + "\n";
        }
        filter.val(currentVal);
    }
    submitForm("CategoryForm");
}



var clearQPointFilter = function () {
    var filter = $("#CategoryListTextArea");
    if (filter) {
        filter.val('');
    }
    // to preserve what's in the qpoint list currently form 
    // will have hidden input of that, must clear that too
    submitForm("CategoryForm");
}

var toggleComponentDisplay = function (divId) {
    var moduleDiv = $("#" + divId);
    //console.log("divId =" + divId); // debug
    var thisLink = moduleDiv.find('.HideArrow');
    var childModules = moduleDiv.children('.ModuleWrapper');
   // console.log("moduleDiv = " + moduleDiv.id + ", thisLink = " + thisLink.id + ", childModules = " + childModules.id); // debug
    var sideArrow = "<svg width='40' height='40'><polygon points='10,20 30,30 10,40' style='fill:white' /></svg>";
    var downArrow = "<svg width='40' height='40'><polygon points='10,20 20,40 30,20' style='fill:white' /></svg>";
    if (childModules.length > 0) {
        if (moduleDiv.data["visible"] === 'True') {
            //console.log("moduleDiv.data = " + moduleDiv.data["visible"]); //debug
            thisLink.html(sideArrow);
            childModules.css("display", "none");
            moduleDiv.data["visible"] = 'False';
        } else {
            //console.log("moduleDiv.data = " + moduleDiv.data["visible"]); //debug
            thisLink.html(downArrow);
            childModules.css("display", "block");
            moduleDiv.data["visible"] = 'True'
        }
    } else {
        if (moduleDiv.data["visible"] === 'True') {
            thisLink.html(sideArrow);
            moduleDiv.data["visible"] = 'False';
        } else {
            thisLink.html(downArrow);
            moduleDiv.data["visible"] = 'True';
        }
    }
}

var populateOrderList = function () {
    
    $("#ComponentList").load("../AvailableProducts/" + this.qPointGUID + "/" + this.parentGUID +  "/AvailableProduct/1");
    //$("#ComponentList").load("../AvailableProducts/c4d4b4ec-ba2b-4af1-a113-2dff66ae03c3/" + this.parentGUID + "/AvailableProduct/1");
}

var addProductToBasket = function(params){
    ParentGUID = params.ParentGUID;
    ObjectGUID = params.ObjectGUID;
    SourceName = params.SourceName;
    OrderID = params.OrderID;
    $("#SearchIcon" + ObjectGUID).load("../Component/_Commerce/", {
        "ParentGUID": ParentGUID,
        "ObjectGUID": ObjectGUID,
        "SourceName": SourceName,
        "OrderID": OrderID
    });
    closeModals();
}

var showEditIcons = function (params) {
    console.log($(this).parent());
    $(this).children("div.Edit").css("visibility","visible");
    $(this).children("div.Delete").css("visibility", "visible");
}

var hideEditIcons = function (params) {
    $(this).children("div.Edit").css("visibility", "hidden");
    $(this).children("div.Delete").css("visibility", "hidden");
}

var buyProduct = function (url, userId) {
    
}

/*********************************************************************************************/

/*********************************************************************************************
scripts for Projects page
**********************************************************************************************/
/**
* @param {object} params .id = the id of the project to add to
*/
var addCollaboratorToProject = function (params) {
    showModal({
        modalId: "AddCollaboratorModal",
        triggerId: "AddCollaborator",
        leftOffset: 100,
        topOffset: 0,
        setParents: true,
        triggerGUID: params.ProductID
    });
}

/**
* @param {object} params .id = the id of the project to add to
*/
var addProductToProject = function (params) {
    showModal({
        modalId: "AddProductModal",
        triggerId: "AddProduct",
        leftOffset: 25,
        topOffset: 25,
        setParents: true,
        triggerGUID: params.ProductID
    });
}

/*********************************************************************************************/

