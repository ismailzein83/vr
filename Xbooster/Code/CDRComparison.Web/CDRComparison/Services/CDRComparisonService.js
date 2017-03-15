(function (appControllers) {

    'use strict';

    CDRComparisonService.$inject = ['VRModalService', 'VRNotificationService'];

    function CDRComparisonService(VRModalService, VRNotificationService) {
        function openTimeOffsetHelper(onTimeOffsetSelected, tableKey) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onTimeOffsetSelected = onTimeOffsetSelected;
            };
            var parameters = {
                tableKey: tableKey
            };

            VRModalService.showModal("/Client/Modules/CDRComparison/Views/TimeOffsetHelper.html", parameters, settings);
        }


        return ({
            openTimeOffsetHelper: openTimeOffsetHelper
        });
    }

    appControllers.service('CDRComparison_CDRComparisonService', CDRComparisonService);

})(appControllers);
