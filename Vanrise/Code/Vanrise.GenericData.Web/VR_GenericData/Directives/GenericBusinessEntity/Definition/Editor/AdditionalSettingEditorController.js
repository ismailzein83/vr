(function (appControllers) {

    "use strict";

    GenericBEActionDefintionController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function GenericBEActionDefintionController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;
        var additionalSettings;
        var context;

        var additionalSettingsSelectiveAPI;
        var additionalSettingsSelectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined ) {
                additionalSettings = parameters.additionalSettings;
                context = parameters.context;
            }
            isEditMode = (additionalSettings != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
         
            $scope.scopeModel.onAdditionalSettingsSelectiveReady = function (api) {
                additionalSettingsSelectiveAPI = api;
                additionalSettingsSelectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveAdditionalSetting = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {

            loadAllControls();

            function loadAllControls() {
                $scope.scopeModel.isLoading = true;
                function setTitle() {
                    if (isEditMode && additionalSettings != undefined)
                        $scope.title = UtilsService.buildTitleForUpdateEditor(additionalSettings.Name, 'Additional Settings Editor');
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('Additional Settings Editor');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;
                }

                function loadAdditionalSettingsSelective() {
                    var additionalSettingsSelectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    additionalSettingsSelectiveReadyPromiseDeferred.promise.then(function () {
                        var additionalSettingsPayload = additionalSettings != undefined ? additionalSettings.Settings : undefined;
                        VRUIUtilsService.callDirectiveLoad(additionalSettingsSelectiveAPI, additionalSettingsPayload, additionalSettingsSelectiveLoadPromiseDeferred);
                    });
                    return additionalSettingsSelectiveLoadPromiseDeferred.promise;
                }


                return UtilsService.waitMultipleAsyncOperations([loadStaticData, setTitle, loadAdditionalSettingsSelective]).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            }

        }

        function buildAdditionalSettingFromScope() {
            return additionalSettingsSelectiveAPI.getData();
        }

        function insert() {
            var additionalSettings = buildAdditionalSettingFromScope();
            if ($scope.onGenericBEAdditionalSettingAdded != undefined) {
                $scope.onGenericBEAdditionalSettingAdded(additionalSettings);
            }
            $scope.modalContext.closeModal();
        }

        function update() {
            var additionalSettings = buildAdditionalSettingFromScope();
            if ($scope.onGenericBEAdditionalSettingUpdated != undefined) {
                $scope.onGenericBEAdditionalSettingUpdated(additionalSettings);
            }
            $scope.modalContext.closeModal();
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }

    }

    appControllers.controller('VR_GenericData_GenericBEAdditionalSettingController', GenericBEActionDefintionController);
})(appControllers);
