
(function (appControllers) {

    'use stict';

    AlertLEvelService.$inject = ['VRModalService'];

    function AlertLEvelService(VRModalService) {

        function addAlertLevel(onAlertLevelAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAlertLevelAdded = onAlertLevelAdded
            };

            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertLevel/VRAlertLevelEditor.html', null, settings);
        }

        function editAlertLevel(alertLevelId, onAlertLevelUpdated) {
            var settings = {};

            var parameters = {
                alertLevelId: alertLevelId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAlertLevelUpdated = onAlertLevelUpdated;
            };
            
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertLevel/VRAlertLevelEditor.html', parameters, settings);
        }


        return {
            addAlertLevel: addAlertLevel,
            editAlertLevel: editAlertLevel
        };
    }

    appControllers.service('VR_Notification_AlertLevelService', AlertLEvelService);

})(appControllers);