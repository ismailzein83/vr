(function (app) {

    'use strict';

    FilterDefinitionBasicAdvancedDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function FilterDefinitionBasicAdvancedDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BasicAdvanceFilterCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/BasicAdvancedFilterDefinitionTemplate.html'
        };

        function BasicAdvanceFilterCtor($scope, ctrl) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one filter.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Name in each filter should be unique.";

                    return null;
                };
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();

                };
                $scope.scopeModel.addFilter = function () {

                    var dataItem = {
                        entity: {
                            BasicAdvancedFilterItemId: UtilsService.guid()
                        }
                    };

                    dataItem.onFilterDefinitionDirectiveReady = function (api) {
                        dataItem.filterDefinitionDirectiveAPI = api;
                        var setLoader = function (value) { dataItem.isFilterDefinitionDirectiveLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.filterDefinitionDirectiveAPI, { context: context }, setLoader);
                    };
                    gridAPI.expandRow(dataItem);

                    ctrl.datasource.push(dataItem);
                };
                $scope.scopeModel.disableAddGridColumn = function () {
                    if (context == undefined) return true;
                    return context.getDataRecordTypeId() == undefined;
                };
                $scope.scopeModel.removeFilter = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var filters;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        filters = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            filters.push({
                                BasicAdvancedFilterItemId:currentItem.entity.BasicAdvancedFilterItemId,
                                Name: currentItem.entity.Name,
                                ShowInBasic: currentItem.entity.ShowInBasic,
                                FilterSettings: currentItem.filterDefinitionDirectiveAPI != undefined ? currentItem.filterDefinitionDirectiveAPI.getData() : currentItem.oldSettings
                            });
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.BasicAdvancedFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        Filters: filters
                    };
                };

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        api.clearDataSource();
                        if (payload.settings != undefined && payload.settings.Filters != undefined) {
                            var dataFilters = payload.settings.Filters;
                            for (var i = 0; i < dataFilters.length; i++) {
                                var filterobject = {
                                    payload: dataFilters[i],
                                };
                                prepareFilter(filterobject);
                            }
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function prepareFilter(filterObject) {
                var dataItem = {
                    entity: {
                        BasicAdvancedFilterItemId: filterObject.payload.BasicAdvancedFilterItemId,
                        Name: filterObject.payload.Name,
                        ShowInBasic: filterObject.payload.ShowInBasic
                    },
                    oldSettings: filterObject.payload.FilterSettings
                };

                dataItem.onFilterDefinitionDirectiveReady = function (api) {
                    dataItem.filterDefinitionDirectiveAPI = api;
                    var filterPayload = {
                        settings: filterObject.payload.FilterSettings,
                        context: context
                    };
                    var setLoader = function (value) { dataItem.isFilterDefinitionDirectiveLoading = value; };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.filterDefinitionDirectiveAPI, filterPayload, setLoader);
                };

                ctrl.datasource.push(dataItem);
            }


            function checkDuplicateName() {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i];
                    for (var j = 0; j < ctrl.datasource.length; j++) {
                        if (i != j && ctrl.datasource[j].entity.Name == currentItem.entity.Name)
                            return true;
                    }
                }
                return false;
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterdefinitionBasicadvanced', FilterDefinitionBasicAdvancedDirective);

})(app);