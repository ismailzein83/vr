'use strict';

app.service('DropdownService', ['BaseDirService', function (BaseDirService) {

    return ({
        dTemplate: BaseDirService.directiveMainURL + "vr-dropdown-standard.html",
        getSelectText: getSelectText,
        setDefaultAttributes: setDefaultAttributes,
        getTemplateByType: getTemplateByType,
        isSingleSelection: isSingleSelection
    });

    function getTemplateByType(type) {
        return BaseDirService.directiveMainURL + 'vr-dropdown-' + type + '.html';
    }

    function getSelectText(singleSelection, length, values, dlabel, labelMsg) {

        if (singleSelection) {
            if (length > 0)
                return values[0];
            else
                return dlabel;
        }

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
            label = length + labelMsg;
        if (label.length > 21)
            label = label.substring(0, 20) + "..";
        return label;
    }

    function setDefaultAttributes(attrs) {

        if (attrs.limitcharactercount == undefined) attrs.$set("limitcharactercount", "4");
        if (attrs.limitplaceholder == undefined) attrs.$set("limitplaceholder", "Type " + attrs.limitcharactercount + " characters..");
        if (attrs.entityname == undefined) return attrs;
        if (attrs.selectlbl == undefined) attrs.$set("selectlbl", "Selected " + attrs.entityname + " :");
        if (attrs.placeholder == undefined) attrs.$set("placeholder", "Select " + attrs.entityname + "...");
        if (attrs.selectplaceholder == undefined) attrs.$set("selectplaceholder", attrs.entityname + " selected");

        return attrs;
    }

    function isSingleSelection(type) {
        if (type == undefined || type == "" || type == "standard") return true;
        return false;
    }

}]);