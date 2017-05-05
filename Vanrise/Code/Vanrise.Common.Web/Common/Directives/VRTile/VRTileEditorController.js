(function (appControllers) {

    "use strict";

    VRTileEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function VRTileEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var vrTileEntity;

        var extendedSettingsDirectiveApi;
        var extendedSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrTileEntity = parameters.vrTileEntity;
            }
            isEditMode = (vrTileEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onExtendedSettingsDirectiveReady = function (api) {
                extendedSettingsDirectiveApi = api;
                extendedSettingsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveVRTile = function () {
                if (isEditMode)
                    return updateVRTile();
                else
                    return insertVRTile();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };


        }

        function load() {
            $scope.isLoading = true;
            loadAllControls()
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && vrTileEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrTileEntity.Name, "Tile");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Tile");
            }

            function loadStaticData() {

                if (vrTileEntity == undefined)
                    return;
                $scope.scopeModel.tileName = vrTileEntity.Name;
            }

            function loadExtendedSettingsDirective() {
                var extendedSettingsLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                extendedSettingsReadyPromiseDeferred.promise
                    .then(function () {
                        var directivePayload = {
                            tileExtendedSettings: vrTileEntity != undefined && vrTileEntity.Settings != undefined ? vrTileEntity.Settings.ExtendedSettings : undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(extendedSettingsDirectiveApi, directivePayload, extendedSettingsLoadPromiseDeferred);
                    });
                return extendedSettingsLoadPromiseDeferred.promise;
            }


            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadExtendedSettingsDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function buildVRTileObjFromScope() {
            var obj = {
                VRTileId: vrTileEntity != undefined ? vrTileEntity.VRTileId : UtilsService.guid(),
                Name: $scope.scopeModel.tileName,
                Settings: {
                    ExtendedSettings: extendedSettingsDirectiveApi.getData()
                }
            };
            return obj;
        }
        function insertVRTile() {
            var vrTileObject = buildVRTileObjFromScope();
            if ($scope.onVRTileAdded != undefined)
                $scope.onVRTileAdded(vrTileObject);
            $scope.modalContext.closeModal();
        }
        function updateVRTile() {
            var vrTileObject = buildVRTileObjFromScope();
            if ($scope.onVRTileUpdated != undefined)
                $scope.onVRTileUpdated(vrTileObject);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('VRCommon_VRTileEditorController', VRTileEditorController);
})(appControllers);
