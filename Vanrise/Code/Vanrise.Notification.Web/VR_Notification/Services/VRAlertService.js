
(function (appControllers) {

    'use stict';

    VRAlertLevelService.$inject = ['VRModalService'];

    function VRAlertLevelService(VRModalService) {

        function addAlertLevel(onAlertLevelAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAlertLevelAdded = onAlertLevelAdded
            };

            VRModalService.showModal('/Client/Modules/Notification/Views/VRAlertLevel/VRALertLevelEditor.html', null, settings);
        }

        function editAlertLevel(alertLevelId, onAlertLevelUpdated) {
            var settings = {};

            var parameters = {
                alertLevelId: alertLevelId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAlertLevelUpdated = onAlertLevelUpdated;
            };
            VRModalService.showModal('/Client/Modules/Notification/Views/VRAlertLevel/VRALertLevelEditor.html', parameters, settings);
        }


        return {
            addAlertLevel: addAlertLevel,
            editAlertLevel: editAlertLevel
        };
    }

    appControllers.service('VR_Notifictation_AlertLevelService', VRAlertLevelService);

})(appControllers);