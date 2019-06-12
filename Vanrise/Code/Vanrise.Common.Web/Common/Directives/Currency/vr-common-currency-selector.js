﻿'use strict';
app.directive('vrCommonCurrencySelector', ['VRCommon_CurrencyAPIService', 'VRCommon_CurrencyService', 'UtilsService', 'VRUIUtilsService', '$filter',
    function (VRCommon_CurrencyAPIService, VRCommon_CurrencyService, UtilsService, VRUIUtilsService, $filter) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                localizedlabel: "@",
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

                $scope.addNewCurrency = function () {
                    var onCurrencyAdded = function (currencyObj) {
                        ctrl.datasource.push(currencyObj.Entity);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(currencyObj.Entity);
                        else
                            ctrl.selectedvalues = currencyObj.Entity;
                    };
                    VRCommon_CurrencyService.addCurrency(onCurrencyAdded);
                };

                ctrl.haspermission = function () {
                    return VRCommon_CurrencyAPIService.HasAddCurrencyPermission();
                };

                ctrl.ViewCurrency = function (obj) {
                    VRCommon_CurrencyService.viewCurrency(obj.CurrencyId);
                };


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
            var localizedlabel = "VRRes.Common.Currency.VREnd";

            if (attrs.ismultipleselection != undefined) {
                label = "Currencies";
                localizedlabel = "VRRes.Common.Currencies.VREnd";

                multipleselection = "ismultipleselection";
            }

            var hidelabel = "";
            if (attrs.hidelabel != undefined)
                hidelabel = " hidelabel ";

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewCurrency"';
            if (attrs.localizedlabel != undefined)
                localizedlabel = attrs.localizedlabel;
            var viewCliked = "";
            if (attrs.showviewbutton != undefined)
                viewCliked = 'onviewclicked="ctrl.ViewCurrency"';
            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";
            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + ' localizedlabel="' + localizedlabel + '"  on-ready="ctrl.onSelectorReady" datatextfield="Symbol" datavaluefield="CurrencyId"' + hidelabel + ' label="' + label + '" ' + addCliked + viewCliked + '  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Currency" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"  ' + hideremoveicon + ' isrequired="ctrl.isrequired" haspermission="ctrl.haspermission"></vr-select></vr-columns>';
        }

        function currencyCtor(ctrl, $scope, attrs) {
            var selectorAPI;
            var systemCurrency;
            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };


            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var selectSystemCurrency;
                    var excludeSystemCurrency;
                    selectorAPI.clearDataSource();

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        selectSystemCurrency = payload.selectSystemCurrency;
                        excludeSystemCurrency = payload.excludeSystemCurrency;
                    }

                    var rootPromiseNode = {};

                    var initialPromises = [];
                    if ((selectedIds == undefined && selectSystemCurrency === true) || excludeSystemCurrency === true) {
                        initialPromises.push(getSystemCurrency());
                        var fillCurrencySelectorDataSource = function () {

                            var getAllCurrenciesPromise = getAllCurrencies();
                            getAllCurrenciesPromise.then(function () {
                                if (!excludeSystemCurrency) {
                                    var systemCurrencyId = (attrs.ismultipleselection != undefined) ? [systemCurrency.CurrencyId] : systemCurrency.CurrencyId;
                                    VRUIUtilsService.setSelectedValues(systemCurrencyId, 'CurrencyId', attrs, ctrl);
                                }
                            });
                            return {
                                promises: [getAllCurrenciesPromise]
                            };
                        };
                        rootPromiseNode.getChildNode = fillCurrencySelectorDataSource;
                    }
                    else {
                        initialPromises.push(getAllCurrencies());
                    }

                    rootPromiseNode.promises = initialPromises;


                    return UtilsService.waitPromiseNode(rootPromiseNode);


                    function getAllCurrencies() {
                        return VRCommon_CurrencyAPIService.GetAllCurrencies().then(function (response) {
                            if (response != null) {
                                var data = $filter('orderBy')(response, 'Symbol');
                                for (var i = 0; i < data.length; i++) {
                                    if (excludeSystemCurrency && systemCurrency != undefined && data[i].CurrencyId == systemCurrency.CurrencyId)
                                        continue;
                                    ctrl.datasource.push(data[i]);
                                }

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