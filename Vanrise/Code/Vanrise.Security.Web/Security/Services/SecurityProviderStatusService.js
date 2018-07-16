(function (appControllers) {

    'use strict';

    SecurityProviderStatusService.$inject = ['VRNotificationService', 'VR_Sec_SecurityProviderAPIService', 'VR_GenericData_GenericBEActionService'];

    function SecurityProviderStatusService(VRNotificationService,VR_Sec_SecurityProviderAPIService, VR_GenericData_GenericBEActionService) {

        function changeSecurityProviderStatus() {

            var actionType = {
                ActionTypeName: "SecurityProviderStatus",
                ExecuteAction: function (payload) {
                    if (payload == undefined)
                        return;
                    var genericBusinessEntityId = payload.genericBusinessEntityId;
                    var isEnabled = payload.genericBEAction.Settings.SetEnable;
                    VR_Sec_SecurityProviderAPIService.ChangeSecurityProviderStatus(genericBusinessEntityId, isEnabled).then(function (response) {
                        if (response != undefined)
                        {
                            if(VRNotificationService.notifyOnItemUpdated("Security provider", response, response.Message))
                            {
                                payload.onItemUpdated(response.UpdatedObject);
                            }
                        }

                       
                    });

                }
            };
            VR_GenericData_GenericBEActionService.registerActionType(actionType);
        }

        return ({
            changeSecurityProviderStatus: changeSecurityProviderStatus
        });
    }

    appControllers.service('VR_Sec_SecurityProviderStatusService', SecurityProviderStatusService);

})(appControllers);
