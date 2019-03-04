(function (appControllers) {

    'use strict';

    VRExclusiveSessionTypeService.$inject = ['VRTimerService', 'VRCommon_VRExclusiveSessionTypeAPIService', 'UtilsService'];

    function VRExclusiveSessionTypeService(VRTimerService, VRCommon_VRExclusiveSessionTypeAPIService, UtilsService) {
        return {
            tryTakeSession: tryTakeSession
        };


        function tryTakeSession(scope, sessionTryTakeInput) {
            var promiseDeffered = UtilsService.createPromiseDeferred();
            if (scope.jobIds) {
                VRTimerService.unregisterJobByIds(scope.jobIds);
                scope.jobIds.length = 0;
            }
            var responseObject = {};
            VRCommon_VRExclusiveSessionTypeAPIService.TryTakeSession(sessionTryTakeInput).then(function (response) {
                if (response.IsSucceeded) {
                    var heartbeatIntervalInSeconds = 1;
                    VRCommon_VRExclusiveSessionTypeAPIService.GetSessionLockHeartbeatIntervalInSeconds().then(function (heartbeatResponse) {
                        heartbeatIntervalInSeconds = heartbeatResponse;
                        responseObject.IsSucceeded = response.IsSucceeded;
                        responseObject.Release = releaseSession;
                        VRTimerService.registerJob(onTimerElapsed, scope, heartbeatIntervalInSeconds);
                        promiseDeffered.resolve(responseObject);
                    });
                }
                else
                    promiseDeffered.resolve(response);
            });
            return promiseDeffered.promise;

            function releaseSession() {
                if (scope.jobIds) {
                    VRTimerService.unregisterJobByIds(scope.jobIds);
                    scope.jobIds.length = 0;
                }
                return VRCommon_VRExclusiveSessionTypeAPIService.ReleaseSession(sessionTryTakeInput);
            }

            function onTimerElapsed() {
                return VRCommon_VRExclusiveSessionTypeAPIService.TryKeepSession(sessionTryTakeInput).then(function (response) {
                    if (!response.IsSucceeded) {
                        VRTimerService.unregisterJobByIds(scope.jobIds);
                        scope.jobIds.length = 0;

                        responseObject.onTryTakeFailure(response);
                    }
                    promiseDeffered.resolve(response);
                });
            }
        }
    }
    appControllers.service('VRCommon_VRExclusiveSessionTypeService', VRExclusiveSessionTypeService);

})(appControllers);