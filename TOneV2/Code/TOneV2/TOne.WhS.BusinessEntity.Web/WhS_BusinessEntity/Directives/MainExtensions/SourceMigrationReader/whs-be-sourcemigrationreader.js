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

            var sellingNumberPlanDirectiveAPI;
            var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.migrationTables = UtilsService.getArrayEnum(WhS_BE_DBTableNameEnum);
                $scope.migrationTablesSelectedValues = [];
                $scope.onSellingNumberPlanDirectiveReady = function (api) {
                    sellingNumberPlanDirectiveAPI = api;
                    sellingNumberPlanReadyPromiseDeferred.resolve();
                }
                defineAPI();
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
                    var selectedTables = [];

                    $scope.migrationTablesSelectedValues;
                    angular.forEach($scope.migrationTablesSelectedValues, function (x) {
                        selectedTables.push(x.value);
                    });
                    schedulerTaskAction.MigrationRequestedTables = selectedTables;

                    console.log(selectedTables)
                    return schedulerTaskAction;
                };

                api.load = function (payload) {
                    var sellingNumberPlanId;

                    if (payload != undefined && payload.data != undefined) {
                        $scope.connectionString = payload.data.ConnectionString;
                        $scope.useTempTables = payload.data.UseTempTables;
                        sellingNumberPlanId = payload.data.DefaultSellingNumberPlanId;
                        angular.forEach(payload.data.MigrationRequestedTables, function (x) {
                            $scope.migrationTablesSelectedValues.push(UtilsService.getEnum(WhS_BE_DBTableNameEnum, 'value', x));
                        });
                    }

                    var sellingNumberPlanPayload;
                    var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
                    if (sellingNumberPlanId != undefined) {
                        sellingNumberPlanPayload = {
                            selectedIds: sellingNumberPlanId
                        };
                    }
                    sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, sellingNumberPlanPayload, loadSellingNumberPlanPromiseDeferred);
                    });

                    return loadSellingNumberPlanPromiseDeferred.promise

                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);
