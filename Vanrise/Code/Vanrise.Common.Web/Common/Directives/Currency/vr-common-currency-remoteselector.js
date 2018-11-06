'use strict';
app.directive('vrCommonCurrencyRemoteselector', ['VRCommon_VRRestCurrencyAPIService', 'UtilsService', 'VRUIUtilsService', '$filter',
    function (VRCommon_VRRestCurrencyAPIService, UtilsService, VRUIUtilsService, $filter) {

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
                isdisabled: "=",
                hideremoveicon: '@',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new currencyCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getCurrencyTemplate(attrs);
            }

        };


        function getCurrencyTemplate(attrs) {

            var multipleselection = "";
            var label = "Currency";
            if (attrs.ismultipleselection != undefined) {
                label = "Currencies";
                multipleselection = "ismultipleselection";
            }
            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";
            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="Symbol" datavaluefield="CurrencyId" label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Currency" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"  ' + hideremoveicon + ' isrequired="ctrl.isrequired"></vr-select></vr-columns>';
        }

        function currencyCtor(ctrl, $scope, attrs) {
            var selectorAPI;
            var connectionId;
            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload)
                {
                    var promises = [];

                    var selectedIds;
                    var selectSystemCurrency;
                    
                    selectorAPI.clearDataSource();

                    if (payload != undefined)
                    {
                        connectionId = payload.connectionId;
                        selectedIds = payload.selectedIds;
                        selectSystemCurrency = payload.selectSystemCurrency;
                    }

                    var getAllCurrenciesPromise = getRemoteAllCurrencies();
                    promises.push(getAllCurrenciesPromise);

                    if (selectedIds == undefined && selectSystemCurrency === true)
                    {
                        var systemCurrency;

                        var getSystemCurrencyPromise = getRemoteSystemCurrency();
                        promises.push(getSystemCurrencyPromise);

                        UtilsService.waitMultiplePromises([getAllCurrenciesPromise, getSystemCurrencyPromise]).then(function ()
                        {
                            var systemCurrencyId = (attrs.ismultipleselection != undefined) ? [systemCurrency.CurrencyId] : systemCurrency.CurrencyId;
                            VRUIUtilsService.setSelectedValues(systemCurrencyId, 'CurrencyId', attrs, ctrl);
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);

                    function getRemoteAllCurrencies()
                    {
                        return VRCommon_VRRestCurrencyAPIService.GetRemoteAllCurrencies(connectionId).then(function (response)
                        {
                            if (response != null)
                            {
                                var data = $filter('orderBy')(response, 'Symbol');
                                for (var i = 0; i < data.length; i++)
                                    ctrl.datasource.push(data[i]);

                                if (selectedIds != undefined)
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'CurrencyId', attrs, ctrl);
                            }
                        });
                    }
                    function getRemoteSystemCurrency() {
                        return VRCommon_VRRestCurrencyAPIService.GetRemoteSystemCurrency(connectionId).then(function (response) {
                            systemCurrency = response;
                        });
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CurrencyId', attrs, ctrl);
                };

                api.selectedCurrency = function (selectedIds) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'CurrencyId', attrs, ctrl);
                };
                api.getSelectedValues = function () {
                    return ctrl.selectedvalues;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);