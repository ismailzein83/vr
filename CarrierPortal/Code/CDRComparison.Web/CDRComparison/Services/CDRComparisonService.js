(function (appControllers) {

    'use strict';

    CDRComparisonService.$inject = ['VRModalService', 'VRNotificationService'];

    function CDRComparisonService(VRModalService, VRNotificationService) {
        function openTimeOffsetHelper(onTimeOffsetSelected) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onTimeOffsetSelected = onTimeOffsetSelected;
            };
           

            VRModalService.showModal("/Client/Modules/CDRComparison/Views/TimeOffsetHelper.html", null, settings);
        }


        return ({
            openTimeOffsetHelper: openTimeOffsetHelper,
        })
    }

    appControllers.service('CDRComparison_CDRComparisonService', CDRComparisonService);

})(appControllers);
