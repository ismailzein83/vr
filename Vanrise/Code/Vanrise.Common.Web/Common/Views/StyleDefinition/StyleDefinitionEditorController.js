(function (appControllers) {

    "use strict";

    StyleDefinitionEditorController.$inject = ['$scope', 'VRCommon_StyleDefinitionAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function StyleDefinitionEditorController($scope, VRCommon_StyleDefinitionAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VRNavigationService) {

        var isEditMode;

        var styleDefinitionId;
        var styleDefinitionEntity;

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                styleDefinitionId = parameters.styleDefinitionId;
            }

            isEditMode = (styleDefinitionId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getStyleDefinition().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }


        function getStyleDefinition() {
            return VRCommon_StyleDefinitionAPIService.GetStyleDefinition(styleDefinitionId).then(function (response) {
                styleDefinitionEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                var styleDefinitionName = (styleDefinitionEntity != undefined) ? styleDefinitionEntity.Name : null;
                $scope.title = UtilsService.buildTitleForUpdateEditor(styleDefinitionName, 'StyleDefinition');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('StyleDefinition');
            }
        }
        function loadStaticData() {
            if (styleDefinitionEntity == undefined)
                return;
            $scope.scopeModel.name = styleDefinitionEntity.Name;
        }
        function loadSettingsDirective() {
            var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            settingsDirectiveReadyDeferred.promise.then(function () {
                var settingsDirectivePayload;
                if (styleDefinitionEntity != undefined) {
                    settingsDirectivePayload = { styleDefinitionSettings: styleDefinitionEntity.StyleDefinitionSettings };
                }
                VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
            });

            return settingsDirectiveLoadDeferred.promise;
        }

        function insert() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_StyleDefinitionAPIService.AddStyleDefinition(buildStyleDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('StyleDefinition', response, 'Name')) {
                    if ($scope.onStyleDefinitionAdded != undefined)
                        $scope.onStyleDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return VRCommon_StyleDefinitionAPIService.UpdateStyleDefinition(buildStyleDefinitionObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('StyleDefinition', response, 'Name')) {
                    if ($scope.onStyleDefinitionUpdated != undefined) {
                        $scope.onStyleDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildStyleDefinitionObjFromScope() {
            var styleDefinitionSettings = settingsDirectiveAPI.getData();

            return {
                StyleDefinitionId: styleDefinitionEntity != undefined ? styleDefinitionEntity.StyleDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                StyleDefinitionSettings: styleDefinitionSettings
            };
        }
    }

    appControllers.controller('VRCommon_StyleDefinitionEditorController', StyleDefinitionEditorController);

})(appControllers);