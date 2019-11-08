'use strict';

app.directive('retailBillingChargetypeSelector', ['Retail_Billing_ChargeTypeAPIService', 'UtilsService', 'VRUIUtilsService', function (Retail_Billing_ChargeTypeAPIService, UtilsService, VRUIUtilsService) {
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
            normalColNum: '@',
            customvalidate: '=',
            customlabel: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var chargetypeSelector = new ChargetypeSelector(ctrl, $scope, $attrs);
            chargetypeSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function ChargetypeSelector(ctrl, $scope, attrs) {

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
                var targetRecordTypeId;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    targetRecordTypeId = payload.targetRecordTypeId;
                    ctrl.title = payload.title;
                }

                return Retail_Billing_ChargeTypeAPIService.GetRetailBillingChargeTypeInfo(targetRecordTypeId).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'RetailBillingChargeTypeId', attrs, ctrl);
                        }
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('RetailBillingChargeTypeId', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate(attrs) {
        var multipleselection = "";
        var label = '{{ctrl.title}}';
        if (attrs.ismultipleselection != undefined) {
            label = '{{ctrl.title}}';
            multipleselection = 'ismultipleselection';
        }

        return '<vr-columns colnum="{{ctrl.normalColNum}}">'
            + '<vr-label>' + label + '</vr-label>'
            + '<vr-select ' + multipleselection + ' datatextfield = "Name" datavaluefield = "RetailBillingChargeTypeId" isrequired = "ctrl.isrequired"  datasource = "ctrl.datasource" '
            + 'on-ready="ctrl.onSelectorReady" selectedvalues = "ctrl.selectedvalues" ' +
            + 'onselectionchanged = "ctrl.onselectionchanged" entityName = "' + label + '" onselectitem = "ctrl.onselectitem" ondeselectitem = "ctrl.ondeselectitem" '
            + 'hideremoveicon = "ctrl.hideremoveicon" customvalidate = "ctrl.customvalidate"></vr-select></vr-columns> ';
    }

}]);