
(function (appControllers) {

    'use strict';

    GenericBEActionService.$inject = ['VRModalService', 'UtilsService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_GenericBusinessEntityAPIService', 'DeleteOperationResultEnum'];

    function GenericBEActionService(VRModalService, UtilsService, VRNotificationService, VR_GenericData_GenericBusinessEntityService, VR_GenericData_GenericBusinessEntityAPIService, DeleteOperationResultEnum) {

        var actionTypes = [];

        function defineGenericBEMenuActions(businessEntityDefinitionId, genericBusinessEntity, gridAPI, genericBEActions, genericBEGridActions, genericBEGridViews, idFieldType, fieldValues) {

            genericBusinessEntity.menuActions = [];

            if (genericBusinessEntity.AvailableGridActionIds != undefined) {
                for (var j = 0; j < genericBusinessEntity.AvailableGridActionIds.length; j++) {
                    var actionId = genericBusinessEntity.AvailableGridActionIds[j];
                    var genericBEGridAction = UtilsService.getItemByVal(genericBEGridActions, actionId, "GenericBEGridActionId");
                    if (genericBEGridAction != undefined) {
                        var genericBEAction = UtilsService.getItemByVal(genericBEActions, genericBEGridAction.GenericBEActionId, "GenericBEActionId");
                        if (genericBEAction != undefined && genericBEAction.Settings != undefined) {
                            var actionType = getActionTypeIfExist(genericBEAction.Settings.ActionTypeName);
                            if (actionType != undefined) {
                                addGridMenuAction(genericBEAction, actionType, genericBEGridAction, fieldValues);
                            }
                        }
                    }

                }
            }

            function addGridMenuAction(genericBEAction, actionType, genericBEGridAction, fieldValues) {
                genericBusinessEntity.menuActions.push({
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
                                defineGenericBEMenuActions(businessEntityDefinitionId, updatedItem, gridAPI, genericBEActions, genericBEGridActions, genericBEGridViews, idFieldType, fieldValues);
                                gridAPI.itemUpdated(updatedItem);
                            },
                            onItemDeleted: function () {
                                gridAPI.itemDeleted(selectedGenericBusinessEntity);
                            }
                        };
                        var promise = actionType.ExecuteAction(payload);

                              
                        if (promise != undefined && promise.then != undefined) {
						gridAPI.showLoader();
						 var promiseDeffered = UtilsService.createPromiseDeferred();
                            promise.then(function(response) {
                                if(genericBEGridAction.ReloadGridItem && response) {
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
            var editActionType = {
                ActionTypeName: "EditGenericBEAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    var genericBusinessEntityId = payload.genericBusinessEntityId;
                    var onItemUpdated = payload.onItemUpdated;
                    var editorSize = undefined;//payload.editorSize;
                    var fieldValues = payload.fieldValues;
                    var onGenericBEUpdated = function (updatedGenericBE) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedGenericBE);
                    };
                    VR_GenericData_GenericBusinessEntityService.editGenericBusinessEntity(onGenericBEUpdated, businessEntityDefinitionId, genericBusinessEntityId, editorSize, fieldValues);
                }
            };
            registerActionType(editActionType);

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
                        BusinessEntityDefinitionId: businessEntityDefinitionId
                    };
                    var onItemDeleted = payload.onItemDeleted;
                    var deletePromiseDeferred = UtilsService.createPromiseDeferred();
                    VRNotificationService.showConfirmation().then(function (response) {
                        if (response) {
                            VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityEditorRuntime(businessEntityDefinitionId, genericBusinessEntityId).then(function (genericBERuntimeEditor) {
                                if (genericBERuntimeEditor!=undefined) {
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

        return ({
            defineGenericBEMenuActions: defineGenericBEMenuActions,
            getActionTypeIfExist: getActionTypeIfExist,
            registerActionType: registerActionType,
            registerEditAccount: registerEditAccount,
        });
    };

    appControllers.service('VR_GenericData_GenericBEActionService', GenericBEActionService);

})(appControllers);

