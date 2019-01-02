(function (appControllers) {

    "use strict";

    qualityConfigurationEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_QualityConfigurationAPIService'];

    function qualityConfigurationEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_Routing_QualityConfigurationAPIService) {

        var isEditMode;
        var qualityConfigurationEntity;
        var qualityConfigurationNames; //for validation

        var qualityConfigurationDefinitionSelectorAPI;
        var qualityConfigurationDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var qualityConfigurationSettingsDirectiveAPI;
        var qualityConfigurationSettingsDirectiveReadyDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                qualityConfigurationEntity = parameters.qualityConfigurationEntity;
                qualityConfigurationNames = parameters.qualityConfigurationNames;
            }

            isEditMode = (qualityConfigurationEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onQualityConfigurationDefinitionSelectorReady = function (api) {
                qualityConfigurationDefinitionSelectorAPI = api;
                qualityConfigurationDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onQualityConfigurationSettingsDirectiveReady = function (api) {
                qualityConfigurationSettingsDirectiveAPI = api;

                var qualityConfigurationSettingsDirectivePayload;
                if ($scope.scopeModel.selectedQualityConfigurationDefinition != undefined) {
                    qualityConfigurationSettingsDirectivePayload = {
                        qualityConfigurationDefinitionId: $scope.scopeModel.selectedQualityConfigurationDefinition.QualityConfigurationDefinitionId
                    };
                }

                var setLoader = function (value) {
                    $scope.scopeModel.isQualityConfigurationSettingsDirectiveLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, qualityConfigurationSettingsDirectiveAPI, qualityConfigurationSettingsDirectivePayload, setLoader, qualityConfigurationSettingsDirectiveReadyDeferred);
            };

            $scope.scopeModel.validateQualityConfigurationName = function () {
                if ($scope.scopeModel.qualityConfigurationyName == undefined || qualityConfigurationNames == undefined)
                    return null;

                //for EditMode
                if (qualityConfigurationEntity != undefined && qualityConfigurationEntity.Name == $scope.scopeModel.qualityConfigurationyName)
                    return null;

                for (var i = 0; i < qualityConfigurationNames.length; i++) {
                    var qualityConfigurationName = qualityConfigurationNames[i];
                    if ($scope.scopeModel.qualityConfigurationyName.toLowerCase() == qualityConfigurationName.toLowerCase())
                        return 'Same Quality Configuration Name Exists';
                }
                return null;
            };

            $scope.scopeModel.saveQualityConfiguration = function () {
                if (isEditMode) {
                    return updateQualityConfiguration();
                }
                else
                    return insertQualityConfiguration();
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

            function setTitle() {
                if (isEditMode && qualityConfigurationEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(qualityConfigurationEntity.Name, "Quality Configuration");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Quality Configuration");
            }

            function loadStaticData() {
                if (isEditMode) {
                    $scope.scopeModel.qualityConfigurationId = qualityConfigurationEntity.QualityConfigurationId;
                    $scope.scopeModel.qualityConfigurationyName = qualityConfigurationEntity.Name;
                    $scope.scopeModel.routingTriggerThreshold = qualityConfigurationEntity.RoutingTriggerThreshold;
                }
            }

            function loadQualityConfigurationDefinitionSelector() {
                var qualityConfigurationDefinitionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                qualityConfigurationDefinitionSelectorReadyDeferred.promise.then(function () {

                    var qualityConfigurationDefinitionSelectorPayload;
                    if (qualityConfigurationEntity != undefined) {
                        qualityConfigurationDefinitionSelectorPayload = { selectedIds: qualityConfigurationEntity.QualityConfigurationDefinitionId };
                    }
                    VRUIUtilsService.callDirectiveLoad(qualityConfigurationDefinitionSelectorAPI, qualityConfigurationDefinitionSelectorPayload, qualityConfigurationDefinitionSelectorLoadPromiseDeferred);
                });

                return qualityConfigurationDefinitionSelectorLoadPromiseDeferred.promise;
            }

            function loadQualityConfigurationSettingsDirectiveWrapper() {
                if (!isEditMode)
                    return;

                qualityConfigurationSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

                var qualityConfigurationSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                qualityConfigurationSettingsDirectiveReadyDeferred.promise.then(function () {
                    qualityConfigurationSettingsDirectiveReadyDeferred = undefined;

                    var qualityConfigurationSettingsDirectivePayload = { qualityConfigurationDefinitionId: qualityConfigurationEntity.QualityConfigurationDefinitionId };
                    if (qualityConfigurationEntity != undefined && qualityConfigurationEntity.Settings != undefined) {
                        qualityConfigurationSettingsDirectivePayload.qualityConfigurationSettings = qualityConfigurationEntity.Settings;
                    }
                    VRUIUtilsService.callDirectiveLoad(qualityConfigurationSettingsDirectiveAPI, qualityConfigurationSettingsDirectivePayload, qualityConfigurationSettingsDirectiveLoadDeferred);
                });

                return qualityConfigurationSettingsDirectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadQualityConfigurationDefinitionSelector, loadQualityConfigurationSettingsDirectiveWrapper])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function insertQualityConfiguration() {
            var qualityConfigurationObject = buildQualityConfigurationObjectFromScope(UtilsService.guid());

            return WhS_Routing_QualityConfigurationAPIService.ValidateRouteRuleQualityConfiguration(UtilsService.serializetoJson(qualityConfigurationObject)).then(function (response) {
                if (response) {
                    if ($scope.onQualityConfigurationAdded != undefined)
                        $scope.onQualityConfigurationAdded(qualityConfigurationObject);
                    $scope.modalContext.closeModal();
                } else {
                    VRNotificationService.showError("Quality Configuration validation has failed. Check Log for more informations");
                }
            });
        }
        function updateQualityConfiguration() {
            var qualityConfigurationObject = buildQualityConfigurationObjectFromScope($scope.scopeModel.qualityConfigurationId);

            return WhS_Routing_QualityConfigurationAPIService.ValidateRouteRuleQualityConfiguration(UtilsService.serializetoJson(qualityConfigurationObject)).then(function (response) {
                if (response) {
                    if ($scope.onQualityConfigurationUpdated != undefined)
                        $scope.onQualityConfigurationUpdated(qualityConfigurationObject);
                    $scope.modalContext.closeModal();
                } else {
                    VRNotificationService.showError("Quality Configuration validation has failed. Check Log for more informations");
                }
            });
        }

        function buildQualityConfigurationObjectFromScope(qualityConfigurationId) {

            var qualityConfigurationDefinitionId;
            if ($scope.scopeModel.selectedQualityConfigurationDefinition != undefined)
                qualityConfigurationDefinitionId = $scope.scopeModel.selectedQualityConfigurationDefinition.QualityConfigurationDefinitionId;

            var obj = {
                QualityConfigurationId: qualityConfigurationId,
                QualityConfigurationDefinitionId: qualityConfigurationDefinitionId,
                Name: $scope.scopeModel.qualityConfigurationyName,
                RoutingTriggerThreshold: $scope.scopeModel.routingTriggerThreshold,
                Settings: qualityConfigurationSettingsDirectiveAPI.getData()
            };

            return obj;
        }
    }

    appControllers.controller('WhS_Routing_RouteRuleQualityConfigurationEditorController', qualityConfigurationEditorController);
})(appControllers);
