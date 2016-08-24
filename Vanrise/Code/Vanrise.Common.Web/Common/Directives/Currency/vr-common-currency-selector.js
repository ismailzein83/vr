'use strict';
app.directive('vrCommonCurrencySelector', ['VRCommon_CurrencyAPIService', 'VRCommon_CurrencyService', 'UtilsService', 'VRUIUtilsService',
    function (VRCommon_CurrencyAPIService, VRCommon_CurrencyService, UtilsService, VRUIUtilsService) {

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
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                $scope.addNewCurrency = function () {
                    var onCurrencyAdded = function (currencyObj) {
                        ctrl.datasource.push(currencyObj);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(currencyObj);
                        else
                            ctrl.selectedvalues = currencyObj;
                    };
                    VRCommon_CurrencyService.addCurrency(onCurrencyAdded);
                }


                var ctor = new currencyCtor(ctrl, $scope, $attrs);
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

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewCurrency"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="Symbol" datavaluefield="CurrencyId" label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Currency" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired"></vr-select>'
               + '</div>'
        }

        function currencyCtor(ctrl, $scope, attrs) {
            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                }


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
                        selectedIds = payload.selectedIds;
                        selectSystemCurrency = payload.selectSystemCurrency;
                    }

                    var getAllCurrenciesPromise = getAllCurrencies();
                    promises.push(getAllCurrenciesPromise);

                    if (selectedIds == undefined && selectSystemCurrency === true)
                    {
                        var systemCurrency;

                        var getSystemCurrencyPromise = getSystemCurrency();
                        promises.push(getSystemCurrencyPromise);

                        UtilsService.waitMultiplePromises([getAllCurrenciesPromise, getSystemCurrencyPromise]).then(function ()
                        {
                            var systemCurrencyId = (attrs.ismultipleselection != undefined) ? [systemCurrency.CurrencyId] : systemCurrency.CurrencyId;
                            VRUIUtilsService.setSelectedValues(systemCurrencyId, 'CurrencyId', attrs, ctrl);
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);

                    function getAllCurrencies()
                    {
                        return VRCommon_CurrencyAPIService.GetAllCurrencies().then(function (response)
                        {
                            if (response != null)
                            {
                                for (var i = 0; i < response.length; i++)
                                    ctrl.datasource.push(response[i]);

                                if (selectedIds != undefined)
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'CurrencyId', attrs, ctrl);
                            }
                        });
                    }
                    function getSystemCurrency() {
                        return VRCommon_CurrencyAPIService.GetSystemCurrency().then(function (response) {
                            systemCurrency = response;
                        });
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CurrencyId', attrs, ctrl);
                }

                api.selectedCurrency = function (selectedIds) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'CurrencyId', attrs, ctrl);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);