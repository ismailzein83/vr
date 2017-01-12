(function (appControllers) {

    'use strict';

    BEParentChildRelationService.$inject = ['VRModalService', 'VRNotificationService'];

    function BEParentChildRelationService(VRModalService, VRNotificationService) {

        function addBEParentChildRelation(onBEParentChildRelationAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onBEParentChildRelationAdded = onBEParentChildRelationAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/BEParentChildRelation/BEParentChildRelationEditor.html', null, settings);
        }

        function editBEParentChildRelation(beParentChildRelationId, onBEParentChildRelationUpdated) {
            var parameters = {
                beParentChildRelationId: beParentChildRelationId
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