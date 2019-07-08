
(function (appControllers) {

    'use strict';

    GenericBECustomActionService.$inject = ['VRModalService', 'UtilsService', 'VRButtonTypeEnum', 'VR_GenericData_GenericBusinessEntityService', 'VR_GenericData_GenericBusinessEntityAPIService', 'DeleteOperationResultEnum', 'VRCommon_ModalWidthEnum', 'VR_GenericData_GenericBEDefinitionAPIService'];

    function GenericBECustomActionService(VRModalService, UtilsService, VRButtonTypeEnum, VR_GenericData_GenericBusinessEntityService, VR_GenericData_GenericBusinessEntityAPIService, DeleteOperationResultEnum, VRCommon_ModalWidthEnum, VR_GenericData_GenericBEDefinitionAPIService) {

        var customActionTypes = [];
        var customActions;

        function getCustomActionTypeIfExist(actionTypeName) {
            for (var i = 0; i < customActionTypes.length; i++) {
                var actionType = customActionTypes[i];
                if (actionType.ActionTypeName == actionTypeName)
                    return actionType;
            }
        }

        function registerCustomActionType(actionType) {
            customActionTypes.push(actionType);
        }
        function registeNewOrExistingCustomAction() {
            var bulkAddCustomAction = {
                ActionTypeName: "NewOrExistingCustomAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var parameters = {
                        dataRecordTypeId: payload.dataRecordTypeId,
                        parentFieldValues: payload.parentFieldValues,
                        customAction: payload.customAction,
                        businessEntityDefinitionId: payload.businessEntityDefinitionId,
                        context: payload.context
                    };
                    var settings = {};
                    if (payload.customAction.Settings != undefined) {
                        var editorEnum = UtilsService.getEnum(VRCommon_ModalWidthEnum, "value", payload.customAction.Settings.EditorSize);
                        settings.size = editorEnum.modalAttr;
                    }
                    var modalPath = "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/CustomActions/Templates/NewOrExistingRuntimeEditor.html";

                    VRModalService.showModal(modalPath, parameters, settings);
                }
            };
            registerCustomActionType(bulkAddCustomAction);
        }
        function registerBulkAddCustomAction() {
            var bulkAddCustomAction = {
                ActionTypeName: "BulkAddCustomAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var parameters = {
                        dataRecordTypeId: payload.dataRecordTypeId,
                        parentFieldValues: payload.parentFieldValues,
                        customAction: payload.customAction,
                        businessEntityDefinitionId: payload.businessEntityDefinitionId
                    };
                    var settings = {};
                    var modalPath = "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/CustomActions/Templates/BulkAddRuntimeEditor.html";

                    VRModalService.showModal(modalPath, parameters, settings);
                }
            };
            registerCustomActionType(bulkAddCustomAction);
        }
        function buildCustomActions(genericBEDefinitionSettings, businessEntityDefinitionId, parentFieldValues, context) {
            customActions = [];
            if (genericBEDefinitionSettings != undefined && genericBEDefinitionSettings.CustomActions != undefined) {
                var actions = genericBEDefinitionSettings.CustomActions;
                var actionsDictionary = {};

                for (var i = 0; i < actions.length; i++) {
                    var customAction = actions[i];
                    var buttonType = UtilsService.getEnum(VRButtonTypeEnum, "value", customAction.ButtonType);
                    if (buttonType != undefined) {
                        if (actionsDictionary[buttonType.value] == undefined) {
                            actionsDictionary[buttonType.value] = [];
                        }
                        actionsDictionary[buttonType.value].push({
                            buttonType: buttonType,
                            customAction: customAction,
                        });
                    }
                }
                buildActionsFromDictionary(actionsDictionary);
            }
            return customActions;
            function buildActionsFromDictionary(actionsDictionary) {
                if (actionsDictionary != undefined) {
                    for (var prop in actionsDictionary) {
                        prepareAction(prop);
                    }
                }
                function prepareAction(prop) {
                    var actionArray = actionsDictionary[prop];
                    if (actionArray != undefined) {
                        var object = actionArray[0];
                        if (actionArray.length > 1) {
                            var menuActions = [];
                            for (var i = 0; i < actionArray.length; i++) {
                                var actionItem = actionArray[i];
                                addMenuAction(actionItem);
                            }
                            function addMenuAction(actionItem) {
                                menuActions.push({
                                    name: actionItem.customAction.Title,
                                    clicked: function () {
                                        return callActionMethod(actionItem.customAction);
                                    },
                                });
                            }
                            addActionToList(object.buttonType, undefined, menuActions);
                        } else {
                            var clickFunc = function () {
                                return callActionMethod(object.customAction);
                            };
                            addActionToList(object.buttonType, clickFunc, undefined);
                        }
                    }
                }
                function addActionToList(buttonType, clickEvent, menuActions) {
                    var type = buttonType != undefined ? buttonType.type : undefined;
                    customActions.push({
                        type: type,
                        onclick: clickEvent,
                        menuActions: menuActions
                    });
                }
            }
            function callActionMethod(customAction) {
                var payload = {
                    customAction: customAction,
                    businessEntityDefinitionId: businessEntityDefinitionId,
                    dataRecordTypeId: genericBEDefinitionSettings.DataRecordTypeId,
                    parentFieldValues: parentFieldValues,
                    context: context
                };
                var actionType = getCustomActionTypeIfExist(customAction.Settings.ActionTypeName);
                if (actionType != undefined)
                    return actionType.ExecuteAction(payload);
            }
        }
        return ({
            getCustomActionTypeIfExist: getCustomActionTypeIfExist,
            registerCustomActionType: registerCustomActionType,
            registerBulkAddCustomAction: registerBulkAddCustomAction,
            buildCustomActions: buildCustomActions,
            registeNewOrExistingCustomAction: registeNewOrExistingCustomAction
        });
    };

    appControllers.service('VR_GenericData_GenericBECustomActionService', GenericBECustomActionService);

})(appControllers);

