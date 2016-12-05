'use strict';
app.directive('vrRulesPricingrulesettingsRatetype', ['UtilsService', '$compile', 'VR_Rules_PricingRuleAPIService', 'VRUIUtilsService',
function (UtilsService, $compile, VR_Rules_PricingRuleAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.rateTypeTemplates = [];
            var ctor = new ratetypeCtor(ctrl, $scope, $attrs);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/VR_Rules/Directives/PricingRule/Templates/PricingRuleRateTypeSettings.html"

    };


    function ratetypeCtor(ctrl, $scope, $attrs) {
        function initializeController() {

            ctrl.datasource = [];
            ctrl.isValid = function () {

                if (ctrl.datasource.length > 0)
                    return null;
                return "You Should Select at least one filter type ";
            };
            ctrl.disableAddButton = true;
            ctrl.addFilter = function () {
                var dataItem = {
                    id: ctrl.datasource.length + 1,
                    configId: ctrl.selectedTemplate.ExtensionConfigurationId,
                    editor: ctrl.selectedTemplate.Editor,
                    name: ctrl.selectedTemplate.Title
                };
                dataItem.onDirectiveReady = function (api) {
                    dataItem.directiveAPI = api;
                    var setLoader = function (value) { ctrl.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
                };

                dataItem.onRateTypeSelectorReady = function (api) {
                    dataItem.rateTypeSelectorAPI = api;
                    var setLoader = function (value) { ctrl.isLoadingRateTypeDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.rateTypeSelectorAPI, undefined, setLoader);
                };

                ctrl.datasource.push(dataItem);

                ctrl.selectedTemplate = undefined;
            };
            ctrl.onActionTemplateChanged = function () {
                ctrl.disableAddButton = (ctrl.selectedTemplate == undefined);
            };
            ctrl.removeFilter = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                ctrl.datasource.splice(index, 1);
            };
            defineAPI();

        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "Vanrise.Rules.Pricing.PricingRuleRateTypeSettings, Vanrise.Rules.Pricing",
                    Items: getItems()
                };
                return obj;

                function getItems() {
                    var itemList = [];

                    angular.forEach(ctrl.datasource, function (item) {
                        var obj = item.directiveAPI.getData();
                        obj.ConfigId = item.configId;
                        obj.RateTypeId = item.rateTypeSelectorAPI.getSelectedIds();
                        itemList.push(obj);
                    });

                    return itemList;
                }
            };
            
            api.load = function (payload) {
                return loadFiltersSection(payload);
            };

            function loadFiltersSection(payload) {
                var promises = [];

                var settings;
                var filterItems;

                if (payload != undefined) {
                    settings = payload.settings;

                    if (settings != undefined && settings.Items != undefined) {
                        filterItems = [];
                        for (var i = 0; i < settings.Items.length; i++) {
                            var filterItem = {
                                payload: settings.Items[i],
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                readyRateTypePromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadRateTypePromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(filterItem.loadPromiseDeferred.promise);
                            filterItems.push(filterItem);
                        }
                    }
                }

                var loadTemplatesPromise = VR_Rules_PricingRuleAPIService.GetPricingRuleRateTypeTemplates().then(function (response) {
                    angular.forEach(response, function (itm) {
                        ctrl.rateTypeTemplates.push(itm);
                    });

                    if (filterItems != undefined) {
                        for (var i = 0; i < filterItems.length; i++) {
                            addFilterItemToGrid(filterItems[i]);
                        }
                    }
                });

                promises.push(loadTemplatesPromise);

                function addFilterItemToGrid(filterItem) {
                    var matchItem = UtilsService.getItemByVal(ctrl.rateTypeTemplates, filterItem.payload.ConfigId, "ExtensionConfigurationId");
                    if (matchItem == null)
                        return;

                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        configId: matchItem.ExtensionConfigurationId,
                        editor: matchItem.Editor,
                        name: matchItem.Title
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

                    dataItem.onRateTypeSelectorReady = function (api) {
                        dataItem.rateTypeSelectorAPI = api;
                        filterItem.readyRateTypePromiseDeferred.resolve();
                    };
                    var dataItemRateTypePayload = { selectedIds: filterItem.payload.RateTypeId };
                    filterItem.readyRateTypePromiseDeferred.promise
                       .then(function () {
                           VRUIUtilsService.callDirectiveLoad(dataItem.rateTypeSelectorAPI, dataItemRateTypePayload, filterItem.loadRateTypePromiseDeferred);
                       });
                    ctrl.datasource.push(dataItem);
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);