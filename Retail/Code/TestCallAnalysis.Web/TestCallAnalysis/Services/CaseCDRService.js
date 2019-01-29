(function (appControllers) {

    'use strict';

    CaseCDRService.$inject = ['VRModalService'];

    function CaseCDRService(VRModalService) {

        return ({
            changeStatusCaseCDR: changeStatusCaseCDR
        });



        function changeStatusCaseCDR(onGenericBEUpdated, genericBusinessEntityId, businessEntityDefinitionId, editorSize) {
            var parameters = {
                genericBusinessEntityId: genericBusinessEntityId,
                businessEntityDefinitionId: businessEntityDefinitionId
            };

            var settings = {
                size: editorSize != undefined ? editorSize : "medium",
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericBEUpdated = onGenericBEUpdated;
            };
            VRModalService.showModal('/Client/Modules/TestCallAnalysis/Elements/CaseCDRChangeStatus/Views/CaseCDRChangeStatus.html', parameters, settings);
        } 
    }

    appControllers.service('VR_TestCallAnalysis_CaseCDRService', CaseCDRService);

})(appControllers);