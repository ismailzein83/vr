(function (appControllers) {

    'use strict';

    GenericBusinessEntityService.$inject = ['VR_GenericData_GenericBusinessEntityAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService', 'UtilsService', 'VRUIUtilsService'];

    function GenericBusinessEntityService(VR_GenericData_GenericBusinessEntityAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService, UtilsService, VRUIUtilsService) {
        var drillDownDefinitions = [];

        function addGenericBusinessEntity(onGenericBEAdded, businessEntityDefinitionId,editorSize,fieldValues) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId,
                fieldValues: fieldValues
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

        function uploadGenericBusinessEntity(businessEntityDefinitionId,editorSize) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId,
            };

            var settings = {
               size: editorSize != undefined ? editorSize : "medium",
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityEditorUploader.html', parameters, settings);
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
                drillDownTab.iconurl = genericBEGridView.Settings.IconPath;

                drillDownTab.loadDirective = function (genericBEViewGridAPI, currentGenericBEEntity) {
                    currentGenericBEEntity.genericBEViewGridAPI = genericBEViewGridAPI;

                    return currentGenericBEEntity.genericBEViewGridAPI.load(buildGenericBEViewPayload());
                };

                function buildGenericBEViewPayload() {
                    var payload = {
                        parentBEEntity: genericBusinessEntity,
                        genericBEGridView: genericBEGridView,
                        businessEntityDefinitionId: businessEntityDefinitionId,
                        genericBusinessEntityId: idFieldType != undefined ? genericBusinessEntity.FieldValues[idFieldType.Name].Value : undefined
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

        function openBulkActionsEditor(viewId, bulkAction, businessEntityDefinitionId,dataRecordTypeId) {
            var parameters = {
                viewId: viewId,
                bulkAction: bulkAction,
                businessEntityDefinitionId: businessEntityDefinitionId,
                dataRecordTypeId: dataRecordTypeId
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityBulkActionsEditor.html', parameters, settings);
        }

        function sendEmailGenericBE(genericBEDefinitionId, businessEntityDefinitionId, genericBEAction) {
            var parameters = {
                genericBEDefinitionId: genericBEDefinitionId,
                businessEntityDefinitionId: businessEntityDefinitionId,
                genericBEAction: genericBEAction
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntitySendEmailEditor.html', parameters, settings);
        }

        function openErrorMessageEditor(errorEntity) {
            var parameters = {
                errorEntity: errorEntity,
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
            };
            VRModalService.showModal('/Client/Modules/VR_GenericData/Views/GenericBusinessEntity/Runtime/GenericBusinessEntityErrorMessageEditor.html', parameters, settings);
        }

        function tryUpdateAllFieldValuesByFieldName(fieldName, fieldValues, allFieldValuesByFieldName) {
            if (allFieldValuesByFieldName == undefined) {
                return false;
            }

            var oldValues = allFieldValuesByFieldName[fieldName];
            if (oldValues == undefined && fieldValues == undefined)
                return false;

            if (!(fieldName in allFieldValuesByFieldName) || fieldValues == undefined || oldValues == undefined || oldValues.length != fieldValues.length) {
                allFieldValuesByFieldName[fieldName] = fieldValues;
                return true;
            }

            for (var i = 0; i < fieldValues.length; i++) {
                var currentValue = fieldValues[i];
                if (oldValues.indexOf(currentValue) == -1) {
                    allFieldValuesByFieldName[fieldName] = fieldValues;
                    return true;
                }
            }

            return false;
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


        return ({
            addGenericBusinessEntity: addGenericBusinessEntity,
            editGenericBusinessEntity: editGenericBusinessEntity,
            defineGenericBEViewTabs: defineGenericBEViewTabs,
            uploadGenericBusinessEntity: uploadGenericBusinessEntity,
            getEntityUniqueName: getEntityUniqueName,
            openBulkActionsEditor: openBulkActionsEditor,
            sendEmailGenericBE: sendEmailGenericBE,
            openErrorMessageEditor: openErrorMessageEditor,
            tryUpdateAllFieldValuesByFieldName: tryUpdateAllFieldValuesByFieldName
            //registerObjectTrackingDrillDownToGenericBusinessEntity: registerObjectTrackingDrillDownToGenericBusinessEntity
            //getDrillDownDefinition: getDrillDownDefinition
        });
    }

    appControllers.service('VR_GenericData_GenericBusinessEntityService', GenericBusinessEntityService);

})(appControllers);