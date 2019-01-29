
'use strict';

app.service('VR_TestCallAnalysis_CaseCDRActionService',
    ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService', 'VR_TestCallAnalysis_CaseCDRService', 'VR_GenericData_GenericBEActionService',
        function (VRModalService, UtilsService, VRNotificationService, SecurityService, VR_TestCallAnalysis_CaseCDRService, VR_GenericData_GenericBEActionService) {


            function registerChangeStatusAction() {

                var actionType = {
                    ActionTypeName: "ChangeStatus",
                    ExecuteAction: function (payload) {
                        if (payload == undefined)
                            return;
                        var genericBusinessEntityId = payload.genericBusinessEntityId;
                        var businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        var onItemUpdated = payload.onItemUpdated;
                        var editorSize;
                        var onGenericBEUpdated = function (updatedGenericBE) {
                            if (onItemUpdated != undefined)
                                onItemUpdated(updatedGenericBE);
                        };
                        VR_TestCallAnalysis_CaseCDRService.changeStatusCaseCDR(onGenericBEUpdated, genericBusinessEntityId, businessEntityDefinitionId, editorSize);
                    }
                };
                VR_GenericData_GenericBEActionService.registerActionType(actionType);
            }

            return ({
                registerChangeStatusAction: registerChangeStatusAction
            });
        }
    ]);




