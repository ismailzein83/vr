﻿'use strict';

app.service('DropdownService', ['BaseDirService', function (BaseDirService) {

    return ({
        dTemplate: "/Client/Javascripts/Directives/Old/vr-dropdown-standard.html",
        getSelectText: getSelectText,
        setDefaultAttributes: setDefaultAttributes,
        getTemplateByType: getTemplateByType,
        isSingleSelection: isSingleSelection,
        isMenu: isMenu,
        dSelectTemplate: "/Client/Javascripts/Directives/Old/vr-select.html",
        setAttributes: setAttributes
    });

    function getTemplateByType(type) {
        return "/Client/Javascripts/Directives/Old/vr-dropdown-" + type + ".html";
    }

    function getSelectText(length, values, dlabel, labelMsg) {

        var label = "";
        if (length == 0)
            label = dlabel;
        else if (length == 1)
            label = values[0];
        else if (length == 2)
            label = values[0] + "," + values[1];
        else if (length == 3)
            label = values[0] + "," + values[1] + "," + values[2];
        else
            label = length +" "+ labelMsg;
        if (label.length > 21)
            label = label.substring(0, 20) + "..";

        return label;
    }

    function setDefaultAttributes(attrs) {

        if (attrs.limitcharactercount == undefined) attrs.$set("limitcharactercount", "2");
        if (attrs.limitplaceholder == undefined) attrs.$set("limitplaceholder", "Type " + attrs.limitcharactercount + " characters..");
        if (attrs.entityname == undefined) return attrs;
        if (attrs.selectlbl == undefined) attrs.$set("selectlbl", "Selected " + attrs.entityname + " :");
        if (attrs.placeholder == undefined) attrs.$set("placeholder", "Select " + attrs.entityname + "...");
        if (attrs.selectplaceholder == undefined) attrs.$set("selectplaceholder", attrs.entityname + " selected");

        return attrs;
    }

    function setAttributes(attrs) {

        if (attrs.limitcharactercount == undefined) attrs.$set("limitcharactercount", "2");
        if (attrs.limitplaceholder == undefined) attrs.$set("limitplaceholder", "Type " + attrs.limitcharactercount + " characters..");
        if (attrs.entityname == undefined) attrs.$set("entityname", "");
        if (attrs.selectlbl == undefined) attrs.$set("selectlbl", "Selected " + attrs.entityname + " :");
        if (attrs.placeholder == undefined) attrs.$set("placeholder", "Select " + attrs.entityname + "...");
        if (attrs.selectplaceholder == undefined) attrs.$set("selectplaceholder", attrs.entityname + " selected");

        return attrs;
    }

    function isSingleSelection(type) {
        if (type == undefined || type == "" || type == "standard" || type == "menu") return true;
        return false;
    }

    function isMenu(type) {
        if (type == "menu") return true;
        return false;
    }

}]);

app.service('SelectService', ['BaseDirService', function (BaseDirService) {

    return ({
        dTemplate: "/Client/Javascripts/Directives/Inputs/Select/vr-select.html",
        getSelectText: getSelectText,
        isMenu: isMenu,
        isMultiple:isMultiple,
        setAttributes: setAttributes,
        validate: validate,
        isActionBarTop: isActionBarTop
    });

    function validate(attrs) {
        if (attrs.isrequired == undefined && attrs.validator == undefined) return false;
        return true;
    }

    function getSelectText(length, values, dlabel, labelMsg) {

        var label = "";
        if (length == 0)
            label = dlabel;
        else if (length == 1)
            label = values[0];
        else if (length == 2)
            label = values[0] + "," + values[1];
        else if (length == 3)
            label = values[0] + "," + values[1] + "," + values[2];
        else
            label = length + " " + labelMsg;
        if (label.length > 21)
            label = label.substring(0, 20) + "..";

        return label;
    }

    function setAttributes(attrs) {

        if (attrs.limitcharactercount == undefined) attrs.$set("limitcharactercount", "2");
        if (attrs.limitplaceholder == undefined) attrs.$set("limitplaceholder", "Type " + attrs.limitcharactercount + " characters..");
        if (attrs.entityname == undefined) attrs.$set("entityname", "");
        if (attrs.selectlbl == undefined) attrs.$set("selectlbl", "Selected " + attrs.entityname + " :");
        if (attrs.placeholder == undefined) attrs.$set("placeholder", "Select " + attrs.entityname + "...");
        if (attrs.selectplaceholder == undefined) attrs.$set("selectplaceholder", attrs.entityname + " selected");

        return attrs;
    }

    function isMultiple(attrs) {
        if (attrs.ismultipleselection == "" || attrs.ismultipleselection) return true;
        return false;
    }

    function isMenu(type) {
        if (type == "menu") return true;
        return false;
    }

    function isActionBarTop(type) {
        if (type == "actionbartop") return true;
        else return false;
    }
}]);