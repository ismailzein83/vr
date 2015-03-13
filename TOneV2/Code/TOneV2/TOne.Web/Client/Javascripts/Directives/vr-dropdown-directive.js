'use strict';

app.service('DropdownService', ['BaseDirService', function (BaseDirService) {

    return ({
        dTemplate: BaseDirService.directiveMainURL + "vr-dropdown-standard.html",
        getSelectText: getSelectText,
        muteAction: muteAction,
        findExsite: findExsite,
        setDefaultAttributes: setDefaultAttributes
    });

    function getSelectText(selectedValues, dlabel, labelMsg) {
        var label = "";
        if (selectedValues.length == 0)
            label = dlabel;
        else if (selectedValues.length == 1)
            label = selectedValues[0];
        else if (selectedValues.length == 2)
            label = selectedValues[0] + "," + selectedValues[1];
        else if (selectedValues.length == 3)
            label = selectedValues[0] + "," + selectedValues[1] + "," + selectedValues[2];
        else
            label = selectedValues.length + labelMsg;
        if (label.length > 21)
            label = label.substring(0, 20) + "..";
        return label;
    }

    function muteAction(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    function findExsite(arr, value, attname) {
        var index = -1;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i][attname] == value) {
                index = i
            }
        }
        return index;
    }

    function setDefaultAttributes(attrs) {
        
        if (attrs.entityname == undefined) return attrs;
        if (attrs.selectlbl == undefined) attrs.$set("selectlbl", "Selected "+attrs.entityname+ " :");
        if (attrs.placeholder == undefined) attrs.$set("placeholder", "Select " + attrs.entityname + "...");
        if (attrs.selectplaceholder == undefined) attrs.$set("selectplaceholder", attrs.entityname + " selected");

        return attrs;
    };

}]);


app.directive('vrDropdown', ['DropdownService', 'BaseDirService', function (DropdownService, BaseDirService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            datasource: '=',
            datasourceurl: '@',
            datatextfield: '@',
            datavaluefield: '@',
            selectlbl: '@',
            placeholder: '@',
            selectplaceholder: '@',
            entityname:'@'
        },
        controller: function () {

            var controller = this;
            this.selectedValues = [];
            this.filtername = '';

            this.getDatasource = function () {
                if (controller.datasource == undefined)
                    return controller.datasource;

                return controller.datasource;
            };

            this.getObjectProperty = function (item, property) {
                return BaseDirService.getObjectProperty(item, property);
            };

            this.getObjectText = function (item) {
                return controller.getObjectProperty(item, controller.datatextfield);
            };

            this.getObjectValue = function (item) {
                return controller.getObjectProperty(item, controller.datavaluefield);
            };

            this.findExsite = function (item) {
                return DropdownService.findExsite(controller.selectedValues, controller.getObjectValue(item), controller.datavaluefield);
            };

            this.muteAction = function (e) {
                DropdownService.muteAction(e);
            };

            this.selectValue = function (e, c) {
                controller.muteAction(e);
                var index = null;
                try {
                    var index = controller.selectedValues.indexOf(c);
                }
                catch (e) {

                }
                if (index >= 0) 
                    controller.selectedValues.splice(index, 1);
                else
                    controller.selectedValues.push(c);
            };

            this.clearFilter = function (e) {
                controller.muteAction(e);
                controller.filtername = '';
            };

            this.clearAllSelected = function (e) {
                controller.muteAction(e);
                controller.selectedValues = [];
                controller.selectedValues.length = 0;
            };

            this.getSelectText = function () {

                return DropdownService.getSelectText(controller.selectedValues, controller.placeholder, controller.selectplaceholder);
            };

            this.getUlClass = function () {
                return controller.selectedValues.length == 0 ? 'single-col-checklist' : 'double-col-checklist';
            };

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            attrs = DropdownService.setDefaultAttributes(attrs);
        },
        templateUrl: function (element, attrs) {
            return DropdownService.dTemplate;
        }

    };

    return directiveDefinitionObject;

}]);