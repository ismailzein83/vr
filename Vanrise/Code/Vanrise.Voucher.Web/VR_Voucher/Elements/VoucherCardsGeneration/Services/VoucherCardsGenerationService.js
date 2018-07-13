(function (appControllers) {

    'use strict';

    VoucherCardsGenerationService.$inject = ['VRModalService'];

    function VoucherCardsGenerationService(VRModalService) {
        
        return ({
            activateVoucherGeneration: activateVoucherGeneration
        });

        

        function activateVoucherGeneration(onGenericBEUpdated, genericBusinessEntityId, editorSize) {
            var parameters = {
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

    
    }
    

    appControllers.service('VR_Voucher_VoucherCardsGenerationService', VoucherCardsGenerationService);

})(appControllers);