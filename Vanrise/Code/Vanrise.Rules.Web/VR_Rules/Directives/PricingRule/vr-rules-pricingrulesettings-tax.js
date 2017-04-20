'use strict';
app.directive('vrRulesPricingrulesettingsTax', ['UtilsService', '$compile', 'VR_Rules_PricingRuleAPIService', 'VRUIUtilsService',
function (UtilsService, $compile, VR_Rules_PricingRuleAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.taxTemplates = [];
            var ctor = new taxCtor(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Rules/Directives/PricingRule/Templates/PricingRuleTaxSettings.html"

    };


    function taxCtor(ctrl, $scope, $attrs) {

        var currencyDirectiveAPI;
        var currencyDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            ctrl.datasource = [];

            ctrl.onCurrencySelectReady = function (api) {
                currencyDirectiveAPI = api;
                currencyDirectiveReadyPromiseDeferred.resolve();
            };

            ctrl.isValid = function () {
                if (ctrl.datasource.length == 0)
                    return "You Should Select at least one filter type ";

                for (var x = 0; x < ctrl.datasource.length; x++) {
                    var currentItem = ctrl.datasource[x].Entity;

                    if (currentItem.fromAmount != undefined && currentItem.toAmount == undefined) 
                        return "To Amount should be filled.";

                    if (currentItem.fromAmount == undefined && currentItem.toAmount != undefined)
                        return "From Amount should be filled.";
                   

                    var currentItemFromAmount = parseFloat(currentItem.fromAmount);
                    var currentItemToAmount = parseFloat(currentItem.toAmount);
                    
                    for (var y = 0; y < ctrl.datasource.length; y++) {
                        if (x == y)
                            continue;
                        var otherItem = ctrl.datasource[y].Entity;
                        var otherItemFromAmount = parseFloat(otherItem.fromAmount);
                        var otherItemToAmount = parseFloat(otherItem.toAmount);

                        if ((currentItemToAmount > otherItemFromAmount && currentItemFromAmount <= otherItemFromAmount)
                            || (currentItemFromAmount < otherItemToAmount && currentItemToAmount >= otherItemToAmount))
                            return "Amounts Overlapped";
                        
                    }
                }

                return null;
            };

            ctrl.disableAddButton = true;
            ctrl.addFilter = function () {
                var dataItem = {
                    id: ctrl.datasource.length + 1,
                    configId: ctrl.selectedTemplate.ExtensionConfigurationId,
                    editor: ctrl.selectedTemplate.Editor,
                    name: ctrl.selectedTemplate.Title,
                    validate: function (item) {
                        if (item.Entity.fromAmount != undefined && item.Entity.toAmount != undefined && parseFloat(item.Entity.fromAmount) >= parseFloat(item.Entity.toAmount))
                            return 'From Amount should be less than To Amount';
                        return null;
                    }
                };
                dataItem.onDirectiveReady = function (api) {
                    dataItem.directiveAPI = api;
                    var setLoader = function (value) { ctrl.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
                };
                ctrl.datasource.push({ Entity: dataItem });


                ctrl.selectedTemplate = undefined;
            };
            ctrl.onActionTemplateChanged = function () {
                ctrl.disableAddButton = (ctrl.selectedTemplate == undefined);
            };
            ctrl.removeFilter = function (dataItem) {
                var index = ctrl.datasource.indexOf(dataItem);
                ctrl.datasource.splice(index, 1);
            };
            defineAPI();

        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "Vanrise.Rules.Pricing.PricingRuleTaxSettings, Vanrise.Rules.Pricing",
                    CurrencyId: currencyDirectiveAPI.getSelectedIds(),
                    Actions: getActions()
                };
                return obj;
            };
            function getActions() {
                var actionList = [];

                angular.forEach(ctrl.datasource, function (item) {
                    var obj = item.Entity.directiveAPI.getData();
                    obj.ConfigId = item.Entity.configId;
                    obj.FromAmount = item.Entity.fromAmount;
                    obj.ToAmount = item.Entity.toAmount;
                    actionList.push(obj);
                });

                return actionList;
            }
            api.load = function (payload) {
                return loadFiltersSection(payload);
            };

            function loadFiltersSection(payload) {
                var promises = [];

                var settings;
                var filterItems;

                if (payload != undefined) {
                    settings = payload.settings;

                    if (settings != undefined && settings.Actions != undefined) {
                        filterItems = [];
                        for (var i = 0; i < settings.Actions.length; i++) {
                            var filterItem = {
                                payload: settings.Actions[i],
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(filterItem.loadPromiseDeferred.promise);
                            filterItems.push(filterItem);
                        }
                    }
                }

                var loadTemplatesPromise = VR_Rules_PricingRuleAPIService.GetPricingRuleTaxTemplates().then(function (response) {
                    angular.forEach(response, function (itm) {
                        ctrl.taxTemplates.push(itm);
                    });

                    if (filterItems != undefined) {
                        for (var i = 0; i < filterItems.length; i++) {
                            addFilterItemToGrid(filterItems[i]);
                        }
                    }
                });

                promises.push(loadTemplatesPromise);

                function addFilterItemToGrid(filterItem) {
                    var matchItem = UtilsService.getItemByVal(ctrl.taxTemplates, filterItem.payload.ConfigId, "ExtensionConfigurationId");
                    if (matchItem == null)
                        return;

                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        configId: matchItem.ExtensionConfigurationId,
                        editor: matchItem.Editor,
                        name: matchItem.Title,
                        fromAmount: filterItem.payload.FromAmount,
                        toAmount: filterItem.payload.ToAmount,
                        validate: function (item) {
                            if (item.Entity.fromAmount != undefined && item.Entity.toAmount != undefined && parseFloat(item.Entity.fromAmount) >= parseFloat(item.Entity.toAmount))
                                return 'From Amount should be less than To Amount';
                            return null;
                        }
                    };
                    var dataItemPayload = filterItem.payload;

                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        filterItem.readyPromiseDeferred.resolve();
                    };

                    filterItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, filterItem.loadPromiseDeferred);
                        });

                    ctrl.datasource.push({ Entity: dataItem });
                }

                var loadCurrencySelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                var currencyPayload;

                if (settings != undefined && settings.CurrencyId > 0) {
                    currencyPayload = { selectedIds: settings.CurrencyId };
                }
                else {
                    currencyPayload = { selectSystemCurrency: true };
                }

                currencyDirectiveReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, currencyPayload, loadCurrencySelectorPromiseDeferred)

                });
                promises.push(loadCurrencySelectorPromiseDeferred.promise);

                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);