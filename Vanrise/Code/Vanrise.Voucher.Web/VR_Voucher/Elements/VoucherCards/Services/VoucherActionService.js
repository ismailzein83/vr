'use strict';

app.service('VR_Voucher_VoucherActionService',
    ['VRModalService', 'VRNotificationService', 'SecurityService', 'VR_Voucher_VoucherCardsAPIService', 'VR_GenericData_GenericBEActionService', 'UtilsService',
    function (VRModalService, VRNotificationService, SecurityService, VR_Voucher_VoucherCardsAPIService, VR_GenericData_GenericBEActionService, UtilsService) {


        function registerUnlockAction() {

            var actionType = {
                ActionTypeName: "UnlockVoucher",
                ExecuteAction: function (payload) {
                    var promiseDeffered = UtilsService.createPromiseDeferred();
                    if (payload == undefined)
                        return;
                    var genericBusinessEntityId = payload.genericBusinessEntityId;
                    var onItemUpdated = payload.onItemUpdated;
                    var editorSize;//payload.editorSize;
                    var onGenericBEUpdated = function (updatedGenericBE) {
                        if (onItemUpdated != undefined)
                            onItemUpdated(updatedGenericBE);
                    };
                    VRNotificationService.showConfirmation().then(function (response) {
                        if (response) {
                            VR_Voucher_VoucherCardsAPIService.UnlockVoucher(genericBusinessEntityId).then(function (response) {
                                promiseDeffered.resolve(response);
                            });
                        } else {
                            promiseDeffered.resolve(response);
                        }
                    });
                    return promiseDeffered.promise;
                }

            };

            
            VR_GenericData_GenericBEActionService.registerActionType(actionType);
        }
    
        return ({
            registerUnlockAction: registerUnlockAction
        });
    }]);