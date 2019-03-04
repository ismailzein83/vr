(function (appControllers) {

    "use strict";

    TimerService.$inject = ['UtilsService', 'VRDateTimeService'];

    function TimerService(UtilsService, VRDateTimeService) {

        var registeredJobs = [];
        var isGettingData = false;
        var defaultJobIntervalInSeconds = 2;
        var currentIndex = 0;

        function registerJob(callToBeExecuted, scope, jobIntervalInSeconds) {
            var job = {
                id: UtilsService.guid(),
                onTimerElapsed: callToBeExecuted,
                jobInterval: jobIntervalInSeconds != undefined ? jobIntervalInSeconds : defaultJobIntervalInSeconds,
                lastRun: undefined
            };

            if (scope != undefined) {
                if (scope.jobIds == undefined) {
                    scope.jobIds = [];
                }
                scope.jobIds.push(job.id);

                scope.job = job;
                scope.$on("$destroy", function () {

                    if (scope.jobIds) {
                        unregisterJobByIds(scope.jobIds);
                    }
                });
            }

            registeredJobs.push(job);
            return job;
        }

        function unregisterJobByIds(jobIds) {
            if (jobIds == undefined)
                return;

            for (var jobIndex = 0; jobIndex < jobIds.length; jobIndex++) {
                var currentJobId = jobIds[jobIndex];
                var index = -1;
                for (var x = 0; x < registeredJobs.length; x++) {
                    if (currentJobId == registeredJobs[x].id) {
                        index = x;
                        break;
                    }
                }
                if (index >= 0)
                    registeredJobs.splice(index, 1);
            }
        }

        var timer = setInterval(function () {

            if (!isGettingData && registeredJobs.length > 0) {
                isGettingData = true;
                currentIndex = 0;
                var currentJob = registeredJobs[currentIndex];
                executeJob(currentJob);
            }
        }, 1000);

        function executeJob(job) {
            var secondsDiff = job.lastRun != undefined ? (VRDateTimeService.getNowDateTime().getTime() - job.lastRun.getTime()) / 1000 : 0;
            if (secondsDiff >= job.jobInterval || job.lastRun == undefined) {
                job.onTimerElapsed(job.id).finally(function () {
                    job.lastRun = VRDateTimeService.getNowDateTime();
                    executeNextJob();
                });
            }
            else {
                executeNextJob();
            }
        }

        function executeNextJob() {
            currentIndex++;
            if (currentIndex < registeredJobs.length)
                executeJob(registeredJobs[currentIndex]);
            else
                isGettingData = false;
        }


        return ({
            registerJob: registerJob,
            unregisterJobByIds: unregisterJobByIds
        });
    }
    appControllers.service('VRTimerService', TimerService);

})(appControllers);