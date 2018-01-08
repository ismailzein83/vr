(function (appControllers) {

    'use strict';

    SwitchReleaseCauseService.$inject = ['WhS_BE_SwitchReleaseCauseAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function SwitchReleaseCauseService(WhS_BE_SwitchReleaseCauseAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
      
        return ({
            addSwitchReleaseCause: addSwitchReleaseCause,
            editSwitchReleaseCause: editSwitchReleaseCause,
            uploadSwitchReleaseCause: uploadSwitchReleaseCause,
            openReleaseCodeDescriptions: openReleaseCodeDescriptions,
            getEntityUniqueName: getEntityUniqueName
        });

        function addSwitchReleaseCause(onSwitchReleaseCauseAdded) {
            var settings = {
            };

            var parameters = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchReleaseCauseAdded = onSwitchReleaseCauseAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SwitchReleaseCause/SwitchReleaseCauseEditor.html', parameters, settings);
        }
        function uploadSwitchReleaseCause() {
            var settings = {
            };
            var parameters = {
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SwitchReleaseCause/UploadSwitchReleaseCauseEditor.html', parameters, settings);
        }
        function editSwitchReleaseCause(switchReleaseCauseId, onSwitchReleaseCauseUpdated) {
            var modalSettings = {
            };
            var parameters = {
                switchReleaseCauseId: switchReleaseCauseId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSwitchReleaseCauseUpdated = onSwitchReleaseCauseUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SwitchReleaseCause/SwitchReleaseCauseEditor.html', parameters, modalSettings);
        }
        function getEntityUniqueName() {
            return "WhS_BE_SwitchReleaseCause";
        }
        function openReleaseCodeDescriptions(code, switchIds) {
            var modalSettings = {
                autoclose: true
            };
            var parameters = {
                code: code,
                switchIds: switchIds
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ReleaseCodeDescription/ReleaseCodeDescriptionViewer.html', parameters, modalSettings);
        }
    }

    appControllers.service('WhS_BE_SwitchReleaseCauseService', SwitchReleaseCauseService);

})(appControllers);
