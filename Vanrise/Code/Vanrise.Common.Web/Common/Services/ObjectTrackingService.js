(function (appControllers) {
    'use strict';

    ObjectTrackingService.$inject = [];

    function ObjectTrackingService() {
        var historyActions = [];
        return ({
            getObjectTrackingGridTitle: getObjectTrackingGridTitle,
            registerActionHistory: registerActionHistory,
            getActionTrackIfExist: getActionTrackIfExist,
            getObjectTrackingGridLocalizedTitle: getObjectTrackingGridLocalizedTitle
        });

        function getObjectTrackingGridTitle() {
            return "History";
        }
        function getObjectTrackingGridLocalizedTitle() {
            return "VRRes.History.VREnd";
        }

        function getActionTrackIfExist(actionHistoryName) {
            for (var i = 0; i < historyActions.length; i++) {
                var actionHistory = historyActions[i];
                
                if (actionHistory.actionHistoryName == actionHistoryName)
                    return actionHistory;
            }
        }
        function registerActionHistory(actionHistory) {
            historyActions.push(actionHistory);
        }
    };

    appControllers.service('VRCommon_ObjectTrackingService', ObjectTrackingService);

})(appControllers);

