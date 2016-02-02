(function (appControllers) {

    'use strict';

    GenericRuleDefinitionService.$inject = ['VR_GenericData_GenericRuleDefinitionAPIService', 'VRModalService', 'VRNotificationService'];

    function GenericRuleDefinitionService(VR_GenericData_GenericRuleDefinitionAPIService, VRModalService, VRNotificationService) {
        return {
            addGenericRuleDefinition: addGenericRuleDefinition,
            editGenericRuleDefinition: editGenericRuleDefinition,
            deleteGenericRuleDefinition: deleteGenericRuleDefinition
        };

        function addGenericRuleDefinition(onGenericRuleDefinitionAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionAdded = onGenericRuleDefinitionAdded;
            };
            
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionEditor.html', undefined, modalSettings);
        }

        function editGenericRuleDefinition(genericRuleDefinitionId, onGenericRuleDefinitionUpdated) {
            var modalParameters = {
                GenericRuleDefinitionId: genericRuleDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onGenericRuleDefinitionUpdated = onGenericRuleDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericRuleDefinition/GenericRuleDefinitionEditor.html', modalParameters, modalSettings);
        }

        function deleteGenericRuleDefinition(scope, genericRuleDefinitionId, onGenericRuleDefinitionDeleted) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    VR_GenericData_GenericRuleDefinitionAPIService.DeleteGenericRuleDefinition(genericRuleDefinitionId).then(function (response) {
                        if (response) {
                            var deleted = VRNotificationService.notifyOnItemDeleted('Generic Rule Definition', response);
                            if (deleted && onGenericRuleDefinitionDeleted != undefined && typeof (onGenericRuleDefinitionDeleted) == 'function') {
                                onGenericRuleDefinitionDeleted();
                            }
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionService', GenericRuleDefinitionService);

})(appControllers);