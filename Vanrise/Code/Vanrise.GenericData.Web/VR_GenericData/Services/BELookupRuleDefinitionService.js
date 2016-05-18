(function (appControllers) {

    'use strict';

    BELookupRuleDefinitionService.$inject = ['VR_GenericData_BELookupRuleDefinitionAPIService', 'VRModalService', 'VRNotificationService'];

    function BELookupRuleDefinitionService(VR_GenericData_BELookupRuleDefinitionAPIService, VRModalService, VRNotificationService)
    {
        var beLookupRuleDefinitionEditorUrl = '/Client/Modules/VR_GenericData/Views/BELookupRuleDefinition/BELookupRuleDefinitionEditor.html';
        var beLookupRuleDefinitionCriteriaFieldEditorUrl = '/Client/Modules/VR_GenericData/Views/BELookupRuleDefinition/BELookupRuleDefinitionCriteriaFieldEditor.html';

        function addBELookupRuleDefinition(onBELookupRuleDefinitionAdded)
        {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBELookupRuleDefinitionAdded = onBELookupRuleDefinitionAdded;
            };

            VRModalService.showModal(beLookupRuleDefinitionEditorUrl, null, settings);
        }

        function editBELookupRuleDefinition(beLookupRuleDefinitionId, onBELookupRuleDefinitionUpdated)
        {
            var parameters = {
                beLookupRuleDefinitionId: beLookupRuleDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBELookupRuleDefinitionUpdated = onBELookupRuleDefinitionUpdated;
            };

            VRModalService.showModal(beLookupRuleDefinitionEditorUrl, parameters, settings);
        }

        function deleteBELookupRuleDefinition(scope, beLookupRuleDefinitionId, onBELookupRuleDefinitionDeleted)
        {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    VR_GenericData_BELookupRuleDefinitionAPIService.DeleteBELookupRuleDefinition(beLookupRuleDefinitionId).then(function (response) {
                        if (response) {
                            var deleted = VRNotificationService.notifyOnItemDeleted('BE Lookup Rule Definition', response);

                            if (deleted && onBELookupRuleDefinitionDeleted != undefined)
                                onBELookupRuleDefinitionDeleted();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }

        function addBELookupRuleDefinitionCriteriaField(criteriaFields, beDefinitionId, onBELookupRuleDefinitionCriteriaFieldAdded)
        {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBELookupRuleDefinitionCriteriaFieldAdded = onBELookupRuleDefinitionCriteriaFieldAdded;
            };

            var parameters = {
                criteriaFields: criteriaFields,
                beDefinitionId: beDefinitionId
            };

            VRModalService.showModal(beLookupRuleDefinitionCriteriaFieldEditorUrl, parameters, settings);
        }

        function editBELookupRuleDefinitionCriteriaField(criteriaFieldTitle, criteriaFields, beDefinitionId, onBELookupRuleDefinitionCriteriaFieldUpdated)
        {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBELookupRuleDefinitionCriteriaFieldUpdated = onBELookupRuleDefinitionCriteriaFieldUpdated;
            };

            var parameters = {
                criteriaFieldTitle: criteriaFieldTitle,
                criteriaFields: criteriaFields,
                beDefinitionId: beDefinitionId
            };

            VRModalService.showModal(beLookupRuleDefinitionCriteriaFieldEditorUrl, parameters, settings);
        }

        return {
            addBELookupRuleDefinition: addBELookupRuleDefinition,
            editBELookupRuleDefinition: editBELookupRuleDefinition,
            deleteBELookupRuleDefinition: deleteBELookupRuleDefinition,
            addBELookupRuleDefinitionCriteriaField: addBELookupRuleDefinitionCriteriaField,
            editBELookupRuleDefinitionCriteriaField: editBELookupRuleDefinitionCriteriaField
        };
    }

    appControllers.service('VR_GenericData_BELookupRuleDefinitionService', BELookupRuleDefinitionService);

})(appControllers);