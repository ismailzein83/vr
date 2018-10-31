
app.directive('demoModulePageRunTimeBooleanFilter', ['VRNotificationService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, UtilsService, VRUIUtilsService) {
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
            isdisabled: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];
            ctrl.label;
            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var booleanFiltersSelector = new BooleanFiltersSelector(ctrl, $scope, $attrs);
            booleanFiltersSelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {

            return getBooleanFiltersTemplate(attrs);
        }
    };

    function getBooleanFiltersTemplate(attrs) {

        attrs.ismultipleselection = true;
        var multipleselection = "";
        var label = "";
        if (attrs.ismultipleselection != undefined) {
            label = "";
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns vr-disabled="ctrl.isdisabled" colnum="{{ctrl.normalColNum}}"  haschildcolumns  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="option" datavaluefield="value" isrequired="ctrl.isrequired"'
           + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Boolean Filter" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    };


    function BooleanFiltersSelector(ctrl, $scope, attrs) {


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

            api.load = function (payload) {
                selectorAPI.clearDataSource();

                ctrl.datasource.push({ option: "Yes", value:true});
                ctrl.datasource.push({ option: "No", value:false});
                var promises = [];
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var object = {};
                var data = VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                object.values = data;

                return {
                   $type: "Demo.Module.MainExtension.PageDefinition.BooleanFilter, Demo.Module.MainExtension",
                    "BoolValues": data
                }
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        };

        this.initializeController = initializeController;
    };
    return directiveDefinitionObject;

}]);