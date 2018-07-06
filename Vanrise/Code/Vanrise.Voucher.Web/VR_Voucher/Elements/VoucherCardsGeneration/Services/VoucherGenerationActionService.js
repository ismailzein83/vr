'use strict';

app.service('VR_Voucher_VoucherGenerationActionService',
    ['VRModalService', 'VRNotificationService', 'SecurityService', 'VR_Voucher_VoucherCardsGenerationService', 'VR_GenericData_GenericBEActionService',
    function (VRModalService, VRNotificationService, SecurityService, VR_Voucher_VoucherCardsGenerationService, VR_GenericData_GenericBEActionService) {


        function registerActivateAccount() {

            var actionType = {
                ActionTypeName: "ActivateVouchers",
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
                    VR_Voucher_VoucherCardsGenerationService.activateVoucherGeneration(onGenericBEUpdated, businessEntityDefinitionId, genericBusinessEntityId, editorSize);
                }
            };
            VR_GenericData_GenericBEActionService.registerActionType(actionType);
        }
    
        return ({
            registerActivateAccount: registerActivateAccount
        });
    }]);
