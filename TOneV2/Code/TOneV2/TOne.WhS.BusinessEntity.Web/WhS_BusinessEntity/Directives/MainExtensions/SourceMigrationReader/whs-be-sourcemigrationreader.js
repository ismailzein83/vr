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

            var sellingNumberPlanDirectiveAPI;
            var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingProductDirectiveAPI;
            var sellingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                    sellingProductReadyPromiseDeferred.resolve();
                }

                UtilsService.waitMultiplePromises([sellingNumberPlanReadyPromiseDeferred.promise, sellingProductReadyPromiseDeferred.promise]).then(function () {
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
                    var selectedTables = [];

                    $scope.migrationTablesSelectedValues;
                    angular.forEach($scope.migrationTablesSelectedValues, function (x) {
                        selectedTables.push(x.value);
                    });
                    selectedTables.push(WhS_BE_DBTableNameEnum.CustomerZone.value);
                    schedulerTaskAction.MigrationRequestedTables = selectedTables;

                    return schedulerTaskAction;
                };

                api.load = function (payload) {

                    if (payload != undefined && payload.data != undefined) {
                        $scope.connectionString = payload.data.ConnectionString;
                        $scope.useTempTables = payload.data.UseTempTables;
                        sellingNumberPlanId = payload.data.DefaultSellingNumberPlanId;
                        sellingProductId = payload.data.SellingProductId;
                        $scope.migrationTablesSelectedValues = [];


                        angular.forEach(payload.data.MigrationRequestedTables, function (x) {
                            if (x != WhS_BE_DBTableNameEnum.CustomerZone.value)
                                 $scope.migrationTablesSelectedValues.push(UtilsService.getEnum(WhS_BE_DBTableNameEnum, 'value', x));
                        })

                    }

                    return UtilsService.waitMultipleAsyncOperations([loadSellingNumberPlanSelector, loadSellingProductSelector])
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


            function loadSellingProductSelector() {
                var loadSellingProductPromiseDeferred = UtilsService.createPromiseDeferred();

                sellingProductReadyPromiseDeferred.promise.then(function () {
                    var sellingProductPayload = {
                        selectedIds: sellingProductId
                    };

                    VRUIUtilsService.callDirectiveLoad(sellingProductDirectiveAPI, sellingProductPayload, loadSellingProductPromiseDeferred);
                });

                return loadSellingProductPromiseDeferred.promise
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);
