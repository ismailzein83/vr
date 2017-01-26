(function (appControllers) {

    'use strict';

    VRModuleVisibilityController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VRModuleVisibilityController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var vrModuleVisibilityEntity;

        var vrModuleVisibilitySelectiveAPI;
        var vrModuleVisibilitySelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                vrModuleVisibilityEntity = parameters.vrModuleVisibility;
            }

            isEditMode = (vrModuleVisibilityEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onVRModuleVisibilitySelectiveReady = function (api) {
                vrModuleVisibilitySelectiveAPI = api;
                vrModuleVisibilitySelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRModuleVisibilitySelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((vrModuleVisibilityEntity != undefined) ? vrModuleVisibilityEntity.Title : null, 'Module Visibility') :
                UtilsService.buildTitleForAddEditor('Module Visibility');
        }
        function loadStaticData() {
            if (vrModuleVisibilityEntity == undefined)
                return;
        }
        function loadVRModuleVisibilitySelective() {
            var vrModuleVisibilitySelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            vrModuleVisibilitySelectiveReadyDeferred.promise.then(function () {

                var payload;
                if (vrModuleVisibilityEntity != undefined) {
                    payload = {
                        vrModuleVisibility: vrModuleVisibilityEntity
                    };
                }
                VRUIUtilsService.callDirectiveLoad(vrModuleVisibilitySelectiveAPI, payload, vrModuleVisibilitySelectiveLoadDeferred);
            });

            return vrModuleVisibilitySelectiveLoadDeferred.promise;
        }

        function insert() {
            var vrModuleVisibilityObject = buildVRModuleVisibilityFromScope();
            if ($scope.onVRModuleVisibilityAdded != undefined && typeof ($scope.onVRModuleVisibilityAdded) == 'function') {
                $scope.onVRModuleVisibilityAdded(vrModuleVisibilityObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var vrModuleVisibilityObject = buildVRModuleVisibilityFromScope();
            if ($scope.onVRModuleVisibilityUpdated != undefined && typeof ($scope.onVRModuleVisibilityUpdated) == 'function') {
                $scope.onVRModuleVisibilityUpdated(vrModuleVisibilityObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildVRModuleVisibilityFromScope() {

            var vrModuleVisibility = vrModuleVisibilitySelectiveAPI.getData()

            return vrModuleVisibility;
        }
    }

    appControllers.controller('VRCommon_VRModuleVisibilityController', VRModuleVisibilityController);

})(appControllers);