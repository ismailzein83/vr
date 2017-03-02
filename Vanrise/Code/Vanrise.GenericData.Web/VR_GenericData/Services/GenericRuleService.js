(function (appControllers) {

    'use strict';

    genericRule.$inject = ['VRModalService', 'VRNotificationService', 'VR_GenericData_GenericRuleAPIService', 'UtilsService'];

    function genericRule(VRModalService, VRNotificationService, VR_GenericData_GenericRuleAPIService, UtilsService) {
        return ({
            addGenericRule: addGenericRule,
            editGenericRule: editGenericRule,
            deleteGenericRule: deleteGenericRule,            
            viewGenericRule: viewGenericRule
        });

        function addGenericRule(genericRuleDefinitionId, onGenericRuleAdded, preDefinedData, accessibility) {
            var modalParameters = {
                genericRuleDefinitionId: genericRuleDefinitionId,
                preDefinedData: preDefinedData,
                accessibility: accessibility
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleAdded = onGenericRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', modalParameters, modalSettings);
        }

        function editGenericRule(genericRuleId, genericRuleDefinitionId, onGenericRuleUpdated, accessibility) {
            var modalParameters = {
                genericRuleId: genericRuleId,
                genericRuleDefinitionId: genericRuleDefinitionId,
                accessibility: accessibility
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleUpdated = onGenericRuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', modalParameters, modalSettings);
        }
        function viewGenericRule(genericRuleId, genericRuleDefinitionId, accessibility) {
            var modalParameters = {
                genericRuleId: genericRuleId,
                genericRuleDefinitionId: genericRuleDefinitionId,
                accessibility: accessibility
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', modalParameters, modalSettings);
        }
        function deleteGenericRule(scope, genericRule, onGenericRuleDeleted) {
            var message = 'Are you sure you want to delete the rule ?';
            VRNotificationService.showConfirmation(message).then(function (confirmed) {
                if (confirmed) {
                    VR_GenericData_GenericRuleAPIService.DeleteGenericRule(genericRule).then(function (response) {
                        if (response) {
                            var deleted = VRNotificationService.notifyOnItemDeleted('Generic Rule', response);

                            if (deleted && onGenericRuleDeleted && typeof onGenericRuleDeleted == 'function') {
                                onGenericRuleDeleted();
                            }
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }
    };

    appControllers.service('VR_GenericData_GenericRule', genericRule);

})(appControllers);
