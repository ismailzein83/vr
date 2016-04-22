'use strict';
app.directive('vrGenericdataDatarecordtypefieldsSelector', ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService', '$compile', 'VRUIUtilsService', function (VR_GenericData_DataRecordTypeAPIService, UtilsService, $compile, VRUIUtilsService) {

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
            hidelabel: '@',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
            ctrl.datasource = [];

            var ctor = new recordTypeFieldsCtor(ctrl, $scope, $attrs);
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
            if (attrs.label == undefined)
                label = 'label="Field"';
            else
                label = 'label="' + attrs.label + '"';

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

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

        return ' <vr-select ' + multipleselection + ' datasource="ctrl.datasource" ' + required + ' ' + hideselectedvaluessection + ' ' + hideremoveicon + ' selectedvalues="ctrl.selectedvalues" ' + disabled + ' onselectionchanged="ctrl.onselectionchanged" datatextfield="Name" datavaluefield="Name"'
               + 'entityname="Field" ' + label + '></vr-select>';

    }

    function recordTypeFieldsCtor(ctrl, $scope, $attrs) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('Name', $attrs, ctrl);
            }

            api.clearDataSource = function () {
                ctrl.datasource.length = 0;
                ctrl.selectedvalues = undefined;
            }


            api.load = function (payload) {
                var selectedIds;
                var dataRecordTypeId;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    dataRecordTypeId = payload.dataRecordTypeId;
                }

                return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                    api.clearDataSource();

                    if (response.Fields != undefined)
                        angular.forEach(response.Fields, function (item) {
                            item.fieldTypeEditor = 'vr-genericdata-fieldtype-text-filtereditor';
                            ctrl.datasource.push(item);
                        });
                    console.log(ctrl.datasource);
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'Name', $attrs, ctrl);
                });
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);

        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

