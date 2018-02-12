'use strict';

app.service('VR_GenericData_GenericBEActionService',
    ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_GenericBusinessEntityAPIService',
    function (VRModalService, UtilsService, VRNotificationService, SecurityService, VR_GenericData_GenericBusinessEntityService, VR_GenericData_GenericBusinessEntityAPIService) {

        var actionTypes = [];

        function defineGenericBEMenuActions(businessEntityDefinitionId, genericBusinessEntity, gridAPI, genericBEActions, genericBEGridActions,genericBEGridViews, idFieldType) {

            genericBusinessEntity.menuActions = [];

            if (genericBusinessEntity.AvailableGridActionIds != undefined) {
                for (var j = 0; j < genericBusinessEntity.AvailableGridActionIds.length; j++) {
                    var actionId = genericBusinessEntity.AvailableGridActionIds[j];
                    var genericBEGridAction = UtilsService.getItemByVal(genericBEGridActions, actionId, "GenericBEGridActionId");
                    if (genericBEGridAction != undefined)
                    {
                        var genericBEAction = UtilsService.getItemByVal(genericBEActions, genericBEGridAction.GenericBEActionId, "GenericBEActionId");
                        if (genericBEAction != undefined && genericBEAction.Settings != undefined) {
                            var actionType = getActionTypeIfExist(genericBEAction.Settings.ActionTypeName);
                            if (actionType != undefined) {
                                addGridMenuAction(genericBEAction, actionType, genericBEGridAction);
                            }
                        }
                    }
                   
                }
            }

            function addGridMenuAction(genericBEAction, actionType, genericBEGridAction) {
                genericBusinessEntity.menuActions.push({
                    name: genericBEGridAction.Title,
                    clicked: function (selectedGenericBusinessEntity) {
                        var genericBusinessEntityId;
                        if (idFieldType != undefined)
                        {
                            var fieldValue = selectedGenericBusinessEntity.FieldValues[idFieldType.Name];
                            if (fieldValue != undefined)
                                genericBusinessEntityId = fieldValue.Value;
                        }
                        var payload = {
                            businessEntityDefinitionId: businessEntityDefinitionId,
                            genericBusinessEntityId: genericBusinessEntityId,
                            genericBEAction: genericBEAction,
                            onItemUpdated: function (updatedItem) {
                                VR_GenericData_GenericBusinessEntityService.defineGenericBEViewTabs(businessEntityDefinitionId, updatedItem, gridAPI, genericBEGridViews, idFieldType);
                                defineGenericBEMenuActions(businessEntityDefinitionId, updatedItem, gridAPI, genericBEActions, genericBEGridActions, genericBEGridViews, idFieldType);
                                gridAPI.itemUpdated(updatedItem);
                            }
                        };
                        var promise = actionType.ExecuteAction(payload);
                        //VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId).then(function (response) {

                        //});
                    }
                });
            }
        }

        function getActionTypeIfExist(actionTypeName) {
            for (var i = 0; i < actionTypes.length; i++) {
                var actionType = actionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }
        function registerActionType(actionType) {
            actionTypes.push(actionType);
        }

        function registerEditAccount() {

            var actionType = {
                ActionTypeName: "EditGenericBEAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    var genericBusinessEntityId = payload.genericBusinessEntityId;
                    var onItemUpdated = payload.onItemUpdated;
                    var editorSize = undefined;//payload.editorSize;
                    var onGenericBEUpdated = function (updatedGenericBE) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedGenericBE);
                    };
                    VR_GenericData_GenericBusinessEntityService.editGenericBusinessEntity(onGenericBEUpdated, businessEntityDefinitionId, genericBusinessEntityId, editorSize);
                }
            };
            registerActionType(actionType);
        }
    
        return ({
            defineGenericBEMenuActions: defineGenericBEMenuActions,
            getActionTypeIfExist: getActionTypeIfExist,
            registerActionType: registerActionType,
            registerEditAccount: registerEditAccount,
        });
    }]);
