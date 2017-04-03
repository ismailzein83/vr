(function (appControllers) {

    'use strict';

    BEParentChildRelationService.$inject = ['VRModalService', 'VRNotificationService'];

    function BEParentChildRelationService(VRModalService, VRNotificationService) {

        function addBEParentChildRelation(onBEParentChildRelationAdded, beParentChildRelationDefinitionId, parentId, childId) {
            var parameters = {
                beParentChildRelationDefinitionId: beParentChildRelationDefinitionId,
                parentId: parentId,
                childId: childId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBEParentChildRelationAdded = onBEParentChildRelationAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/BEParentChildRelation/BEParentChildRelationEditor.html', parameters, settings);
        }

        function editBEParentChildRelation(onBEParentChildRelationUpdated, beParentChildRelationId, beParentChildRelationDefinitionId, parentId, childId) {
            var parameters = {
                beParentChildRelationId: beParentChildRelationId,
                beParentChildRelationDefinitionId: beParentChildRelationDefinitionId,
                parentId: parentId,
                childId: childId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBEParentChildRelationUpdated = onBEParentChildRelationUpdated;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/BEParentChildRelation/BEParentChildRelationEditor.html', parameters, settings);
        }


        return {
            addBEParentChildRelation: addBEParentChildRelation,
            editBEParentChildRelation: editBEParentChildRelation
        };
    }

    appControllers.service('VR_GenericData_BEParentChildRelationService', BEParentChildRelationService);

})(appControllers);