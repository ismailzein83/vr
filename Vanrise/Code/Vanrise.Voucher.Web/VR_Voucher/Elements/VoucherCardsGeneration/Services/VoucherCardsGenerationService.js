(function (appControllers) {

    'use strict';

    VoucherCardsGenerationService.$inject = ['VRModalService', 'VRNotificationService'];

    function VoucherCardsGenerationService(VRModalService, VRNotificationService) {
        
        return ({
            activateVoucherGeneration: activateVoucherGeneration
        });

        

        function activateVoucherGeneration(onGenericBEUpdated, businessEntityDefinitionId, genericBusinessEntityId, editorSize) {
            var parameters = {
                businessEntityDefinitionId: businessEntityDefinitionId,
                genericBusinessEntityId: genericBusinessEntityId
            };

            var settings = {
                size: editorSize != undefined ? editorSize : "medium",
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEUpdated = onGenericBEUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_Voucher/Elements/VoucherCardsGeneration/Views/VoucherCardsActivator.html', parameters, settings);
        }

    
    };

    appControllers.service('VR_Voucher_VoucherCardsGenerationService', VoucherCardsGenerationService);

})(appControllers);