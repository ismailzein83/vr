(function (appControllers) {

    'use strict';

    zooEditorController.$inject = ['$scope', 'Demo_Module_ZooAPIService', 'VRNavigationService', 'VRNotificationService', 'VRUIUtilsService', 'UtilsService'];

    function zooEditorController($scope, Demo_Module_ZooAPIService, VRNavigationService, VRNotificationService, VRUIUtilsService, UtilsService) {

        var isEditMode;
        var zooId;
        var zooEntity;

        var sizeSelectorAPI;
        var sizeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                zooId = parameters.zooId;
            }

            isEditMode = (zooId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSizeSelectorReady = function (api) {
                sizeSelectorAPI = api;
                sizeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.saveZoo = function () {
                if (isEditMode)
                    return updateZoo();
                else
                    return insertZoo();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getZoo().then(function () {
                    loadAllControls().finally(function () {
                        zooEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }
        }

        function getZoo() {
            return Demo_Module_ZooAPIService.GetZooById(zooId).then(function (response) {
                zooEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && zooEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(zooEntity.Name, 'Zoo');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('Zoo');
            }

            function loadStaticData() {
                if (zooEntity != undefined) {
                    $scope.scopeModel.name = zooEntity.Name;
                    $scope.scopeModel.city = zooEntity.City;
                }
            }

            function loadSizeSelector() {
                var sizeLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                sizeSelectorReadyDeferred.promise.then(function () {
                    var payload;

                    if (zooEntity != undefined) {
                        payload = {
                            selectedIds: zooEntity.Size
                        };
                    }

                    VRUIUtilsService.callDirectiveLoad(sizeSelectorAPI, payload, sizeLoadPromiseDeferred);
                });

                return sizeLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSizeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertZoo() {
            $scope.scopeModel.isLoading = true;

            var zooObject = buildZooObjectFromScope();
            return Demo_Module_ZooAPIService.AddZoo(zooObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Zoo', response, 'Name')) {
                    if ($scope.onZooAdded != undefined) {
                        $scope.onZooAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateZoo() {
            $scope.scopeModel.isLoading = true;

            var zooObject = buildZooObjectFromScope();
            return Demo_Module_ZooAPIService.UpdateZoo(zooObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Zoo', response, 'Name')) {
                    if ($scope.onZooUpdated != undefined) {
                        $scope.onZooUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildZooObjectFromScope() {
            return {
                ZooId: zooId,
                Name: $scope.scopeModel.name,
                City: $scope.scopeModel.city,
                Size: sizeSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Demo_Module_ZooEditorController', zooEditorController);
})(appControllers);