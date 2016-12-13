'use strict';
app.directive('vrWhsBeInvoiceSettingSelector', ['WhS_BE_InvoiceSettingAPIService', 'UtilsService', 'VRUIUtilsService',
function (WhS_BE_InvoiceSettingAPIService, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "@",
            selectedvalues: '=',
            normalColNum: '@'

        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
            var ctor = new invoiceSettingCtor(ctrl, $scope, $attrs);
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

        var multipleselection = "";
        var label = "Customer Invoice Settings";
        if (attrs.ismultipleselection != undefined) {
            label = "Customer Invoice Settings";
            multipleselection = "ismultipleselection";
        }

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
            + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="InvoiceSettingId" '
        + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled"></vr-select>'
           + '</vr-columns>';
    }

    function invoiceSettingCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('InvoiceSettingId', $attrs, ctrl);
            };
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return WhS_BE_InvoiceSettingAPIService.GetInvoiceSettingsInfo().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);
                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'InvoiceSettingId', $attrs, ctrl);

                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);