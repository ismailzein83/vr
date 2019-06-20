﻿
(function (appControllers) {

    'use strict';

    GenericBEActionService.$inject = ['VRModalService', 'UtilsService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_GenericBusinessEntityAPIService', 'DeleteOperationResultEnum', 'VRCommon_ModalWidthEnum', 'VR_GenericData_GenericBEDefinitionAPIService','VR_GenericData_GenericBEDownloadActionAPIService'];

    function GenericBEActionService(VRModalService, UtilsService, VRNotificationService, VR_GenericData_GenericBusinessEntityService, VR_GenericData_GenericBusinessEntityAPIService, DeleteOperationResultEnum, VRCommon_ModalWidthEnum, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_GenericBEDownloadActionAPIService) {

        var actionTypes = [];

        function defineGenericBEMenuActions(businessEntityDefinitionId, genericBusinessEntity, gridAPI, genericBEActions, genericBEGridActions, genericBEGridActionGroups, genericBEGridViews, idFieldType, fieldValues) {

            genericBusinessEntity.menuActions = [];

            if (genericBusinessEntity.AvailableGridActionIds != undefined) {
                var groupedActionsByGroupId = getGroupedActions(genericBusinessEntity.AvailableGridActionIds);
                for (var i = 0; i < genericBusinessEntity.AvailableGridActionIds.length; i++) {
                    var availableGridActionId = genericBusinessEntity.AvailableGridActionIds[i];
                    var genericBEGridAction = UtilsService.getItemByVal(genericBEGridActions, availableGridActionId, "GenericBEGridActionId");
                    if (genericBEGridAction != undefined) {
                        var groupedActions = groupedActionsByGroupId[genericBEGridAction.GenericBEGridActionGroupId];
                        if (groupedActions != undefined) {
                            if (!groupedActions.isUsed) {
                                groupedActions.isUsed = true;
                                var groupGridAction = UtilsService.getItemByVal(genericBEGridActionGroups, genericBEGridAction.GenericBEGridActionGroupId, "GenericBEGridActionGroupId");
                                genericBusinessEntity.menuActions.push({
                                    name: groupGridAction.Title,
                                    childsactions: groupedActions.childActions
                                });
                            }
                           
                        } else {
                            var genericBEAction = UtilsService.getItemByVal(genericBEActions, genericBEGridAction.GenericBEActionId, "GenericBEActionId");
                            if (genericBEAction != undefined && genericBEAction.Settings != undefined) {
                                var actionType = getActionTypeIfExist(genericBEAction.Settings.ActionTypeName);
                                if (actionType != undefined) {
                                    genericBusinessEntity.menuActions.push(getGridMenuAction(genericBEAction, actionType, genericBEGridAction, fieldValues));
                                }
                            }
                        }
                    } 
                }
            }


            function getGroupedActions(availableGridActionIds) {
                var groupActions = {};
                if (availableGridActionIds != undefined) {
                    for (var j = 0; j < availableGridActionIds.length; j++) {
                        var actionId = availableGridActionIds[j];
                        var genericBEGridAction = UtilsService.getItemByVal(genericBEGridActions, actionId, "GenericBEGridActionId");
                        if (genericBEGridAction != undefined && genericBEGridAction.GenericBEGridActionGroupId != undefined) {
                            if (genericBEGridAction != undefined) {
                                var genericBEAction = UtilsService.getItemByVal(genericBEActions, genericBEGridAction.GenericBEActionId, "GenericBEActionId");
                                var actionType = getActionTypeIfExist(genericBEAction.Settings.ActionTypeName);
                                if (actionType != undefined) {
                                    if (groupActions[genericBEGridAction.GenericBEGridActionGroupId] == undefined)
                                        groupActions[genericBEGridAction.GenericBEGridActionGroupId] = {
                                            isUsed: false,
                                            childActions: []
                                        };
                                    groupActions[genericBEGridAction.GenericBEGridActionGroupId].childActions.push(getGridMenuAction(genericBEAction, actionType, genericBEGridAction, fieldValues));
                                }
                            }
                        }
                    }
                }
                return groupActions;
            }

            function getGridMenuAction(genericBEAction, actionType, genericBEGridAction, fieldValues) {
                return {
                    name: genericBEGridAction.Title,
                    clicked: function (selectedGenericBusinessEntity) {
                        var genericBusinessEntityId;
                        if (idFieldType != undefined) {
                            var fieldValue = selectedGenericBusinessEntity.FieldValues[idFieldType.Name];
                            if (fieldValue != undefined)
                                genericBusinessEntityId = fieldValue.Value;
                        }
                        var payload = {
                            businessEntityDefinitionId: businessEntityDefinitionId,
                            genericBusinessEntityId: genericBusinessEntityId,
                            genericBEAction: genericBEAction,
                            fieldValues: fieldValues,
                            onItemUpdated: function (updatedItem) {
                                VR_GenericData_GenericBusinessEntityService.defineGenericBEViewTabs(businessEntityDefinitionId, updatedItem, gridAPI, genericBEGridViews, idFieldType);
                                defineGenericBEMenuActions(businessEntityDefinitionId, updatedItem, gridAPI, genericBEActions, genericBEGridActions, genericBEGridActionGroups, genericBEGridViews, idFieldType, fieldValues);
                                gridAPI.itemUpdated(updatedItem);
                            },
                            onItemDeleted: function () {
                                gridAPI.itemDeleted(selectedGenericBusinessEntity);
                            },
                            refreshGrid: function () {
                                gridAPI.refreshGrid();
                            },
                        };
                        var promise = actionType.ExecuteAction(payload);


                        if (promise != undefined && promise.then != undefined) {
                            gridAPI.showLoader();
                            var promiseDeffered = UtilsService.createPromiseDeferred();
                            promise.then(function (response) {
                                if (genericBEGridAction.ReloadGridItem && response) {
                                    VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityDetail(genericBusinessEntityId, businessEntityDefinitionId).then(function (response) {
                                        if (payload != undefined)
                                            payload.onItemUpdated(response);
                                        promiseDeffered.resolve();
                                    }).catch(function (error) {
                                        promiseDeffered.reject(error);
                                    });
                                } else {
                                    promiseDeffered.resolve();
                                }
                            }).catch(function (error) {
                                promiseDeffered.reject(error);
                            }).finally(function () {
                                gridAPI.hideLoader();
                            });
                            return promiseDeffered.promise;

                        }

                    }
                };
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

        function registerEditBEAction() {
            var editActionType = {
                ActionTypeName: "EditGenericBEAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var businessEntityDefinitionId = payload.businessEntityDefinitionId;

                    VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
                        var genericBusinessEntityId = payload.genericBusinessEntityId;
                        var onItemUpdated = payload.onItemUpdated;
                        var editorEnum = UtilsService.getEnum(VRCommon_ModalWidthEnum, "value", response.EditorSize);
                        var editorSize = editorEnum != undefined ? editorEnum.modalAttr : undefined;
                        var fieldValues = payload.fieldValues;
                        var onGenericBEUpdated = function (updatedGenericBE, errorEntity) {
                            if (errorEntity != undefined && errorEntity.message != undefined) {
                                VR_GenericData_GenericBusinessEntityService.openErrorMessageEditor(errorEntity);
                            }
                            if (onItemUpdated != undefined)
                                onItemUpdated(updatedGenericBE);
                        };
                        VR_GenericData_GenericBusinessEntityService.editGenericBusinessEntity(onGenericBEUpdated, businessEntityDefinitionId, genericBusinessEntityId, editorSize, fieldValues);
                    });
                }
            };
            registerActionType(editActionType);
        }

        function registerDeleteBEAction() {
            var deleteActionType = {
                ActionTypeName: "DeleteGenericBEAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var genericBusinessEntityId = payload.genericBusinessEntityId;
                    var businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    var genericBusinessEntityIds = [genericBusinessEntityId];
                    var input = {
                        GenericBusinessEntityIds: genericBusinessEntityIds,
                        BusinessEntityDefinitionId: businessEntityDefinitionId,
                        BusinessEntityActionTypeId: payload.genericBEAction.GenericBEActionId
                    };
                    var onItemDeleted = payload.onItemDeleted;
                    var deletePromiseDeferred = UtilsService.createPromiseDeferred();
                    VRNotificationService.showDeleteConfirmation().then(function (response) {
                        if (response) {
                            VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityEditorRuntime(businessEntityDefinitionId, genericBusinessEntityId).then(function (genericBERuntimeEditor) {
                                if (genericBERuntimeEditor != undefined) {
                                    VR_GenericData_GenericBusinessEntityAPIService.DeleteGenericBusinessEntity(input).then(function (result) {
                                        if (result != undefined) {
                                            var deleted = VRNotificationService.notifyOnItemDeleted(genericBERuntimeEditor.TitleFieldName, result);
                                            if (deleted && onItemDeleted != undefined && typeof (onItemDeleted) == 'function') {
                                                deletePromiseDeferred.resolve();
                                                onItemDeleted();
                                            }
                                            else {
                                                deletePromiseDeferred.reject();
                                            }
                                        }
                                        else {
                                            deletePromiseDeferred.reject();
                                        }
                                    });
                                }
                                else {
                                    deletePromiseDeferred.reject();
                                }
                            });
                        }
                        else {
                            deletePromiseDeferred.resolve();
                        }
                    });
                    return deletePromiseDeferred.promise;
                }
            };
            registerActionType(deleteActionType);
        }

        function registerSendEmailGenericBEAction() {
            var sendEmailActionType = {
                ActionTypeName: "SendEmailGenericBEAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var businessEntityDefinitionId = payload.businessEntityDefinitionId; 
                    var genericBusinessEntityId = payload.genericBusinessEntityId;
                    var genericBEAction = payload.genericBEAction;
                    
                    VR_GenericData_GenericBusinessEntityService.sendEmailGenericBE(genericBusinessEntityId, businessEntityDefinitionId, genericBEAction);
                }
            };
            registerActionType(sendEmailActionType);
        }

        function registerDownloadFileGenericBEAction() {
            var sendEmailActionType = {
                ActionTypeName: "DownloadFileGenericBEAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var downloadFileInput = {
                        GenericBusinessEntityId: payload.genericBusinessEntityId,
                        GenericBEAction: payload.genericBEAction,
                        BusinessEntityDefinitionId: payload.businessEntityDefinitionId
                    };
                    VR_GenericData_GenericBEDownloadActionAPIService.DownloadGenericBEFile(downloadFileInput).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
                }
            };
            registerActionType(sendEmailActionType);
        }
        return ({
            defineGenericBEMenuActions: defineGenericBEMenuActions,
            getActionTypeIfExist: getActionTypeIfExist,
            registerActionType: registerActionType,
            registerEditBEAction: registerEditBEAction,
            registerDeleteBEAction: registerDeleteBEAction,
            registerSendEmailGenericBEAction: registerSendEmailGenericBEAction,
            registerDownloadFileGenericBEAction: registerDownloadFileGenericBEAction
        });
    };

    appControllers.service('VR_GenericData_GenericBEActionService', GenericBEActionService);

})(appControllers);

