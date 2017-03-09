(function (appControllers) {
    'use strict';

    ObjectTrackingService.$inject = [];

    function ObjectTrackingService() {
        return ({
            getObjectTrackingGridTitle: getObjectTrackingGridTitle
        });

        function getObjectTrackingGridTitle() {
            return "History";
        }
    };

    appControllers.service('VRCommon_ObjectTrackingService', ObjectTrackingService);

})(appControllers);

