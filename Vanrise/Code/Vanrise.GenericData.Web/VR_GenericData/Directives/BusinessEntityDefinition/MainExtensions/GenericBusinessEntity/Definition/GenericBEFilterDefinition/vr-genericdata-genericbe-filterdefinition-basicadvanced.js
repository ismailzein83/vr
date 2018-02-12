(function (app) {

    'use strict';

    FilterDefinitionBasicAdvancedDirective.$inject = ['UtilsService', 'VRNotificationService', 'VR_GenericData_GenericBEDefinitionService'];

    function FilterDefinitionBasicAdvancedDirective(UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/BasicAdvancedFilterDefinitionTemplate.html'
        };

        function BasicAdvanceFilterCtor($scope, ctrl) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one filter.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Name in each should be unique.";

                    return null;
                };

                ctrl.addFilter = function () {
                    var onFilterAdded = function (addedItem) {
                        ctrl.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBEBasicAdvanceFilter(onFilterAdded, getContext());
                };
                ctrl.disableAddGridColumn = function () {
                    if (context == undefined) return true;
                    return context.getDataRecordTypeId() == undefined;
                };
                ctrl.removeFilter = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };



                defineMenuActions();
                defineAPI();
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
                                BasicAdvancedFilterItemId:currentItem.BasicAdvancedFilterItemId,
                                Name: currentItem.Name,
                                ShowInBasic: currentItem.ShowInBasic,
                                FilterSettings: currentItem.FilterSettings
                            });
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.BasicAdvancedFilterDefinitionSettings, Vanrise.GenericData.MainExtensions",
                        Filters: filters
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        api.clearDataSource();
                        if (payload.settings != undefined && payload.settings.Filters != undefined) {
                            var dataFilters = payload.settings.Filters;
                            for (var i = 0; i < dataFilters.length; i++) {
                                var item = dataFilters[i];
                                ctrl.datasource.push(item);
                            }
                        }
                    }
                };


                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editFilter,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editFilter(filterObj) {
                var onFilterUpdated = function (filter) {
                    var index = ctrl.datasource.indexOf(filterObj);
                    ctrl.datasource[index] = filter;
                };

                VR_GenericData_GenericBEDefinitionService.editGenericBEBasicAdvanceFilter(onFilterUpdated, filterObj, getContext());
            }
            function getContext() {
                return context;
            }

            function checkDuplicateName() {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i];
                    for (var j = 0; j < ctrl.datasource.length; j++) {
                        if (i != j && ctrl.datasource[j].FieldName == currentItem.FieldName)
                            return true;
                    }
                }
                return false;
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterdefinitionBasicadvanced', FilterDefinitionBasicAdvancedDirective);

})(app);