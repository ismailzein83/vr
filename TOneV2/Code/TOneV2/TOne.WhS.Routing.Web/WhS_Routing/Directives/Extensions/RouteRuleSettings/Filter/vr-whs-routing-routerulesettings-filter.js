'use strict';
app.directive('vrWhsRoutingRouterulesettingsFilter', ['WhS_Routing_RoutRuleSettingsAPIService', 'UtilsService', 'VRUIUtilsService',
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
            return '/Client/Modules/WhS_Routing/Directives/Extensions/RouteRuleSettings/Filter/Templates/RouteRuleFilterDirective.html';
        }
    };

    function filterCtor(ctrl, $scope) {
        var existingItems = [];

        ctrl.optionFilterSettingsGroupTemplates = [];
        ctrl.datasource = [];

        ctrl.addFilter = function () {
            var dataItem = {
                id: ctrl.datasource.length + 1,
                configId: ctrl.selectedOptionFilterSettingsGroupTemplate.ExtensionConfigurationId,
                editor: ctrl.selectedOptionFilterSettingsGroupTemplate.Editor,
                name: ctrl.selectedOptionFilterSettingsGroupTemplate.Title
            };

            dataItem.onDirectiveReady = function (api) {
                dataItem.directiveAPI = api;
                var setLoader = function (value) { ctrl.isLoadingDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
            };

            for (var x = 0; x < ctrl.optionFilterSettingsGroupTemplates.length; x++) {
                if (ctrl.optionFilterSettingsGroupTemplates[x].ExtensionConfigurationId == ctrl.selectedOptionFilterSettingsGroupTemplate.ExtensionConfigurationId) {
                    existingItems.push(ctrl.optionFilterSettingsGroupTemplates[x]);
                    ctrl.optionFilterSettingsGroupTemplates.splice(x, 1);
                    break;
                }
            }

            ctrl.datasource.push(dataItem);
            ctrl.selectedOptionFilterSettingsGroupTemplate = undefined;
        };

        ctrl.removeFilter = function (dataItem) {
            var configId = dataItem.configId;
            ctrl.datasource.splice(ctrl.datasource.indexOf(dataItem), 1);
            for (var x = 0; x < existingItems.length; x++) {
                if (existingItems[x].ExtensionConfigurationId == configId) {
                    ctrl.optionFilterSettingsGroupTemplates.push(existingItems[x]);
                    existingItems.splice(x, 1);
                    break;
                }
            }
        };

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                return loadFiltersSection(payload);
            }

            api.getData = function () {
                if (ctrl.datasource.length > 0)
                    return getFilters();
                else
                    return null;
            }

            function loadFiltersSection(payload) {
                var promises = [];

                var filterItems;
                if (payload != undefined) {
                    filterItems = [];
                    for (var i = 0; i < payload.length; i++) {
                        var filterItem = {
                            payload: payload[i],
                            readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                            loadPromiseDeferred: UtilsService.createPromiseDeferred()
                        };
                        promises.push(filterItem.loadPromiseDeferred.promise);
                        filterItems.push(filterItem);
                    }
                }

                var loadTemplatesPromise = WhS_Routing_RoutRuleSettingsAPIService.GetRouteOptionFilterSettingsTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        ctrl.optionFilterSettingsGroupTemplates.push(item);
                    });

                    if (filterItems != undefined) {
                        for (var i = 0; i < filterItems.length; i++) {
                            addFilterItemToGrid(filterItems[i]);
                        }
                    }

                    if (ctrl.datasource.length > 0) {
                        for (var x = 0; x < ctrl.datasource.length; x++) {
                            var itemFound = false;
                            for (var j = 0; j < ctrl.optionFilterSettingsGroupTemplates.length; j++) {
                                if (ctrl.datasource[x].configId == ctrl.optionFilterSettingsGroupTemplates[j].ExtensionConfigurationId) {
                                    existingItems.push(ctrl.optionFilterSettingsGroupTemplates[j]);
                                    ctrl.optionFilterSettingsGroupTemplates.splice(j, 1);
                                    itemFound = true;
                                    break;
                                }
                            }
                            if (itemFound) {
                                continue;
                            }
                        }
                    }
                });

                promises.push(loadTemplatesPromise);

                function addFilterItemToGrid(filterItem) {
                    var matchItem = UtilsService.getItemByVal(ctrl.optionFilterSettingsGroupTemplates, filterItem.payload.ConfigId, "ExtensionConfigurationId");
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

                    ctrl.datasource.push(dataItem);
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            function getFilters()
            {
                var filters = [];

                angular.forEach(ctrl.datasource, function (dataItem) {

                    var filter = dataItem.directiveAPI.getData();
                    filter.ConfigId = dataItem.configId;

                    filters.push(filter);
                });

                return filters;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);