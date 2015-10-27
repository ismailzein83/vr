(function (appControllers) {

    "use strict";

    normalizationRuleEditorController.$inject = ["$scope", "WhS_CDRProcessing_NormalizationRuleAPIService", "WhS_CDRProcessing_PhoneNumberTypeEnum", "UtilsService", "VRUIUtilsService", "VRNavigationService", "VRNotificationService"];

    function normalizationRuleEditorController($scope, WhS_CDRProcessing_NormalizationRuleAPIService, WhS_CDRProcessing_PhoneNumberTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var editMode;

        var normalizationRuleId;
        var appendixDirectiveData; 
        var normalizationRuleSettingsDirectiveAPI;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                normalizationRuleId = parameters.NormalizationRuleId;
            }
            editMode = (normalizationRuleId != undefined);
        }

        function defineScope() {

            $scope.phoneNumberTypes = UtilsService.getArrayEnum(WhS_CDRProcessing_PhoneNumberTypeEnum);
            $scope.selectedPhoneNumberType = undefined;

            $scope.phoneNumberLength = undefined;
            $scope.phoneNumberPrefix = undefined;

            $scope.beginEffectiveDate = Date.now();
            $scope.endEffectiveDate = undefined;
            $scope.onNormalizeNumberSettingsDirectiveReady = function (api) {
                normalizationRuleSettingsDirectiveAPI = api;
                load();
            }
            
            $scope.saveNormalizationRule = function () {
                if (editMode)
                    return updateNormalizationRule();
                else
                    return insertNormalizationRule();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isGettingData = true;
            if (normalizationRuleSettingsDirectiveAPI == undefined)
                return;
            normalizationRuleSettingsDirectiveAPI.load().then(function () {
                if (editMode) {
                    WhS_CDRProcessing_NormalizationRuleAPIService.GetRule(normalizationRuleId)
                        .then(function (response) {
                            appendixDirectiveData = response;
                            fillScopeFromNormalizationRuleObj(response);
                            tryLoadAppendixDirectives();
                        })
                        .catch(function (error) {
                            $scope.isGettingData = false;
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });
                }
                else {
                    setDefaultValues();
                    $scope.isGettingData = false;
                }
            }).catch(function (error) {
                $scope.isGettingData = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
            
                   
               
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

        function fillScopeFromNormalizationRuleObj(rule) {

            $scope.description = rule.Description;
        
            $scope.selectedPhoneNumberType = (rule.Criteria.PhoneNumberType != null) ?
                UtilsService.getItemByVal($scope.phoneNumberTypes, rule.Criteria.PhoneNumberType, "value") : undefined;

            $scope.phoneNumberLength = rule.Criteria.PhoneNumberLength;
            $scope.phoneNumberPrefix = rule.Criteria.PhoneNumberPrefix;
            $scope.title = UtilsService.buildTitleForUpdateEditor("Normalization Rule");

            $scope.beginEffectiveTime = rule.BeginEffectiveTime;
            $scope.endEffectiveTime = rule.EndEffectiveTime;
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
            console.log(normalizationRuleSettingsDirectiveAPI.getData());
            var normalizationRule = {
                RuleId: (normalizationRuleId != undefined) ? normalizationRuleId : null,
                Criteria: {
                    PhoneNumberType: ($scope.selectedPhoneNumberType != undefined) ? $scope.selectedPhoneNumberType.value : null,
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
