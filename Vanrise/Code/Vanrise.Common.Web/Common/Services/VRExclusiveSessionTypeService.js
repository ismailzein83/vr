//(function (appControllers) {

//    'use strict';

//    VRExclusiveSessionTypeService.$inject = ['VRTimerService', 'VRCommon_VRExclusiveSessionTypeAPIService', 'UtilsService'];

//    function VRExclusiveSessionTypeService(VRTimerService, VRCommon_VRExclusiveSessionTypeAPIService, UtilsService) {
//        return {
//            tryTakeSession: tryTakeSession
//        };


//        function tryTakeSession(scope, onTryTakeFailure, sessionTryTakeInput) {
//            var promiseDeffered = UtilsService.createPromiseDeferred();
//            if (scope.job) {
//                VRTimerService.unregisterJob(scope.job);
//            }
//            VRCommon_VRExclusiveSessionTypeAPIService.TryTakeSession(sessionTryTakeInput).then(function (response) {
//                if (response.IsSucceeded) {
//                    var timeIntervalInSeconds = 10;
//                    VRTimerService.registerJob(onTimerElapsed, scope, timeIntervalInSeconds);
//                }
//                promiseDeffered.resolve(response);
//            });
//            return promiseDeffered.promise;

//            function releaseSession(sessionTryTakeInput) {
//                return VRCommon_VRExclusiveSessionTypeAPIService.ReleaseSession(sessionTryTakeInput);
//            }

//            function onTimerElapsed() {
//                var promiseDeffered = UtilsService.createPromiseDeferred();
//                VRCommon_VRExclusiveSessionTypeAPIService.TryKeepSession(sessionTryTakeInput).then(function (response) {
//                    if (!response.IsSucceeded) {
//                        VRTimerService.unregisterJob(scope.job);
//                        var failuerObject = {
//                            Release: releaseSession,
//                            FailureMessage: response.FailureMessage,
//                        }
//                        onTryTakeFailure(failuerObject);
//                    }
//                    promiseDeffered.resolve(response);
//                });
//                return promiseDeffered.promise;
//            }
//        }
//    }
//    appControllers.service('VRCommon_VRExclusiveSessionTypeService', VRExclusiveSessionTypeService);

//})(appControllers);