//(function (app) {

//    'use strict';

//    ericssonTrunkTrunkGroupBackup.$inject = ['UtilsService', 'VRUIUtilsService'];

//    function ericssonTrunkTrunkGroupBackup(UtilsService, VRUIUtilsService) {

//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new ericssonTrunkTrunkGroupBackupCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/WhS_RouteSync/Directives/MainExtensions/EricssonSynchronizer/Templates/EricssonTrunkTrunkGroupBackupTemplate.html'
//        }

//        function ericssonTrunkTrunkGroupBackupCtor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var supplierSelectorAPI;
//            var supplierSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.isSupplierSelected = false;
//                $scope.scopeModel.areTrunksSelected = true;

//                $scope.scopeModel.onSupplierSelectorReady = function (api) {
//                    supplierSelectorAPI = api;
//                    supplierSelectorReadyDeferred.resolve();
//                };

//                $scope.scopeModel.onSupplierSelectionChanged = function (selectedSupplier) {
//                    $scope.scopeModel.isSupplierSelected = selectedSupplier != undefined;
//                    if (selectedSupplier == undefined) {
//                        return;
//                    }


//                };

//                UtilsService.waitMultiplePromises([supplierSelectorReadyDeferred.promise]).then(function () {
//                    defineAPI();
//                });
//            }

//            function defineAPI() {

//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    var backups;

//                    if (payload != undefined) {
//                        backups = payload.Backups;
//                    }

//                    var loadSupplierSelectorPromise = getLoadSupplierSelectorPromise();
//                    promises.push(loadSupplierSelectorPromise);

//                    function getLoadSupplierSelectorPromise() {
//                        var loadSupplierSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

//                        supplierSelectorReadyDeferred.promise.then(function () {
//                            var supplierSelectorPayload = {};
//                            VRUIUtilsService.callDirectiveLoad(supplierSelectorAPI, supplierSelectorPayload, loadSupplierSelectorPromiseDeferred);
//                        });

//                        return loadSupplierSelectorPromiseDeferred.promise;
//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                }

//                api.getData = function () {
                    
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }

//    app.directive('whsRoutesyncEricssonTrunktrunkgroupbackup', ericssonTrunkTrunkGroupBackup)
//})(app);