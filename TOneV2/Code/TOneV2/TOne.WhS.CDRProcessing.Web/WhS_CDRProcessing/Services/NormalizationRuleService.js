(function (appControllers) {

    'use strict';

    NormalizationRuleService.$inject = ['WhS_CDRProcessing_NormalizationRuleAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function NormalizationRuleService(WhS_CDRProcessing_NormalizationRuleAPIService, UtilsService, VRModalService, VRNotificationService) {
        return {
            addNormalizationRule: addNormalizationRule,
            editNormalizationRule: editNormalizationRule,
            deleteNormalizationRule: deleteNormalizationRule
        };

        function addNormalizationRule(onNormalizationRuleAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onNormalizationRuleAdded = onNormalizationRuleAdded;
            };

            var parameters = {
            };

            VRModalService.showModal("/Client/Modules/WhS_CDRProcessing/Views/NormalizationRule/NormalizationRuleEditor.html", parameters, modalSettings);
        }

        function editNormalizationRule(normalizationRuleDetail, onNormalizationRuleUpdated) {
            var modalSettings = {};

            var parameters = {
                NormalizationRuleId: normalizationRuleDetail.Entity.RuleId,
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onNormalizationRuleUpdated = onNormalizationRuleUpdated;
            };

            VRModalService.showModal("/Client/Modules/WhS_CDRProcessing/Views/NormalizationRule/NormalizationRuleEditor.html", parameters, modalSettings);
        }

        function deleteNormalizationRule(ruleDetail, onNormalizationRuleDeleted) {

            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response == true) {

                        return WhS_CDRProcessing_NormalizationRuleAPIService.DeleteRule(ruleDetail.Entity.RuleId)
                            .then(function (deletionResponse) {
                                if (VRNotificationService.notifyOnItemDeleted("Normalization Rule", deletionResponse))
                                    onNormalizationRuleDeleted(ruleDetail);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_CDRProcessing_NormalizationRuleService', NormalizationRuleService);

})(appControllers);
