(function (appControllers) {

    'use strict';

    GenericBusinessEntityService.$inject = ['VR_GenericData_GenericBusinessEntityAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function GenericBusinessEntityService(VR_GenericData_GenericBusinessEntityAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addGenericBusinessEntity: addGenericBusinessEntity,
          //  editGenericBusinessEntity: editGenericBusinessEntity,
          //  deleteGenericBusinessEntity: deleteGenericBusinessEntity,
           // registerObjectTrackingDrillDownToGenericBusinessEntity: registerObjectTrackingDrillDownToGenericBusinessEntity,
         //   getDrillDownDefinition: getDrillDownDefinition

        });

        function addGenericBusinessEntity(onGenericBusinessEntityAdded,businessEntityDefinitionId) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBusinessEntityAdded = onGenericBusinessEntityAdded;
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', parameters, settings);
        }

        //function editGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId, onGenericBusinessEntityUpdated) {
        //    var parameters = {
        //        genericBusinessEntityId: genericBusinessEntityId,
        //        businessEntityDefinitionId: businessEntityDefinitionId
        //    };

        //    var settings = {};

        //    settings.onScopeReady = function (modalScope) {
        //        modalScope.onGenericBusinessEntityUpdated = onGenericBusinessEntityUpdated;
        //    };

        //    VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', parameters, settings);
        //}

        //function deleteGenericBusinessEntity(scope, genericBusinessEntity, onGenericBusinessEntityDeleted) {
        //    VRNotificationService.showConfirmation().then(function (confirmed) {
        //        if (confirmed) {
        //            var entityId = genericBusinessEntity.Entity.GenericBusinessEntityId;
        //            var definitionId = genericBusinessEntity.Entity.BusinessEntityDefinitionId;
        //            return VR_GenericData_GenericBusinessEntityAPIService.DeleteGenericBusinessEntity(entityId, definitionId).then(function (deletionResponse) {
        //                var deleted = VRNotificationService.notifyOnItemDeleted("Generic Business Entity", deletionResponse);
        //                if (deleted && onGenericBusinessEntityDeleted != undefined) {
        //                    onGenericBusinessEntityDeleted();
        //                }
        //            }).catch(function (error) {
        //                VRNotificationService.notifyException(error, scope);
        //            });
        //        }
        //    });
        //}
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