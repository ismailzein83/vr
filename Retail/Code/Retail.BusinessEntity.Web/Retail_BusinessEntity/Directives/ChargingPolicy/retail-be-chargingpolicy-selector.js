'use strict';

app.directive('retailBeChargingpolicySelector', ['Retail_BE_ChargingPolicyAPIService', 'UtilsService', 'VRUIUtilsService', function (Retail_BE_ChargingPolicyAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: '@',
            selectedvalues: '=',
            onselectionchanged: '=',
            onselectitem: '=',
            ondeselectitem: '=',
            isrequired: '=',
            hideremoveicon: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var chargingPolicySelector = new ChargingPolicySelector(ctrl, $scope, $attrs);
            chargingPolicySelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function ChargingPolicySelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        function initializeController() {
            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;

                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                }

                return Retail_BE_ChargingPolicyAPIService.GetChargingPoliciesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'ChargingPolicyId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('ChargingPolicyId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "Charging Policy";

        if (attrs.ismultipleselection != undefined) {
            label = "Charging Policies";
            multipleselection = "ismultipleselection";
        }
        var hideremoveicon;
        if (attrs.hideremoveicon != undefined)
        {
            hideremoveicon = "hideremoveicon";
        }
        return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="ChargingPolicyId" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon +' ></vr-select></vr-columns>';
    }

}]);