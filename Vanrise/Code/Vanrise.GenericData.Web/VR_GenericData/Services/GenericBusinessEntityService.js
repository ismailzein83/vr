(function (appControllers) {

    'use strict';

    GenericBusinessEntityService.$inject = ['VR_GenericData_GenericBusinessEntityAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService', 'UtilsService', 'VRUIUtilsService'];

    function GenericBusinessEntityService(VR_GenericData_GenericBusinessEntityAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService, UtilsService, VRUIUtilsService) {
        var drillDownDefinitions = [];
        return ({
            addGenericBusinessEntity: addGenericBusinessEntity,
            editGenericBusinessEntity: editGenericBusinessEntity,
            defineGenericBEViewTabs: defineGenericBEViewTabs,
            getEntityUniqueName: getEntityUniqueName
         //   getDrillDownDefinition: getDrillDownDefinition

        });

        function addGenericBusinessEntity(onGenericBEAdded, businessEntityDefinitionId,editorSize,fieldValues) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId,
                fieldValues:fieldValues
            };

            var settings = {
               size: editorSize != undefined ? editorSize : "medium",
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEAdded = onGenericBEAdded;
             
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', parameters, settings);
        }

        function editGenericBusinessEntity(onGenericBEUpdated, businessEntityDefinitionId, genericBusinessEntityId, editorSize, fieldValues) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId,
                genericBusinessEntityId: genericBusinessEntityId,
                fieldValues : fieldValues
            };

            var settings = {
                size: editorSize != undefined ? editorSize : "medium",
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEUpdated = onGenericBEUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditor.html', parameters, settings);
        }

        function defineGenericBEViewTabs(businessEntityDefinitionId, genericBusinessEntity, gridAPI, genericBEGridViews, idFieldType)
        {
            if (businessEntityDefinitionId == undefined || genericBusinessEntity == undefined || genericBusinessEntity.AvailableGridViewIds == undefined || genericBusinessEntity.AvailableGridViewIds.length == 0)
                return;

            var drillDownTabs = [];

            for (var index = 0; index < genericBusinessEntity.AvailableGridViewIds.length; index++) {

                var currentGenricBEViewId = genericBusinessEntity.AvailableGridViewIds[index];
                var genericBEGridView = UtilsService.getItemByVal(genericBEGridViews, currentGenricBEViewId, "GenericBEViewDefinitionId");

                addDrillDownTab(genericBEGridView);
            }

            setDrillDownTabs();

            function addDrillDownTab(genericBEGridView) {
                if (genericBEGridView == undefined || genericBEGridView.Name == undefined ||
                    genericBEGridView.Settings == undefined || genericBEGridView.Settings.RuntimeDirective == undefined)
                    return;

                var drillDownTab = {};

                drillDownTab.title = genericBEGridView.Name;
                drillDownTab.directive = genericBEGridView.Settings.RuntimeDirective;

                drillDownTab.loadDirective = function (genericBEViewGridAPI, currentGenericBEEntity) {
                    currentGenericBEEntity.genericBEViewGridAPI = genericBEViewGridAPI;

                    return currentGenericBEEntity.genericBEViewGridAPI.load(buildGenericBEViewPayload());
                };

                function buildGenericBEViewPayload() {

                    var payload = {
                        genericBEGridView: genericBEGridView,
                        businessEntityDefinitionId: businessEntityDefinitionId,
                        genericBusinessEntityId: idFieldType != undefined? genericBusinessEntity.FieldValues[idFieldType.Name].Value :undefined
                    };
                    return payload;
                }

                drillDownTabs.push(drillDownTab);
            }
            function setDrillDownTabs() {
                var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI);
                drillDownManager.setDrillDownExtensionObject(genericBusinessEntity);
            }
        }

  
        function getEntityUniqueName(businessEntityDefinitionId) {
            return "VR_GenericData_GenericBusinessEntity_"+ businessEntityDefinitionId;
        }

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