
(function (appControllers) {

    "use strict";

    VRMailMessageTemplateService.$inject = ['VRModalService'];

    function VRMailMessageTemplateService(VRModalService) {

        function addMailMessageTemplate(onMailMessageTemplateAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onMailMessageTemplateAdded = onMailMessageTemplateAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageTemplateEditor.html', null, settings);
        };

        function editMailMessageTemplate(mailMessageTemplateId, onMailMessageTemplateUpdated) {
            var settings = {};

            var parameters = {
                mailMessageTemplateId: mailMessageTemplateId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onMailMessageTemplateUpdated = onMailMessageTemplateUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageTemplateEditor.html', parameters, settings);
        }


        return {
            addMailMessageTemplate: addMailMessageTemplate,
            editMailMessageTemplate: editMailMessageTemplate
        };
    }

    appControllers.service('VRCommon_VRMailMessageTemplateService', VRMailMessageTemplateService);

})(appControllers);