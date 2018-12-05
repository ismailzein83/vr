(function (appControllers) {

    "use strict";

    VRDynamicCodeEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService', 'VRCommon_VRNamespaceService'];

    function VRDynamicCodeEditorController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService,  VRCommon_VRNamespaceService) {

        var isEditMode;

        var vrDynamicCodeEntity;
        var vrDynamicCodeSettingsDirectiveApi;
        var vrDynamicCodeSettingsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                vrDynamicCodeEntity = parameters.vrDynamicCodeEntity;
            }

            isEditMode = (vrDynamicCodeEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onVRDynamicCodeSettingsReady = function (api) {
                vrDynamicCodeSettingsDirectiveApi = api;
                vrDynamicCodeSettingsDirectiveReadyPromiseDeferred.resolve();
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
            if (isEditMode) {
                loadAllControls().finally(function () {
                    vrDynamicCodeEntity = undefined;
                });
            }
            else
                loadAllControls();
        }

 

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRDynamicCodeSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var vrDynamicCodeTitle = (vrDynamicCodeEntity != undefined) ? vrDynamicCodeEntity.Title : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrDynamicCodeTitle, 'Dynamic Code');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Dynamic Code');
                }
            }
            function loadStaticData() {
                if (vrDynamicCodeEntity == undefined)
                    return;
                $scope.scopeModel.title = vrDynamicCodeEntity.Title;
            }

            function loadVRDynamicCodeSettingsDirective() {

                var promises = [];
                var vrDynamicCodeSettingsDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                promises.push(vrDynamicCodeSettingsDirectiveReadyPromiseDeferred.promise);
                UtilsService.waitMultiplePromises(promises).then(function (response) {
                    if (isEditMode) {
                        if (vrDynamicCodeEntity.Settings != undefined) {
                            var settingsPayload = { Settings: vrDynamicCodeEntity.Settings };
                        }
                    }
                    VRUIUtilsService.callDirectiveLoad(vrDynamicCodeSettingsDirectiveApi, settingsPayload, vrDynamicCodeSettingsDirectiveLoadPromiseDeferred);
                });
                return vrDynamicCodeSettingsDirectiveLoadPromiseDeferred.promise;

            }

        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            if ($scope.onVRDynamicCodeAdded != undefined)
                $scope.onVRDynamicCodeAdded(buildVRDynamicCodeObjFromScope());
            $scope.scopeModel.isLoading = false;
            $scope.modalContext.closeModal();
        }
        function update() {

            $scope.scopeModel.isLoading = true;
            if ($scope.onVRDynamicCodeUpdated != undefined) 
                $scope.onVRDynamicCodeUpdated(buildVRDynamicCodeObjFromScope());
            $scope.scopeModel.isLoading = false;
            $scope.modalContext.closeModal();
          
        }

        function buildVRDynamicCodeObjFromScope() {
            return {
                VRDynamicCodeId: UtilsService.guid(),
                Title: $scope.scopeModel.title,
                Settings: vrDynamicCodeSettingsDirectiveApi.getData()
            };
        }

    }

    appControllers.controller('VRCommon_VRDynamicCodeEditorController', VRDynamicCodeEditorController);

})(appControllers);