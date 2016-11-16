﻿'use strict';

app.directive('vrWhsRoutingRouterulesettingsOrder', ['WhS_Routing_RoutRuleSettingsAPIService', 'WhS_Routing_OrderTypeEnum', 'UtilsService', 'VRUIUtilsService',
function (WhS_Routing_RoutRuleSettingsAPIService, WhS_Routing_OrderTypeEnum, UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new filterCtor(ctrl, $scope);
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
        templateUrl: function (element, attrs) {
            return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Order/Templates/RouteRuleOrderDirective.html';
        }
    };

    function filterCtor(ctrl, $scope) {
        var existingItems = [];

        var routeRuleSettingsOrderTypeSelectorAPI;
        var routeRuleSettingsOrderTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        ctrl.optionOrderSettingsGroupTemplates = [];
        ctrl.datasource = [];
        ctrl.isValid = function () {
            var result = checkRequiredFilters();
            if (result != null) {
                return result;
            }

            if (ctrl.datasource.length < 2)
                return null;

            if ($scope.scopeModel.selectedRouteRuleSettingsOrderType != undefined) {
                if (!$scope.scopeModel.selectedRouteRuleSettingsOrderType.hasCheckValidation)
                    return null;

                if ($scope.scopeModel.selectedRouteRuleSettingsOrderType == WhS_Routing_OrderTypeEnum.Percentage) {
                    var total = 0;
                    for (var x = 0; x < ctrl.datasource.length; x++) {
                        total += parseFloat(ctrl.datasource[x].percentageValue);
                    }

                    if (total != 100)
                        return "Sum of all Percentages must be equal to 100";
                }
            }
            return null;
        };
        ctrl.addOptionOrderType = function () {
            addNewItem(ctrl.selectedOptionOrderSettingsGroupTemplate);
            ctrl.selectedOptionOrderSettingsGroupTemplate = undefined;
        };
        ctrl.removeOption = function (dataItem) {
            var configId = dataItem.configId;
            ctrl.datasource.splice(ctrl.datasource.indexOf(dataItem), 1);
            for (var x = 0; x < existingItems.length; x++) {
                if (existingItems[x].ExtensionConfigurationId == configId) {
                    ctrl.optionOrderSettingsGroupTemplates.push(existingItems[x]);
                    existingItems.splice(x, 1);
                    break;
                }
            }
        };

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onRouteRuleSettingsOrderTypeSelectorReady = function (api) {
                routeRuleSettingsOrderTypeSelectorAPI = api;
                routeRuleSettingsOrderTypeSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.showPercentage = function () {

                if (ctrl.datasource.length > 1 && $scope.scopeModel.selectedRouteRuleSettingsOrderType != undefined
                    && $scope.scopeModel.selectedRouteRuleSettingsOrderType == WhS_Routing_OrderTypeEnum.Percentage)
                    return true;

                return false;
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                return loadOrderOptionSection(payload);
            };

            api.getData = function () {
                if (ctrl.datasource.length > 0)
                    return getOrderOptions();
                else
                    return null;
            };

            function loadOrderOptionSection(payload) {
                var promises = [];

                var orderOptions = [];
                var requiredItems = [];

                var optionOrderSettings;
                var orderType;

                if (payload != undefined) {
                    optionOrderSettings = payload.optionOrderSettings;
                    orderType = payload.orderType;
                }

                //loading RouteRuleSettingsOrderType Selector 
                var loadRouteRuleSettingsOrderTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                routeRuleSettingsOrderTypeSelectorReadyPromiseDeferred.promise.then(function () {

                    var payload = {
                        selectedIds: orderType != undefined ? orderType : WhS_Routing_OrderTypeEnum.Percentage.value
                    };

                    VRUIUtilsService.callDirectiveLoad(routeRuleSettingsOrderTypeSelectorAPI, payload, loadRouteRuleSettingsOrderTypeSelectorPromiseDeferred);
                });
                promises.push(loadRouteRuleSettingsOrderTypeSelectorPromiseDeferred.promise);


                if (optionOrderSettings != undefined) {

                    for (var i = 0; i < optionOrderSettings.length; i++) {
                        var optionOrderItem = {
                            payload: optionOrderSettings[i],
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred()
                        };
                        promises.push(optionOrderItem.loadPromiseDeferred.promise);
                        orderOptions.push(optionOrderItem);
                    }
                }

                var loadTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionOrderSettingsTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        if (item.IsRequired) {
                            requiredItems.push(item);
                        }
                        ctrl.optionOrderSettingsGroupTemplates.push(item);
                    });

                    if (orderOptions != undefined) {
                        for (var i = 0; i < orderOptions.length; i++) {
                            addOrderOptionItemToGrid(orderOptions[i]);
                        }
                    }

                    if (ctrl.datasource.length > 0) {
                        for (var x = 0; x < ctrl.datasource.length; x++) {
                            var itemFound = false;
                            for (var j = 0; j < ctrl.optionOrderSettingsGroupTemplates.length; j++) {
                                if (ctrl.datasource[x].configId == ctrl.optionOrderSettingsGroupTemplates[j].ExtensionConfigurationId) {
                                    existingItems.push(ctrl.optionOrderSettingsGroupTemplates[j]);
                                    ctrl.optionOrderSettingsGroupTemplates.splice(j, 1);
                                    itemFound = true;
                                    break;
                                }
                            }
                            if (itemFound) {
                                continue;
                            }
                        }
                    }

                    if (requiredItems.length > 0 && ctrl.optionOrderSettingsGroupTemplates.length > 0) {
                        for (var y = 0; y < requiredItems.length; y++) {
                            var currentRequiredItem = requiredItems[y];
                            for (var z = 0 ; z < ctrl.optionOrderSettingsGroupTemplates.length; z++) {
                                var currentItem = ctrl.optionOrderSettingsGroupTemplates[z];
                                if (currentItem.ExtensionConfigurationId == currentRequiredItem.ExtensionConfigurationId) {
                                    addNewItem(currentItem);
                                    break;
                                }
                            }
                        }
                    }

                });
                promises.push(loadTemplatesPromise);

                function addOrderOptionItemToGrid(optionOrderItem) {
                    var matchItem = UtilsService.getItemByVal(ctrl.optionOrderSettingsGroupTemplates, optionOrderItem.payload.ConfigId, "ExtensionConfigurationId");
                    if (matchItem == null)
                        return;
                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        configId: matchItem.ExtensionConfigurationId,
                        editor: matchItem.Editor,
                        name: matchItem.Title,
                        percentageValue: optionOrderItem.payload.PercentageValue
                    };
                    var dataItemPayload = optionOrderItem.payload;

                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        optionOrderItem.readyPromiseDeferred.resolve();
                    };

                    optionOrderItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, optionOrderItem.loadPromiseDeferred);
                        });

                    ctrl.datasource.push(dataItem);
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            function getOrderOptions() {
                var orderOptions = [];

                angular.forEach(ctrl.datasource, function (dataItem) {

                    var orderOption = dataItem.directiveAPI.getData();
                    orderOption.ConfigId = dataItem.configId;
                    orderOption.PercentageValue = ctrl.datasource.length > 1 ? dataItem.percentageValue : undefined;
                    orderOptions.push(orderOption);
                });

                return {
                    OrderType: routeRuleSettingsOrderTypeSelectorAPI.getSelectedIds(),
                    OptionOrderSettings: orderOptions,
                };
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function checkRequiredFilters() {
            var requiredFiltersNames = [];
            if (ctrl.optionOrderSettingsGroupTemplates.length == 0) {
                return null;
            }
            angular.forEach(ctrl.optionOrderSettingsGroupTemplates, function (dataItem) {
                if (dataItem.IsRequired) {
                    requiredFiltersNames.push(dataItem.Title);
                }
            });
            var result = null;
            if (requiredFiltersNames.length > 0) {
                result = '';
                if (requiredFiltersNames.length == 1) {
                    result = 'Order ' + requiredFiltersNames[0] + ' is required';
                }
                else {
                    result = result = 'Order ' + requiredFiltersNames[0];
                    for (var x = 1; x < requiredFiltersNames.length; x++) {
                        var currentItem = requiredFiltersNames[x];
                        if (x == requiredFiltersNames.length - 1) {
                            result += ' and ' + currentItem + ' are required';
                        }
                        else {
                            result += ', ' + currentItem;
                        }
                    }
                }
            }
            return result;
        }

        function addNewItem(itemAdded) {
            var dataItem = {
                id: ctrl.datasource.length + 1,
                configId: itemAdded.ExtensionConfigurationId,
                editor: itemAdded.Editor,
                name: itemAdded.Title,
                percentageValue: undefined
            };

            dataItem.onDirectiveReady = function (api) {
                dataItem.directiveAPI = api;
                var setLoader = function (value) { ctrl.isLoadingDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
            };

            for (var x = 0; x < ctrl.optionOrderSettingsGroupTemplates.length; x++) {
                if (ctrl.optionOrderSettingsGroupTemplates[x].ExtensionConfigurationId == itemAdded.ExtensionConfigurationId) {
                    existingItems.push(ctrl.optionOrderSettingsGroupTemplates[x]);
                    ctrl.optionOrderSettingsGroupTemplates.splice(x, 1);
                    break;
                }
            }

            ctrl.datasource.push(dataItem);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);