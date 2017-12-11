'use strict';
app.directive('vrCommonCompanySettingSelector', ['VRCommon_CompanySettingsAPIService', 'UtilsService', 'VRUIUtilsService', 'VRCommon_CompanySettingService',
function (VRCommon_CompanySettingsAPIService, UtilsService, VRUIUtilsService, VRCommon_CompanySettingService) {

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


            $scope.addNewCompany = function () {
                var isDefault = ctrl.datasource.length == 0;
                var onCompanyAdded = function (companyObj) {
                    ctrl.datasource.push(companyObj);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(companyObj);
                    else
                        ctrl.selectedvalues = companyObj;
                };
                VRCommon_CompanySettingService.addCompanySetting(onCompanyAdded, isDefault, undefined, true);
            };
            var ctor = new CompanySettingsCtor(ctrl, $scope, $attrs);
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
        var label = "Company Setting";
        if (attrs.ismultipleselection != undefined) {
            label = "Company Settings";
            multipleselection = "ismultipleselection";
        }

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";
        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewCompany"';

        return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
            + '<vr-select ' + multipleselection + ' ' + addCliked + ' datatextfield="CompanyName" datavaluefield="CompanySettingId" '
        + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged" vr-disabled="ctrl.isdisabled"></vr-select>'
           + '</vr-columns>';
    }

    function CompanySettingsCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {

            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('CompanySettingId', $attrs, ctrl);
            };
            api.load = function (payload) {

                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return VRCommon_CompanySettingsAPIService.GetCompanySettingsInfo().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.datasource.push(item);
                    });
                    if (selectedIds != undefined)
                        VRUIUtilsService.setSelectedValues(selectedIds, 'CompanySettingId', $attrs, ctrl);

                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);