(function (appControllers) {

    'use strict';

    SwitchIdentificationRuleService.$inject = ['WhS_CDRProcessing_CustomerIdentificationRuleAPIService', 'VRModalService', 'VRNotificationService'];

    function SwitchIdentificationRuleService(WhS_CDRProcessing_CustomerIdentificationRuleAPIService, VRModalService, VRNotificationService) {
        return {
            addSwitchIdentificationRule: addSwitchIdentificationRule,
            editSwitchIdentificationRule: editSwitchIdentificationRule,
            deleteSwitchIdentificationRule: deleteSwitchIdentificationRule
        };

        function addSwitchIdentificationRule(onSwitchIdentificationRuleAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchIdentificationRuleAdded = onSwitchIdentificationRuleAdded;
            };
            var parameters = {
            };
            VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/SwitchRule/SwitchIdentificationRuleEditor.html', parameters, settings);
        }

        function editSwitchIdentificationRule(obj, onSwitchIdentificationRuleUpdated) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchIdentificationRuleUpdated = onSwitchIdentificationRuleUpdated;
            };
            var parameters = {
                RuleId: obj.RuleId,
            };
            VRModalService.showModal('/Client/Modules/WhS_CDRProcessing/Views/SwitchRule/SwitchIdentificationRuleEditor.html', parameters, settings);
        }

        function deleteSwitchIdentificationRule($scope, switchRuleObj, onSwitchIdentificationRuleObjDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        return WhS_CDRProcessing_CustomerIdentificationRuleAPIService.DeleteRule(switchRuleObj.Entity.RuleId)
                            .then(function (deletionResponse) {
                                VRNotificationService.notifyOnItemDeleted("Switch Identification Rule", deletionResponse);
                                onSwitchIdentificationRuleObjDeleted(switchRuleObj);
                            })
                            .catch(function (error) {
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                });
        }
    }

    appControllers.service('WhS_CDRProcessing_SwitchIdentificationRuleService', SwitchIdentificationRuleService);

})(appControllers);
