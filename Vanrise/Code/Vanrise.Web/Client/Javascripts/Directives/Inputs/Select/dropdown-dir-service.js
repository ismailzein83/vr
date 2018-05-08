(function (app) {
    
    "use strict";

    var keys = {
        37: 1, 38: 1, 39: 1, 40: 1
    };

    function preventDefault(e) {
        e = e || window.event;
        if (e.preventDefault)
            e.preventDefault();
        e.returnValue = false;
    }

    function preventDefaultForScrollKeys(e) {
        if (keys[e.keyCode]) {
            preventDefault(e);
            return false;
        }
    }

    function disableScroll() {
        if (window.addEventListener) // older FF
            window.addEventListener('DOMMouseScroll', preventDefault, false);
        window.onwheel = preventDefault; // modern standard
        window.onmousewheel = document.onmousewheel = preventDefault; // older browsers, IE
        window.ontouchmove = preventDefault; // mobile
        document.onkeydown = preventDefaultForScrollKeys;
    }

    function enableScroll() {
        if (window.removeEventListener)
            window.removeEventListener('DOMMouseScroll', preventDefault, false);
        window.onmousewheel = document.onmousewheel = null;
        window.onwheel = null;
        window.ontouchmove = null;
        document.onkeydown = null;
    }

    function getSelectText(length, values, dlabel, labelMsg) {
        var label;
        if (length === 0)
            label = dlabel;
        else if (length === 1)
            label = values[0];
        else if (length === 2)
            label = values[0] + "," + values[1];
        else if (length === 3)
            label = values[0] + "," + values[1] + "," + values[2];
        else
            label = length + " " + labelMsg;
        if (label != undefined)
            if (label.length > 21)
                label = label.substring(0, 20) + "..";
        return label;
    }

    function isMenu(type) {
        if (type === "menu") return true;
        return false;
    }

    function setAttributes(attrs) {
        if (attrs.limitcharactercount == undefined) attrs.$set("limitcharactercount", "2");
        if (attrs.limitplaceholder == undefined) attrs.$set("limitplaceholder", "Type " + attrs.limitcharactercount + " characters..");
        if (attrs.entityname == undefined) attrs.$set("entityname", "");
        if (attrs.selectlbl == undefined) attrs.$set("selectlbl", "Selected " + attrs.entityname + " :");
        if (attrs.placeholder == undefined) attrs.$set("placeholder", "Select " + attrs.entityname + "...");
        if (attrs.text != undefined) attrs.$set("placeholder", attrs.text);
        if (attrs.selectplaceholder == undefined) attrs.$set("selectplaceholder", attrs.entityname + " selected");

        return attrs;
    }

    app.service('DropdownService',function () {

        function getTemplateByType(type) {
            return "/Client/Javascripts/Directives/Old/vr-dropdown-" + type + ".html";
        }

        function isSingleSelection(type) {
            if (type == undefined || type === "" || type === "standard" || type === "menu") return true;
            return false;
        }

        return ({
            dTemplate: "/Client/Javascripts/Directives/Old/vr-dropdown-standard.html",
            getSelectText: getSelectText,
            setDefaultAttributes: setAttributes,
            getTemplateByType: getTemplateByType,
            isSingleSelection: isSingleSelection,
            isMenu: isMenu,
            dSelectTemplate: "/Client/Javascripts/Directives/Old/vr-select.html",
            setAttributes: setAttributes
        });

    });

    app.service('SelectService',function () {

        function validate(attrs) {
            if (attrs.isrequired == undefined && attrs.validator == undefined) return false;
            return true;
        }

        function isMultiple(attrs) {
            if (attrs.ismultipleselection === "" || attrs.ismultipleselection) return true;
            return false;
        }

        function isActionBarTop(type) {
            if (type === "actionbartop") return true;
            else return false;
        }

        return ({
            dTemplate: "/Client/Javascripts/Directives/Inputs/Select/vr-select.html",
            toolboxSelectTemplate: "/Client/Javascripts/Directives/Inputs/Select/vr-toolbox-select.html",
            getSelectText: getSelectText,
            isMenu: isMenu,
            isMultiple: isMultiple,
            setAttributes: setAttributes,
            validate: validate,
            isActionBarTop: isActionBarTop,
            enableScroll: enableScroll,
            disableScroll: disableScroll
        });

    });

})(app);



