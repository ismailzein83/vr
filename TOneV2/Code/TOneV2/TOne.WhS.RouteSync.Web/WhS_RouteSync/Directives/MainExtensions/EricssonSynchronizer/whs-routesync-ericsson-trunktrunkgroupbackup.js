(function (app) {

    'use strict';

    ericssonTrunkTrunkGroupBackup.$inject = ['UtilsService', 'VRUIUtilsService'];

    function ericssonTrunkTrunkGroupBackup(UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ericssonTrunkTrunkGroupBackupCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonTrunkTrunkGroupBackupTemplate.html'
        };

        function ericssonTrunkTrunkGroupBackupCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var supplierOutTrunksMappings;

            var trunkTrunkGroupBackupGridAPI;
            var trunkTrunkGroupBackupReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.trunkTrunkGroupBackups = [];

                $scope.scopeModel.onTrunkTrunkGroupBackupGridReady = function (api) {
                    trunkTrunkGroupBackupGridAPI = api;
                    trunkTrunkGroupBackupReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddBackupSupplier = function () {
                    extendBackupGrid();
                };

                $scope.scopeModel.onDeleteTrunkTrunkGroupBackup = function (deletedBackup) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.trunkTrunkGroupBackups, deletedBackup.rowIndex, 'rowIndex');
                    if (index > -1) {
                        $scope.scopeModel.trunkTrunkGroupBackups.splice(index, 1);
                    }
                };

                $scope.scopeModel.isTrunkTrunkGroupBackupGridValid = function () {
                    for (var i = 0; i < $scope.scopeModel.trunkTrunkGroupBackups.length; i++) {
                        var dataItem1 = $scope.scopeModel.trunkTrunkGroupBackups[i];
                        if (dataItem1.selectedSupplierOutTrunks == undefined)
                            continue;

                        for (var j = i + 1; j < $scope.scopeModel.trunkTrunkGroupBackups.length; j++) {
                            var dataItem2 = $scope.scopeModel.trunkTrunkGroupBackups[j];
                            if (dataItem2.selectedSupplierOutTrunks == undefined)
                                continue;

                            if (dataItem1.selectedSupplierOutTrunks.TrunkId == dataItem2.selectedSupplierOutTrunks.TrunkId) {
                                return 'Cannot select the same trunk more than one time';
                            }
                        }
                    }
                    return null;
                };

                UtilsService.waitMultiplePromises([trunkTrunkGroupBackupReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var backups;

                    if (payload != undefined) {
                        backups = payload.Backups;
                        supplierOutTrunksMappings = payload.supplierOutTrunksMappings;
                    }

                    if (backups != undefined) {
                        for (var i = 0; i < backups.length; i++) {
                            var supplierId = backups[i].SupplierId;
                            var backup = {
                                selectedSupplierId: supplierId,
                                selectedTrunkIds: backups[i].Trunks[0].TrunkId
                            };

                            extendBackupGrid(backup);
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var backups = $scope.scopeModel.trunkTrunkGroupBackups.length != 0 ? [] : undefined;
                    for (var i = 0; i < $scope.scopeModel.trunkTrunkGroupBackups.length; i++) {
                        var dataItem = $scope.scopeModel.trunkTrunkGroupBackups[i];
                        if (dataItem.selectedSupplier == undefined || dataItem.selectedSupplierOutTrunks == undefined)
                            continue;

                        backups.push({
                            SupplierId: dataItem.selectedSupplier.CarrierAccountId,
                            Trunks: [{ TrunkId: dataItem.selectedSupplierOutTrunks.TrunkId }]
                        });
                    }
                    return backups;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function extendBackupGrid(backup) {
                var supplierSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                if (backup == undefined) {
                    supplierSelectionChangedDeferred.resolve();
                }

                var dataItem = {
                    isLoading: true,
                    supplierOutTrunks: [],
                    supplierSelectorReadyDeferred: UtilsService.createPromiseDeferred(),
                    trunkSelectorReadyDeferred: UtilsService.createPromiseDeferred()
                };

                dataItem.onSupplierSelectorReady = function (api) {
                    dataItem.supplierSelectorAPI = api;
                    dataItem.supplierSelectorReadyDeferred.resolve();
                };

                dataItem.onTrunkSelectorReady = function (api) {
                    dataItem.trunkSelectorAPI = api;
                    dataItem.trunkSelectorReadyDeferred.resolve();
                };

                dataItem.onSupplierSelectionChanged = function (selectedSupplier) {
                    if (selectedSupplier == undefined) {
                        dataItem.selectedSupplierOutTrunks = undefined;
                        dataItem.supplierOutTrunks = [];
                        return;
                    }

                    if (supplierSelectionChangedDeferred != undefined) {
                        supplierSelectionChangedDeferred.resolve();
                    } else {
                        var supplierId = selectedSupplier.CarrierAccountId;
                        dataItem.supplierOutTrunks = supplierOutTrunksMappings[supplierId];
                    }
                };

                var loadSupplierSelectorPromise = getLoadSupplierSelectorPromise();
                var loadTrunkSelectorPromise = getLoadTrunkSelectorPromise();

                $scope.scopeModel.trunkTrunkGroupBackups.push(dataItem);

                function getLoadSupplierSelectorPromise() {
                    var loadSupplierSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                    dataItem.supplierSelectorReadyDeferred.promise.then(function () {

                        var supplierSelectorPayload;
                        if (backup != undefined && backup.selectedSupplierId != undefined) {
                            supplierSelectorPayload = {
                                selectedIds: backup.selectedSupplierId
                            };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataItem.supplierSelectorAPI, supplierSelectorPayload, loadSupplierSelectorPromiseDeferred);
                    });

                    return loadSupplierSelectorPromiseDeferred.promise;
                }
                function getLoadTrunkSelectorPromise() {
                    var loadTrunkSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([dataItem.trunkSelectorReadyDeferred.promise, supplierSelectionChangedDeferred.promise]).then(function () {

                        if (backup != undefined) {
                            dataItem.supplierOutTrunks = supplierOutTrunksMappings[backup.selectedSupplierId];
                            dataItem.selectedSupplierOutTrunks = UtilsService.getItemByVal(supplierOutTrunksMappings[backup.selectedSupplierId], backup.selectedTrunkIds, 'TrunkId');
                        }
                        loadTrunkSelectorPromiseDeferred.resolve();
                    });

                    return loadTrunkSelectorPromiseDeferred.promise;
                }

                return UtilsService.waitMultiplePromises([loadTrunkSelectorPromise]).then(function () {
                    dataItem.isLoading = false;
                    supplierSelectionChangedDeferred = undefined;
                });
            }
        }
    }

    app.directive('whsRoutesyncEricssonTrunktrunkgroupbackup', ericssonTrunkTrunkGroupBackup);
})(app);