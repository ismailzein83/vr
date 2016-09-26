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
                }
            },
            templateUrl: "/Client/Modules/Whs_BusinessEntity/Directives/MainExtensions/SourceMigrationReader/Templates/SourceMigrationReader.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            var sellingNumberPlanId;
            var sellingProductId;
            var offPeakRateTypeId;
            var weekendRateTypeId;

            var sellingNumberPlanDirectiveAPI;
            var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var offPeakRateTypeSelectorAPI;
            var offPeakRateTypeSelectorReadyPrmoiseDeferred = UtilsService.createPromiseDeferred();

            var weekendRateTypeSelectorAPI;
            var weekendRateTypeSelectorReadyPrmoiseDeferred = UtilsService.createPromiseDeferred();

            var sellingProductDirectiveAPI;

            function initializeController() {
                $scope.useTempTables = true;
                $scope.migrationTables = [];
                angular.forEach(UtilsService.getArrayEnum(WhS_BE_DBTableNameEnum), function (dbTable) {
                    if (dbTable.defaultMigrate)
                        $scope.migrationTables.push(dbTable);
                });
                $scope.migrationTablesSelectedValues = $scope.migrationTables;

                $scope.onSellingNumberPlanDirectiveReady = function (api) {
                    sellingNumberPlanDirectiveAPI = api;
                    sellingNumberPlanReadyPromiseDeferred.resolve();
                }

                $scope.onSellingProductsDirectiveReady = function (api) {
                    sellingProductDirectiveAPI = api;
                }


                $scope.onOffPeakRateTypeSelectorReady = function (api) {
                    offPeakRateTypeSelectorAPI = api;
                    offPeakRateTypeSelectorReadyPrmoiseDeferred.resolve();
                }

                $scope.onWeekendRateTypeSelectorReady = function (api) {
                    weekendRateTypeSelectorAPI = api;
                    weekendRateTypeSelectorReadyPrmoiseDeferred.resolve();
                }


                $scope.validateOffPeakAndWeekendRate = function () {
                    if (weekendRateTypeSelectorAPI != undefined && offPeakRateTypeSelectorAPI != undefined && weekendRateTypeSelectorAPI.getSelectedIds() == offPeakRateTypeSelectorAPI.getSelectedIds())
                        return "offPeak rate must be different to weekend rate";
                    return null;
                }

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
                    else if (sellingProductDirectiveAPI != undefined)
                        sellingProductDirectiveAPI.clearDataSource();
                }

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
                    var selectedTables = [];

                    $scope.migrationTablesSelectedValues;
                    angular.forEach($scope.migrationTablesSelectedValues, function (x) {
                        selectedTables.push(x.value);
                    });
                    selectedTables.push(WhS_BE_DBTableNameEnum.CustomerZone.value);
                    selectedTables.push(WhS_BE_DBTableNameEnum.CustomerSellingProduct.value);
                    selectedTables.push(WhS_BE_DBTableNameEnum.File.value);
                    schedulerTaskAction.MigrationRequestedTables = selectedTables;

                    return schedulerTaskAction;
                };

                api.load = function (payload) {

                    if (payload != undefined && payload.data != undefined) {
                        $scope.connectionString = payload.data.ConnectionString;
                        $scope.useTempTables = payload.data.UseTempTables;
                        sellingNumberPlanId = payload.data.DefaultSellingNumberPlanId;
                        sellingProductId = payload.data.SellingProductId;
                        offPeakRateTypeId = payload.data.OffPeakRateTypeId;
                        weekendRateTypeId = payload.data.WeekendRateTypeId;
                        $scope.migrationTablesSelectedValues = [];


                        angular.forEach(payload.data.MigrationRequestedTables, function (x) {
                            if (x != WhS_BE_DBTableNameEnum.CustomerZone.value && x != WhS_BE_DBTableNameEnum.CustomerSellingProduct.value && x != WhS_BE_DBTableNameEnum.File.value)
                                $scope.migrationTablesSelectedValues.push(UtilsService.getEnum(WhS_BE_DBTableNameEnum, 'value', x));
                        })

                    }

                    return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlanSelector, loadOffPeakRateTypeSelector, loadWeekendRateTypeSelector])
                         .catch(function (error) {
                             VRNotificationService.notifyExceptionWithClose(error, $scope);
                         });

                }

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

                return loadSellingNumberPlanPromiseDeferred.promise
            }


            function loadOffPeakRateTypeSelector() {
                var loadOffPeakRateTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                offPeakRateTypeSelectorReadyPrmoiseDeferred.promise.then(function () {
                   var offPeakRatePayload = {
                        selectedIds: offPeakRateTypeId
                    }
                    VRUIUtilsService.callDirectiveLoad(offPeakRateTypeSelectorAPI, offPeakRatePayload, loadOffPeakRateTypeSelectorPromiseDeferred);
                });

                return loadOffPeakRateTypeSelectorPromiseDeferred.promise
            }

            function loadWeekendRateTypeSelector() {
                var loadWeekendRateTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                weekendRateTypeSelectorReadyPrmoiseDeferred.promise.then(function () {
                  var  weekendRatePayload = {
                        selectedIds: weekendRateTypeId
                    }
                  VRUIUtilsService.callDirectiveLoad(weekendRateTypeSelectorAPI, weekendRatePayload, loadWeekendRateTypeSelectorPromiseDeferred);
                });

                return loadWeekendRateTypeSelectorPromiseDeferred.promise
            }


            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);
