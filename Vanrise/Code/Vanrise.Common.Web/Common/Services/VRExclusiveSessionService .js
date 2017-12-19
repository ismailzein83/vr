(function (appControllers) {

    'use strict';

    VRExclusiveSessionService.$inject = [ 'VRCommon_VRExclusiveSessionAPIService', 'VRNotificationService', 'UtilsService'];

    function VRExclusiveSessionService(VRCommon_VRExclusiveSessionAPIService, VRNotificationService, UtilsService) {
        return {
            forceRelease: forceRelease,
            forceReleaseAll: forceReleaseAll
        };

        function forceRelease(vrExclusiveSessionId, onVRExclusiveSessionForceRelease) {          

            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    return VRCommon_VRExclusiveSessionAPIService.ForceReleaseSession(vrExclusiveSessionId).then(function (response) {
                        if (onVRExclusiveSessionForceRelease && typeof onVRExclusiveSessionForceRelease == 'function') {
                            onVRExclusiveSessionForceRelease();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }

        function forceReleaseAll(onVRExclusiveSessionForceRelease) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    return VRCommon_VRExclusiveSessionAPIService.ForceReleaseAllSessions().then(function (response) {
                        if (onVRExclusiveSessionForceRelease && typeof onVRExclusiveSessionForceRelease == 'function') {
                            onVRExclusiveSessionForceRelease();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }

    }
    appControllers.service('VRCommon_VRExclusiveSessionService', VRExclusiveSessionService);

})(appControllers);