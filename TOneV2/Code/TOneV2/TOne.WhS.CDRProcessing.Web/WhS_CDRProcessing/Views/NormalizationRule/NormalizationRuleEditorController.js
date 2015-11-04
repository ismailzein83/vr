(function (appControllers) {

    "use strict";

    normalizationRuleEditorController.$inject = ["$scope", "WhS_CDRProcessing_NormalizationRuleAPIService", "WhS_CDRProcessing_PhoneNumberTypeEnum", "UtilsService", "VRUIUtilsService", "VRNavigationService", "VRNotificationService"];

    function normalizationRuleEditorController($scope, WhS_CDRProcessing_NormalizationRuleAPIService, WhS_CDRProcessing_PhoneNumberTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

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

            $scope.phoneNumberTypes = UtilsService.getArrayEnum(WhS_CDRProcessing_PhoneNumberTypeEnum);
            $scope.selectedPhoneNumberTypes = [];

            $scope.phoneNumberLength = undefined;
            $scope.phoneNumberPrefix = undefined;

            $scope.beginEffectiveDate = Date.now();
            $scope.endEffectiveDate = undefined;
            $scope.onNormalizeNumberSettingsDirectiveReady = function (api) {
                normalizationRuleSettingsDirectiveAPI = api;
                normalizationRuleReadyPromiseDeferred.resolve();
            }
            
            $scope.saveNormalizationRule = function () {
                if (isEditMode)
                    return updateNormalizationRule();
                else
                    return insertNormalizationRule();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;



            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Supplier Rule");
                getNormalizationRule().then(function () {
                    loadAllControls()
                        .finally(function () {
                            customerRuleEntity = undefined;
                        });
                }).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Supplier Rule");
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
                   $scope.isLoading = false;
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
                $scope.description = normalizationRuleEntity.Description;
                for (var i = 0; i < normalizationRuleEntity.Criteria.PhoneNumberTypes.length; i++) {
                    $scope.selectedPhoneNumberTypes.push(UtilsService.getItemByVal($scope.phoneNumberTypes, normalizationRuleEntity.Criteria.PhoneNumberTypes[i], "value"));
                }
                $scope.phoneNumberLength = normalizationRuleEntity.Criteria.PhoneNumberLength;
                $scope.phoneNumberPrefix = normalizationRuleEntity.Criteria.PhoneNumberPrefix;
                $scope.title = UtilsService.buildTitleForUpdateEditor("Normalization Rule");
                $scope.beginEffectiveTime = normalizationRuleEntity.BeginEffectiveTime;
                $scope.endEffectiveTime = normalizationRuleEntity.EndEffectiveTime;
            }
        }
    

        function tryLoadAppendixDirectives() {

            var loadOperations = [];
            var setOperations = [];

            if (normalizationRuleSettingsDirectiveAPI == undefined)
                return;

            loadOperations.push(normalizationRuleSettingsDirectiveAPI.load);
            setOperations.push(setNormalizationRuleSettingsDirective);

            UtilsService.waitMultipleAsyncOperations(loadOperations)
                .then(function () {
                    if(appendixDirectiveData!=undefined)
                     setAppendixDirectives();
                })
                .catch(function (error) {
                    $scope.isGettingData = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });

            function setAppendixDirectives() {
                UtilsService.waitMultipleAsyncOperations(setOperations)
                    .then(function () {
                        appendixDirectiveData = undefined;
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                    .finally(function () {
                        $scope.isGettingData = false;
                    });
            }

            function setNormalizationRuleSettingsDirective() {
                normalizationRuleSettingsDirectiveAPI.setData(appendixDirectiveData.Settings);
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
                    PhoneNumberTypes: UtilsService.getPropValuesFromArray($scope.selectedPhoneNumberTypes, "value"),
                    PhoneNumberLength: $scope.phoneNumberLength,
                    PhoneNumberPrefix: $scope.phoneNumberPrefix
                },
              
                Settings: normalizationRuleSettingsDirectiveAPI.getData(),
                Description: $scope.description,
                BeginEffectiveTime: $scope.beginEffectiveTime,
                EndEffectiveTime: $scope.endEffectiveTime
            };

            return normalizationRule;
        }
    }

    appControllers.controller("WhS_CDRProcessing_NormalizationRuleEditorController", normalizationRuleEditorController);

})(appControllers);
