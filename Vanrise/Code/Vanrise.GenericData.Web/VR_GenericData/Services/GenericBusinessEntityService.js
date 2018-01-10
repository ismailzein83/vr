(function (appControllers) {

    'use strict';

    GenericBusinessEntityService.$inject = ['VR_GenericData_GenericBusinessEntityAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function GenericBusinessEntityService(VR_GenericData_GenericBusinessEntityAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addGenericBusinessEntity: addGenericBusinessEntity,
            editGenericBusinessEntity: editGenericBusinessEntity,
           // registerObjectTrackingDrillDownToGenericBusinessEntity: registerObjectTrackingDrillDownToGenericBusinessEntity,
         //   getDrillDownDefinition: getDrillDownDefinition

        });

        function addGenericBusinessEntity(onGenericBEAdded, businessEntityDefinitionId) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId
            };

            var settings = { };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEAdded = onGenericBEAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', parameters, settings);
        }

        function editGenericBusinessEntity(onGenericBEUpdated, businessEntityDefinitionId, genericBusinessEntityId) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId,
                genericBusinessEntityId: genericBusinessEntityId
            };

            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEUpdated = onGenericBEUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', parameters, settings);
        }


  
        //function getEntityUniqueName(businessEntityDefinitionId) {
        //    return "VR_GenericData_GenericBusinessEntity_"+businessEntityDefinitionId;
        //}

        //function registerObjectTrackingDrillDownToGenericBusinessEntity() {
        //    var drillDownDefinition = {};

        //    drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
        //    drillDownDefinition.directive = "vr-common-objecttracking-grid";


        //    drillDownDefinition.loadDirective = function (directiveAPI, genericBusinessEntityItem) {
               
        //        genericBusinessEntityItem.objectTrackingGridAPI = directiveAPI;
        //        var query = {
        //            ObjectId: genericBusinessEntityItem.Entity.GenericBusinessEntityId,
        //            EntityUniqueName: getEntityUniqueName(genericBusinessEntityItem.Entity.BusinessEntityDefinitionId),

        //        };
        //        return genericBusinessEntityItem.objectTrackingGridAPI.load(query);
        //    };

        //    addDrillDownDefinition(drillDownDefinition);

        //}
        //function addDrillDownDefinition(drillDownDefinition) {

        //    drillDownDefinitions.push(drillDownDefinition);
        //}

        //function getDrillDownDefinition() {
        //    return drillDownDefinitions;
        //}
    };

    appControllers.service('VR_GenericData_GenericBusinessEntityService', GenericBusinessEntityService);

})(appControllers);