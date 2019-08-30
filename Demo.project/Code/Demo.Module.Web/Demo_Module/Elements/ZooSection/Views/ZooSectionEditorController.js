(function (appControllers) {

    'use strict';

    zooSectionEditorController.$inject = ['$scope', 'Demo_Module_ZooSectionAPIService', 'VRNavigationService', 'VRNotificationService', 'VRUIUtilsService', 'UtilsService'];

    function zooSectionEditorController($scope, Demo_Module_ZooSectionAPIService, VRNavigationService, VRNotificationService, VRUIUtilsService, UtilsService) {

        var isEditMode;
        var zooSectionId;
        var zooSectionEntity;
        var zooIdItem;

        var zooSelectorAPI;
        var zooSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var zooSectionTypeDirectiveAPI;
        var zooSectionTypeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                zooSectionId = parameters.zooSectionId;
                zooIdItem = parameters.zooIdItem;
            }

            isEditMode = (zooSectionId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.disableZoo = (zooIdItem != undefined);

            $scope.scopeModel.onZooSelectorReady = function (api) {
                zooSelectorAPI = api;
                zooSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onZooSectionTypeDirectiveReady = function (api) {
                zooSectionTypeDirectiveAPI = api;
                zooSectionTypeDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.saveZooSection = function () {
                if (isEditMode)
                    return updateZooSection();
                else
                    return insertZooSection();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getZooSection().then(function () {
                    loadAllControls().finally(function () {
                        zooSectionEntity = undefined;
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

        function getZooSection() {
            return Demo_Module_ZooSectionAPIService.GetZooSectionById(zooSectionId).then(function (response) {
                zooSectionEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && zooSectionEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(zooSectionEntity.Name, 'ZooSection');
                else
                    $scope.title = UtilsService.buildTitleForAddEditor('ZooSection');
            }

            function loadStaticData() {
                if (zooSectionEntity != undefined) {
                    $scope.scopeModel.name = zooSectionEntity.Name;
                }
            }

            function loadZooSelector() {
                var zooLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                zooSelectorReadyDeferred.promise.then(function () {
                    var selectorPayload = {};

                    if (zooSectionEntity != undefined) {
                        selectorPayload.selectedIds = zooSectionEntity.ZooId;
                    }
                    if (zooIdItem != undefined) {
                        selectorPayload.selectedIds = zooIdItem.ZooId;
                    }

                    VRUIUtilsService.callDirectiveLoad(zooSelectorAPI, selectorPayload, zooLoadPromiseDeferred);
                });

                return zooLoadPromiseDeferred.promise;
            }

            function loadZooSectionTypeDirective() {
                var zooSectionTypeLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                zooSectionTypeDirectiveReadyDeferred.promise.then(function () {
                    var directivePayload;

                    if (zooSectionEntity != undefined)
                        directivePayload = {
                            zooSectionTypeEntity: zooSectionEntity.Type
                        };

                    VRUIUtilsService.callDirectiveLoad(zooSectionTypeDirectiveAPI, directivePayload, zooSectionTypeLoadPromiseDeferred);
                });

                return zooSectionTypeLoadPromiseDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadZooSelector, loadZooSectionTypeDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function insertZooSection() {
            $scope.scopeModel.isLoading = true;

            var zooSectionObject = buildZooSectionObjectFromScope();

            return Demo_Module_ZooSectionAPIService.AddZooSection(zooSectionObject).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('ZooSection', response, 'Name')) {
                    if ($scope.onZooSectionAdded != undefined) {
                        $scope.onZooSectionAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateZooSection() {
            $scope.scopeModel.isLoading = true;

            var zooSectionObject = buildZooSectionObjectFromScope();
            return Demo_Module_ZooSectionAPIService.UpdateZooSection(zooSectionObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('ZooSection', response, 'Name')) {
                    if ($scope.onZooSectionUpdated != undefined) {
                        $scope.onZooSectionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildZooSectionObjectFromScope() {
            return {
                ZooSectionId: zooSectionId,
                Name: $scope.scopeModel.name,
                ZooId: zooSelectorAPI.getSelectedIds(),
                Type: zooSectionTypeDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Demo_Module_ZooSectionEditorController', zooSectionEditorController);
})(appControllers);