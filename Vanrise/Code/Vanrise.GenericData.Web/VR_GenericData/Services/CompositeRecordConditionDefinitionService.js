(function (appControllers) {

    'use strict';

    CompositeRecordConditionDefinitionService.$inject = ['VRModalService'];

    function CompositeRecordConditionDefinitionService(VRModalService) {

        function addCompositeRecordConditionDefinition(onCompositeRecordConditionDefinitionAdded, conditionRecordNames, conditionRecordTitles) {

            var parameters = {
                conditionRecordNames: conditionRecordNames,
                conditionRecordTitles: conditionRecordTitles
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onCompositeRecordConditionDefinitionAdded = onCompositeRecordConditionDefinitionAdded;
            };
            VRModalService.showModal("/Client/Modules/VR_GenericData/Directives/CompositeRecordCondition/Definition/Templates/CompositeRecordConditionDefinitionEditorTemplate.html", parameters, settings);
        }

        function editCompositeRecordConditionDefinition(onCompositeRecordConditionDefinitionUpdated, compositeRecordConditionDefinition, conditionRecordNames, conditionRecordTitles) {
            var parameters = {
                CompositeRecordConditionDefinition: compositeRecordConditionDefinition,
                conditionRecordNames: conditionRecordNames,
                conditionRecordTitles: conditionRecordTitles
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onCompositeRecordConditionDefinitionUpdated = onCompositeRecordConditionDefinitionUpdated;
            };
            VRModalService.showModal("/Client/Modules/VR_GenericData/Directives/CompositeRecordCondition/Definition/Templates/CompositeRecordConditionDefinitionEditorTemplate.html", parameters, settings);
        }

        return ({
            addCompositeRecordConditionDefinition: addCompositeRecordConditionDefinition,
            editCompositeRecordConditionDefinition: editCompositeRecordConditionDefinition
        });
    }

    appControllers.service('VR_GenericData_CompositeRecordConditionDefinitionService', CompositeRecordConditionDefinitionService);
})(appControllers);