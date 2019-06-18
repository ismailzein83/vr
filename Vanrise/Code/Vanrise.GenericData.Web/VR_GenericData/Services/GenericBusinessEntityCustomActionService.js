
//(function (appControllers) {

//    'use strict';

//    GenericBECustomActionService.$inject = ['VRModalService', 'UtilsService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_GenericBusinessEntityAPIService', 'DeleteOperationResultEnum', 'VRCommon_ModalWidthEnum', 'VR_GenericData_GenericBEDefinitionAPIService'];

//    function GenericBECustomActionService(VRModalService, UtilsService, VRNotificationService, VR_GenericData_GenericBusinessEntityService, VR_GenericData_GenericBusinessEntityAPIService, DeleteOperationResultEnum, VRCommon_ModalWidthEnum, VR_GenericData_GenericBEDefinitionAPIService) {

//        var customActionTypes = [];

//        function defineGenericBECustomActions(businessEntityDefinitionId, genericBusinessEntitySettings, gridAPI, genericBECustomActions, parentFieldValues, dataRecordTypeId) {

//            genericBusinessEntity.customActions = [];

//            //if (genericBECustomActions != undefined) {
//            //    for (var i = 0; i < genericBECustomActions.length; i++) {
//            //        var genericBECustomAction = genericBECustomActions[i];
//            //        if (genericBECustomAction != undefined) {

//            //            if (genericBECustomAction.Settings != undefined) {
//            //                var actionType = getCustomActionTypeIfExist(genericBECustomAction.Settings.ActionTypeName);
//            //                if (actionType != undefined) {
//            //                    genericBusinessEntity.customActions.push(getCustomAction(genericBECustomAction, actionType, parentFieldValues));
//            //                }
//            //            }
//            //        }
//            //    }
//            //}

//            function getCustomAction(genericBECustomAction, actionType, parentFieldValues) {
//                return {
//                    name: genericBEGridAction.Title,
//                    clicked: function () {
//                        var parameters = {
//                            businessEntityDefinitionId: businessEntityDefinitionId,
//                            genericBECustomActionSettings: genericBECustomAction.Settings,
//                            parentFieldValues: parentFieldValues,
//                            dataRecordTypeId: dataRecordTypeId
//                        };

//                        //settings.onScopeReady = function (modalScope) {
//                        //    modalScope.onGenericBEColumnDefinitionUpdated = onGenericBEColumnDefinitionUpdated;
//                        //};
                  
//                        //var promise = actionType.ExecuteAction(payload);

//                        //if (promise != undefined && promise.then != undefined) {
//                        //    gridAPI.showLoader();
//                        //    var promiseDeffered = UtilsService.createPromiseDeferred();
//                        //    promise.then(function () {
//                        //        promiseDeffered.resolve();
//                        //    }).catch(function (error) {
//                        //        promiseDeffered.reject(error);
//                        //    }).finally(function () {
//                        //        gridAPI.hideLoader();
//                        //    });
//                        //    return promiseDeffered.promise;
//                        //}
//                    }
//                };
//            }
//        }

//        function getCustomActionTypeIfExist(actionTypeName) {
//            for (var i = 0; i < customActionTypes.length; i++) {
//                var actionType = customActionTypes[i];
//                if (actionType.ActionTypeName == actionTypeName)
//                    return actionType;
//            }
//        }

//        function registerCustomActionType(actionType) {
//            customActionTypes.push(actionType);
//        }

//        function registerBulkAddCustomAction() {
//            var bulkAddCustomAction = {
//                ActionTypeName: "BulkAddCustomAction",
//                ExecuteAction: function (payload) {
//                    if (payload == undefined)
//                        return;
//                    var settings = {};

//                    var modalPath = "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/CustomActions/Templates/BulkAddRuntimeEditor.html";

//                    VRModalService.showModal(modalPath, payload, settings);
//                    //VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
//                    //    var genericBusinessEntityId = payload.genericBusinessEntityId;
//                    //    var onItemUpdated = payload.onItemUpdated;
//                    //    var editorEnum = UtilsService.getEnum(VRCommon_ModalWidthEnum, "value", response.EditorSize);
//                    //    var editorSize = editorEnum != undefined ? editorEnum.modalAttr : undefined;
//                    //    var fieldValues = payload.fieldValues;
//                    //    var onGenericBEUpdated = function (updatedGenericBE, errorEntity) {
//                    //        if (errorEntity != undefined && errorEntity.message != undefined) {
//                    //            VR_GenericData_GenericBusinessEntityService.openErrorMessageEditor(errorEntity);
//                    //        }
//                    //        if (onItemUpdated != undefined)
//                    //            onItemUpdated(updatedGenericBE);
//                    //    };
//                    //    VR_GenericData_GenericBusinessEntityService.editGenericBusinessEntity(onGenericBEUpdated, businessEntityDefinitionId, genericBusinessEntityId, editorSize, fieldValues);
//                    //});
//                }
//            };
//            registerCustomActionType(bulkAddCustomAction);
//        }

//        return ({
//            defineGenericBECustomActions: defineGenericBECustomActions,
//            getCustomActionTypeIfExist: getCustomActionTypeIfExist,
//            registerCustomActionType: registerCustomActionType,
//            registerBulkAddCustomAction: registerBulkAddCustomAction,
//        });
//    };

//    appControllers.service('VR_GenericData_GenericBECustomActionService', GenericBECustomActionService);

//})(appControllers);

