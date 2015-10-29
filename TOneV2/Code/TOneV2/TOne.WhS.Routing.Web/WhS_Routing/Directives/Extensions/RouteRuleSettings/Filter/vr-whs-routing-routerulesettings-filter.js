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

            ctrl.optionFilterSettingsGroupTemplates = [];
            ctrl.datasource = [];

            ctrl.addFilter = function () {
                var dataItem = {
                    id: ctrl.datasource.length + 1,
                    configId: ctrl.selectedOptionFilterSettingsGroupTemplate.TemplateConfigID,
                    editor: ctrl.selectedOptionFilterSettingsGroupTemplate.Editor,
                    name: ctrl.selectedOptionFilterSettingsGroupTemplate.Name
                };

                dataItem.onDirectiveReady = function (api) {
                    dataItem.directiveAPI = api;
                    var setLoader = function (value) { ctrl.isLoadingDirective = value };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.directiveAPI, undefined, setLoader);
                };

                ctrl.datasource.push(dataItem);
                ctrl.selectedOptionFilterSettingsGroupTemplate = undefined;
            };

            ctrl.removeFilter = function (dataItem) {
                var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                ctrl.datasource.splice(index, 1);
            };

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
                });

                promises.push(loadTemplatesPromise);

                function addFilterItemToGrid(filterItem) {
                    var matchItem = UtilsService.getItemByVal(ctrl.optionFilterSettingsGroupTemplates, filterItem.payload.ConfigId, "TemplateConfigID");
                    if (matchItem == null)
                        return;

                    var dataItem = {
                        id: ctrl.datasource.length + 1,
                        configId: matchItem.TemplateConfigID,
                        editor: matchItem.Editor,
                        name: matchItem.Name
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