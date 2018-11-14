'use strict';

app.directive('vrWhsRoutingRouterulesettingsOrder', ['WhS_Routing_RoutRuleSettingsAPIService', 'WhS_Routing_OrderTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRDragdropService',
    function (WhS_Routing_RoutRuleSettingsAPIService, WhS_Routing_OrderTypeEnum, UtilsService, VRUIUtilsService, VRDragdropService) {

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
            var context;

            ctrl.optionOrderSettingsGroupTemplates = [];
            ctrl.dragdropGroupCorrelation = VRDragdropService.createCorrelationGroup();
            ctrl.dragdropSetting = {
                groupCorrelation: ctrl.dragdropGroupCorrelation,
                canReceive: true,
                onItemReceived: function (itemAdded, dataSource) {

                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        configId: itemAdded.ExtensionConfigurationId,
                        editor: itemAdded.Editor,
                        name: itemAdded.Title,
                        percentageValue: undefined
                    };

                    dataItem.onDirectiveReady = function (api) {
                        dataItem.directiveAPI = api;
                        var setLoader = function (value) { ctrl.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
                    };
                    var filter = {
                        OrderOptionCriteriaLength: ctrl.datasource.length
                    };
                    loadOrderTypeSelector(filter);
                    return dataItem;
                },
                enableSorting: true
            };
            ctrl.datasource = [];
            ctrl.isValid = function () {
                var result = checkRequiredFilters();
                if (result != null) {
                    return result;
                }

                if (ctrl.datasource.length <= 1)
                    return null;

                if ($scope.scopeModel.selectedRouteRuleSettingsOrderType != undefined) {
                    if (!$scope.scopeModel.selectedRouteRuleSettingsOrderType.hasCheckValidation)
                        return null;

                    if ($scope.scopeModel.selectedRouteRuleSettingsOrderType == WhS_Routing_OrderTypeEnum.Percentage || $scope.scopeModel.selectedRouteRuleSettingsOrderType == WhS_Routing_OrderTypeEnum.OptionDistribution) {
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

            ctrl.removeOption = function (dataItem) {
                ctrl.datasource.splice(ctrl.datasource.indexOf(dataItem), 1);

                var filter = {
                    OrderOptionCriteriaLength: ctrl.datasource.length
                };
                loadOrderTypeSelector(filter);
            };

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.isLoadingOrderType = false;
                $scope.scopeModel.onRouteRuleSettingsOrderTypeSelectorReady = function (api) {
                    routeRuleSettingsOrderTypeSelectorAPI = api;
                    routeRuleSettingsOrderTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.showPercentage = function () {
                    if (routeRuleSettingsOrderTypeSelectorAPI != undefined) {
                        var selectedId = routeRuleSettingsOrderTypeSelectorAPI.getSelectedIds();
                        var orderTypeObj = UtilsService.getEnum(WhS_Routing_OrderTypeEnum, 'value', selectedId);
                        if (orderTypeObj && ctrl.datasource.length > 1) {
                            return orderTypeObj.showPercentageColumn;
                        }
                    }
                    return false;
                };

                $scope.scopeModel.onOrderTypeSelectionChange = function () {
                    var selectedId = routeRuleSettingsOrderTypeSelectorAPI.getSelectedIds();
                    var orderTypeObj = UtilsService.getEnum(WhS_Routing_OrderTypeEnum, 'value', selectedId);
                    if (orderTypeObj) {
                        context.showPercentageSettings(orderTypeObj.showPercentageSettings);
                    }
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.optionOrderSettingsGroupTemplates.length = 0;
                    ctrl.datasource.length = 0;

                    if (payload != undefined)
                        context = payload.context;

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
                    var filter;

                    if (payload != undefined) {
                        optionOrderSettings = payload.optionOrderSettings;
                        orderType = payload.orderType;
                    }

                    //loading RouteRuleSettingsOrderType Selector 
                    if (optionOrderSettings != undefined)
                        filter = {
                            OrderOptionCriteriaLength: optionOrderSettings.length
                        };
                    var loadRouteRuleSettingsOrderTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                    routeRuleSettingsOrderTypeSelectorReadyPromiseDeferred.promise.then(function () {

                        var payload = {
                            selectedIds: orderType,
                            filter: filter
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


                        if (requiredItems.length > 0 && ctrl.optionOrderSettingsGroupTemplates.length > 0) {
                            for (var y = 0; y < requiredItems.length; y++) {
                                var currentRequiredItem = requiredItems[y];
                                for (var z = 0; z < ctrl.optionOrderSettingsGroupTemplates.length; z++) {
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
                    var showPercentage = $scope.scopeModel.showPercentage();

                    angular.forEach(ctrl.datasource, function (dataItem) {
                        var orderOption = dataItem.directiveAPI.getData();
                        orderOption.ConfigId = dataItem.configId;
                        if (ctrl.datasource.length == 1 && $scope.scopeModel.selectedRouteRuleSettingsOrderType != undefined && $scope.scopeModel.selectedRouteRuleSettingsOrderType.showPercentageColumn)
                            orderOption.PercentageValue = 100;
                        else
                            orderOption.PercentageValue = showPercentage ? dataItem.percentageValue : undefined;
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
                    var setLoader = function (value) { ctrl.isLoadingDirective = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
                };


                ctrl.datasource.push(dataItem);
            }

            function loadOrderTypeSelector(filter) {
                var payload = {
                    filter: filter
                };
                var setLoader = function (value) { $scope.scopeModel.isLoadingOrderType = value; };
                routeRuleSettingsOrderTypeSelectorAPI.load(payload);
            }
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);