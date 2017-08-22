$(document).ready(function () {
    $(document).on("click", "[data-showmodule]", function () {
        var dml = $(this).data("showmodule");
        var modal = $("#" + dml);
        var trigger = $("#CreateComponentMenu");
        modal.offset({ top: trigger.offset().top, left: trigger.offset().left });
        modal.css("visibility", "visible");
        $("#CreateComponentMenu").css("visibility", "hidden");

        if ($(this).data("moduleguid")) {
            var moduleGUID = $(this).data("moduleguid");
            setFormModuleId(moduleGUID);
        }
    });


    $(document).on("click", ".close-modal", function () {
        $(this).parent().css("visibility", "hidden");
    });

});

var showModal = function (params) {
    closeModals();
    var modal = $("#" + params.modalId);
    var trigger = $("#" + params.triggerId);
    if(params.leftOffset){
        positionModal(modal, trigger, params.leftOffset, params.topOffset);
    }
    if (params.setParents) {
        setMenuParents(modal, params.triggerGUID);
    }
    if (params.callBack) {
        window[params.callBack].apply(params);
    }
    modal.css("visibility", "visible");
}

var positionModal = function (modal, trigger, leftOffset, topOffset) {
    if (leftOffset < 0) {
        var leftPosition = trigger.offset().left + leftOffset - modal.width();
    } else {
        var leftPosition = trigger.offset().left + leftOffset;
    }
    if (topPosition < 0) {
        var topPosition = trigger.offset().top + topOffset - modal.height();
    } else {
        var topPosition = trigger.offset().top + topOffset;
    }
    var modalHeight = modal.innerHeight();
    var modalWidth = modal.innerWidth();
    var contentHeight = $("#content").innerHeight();
    var contentWidth = $("#content").innerWidth();
    topPosition = Math.min(topPosition, contentHeight - modalHeight);
    leftPosition = Math.min(leftPosition, contentWidth - modalWidth);

    modal.offset({ top: topPosition, left: leftPosition });
}


    $.fn.addInput = function (triggerGUID) {

        var html = this.html();
        this.html(html + hiddenInput);

    };

var setMenuParents = function (modal, triggerGUID){
    modal.children("a").data("moduleguid", triggerGUID);
    var forms = modal.children("form");
    var hiddenInput = "<input class = 'ParentGUIDInput' type = 'hidden' name = 'ParentGUID' value = " + triggerGUID + " />";
    forms.remove(".ParentGUIDInput");
    forms.append(triggerGUID);
    
}

var setFormModuleId = function (moduleGUID) {
    var moduleIdInput = $(".FormParentGUID");
    // console.log("setting module guid to " + moduleGUID); // debug
    moduleIdInput.val(moduleGUID);
}
var newComponentMenu = function (triggerGUID) {
    closeModals();
    var modal = $("#CreateComponentMenu");
    var trigger = $("#addIcon" + triggerGUID);
    var kids = modal.children("a");
    modal.children("a").data("moduleguid", triggerGUID);
    var leftPosition = trigger.offset().left - 25;
    var topPosition = trigger.offset().top + 25;
    modal.offset({ top: topPosition, left: leftPosition });
    modal.css("visibility", "visible");
}

var closeModals = function () {
    $(".modal").css("visibility","hidden");
}
