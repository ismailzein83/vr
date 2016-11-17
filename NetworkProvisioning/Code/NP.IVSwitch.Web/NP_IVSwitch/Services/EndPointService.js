
(function (appControllers) {

    "use strict";

    EndPointService.$inject = ['VRModalService'];

    function EndPointService(NPModalService) {

        function addEndPoint(onEndPointAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onEndPointAdded = onEndPointAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/EndPoint/EndPointEditor.html', null, settings);
        };
        function editEndPoint(EndPointId, onEndPointUpdated) {
            var settings = {};

            var parameters = {
                EndPointId: EndPointId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onEndPointUpdated = onEndPointUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/EndPoint/EndPointEditor.html', parameters, settings);
        }


        return {
            addEndPoint: addEndPoint,
            editEndPoint: editEndPoint
        };
    }

    appControllers.service('NP_IVSwitch_EndPointService', EndPointService);

})(appControllers);