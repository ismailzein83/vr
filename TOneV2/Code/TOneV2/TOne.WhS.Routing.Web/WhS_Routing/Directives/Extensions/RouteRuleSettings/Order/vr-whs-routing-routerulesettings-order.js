'use strict';

app.directive('vrWhsRoutingRouterulesettingsOrder', ['WhS_Routing_RoutRuleSettingsAPIService', 'UtilsService', 'VRUIUtilsService',
function (WhS_Routing_RoutRuleSettingsAPIService, UtilsService, VRUIUtilsService) {

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
            }
        },
        templateUrl: function (element, attrs) {
            return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Order/Templates/RouteRuleOrderDirective.html';
        }
    };

    function filterCtor(ctrl, $scope) {
        var existingItems = [];
        ctrl.isValid = function () {
            var result = checkRequiredFilters();
            if (result != null) {
                return result;
            }

            if (ctrl.datasource.length < 2)
                return null;

            var total = 0;
            for (var x = 0; x < ctrl.datasource.length; x++) {
                total += parseFloat(ctrl.datasource[x].percentageValue);
            }

            if (total != 100)
                return "Sum of all Percentages must be equal to 100";

            return null;
        }
        ctrl.optionOrderSettingsGroupTemplates = [];
        ctrl.datasource = [];

        ctrl.addOptionOrderType = function () {
            addNewItem(ctrl.selectedOptionOrderSettingsGroupTemplate);
            ctrl.selectedOptionOrderSettingsGroupTemplate = undefined;
        };

        function addNewItem(itemAdded) {
            var dataItem = {
                id: ctrl.datasource.length + 1,
                configId: itemAdded.TemplateConfigID,
                editor: itemAdded.Editor,
                name: itemAdded.Name,
                percentageValue: undefined
            };

            dataItem.onDirectiveReady = function (api) {
                dataItem.directiveAPI = api;
                var setLoader = function (value) { ctrl.isLoadingDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
            };

            for (var x = 0; x < ctrl.optionOrderSettingsGroupTemplates.length; x++) {
                if (ctrl.optionOrderSettingsGroupTemplates[x].TemplateConfigID == itemAdded.TemplateConfigID) {
                    existingItems.push(ctrl.optionOrderSettingsGroupTemplates[x]);
                    ctrl.optionOrderSettingsGroupTemplates.splice(x, 1);
                    break;
                }
            }

            ctrl.datasource.push(dataItem);
        }

        ctrl.removeOption = function (dataItem) {
            var configId = dataItem.configId;
            ctrl.datasource.splice(ctrl.datasource.indexOf(dataItem), 1);
            for (var x = 0; x < existingItems.length; x++) {
                if (existingItems[x].TemplateConfigID == configId) {
                    ctrl.optionOrderSettingsGroupTemplates.push(existingItems[x]);
                    existingItems.splice(x, 1);
                    break;
                }
            }
        };

        function checkRequiredFilters() {
            var requiredFiltersNames = [];
            if (ctrl.optionOrderSettingsGroupTemplates.length == 0) {
                return null;
            }
            angular.forEach(ctrl.optionOrderSettingsGroupTemplates, function (dataItem) {
                if (dataItem.Settings != null && dataItem.Settings.IsRequired) {
                    requiredFiltersNames.push(dataItem.Name);
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

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                return loadOrderOptionSection(payload);
            }

            api.getData = function () {
                if (ctrl.datasource.length > 0)
                    return getOrderOptions();
                else
                    return null;
            }

            function loadOrderOptionSection(payload) {
                var promises = [];

                var orderOptions;

                if (payload != undefined) {
                    orderOptions = [];
                    for (var i = 0; i < payload.length; i++) {
                        var optionOrderItem = {
                            payload: payload[i],
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred()
                        };
                        promises.push(optionOrderItem.loadPromiseDeferred.promise);
                        orderOptions.push(optionOrderItem);
                    }
                }

                var loadTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionOrderSettingsTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
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
                                if (ctrl.datasource[x].configId == ctrl.optionOrderSettingsGroupTemplates[j].TemplateConfigID) {
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
                    
                    if (ctrl.optionOrderSettingsGroupTemplates.length > 0) {
                        for (var m = (ctrl.optionOrderSettingsGroupTemplates.length - 1) ; m >= 0; m--) {
                            var currentItem = ctrl.optionOrderSettingsGroupTemplates[m];
                            if (currentItem.Settings != null && currentItem.Settings.IsRequired) {
                                addNewItem(currentItem);
                                ctrl.optionOrderSettingsGroupTemplates.splice(m, 1);
                            }
                        }
                    }

                });

                promises.push(loadTemplatesPromise);

                function addOrderOptionItemToGrid(optionOrderItem) {
                    var matchItem = UtilsService.getItemByVal(ctrl.optionOrderSettingsGroupTemplates, optionOrderItem.payload.ConfigId, "TemplateConfigID");
                    if (matchItem == null)
                        return;
                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        configId: matchItem.TemplateConfigID,
                        editor: matchItem.Editor,
                        name: matchItem.Name,
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

                return orderOptions;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);