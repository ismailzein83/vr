
(function (appControllers) {

    "use strict";

    VRMailMessageTypeService.$inject = ['VRModalService'];

    function VRMailMessageTypeService(VRModalService) {

        function addMailMessageType(onMailMessageTypeAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onMailMessageTypeAdded = onMailMessageTypeAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageTypeEditor.html', null, settings);
        };

        function editMailMessageType(mailMessageTypeId, onMailMessageTypeUpdated) {
            var settings = {};

            var parameters = {
                mailMessageTypeId: mailMessageTypeId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onMailMessageTypeUpdated = onMailMessageTypeUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRMail/VRMailMessageTypeEditor.html', parameters, settings);
        }

        
        return {
            addMailMessageType: addMailMessageType,
            editMailMessageType: editMailMessageType
        };
    }

    appControllers.service('VRCommon_MailMessageTypeService', VRMailMessageTypeService);

})(appControllers);