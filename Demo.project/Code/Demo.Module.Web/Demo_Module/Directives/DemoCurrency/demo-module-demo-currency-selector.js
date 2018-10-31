
app.directive('demoModuleDemoCurrencySelector', ['VRNotificationService', 'Demo_Module_DemoCurrencyAPIService', 'UtilsService', 'VRUIUtilsService',
function (VRNotificationService, Demo_Module_DemoCurrencyAPIService, UtilsService, VRUIUtilsService) {
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

            var demoCurrencySelector = new DemoCurrencySelector(ctrl, $scope, $attrs);
            demoCurrencySelector.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getDemoCurrencyTemplate(attrs);
        }
    };

    function getDemoCurrencyTemplate(attrs) {

        var multipleselection = "";
        var label = "DemoCurrency";
        if (attrs.ismultipleselection != undefined) {
            label = "DemoCurrency";
            multipleselection = "ismultipleselection";
        }

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns vr-disabled="ctrl.isdisabled" colnum="{{ctrl.normalColNum}}"  haschildcolumns  ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="DemoCurrencyId" isrequired="ctrl.isrequired"'
            + ' label="' + label + '" ' + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="DemoCurrency" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"' + hideremoveicon + '>'

            + '</vr-select></vr-columns>';
    };


    function DemoCurrencySelector(ctrl, $scope, attrs) {

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

                var CurrencyID;
                var filter;
                if (payload != undefined) {
                    if (payload.CurrencyID != undefined) {
                        CurrencyID = [];
                        CurrencyID.push(payload.CurrencyID);
                    }
                    filter = payload.filter;
                }
                return Demo_Module_DemoCurrencyAPIService.GetDemoCurrenciesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }
                        if (CurrencyID != undefined) {
                            VRUIUtilsService.setSelectedValues(CurrencyID, 'DemoCurrencyId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getSelectedIds = function () {
                return {
                    ID: VRUIUtilsService.getIdSelectedIds('DemoCurrencyId', attrs, ctrl),
                    Name: VRUIUtilsService.getIdSelectedIds('Name', attrs, ctrl)
                }
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };

        this.initializeController = initializeController;
    };
    return directiveDefinitionObject;

}]);