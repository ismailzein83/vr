
app.directive('demoModuleDemoCountrySelector', ['VRNotificationService', 'Demo_Module_DemoCountryAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_DemoCountryAPIService, UtilsService, VRUIUtilsService) {
    'use strict';
    
    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchanged: '=',
            selectedvalues: '=',
            isrequired: "=",
            onselectitem: "=",
            ondeselectitem: "=",
            hideremoveicon: '@',
            normalColNum: '@',
            isdisabled:'='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var demoCountrySelector = new DemoCountrySelector(ctrl, $scope, $attrs);
            demoCountrySelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {

            return getDemoCountryTemplate(attrs);
        }
    };

    function getDemoCountryTemplate(attrs) {


        var multipleselection = "";
        var label = "DemoCountry";
        if (attrs.ismultipleselection != undefined) {
            label = "DemoCountry";
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns vr-disabled="ctrl.isdisabled" colnum="{{ctrl.normalColNum}}"  haschildcolumns  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="DemoCountryId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="DemoCountry" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    };


    function DemoCountrySelector(ctrl, $scope, attrs) {


        var selectorAPI;

        function initializeController() {


            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        };

        function defineAPI() {
            var api = {};

            api.load = function (payload) { //payload is an object that has selectedids and filter

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;
                if (payload != undefined) {
                    if (payload.selectedIds != undefined) {
                        selectedIds = [];
                        selectedIds.push(payload.selectedIds);
                    }
                    filter = payload.filter;
                }
                return Demo_Module_DemoCountryAPIService.GetDemoCountriesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'DemoCountryId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('DemoCountryId', attrs, ctrl);
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        };

        this.initializeController = initializeController;
    };
    return directiveDefinitionObject;

}]);