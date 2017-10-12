(function (appControllers) {

    'use strict';

    SwitchReleaseCauseService.$inject = ['WhS_BE_SwitchReleaseCauseAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService'];

    function SwitchReleaseCauseService(WhS_BE_SwitchReleaseCauseAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService) {
      
        return ({
            addSwitchReleaseCause: addSwitchReleaseCause,
            editSwitchReleaseCause:editSwitchReleaseCause
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


    }

    appControllers.service('WhS_BE_SwitchReleaseCauseService', SwitchReleaseCauseService);

})(appControllers);
