﻿'use strict';
app.directive('vrGenericdataDatarecordtypeSelector', ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService', '$compile', 'VRUIUtilsService', function (VR_GenericData_DataRecordTypeAPIService, UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            onReady: '=',
            label: "@",
            ismultipleselection: "@",
            hideselectedvaluessection: '@',
            onselectionchanged: '=',
            isrequired: '@',
            isdisabled: "=",
            selectedvalues: "=",
            showaddbutton: '@',
            hidelabel: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];
            ctrl.datasource = [];



            ctrl.datasource = [];
            var ctor = new recordTypeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function (element, attrs) {
            return getTemplate(attrs);
        }

    };
    function getTemplate(attrs) {
        var label;
        if (attrs.hidelabel == undefined)
            label = 'label="Record Type"';
        var disabled = "";
        if (attrs.isdisabled)
            disabled = "vr-disabled='true'"

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";
        var multipleselection = "";
        if (attrs.ismultipleselection != undefined)
            multipleselection = "ismultipleselection"

        return ' <vr-select ' + multipleselection + ' datasource="ctrl.datasource" ' + required + ' ' + hideselectedvaluessection + ' selectedvalues="ctrl.selectedvalues" ' + disabled + ' onselectionchanged="ctrl.onselectionchanged" datatextfield="Name" datavaluefield="DataRecordTypeId"'
               + 'entityname="Record Type" ' + label + '></vr-select>';

    }
    function recordTypeCtor(ctrl, $scope, $attrs) {

        function initializeController() {
         
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('DataRecordTypeId', $attrs, ctrl);
            }
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) { 
                    selectedIds = payload.selectedIds;
                }

                return VR_GenericData_DataRecordTypeAPIService.GetDataRecordTypeInfo().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);

                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'DataRecordTypeId', $attrs, ctrl);

                });
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

