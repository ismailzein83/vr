'use strict';

app.service('VR_Voucher_VoucherGenerationActionService',
    ['VRModalService', 'UtilsService', 'VRNotificationService', 'SecurityService', 'VR_Voucher_VoucherCardsGenerationService', 'VR_GenericData_GenericBEActionService',
    function (VRModalService, UtilsService,VRNotificationService, SecurityService, VR_Voucher_VoucherCardsGenerationService, VR_GenericData_GenericBEActionService) {


        function registerActivateAction() {

            var actionType = {
                ActionTypeName: "ActivateVouchers",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var genericBusinessEntityId = payload.genericBusinessEntityId;
                    var onItemUpdated = payload.onItemUpdated;
                    var editorSize;//payload.editorSize;
                    var onGenericBEUpdated = function (updatedGenericBE) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedGenericBE);
                    };
                    VR_Voucher_VoucherCardsGenerationService.activateVoucherGeneration(onGenericBEUpdated, genericBusinessEntityId, editorSize);
                }
            };
            VR_GenericData_GenericBEActionService.registerActionType(actionType);
        }
    
        return ({
            registerActivateAction: registerActivateAction
        });
    }]);
