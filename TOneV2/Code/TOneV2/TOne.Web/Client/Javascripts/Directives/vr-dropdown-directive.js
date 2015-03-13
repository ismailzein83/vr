'use strict';

app.service('DropdownService', ['BaseDirService', function (BaseDirService) {

    return ({
        dTemplate: BaseDirService.directiveMainURL + "vr-dropdown-standard.html",
        getSelectText: getSelectText,
        muteAction: muteAction,
        findExsite: findExsite
    });

    function getSelectText(selectedValues, dlabel, labelMsg) {
        var label;
        if (selectedValues.length == 0)
            label = dlabel;
        else if (selectedValues.length == 1)
            label = selectedValues[0].Name;
        else if (selectedValues.length == 2)
            label = selectedValues[0].Name + "," + selectedValues[1].Name;
        else if (selectedValues.length == 3)
            label = selectedValues[0].Name + "," + selectedValues[1].Name + "," + selectedValues[2].Name;
        else
            label = selectedValues.length + labelMsg;
        if (label.length > 21)
            label = label.substring(0, 20) + "..";
        return label;
    };

    function muteAction(e) {
        e.preventDefault();
        e.stopPropagation();
    };

    function findExsite(arr, value, attname) {
        var index = -1;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i][attname] == value) {
                index = i
            }
        }
        return index;
    };

}]);

app.directive('vrDropdown', ['DropdownService', function (DropdownService) {

    var directiveDefinitionObject = {

        restrict: 'E',
        scope: {
            datasource: '@',
            //value: '=',
            //btnClick: '&'
        },
        controller: function () {

            var controller = this;
            this.selectedValues = [];

            this.findExsite = function (arr, value, attname) {
                return DropdownService.findExsite(arr, value, attname);
            }

            this.muteAction = function (e) {
                DropdownService.muteAction(e);
            };

            this.selectValue = function ($event, c) {
                controller.muteAction(e);
                var index = null;
                try {
                    var index = controller.selectedValues.indexOf(c);
                }
                catch (e) {

                }
                if (index >= 0) {
                    controller.selectedValues.splice(index, 1);
                }
                else
                    controller.selectedValues.push(c);
            };
            
            this.getSelectText = function () {
                return DropdownService.getSelectText(controller.selectedValues, "Select Customers...", " Customers selected");
            };

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: function (element, attrs) {
            return DropdownService.dTemplate;
        }

    };

    return directiveDefinitionObject;

}]);