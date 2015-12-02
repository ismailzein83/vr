(function (appControllers) {

    "use strict";

    normalizationRuleEditorController.$inject = ["$scope", "WhS_CDRProcessing_NormalizationRuleAPIService", "WhS_CDRProcessing_PhoneNumberTypeEnum", "UtilsService", "VRUIUtilsService", "VRNavigationService", "VRNotificationService",'VRValidationService'];

    function normalizationRuleEditorController($scope, WhS_CDRProcessing_NormalizationRuleAPIService, WhS_CDRProcessing_PhoneNumberTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VRValidationService) {

        var isEditMode;

        var normalizationRuleId;
        var normalizationRuleEntity;
        var normalizationRuleSettingsDirectiveAPI;
        var normalizationRuleReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                normalizationRuleId = parameters.NormalizationRuleId;
            }
            isEditMode = (normalizationRuleId != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};
            $scope.scopeModal.validateDateTime = function () {
                return VRValidationService.validateTimeRange($scope.scopeModal.beginEffectiveDate, $scope.scopeModal.endEffectiveDate);
            }
            $scope.scopeModal.phoneNumberTypes = UtilsService.getArrayEnum(WhS_CDRProcessing_PhoneNumberTypeEnum);
            $scope.scopeModal.selectedPhoneNumberTypes = [];

            $scope.scopeModal.phoneNumberLength = undefined;
            $scope.scopeModal.phoneNumberPrefix = undefined;

            $scope.scopeModal.beginEffectiveDate = Date.now();
            $scope.scopeModal.endEffectiveDate = undefined;
            $scope.scopeModal.onNormalizeNumberSettingsDirectiveReady = function (api) {
                normalizationRuleSettingsDirectiveAPI = api;
                normalizationRuleReadyPromiseDeferred.resolve();
            }
            
            $scope.scopeModal.saveNormalizationRule = function () {
                if (isEditMode)
                    return updateNormalizationRule();
                else
                    return insertNormalizationRule();
            };

            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModal.isLoading = true;



            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Normalization Rule");
                getNormalizationRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            customerRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModal.isLoading = false;
                });
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Normalization Rule");
                loadAllControls();
            }     
                          
        }
        function getNormalizationRule() {
            return WhS_CDRProcessing_NormalizationRuleAPIService.GetRule(normalizationRuleId).then(function (normalizationRule) {
                normalizationRuleEntity = normalizationRule;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection, loadNormalizationRuleDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.scopeModal.isLoading = false;
               });
        }
        function loadNormalizationRuleDirective() {

            var loadNormalizationRulePromiseDeferred = UtilsService.createPromiseDeferred();

            normalizationRuleReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = normalizationRuleEntity != undefined ? normalizationRuleEntity.Settings : undefined;
                    VRUIUtilsService.callDirectiveLoad(normalizationRuleSettingsDirectiveAPI, directivePayload, loadNormalizationRulePromiseDeferred);
                });

            return loadNormalizationRulePromiseDeferred.promise;
        }
        function loadFilterBySection() {
            if (normalizationRuleEntity != undefined) {
                $scope.scopeModal.description = normalizationRuleEntity.Description;
                for (var i = 0; i < normalizationRuleEntity.Criteria.PhoneNumberTypes.length; i++) {
                    $scope.scopeModal.selectedPhoneNumberTypes.push(UtilsService.getItemByVal($scope.scopeModal.phoneNumberTypes, normalizationRuleEntity.Criteria.PhoneNumberTypes[i], "value"));
                }
                $scope.scopeModal.phoneNumberLength = normalizationRuleEntity.Criteria.PhoneNumberLength;
                $scope.scopeModal.phoneNumberPrefix = normalizationRuleEntity.Criteria.PhoneNumberPrefix;
                $scope.title = UtilsService.buildTitleForUpdateEditor("Normalization Rule");
                $scope.scopeModal.beginEffectiveTime = normalizationRuleEntity.BeginEffectiveTime;
                $scope.scopeModal.endEffectiveTime = normalizationRuleEntity.EndEffectiveTime;
            }
        }
    
        function setDefaultValues() {
            $scope.title = UtilsService.buildTitleForAddEditor("Normalization Rule");
        }

        function updateNormalizationRule() {
            var normalizationRuleObj = buildNormalizationRuleObjFromScope();

            return WhS_CDRProcessing_NormalizationRuleAPIService.UpdateRule(normalizationRuleObj)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Normalization Rule", response)) {
                        if ($scope.onNormalizationRuleUpdated != undefined)
                            $scope.onNormalizationRuleUpdated(response.UpdatedObject);

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        function insertNormalizationRule() {
            var normalizationRuleObj = buildNormalizationRuleObjFromScope();

            return WhS_CDRProcessing_NormalizationRuleAPIService.AddRule(normalizationRuleObj)
                .then(function (responseObject) {

                    if (VRNotificationService.notifyOnItemAdded("Normalization Rule", responseObject)) {
                        if ($scope.onNormalizationRuleAdded != undefined)
                            $scope.onNormalizationRuleAdded(responseObject.InsertedObject);

                        $scope.modalContext.closeModal();
                    }
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        function buildNormalizationRuleObjFromScope() {
            var normalizationRule = {
                RuleId: (normalizationRuleId != undefined) ? normalizationRuleId : null,
                Criteria: {
                    PhoneNumberTypes: UtilsService.getPropValuesFromArray($scope.scopeModal.selectedPhoneNumberTypes, "value"),
                    PhoneNumberLength: $scope.scopeModal.phoneNumberLength,
                    PhoneNumberPrefix: $scope.scopeModal.phoneNumberPrefix
                },
              
                Settings: normalizationRuleSettingsDirectiveAPI.getData(),
                Description: $scope.scopeModal.description,
                BeginEffectiveTime: $scope.scopeModal.beginEffectiveTime,
                EndEffectiveTime: $scope.scopeModal.endEffectiveTime
            };

            return normalizationRule;
        }
    }

    appControllers.controller("WhS_CDRProcessing_NormalizationRuleEditorController", normalizationRuleEditorController);

})(appControllers);
