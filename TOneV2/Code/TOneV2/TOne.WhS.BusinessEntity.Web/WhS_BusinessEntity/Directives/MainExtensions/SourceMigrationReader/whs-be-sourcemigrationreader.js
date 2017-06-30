"use strict";

app.directive("whsBeSourcemigrationreader", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_SwitchAPIService', 'WhS_BE_DBTableNameEnum',
    function (UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SwitchAPIService, WhS_BE_DBTableNameEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/Whs_BusinessEntity/Directives/MainExtensions/SourceMigrationReader/Templates/SourceMigrationReader.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            var sellingNumberPlanId;
            var sellingProductId;
            var offPeakRateTypeId;
            var weekendRateTypeId;
            var holidayRateTypeId;


            var sellingNumberPlanDirectiveAPI;
            var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var offPeakRateTypeSelectorAPI;
            var offPeakRateTypeSelectorReadyPrmoiseDeferred = UtilsService.createPromiseDeferred();

            var weekendRateTypeSelectorAPI;
            var weekendRateTypeSelectorReadyPrmoiseDeferred = UtilsService.createPromiseDeferred();

            var holidayRateTypeSelectorAPI;
            var holidayRateTypeSelectorReadyPrmoiseDeferred = UtilsService.createPromiseDeferred();

            var sellingProductDirectiveAPI;

            function initializeController() {
                $scope.useTempTables = true;
                $scope.onlyEffective = false;
                var migrationTablesBasetable = new Array();
                $scope.migrationTables = [];
                $scope.selectedParameterDefinitions = [];
                $scope.selectedItems = [];
                $scope.parameterDefinitions = loadParameterDefinitions();

                angular.forEach(UtilsService.getArrayEnum(WhS_BE_DBTableNameEnum), function (dbTable) {
                    if (dbTable.defaultMigrate)
                        migrationTablesBasetable.push(dbTable);
                });
                $scope.migrationTables = UtilsService.cloneObject(migrationTablesBasetable, true);
                $scope.migrationTablesSelectedValues = UtilsService.cloneObject(migrationTablesBasetable, true);
                $scope.onSellingNumberPlanDirectiveReady = function (api) {
                    sellingNumberPlanDirectiveAPI = api;
                    sellingNumberPlanReadyPromiseDeferred.resolve();
                };

                $scope.onSellingProductsDirectiveReady = function (api) {
                    sellingProductDirectiveAPI = api;
                };

                $scope.onOffPeakRateTypeSelectorReady = function (api) {
                    offPeakRateTypeSelectorAPI = api;
                    offPeakRateTypeSelectorReadyPrmoiseDeferred.resolve();
                };

                $scope.onWeekendRateTypeSelectorReady = function (api) {
                    weekendRateTypeSelectorAPI = api;
                    weekendRateTypeSelectorReadyPrmoiseDeferred.resolve();
                };

                $scope.onHolidayRateTypeSelectorReady = function (api) {
                    holidayRateTypeSelectorAPI = api;
                    holidayRateTypeSelectorReadyPrmoiseDeferred.resolve();
                };

                $scope.validateOffPeakRate = function () {
                    if (offPeakRateTypeSelectorAPI != undefined && offPeakRateTypeSelectorAPI.getSelectedIds() == undefined)
                        return "Required Field";

                    if (offPeakRateTypeSelectorAPI != undefined
                         && weekendRateTypeSelectorAPI != undefined
                         && (weekendRateTypeSelectorAPI.getSelectedIds() == offPeakRateTypeSelectorAPI.getSelectedIds()))
                        return "Rate Type must be different than Weekend Rate";

                    if (offPeakRateTypeSelectorAPI != undefined
                          && holidayRateTypeSelectorAPI != undefined
                          && (offPeakRateTypeSelectorAPI.getSelectedIds() == holidayRateTypeSelectorAPI.getSelectedIds()))
                        return "Rate Type must be different than Holiday Rate";

                    return null;
                };

                $scope.validateWeekendRate = function () {
                    if (weekendRateTypeSelectorAPI != undefined && weekendRateTypeSelectorAPI.getSelectedIds() == undefined)
                        return "Required Field";
                    if (offPeakRateTypeSelectorAPI != undefined
                            && weekendRateTypeSelectorAPI != undefined
                            && (weekendRateTypeSelectorAPI.getSelectedIds() == offPeakRateTypeSelectorAPI.getSelectedIds()))
                        return "Rate Type must be different than OffPeak Rate";

                    if (weekendRateTypeSelectorAPI != undefined
                          && holidayRateTypeSelectorAPI != undefined
                          && (weekendRateTypeSelectorAPI.getSelectedIds() == holidayRateTypeSelectorAPI.getSelectedIds()))
                        return "Rate Type must be different than Holiday Rate";

                    return null;
                };

                $scope.validateHolidayRate = function () {
                    if (holidayRateTypeSelectorAPI != undefined && holidayRateTypeSelectorAPI.getSelectedIds() == undefined)
                        return "Required Field";
                    if (holidayRateTypeSelectorAPI != undefined
                                          && weekendRateTypeSelectorAPI != undefined
                                          && (holidayRateTypeSelectorAPI.getSelectedIds() == weekendRateTypeSelectorAPI.getSelectedIds()))
                        return "Rate Type must be different than Weekend Rate";

                    if (holidayRateTypeSelectorAPI != undefined
                          && offPeakRateTypeSelectorAPI != undefined
                          && (holidayRateTypeSelectorAPI.getSelectedIds() == offPeakRateTypeSelectorAPI.getSelectedIds()))
                        return "Rate Type must be different than Offpeak Rate";

                    return null;
                };

                $scope.onSellingNumberPlanSelectionChanged = function () {
                    var selectedSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
                    if (selectedSellingNumberPlanId != undefined) {
                        var setLoader = function (value) { $scope.isLoadingSellingProduct = value };

                        var sellingProductPayload = {
                            filter: {
                                SellingNumberPlanId: selectedSellingNumberPlanId
                            },
                            selectedIds: sellingProductId
                        };
                        sellingProductId = undefined;
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingProductDirectiveAPI, sellingProductPayload, setLoader);
                    }
                    else if (sellingProductDirectiveAPI != undefined) {
                        sellingProductDirectiveAPI.clearDataSource();
                        sellingProductDirectiveAPI.resetFilter();
                    }
                };               
                $scope.onSelectParameterDefinition = function (item) {
                    var gridItem = {
                        DisplayName: item.DisplayName,
                        Name: item.Name
                    };
                    $scope.selectedParameterDefinitions.push(gridItem);

                };
                $scope.onDeselectParameterDefinition = function (item) {
                    var index = UtilsService.getItemIndexByVal($scope.selectedParameterDefinitions, item.Name, "Name");
                    $scope.selectedParameterDefinitions.splice(index, 1);
                };

                UtilsService.waitMultiplePromises([sellingNumberPlanReadyPromiseDeferred.promise, offPeakRateTypeSelectorReadyPrmoiseDeferred.promise, weekendRateTypeSelectorReadyPrmoiseDeferred.promise]).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {

                var api = {};

                api.getData = function () {
                    var schedulerTaskAction;
                    schedulerTaskAction = {};
                    schedulerTaskAction.$type = "TOne.WhS.DBSync.Business.DBSyncTaskActionArgument, TOne.WhS.DBSync.Business";
                    schedulerTaskAction.ConnectionString = $scope.connectionString;
                    schedulerTaskAction.DefaultSellingNumberPlanId = sellingNumberPlanDirectiveAPI.getSelectedIds();
                    schedulerTaskAction.UseTempTables = ($scope.useTempTables == true) ? true : false;
                    schedulerTaskAction.SellingProductId = sellingProductDirectiveAPI.getSelectedIds();
                    schedulerTaskAction.OffPeakRateTypeId = offPeakRateTypeSelectorAPI.getSelectedIds();
                    schedulerTaskAction.WeekendRateTypeId = weekendRateTypeSelectorAPI.getSelectedIds();
                    schedulerTaskAction.HolidayRateTypeId = holidayRateTypeSelectorAPI.getSelectedIds();
                    schedulerTaskAction.MigratePriceListData = $scope.migratePriceListData;
                    schedulerTaskAction.IsCustomerCommissionNegative = $scope.isCustomerCommissionNegative;
                    schedulerTaskAction.OnlyEffective = $scope.onlyEffective;
                    var selectedTables = [];

                    $scope.migrationTablesSelectedValues;
                    angular.forEach($scope.migrationTablesSelectedValues, function (x) {
                        selectedTables.push(x.value);
                    });
                    selectedTables.push(WhS_BE_DBTableNameEnum.CustomerCountry.value);
                    selectedTables.push(WhS_BE_DBTableNameEnum.File.value);
                    schedulerTaskAction.MigrationRequestedTables = selectedTables;
                    schedulerTaskAction.ParameterDefinitions = getParameterDefinitions();
                    return schedulerTaskAction;
                };

                api.load = function (payload) {

                    $scope.migratePriceListData = true;

                    if (payload != undefined && payload.data != undefined) {

                        $scope.connectionString = payload.data.ConnectionString;
                        $scope.useTempTables = payload.data.UseTempTables;
                        sellingNumberPlanId = payload.data.DefaultSellingNumberPlanId;
                        sellingProductId = payload.data.SellingProductId;
                        offPeakRateTypeId = payload.data.OffPeakRateTypeId;
                        weekendRateTypeId = payload.data.WeekendRateTypeId;
                        holidayRateTypeId = payload.data.HolidayRateTypeId;
                        $scope.migratePriceListData = payload.data.MigratePriceListData;
                        $scope.isCustomerCommissionNegative = payload.data.IsCustomerCommissionNegative;
                        $scope.onlyEffective = payload.data.OnlyEffective;
                        $scope.migrationTablesSelectedValues = [];
                        fillParameterDefinitions(payload.data.ParameterDefinitions);

                        angular.forEach(payload.data.MigrationRequestedTables, function (x) {
                            if (x != WhS_BE_DBTableNameEnum.CustomerCountry.value && x != WhS_BE_DBTableNameEnum.File.value)
                                $scope.migrationTablesSelectedValues.push(UtilsService.getEnum(WhS_BE_DBTableNameEnum, 'value', x));
                        })

                    }

                    return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlanSelector, loadOffPeakRateTypeSelector, loadWeekendRateTypeSelector, loadHolidayRateTypeSelector])
                         .catch(function (error) {
                             VRNotificationService.notifyExceptionWithClose(error, $scope);
                         });

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadSellingNumberPlanSelector() {

                var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();

                sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                    var sellingNumberPlanPayload = {
                        selectedIds: sellingNumberPlanId
                    };

                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, sellingNumberPlanPayload, loadSellingNumberPlanPromiseDeferred);
                });

                return loadSellingNumberPlanPromiseDeferred.promise;
            }

            function loadOffPeakRateTypeSelector() {
                var loadOffPeakRateTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                offPeakRateTypeSelectorReadyPrmoiseDeferred.promise.then(function () {
                    var offPeakRatePayload = {
                        selectedIds: offPeakRateTypeId
                    };
                    VRUIUtilsService.callDirectiveLoad(offPeakRateTypeSelectorAPI, offPeakRatePayload, loadOffPeakRateTypeSelectorPromiseDeferred);
                });

                return loadOffPeakRateTypeSelectorPromiseDeferred.promise;
            }

            function loadWeekendRateTypeSelector() {
                var loadWeekendRateTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                weekendRateTypeSelectorReadyPrmoiseDeferred.promise.then(function () {
                    var weekendRatePayload = {
                        selectedIds: weekendRateTypeId
                    };
                    VRUIUtilsService.callDirectiveLoad(weekendRateTypeSelectorAPI, weekendRatePayload, loadWeekendRateTypeSelectorPromiseDeferred);
                });

                return loadWeekendRateTypeSelectorPromiseDeferred.promise;
            }

            function loadHolidayRateTypeSelector() {
                var loadHolidayRateTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                holidayRateTypeSelectorReadyPrmoiseDeferred.promise.then(function () {
                    var holidayRatePayload = {
                        selectedIds: holidayRateTypeId
                    };
                    VRUIUtilsService.callDirectiveLoad(holidayRateTypeSelectorAPI, holidayRatePayload, loadHolidayRateTypeSelectorPromiseDeferred);
                });

                return loadHolidayRateTypeSelectorPromiseDeferred.promise;
            }

            function loadParameterDefinitions() {
                var parameterDefinitions = [];
                parameterDefinitions.push({
                    Name: 'MVTSMaxDoP',
                    DisplayName: 'MVTS Max Degree of Parallelism'
                });
                parameterDefinitions.push({
                    Name: 'MVTSMaxDoP_Redundant',
                    DisplayName: 'MVTS Redundant Max Degree of Parallelism'
                });
                return parameterDefinitions;
            }

            function getParameterDefinitions() {
                var definitions;
                if ($scope.selectedParameterDefinitions.length > 0) {
                    definitions = {};
                    for (var i = 0; i < $scope.selectedParameterDefinitions.length; i++) {
                        var currentItem = $scope.selectedParameterDefinitions[i];
                        definitions[currentItem.Name] = {
                            Value: currentItem.Value
                        };
                    }
                }
                return definitions;
            }

            function fillParameterDefinitions(parameterDefinitions) {
                if (parameterDefinitions != undefined) {
                    for (var key in parameterDefinitions) {
                        var selectedItem = UtilsService.getItemByVal($scope.parameterDefinitions, key, "Name");
                        if (selectedItem != undefined) {
                            $scope.selectedItems.push(selectedItem);
                            $scope.selectedParameterDefinitions.push({
                                DisplayName: selectedItem.DisplayName,
                                Name: selectedItem.Name,
                                Value: parameterDefinitions[key].Value
                            });
                        }
                    }
                }
            }
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);
