﻿(function (appControllers) {

    'use strict';

    genericRule.$inject = ['VRModalService', 'VRNotificationService', 'VR_GenericData_GenericRuleAPIService'];

    function genericRule(VRModalService, VRNotificationService, VR_GenericData_GenericRuleAPIService) {
        return ({
            addGenericRule: addGenericRule,
            editGenericRule: editGenericRule,
            deleteGenericRule: deleteGenericRule
        });

        function addGenericRule(genericRuleDefinitionId, onGenericRuleAdded) {
            var modalParameters = {
                genericRuleDefinitionId: genericRuleDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleAdded = onGenericRuleAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRule/GenericRuleEditor.html', modalParameters, modalSettings);
        }

        function editGenericRule(genericRuleId, genericRuleDefinitionId, onGenericRuleUpdated) {
            var modalParameters = {
                genericRuleId: genericRuleId,
                genericRuleDefinitionId: genericRuleDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleUpdated = onGenericRuleUpdated;
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
