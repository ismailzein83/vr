(function (appControllers) {

    "use strict";
    TimerService.$inject = ['UtilsService'];

    function TimerService(UtilsService) {
        var registeredJobs = [];
        var isGettingData = false;
        var defaultJobIntervalInSeconds = 2;
        var currentIndex = 0;
        function registerJob(callToBeExecuted, scope, jobIntervalInSeconds) {
            var job = {
                id: UtilsService.guid(),
                onTimerElapsed: callToBeExecuted,
                jobInterval: jobIntervalInSeconds != undefined ? jobIntervalInSeconds : defaultJobIntervalInSeconds,
                lastRun: undefined//new Date()
            };

            if (scope != undefined) {
                scope.job = job;
                scope.$on("$destroy", function () {

                    if (scope.job) {
                        unregisterJob(scope.job);
                    }
                });
            }

            registeredJobs.push(job);
            return job;
        }

        function unregisterJob(job) {
            var index = -1;
            for (var x = 0; x < registeredJobs.length; x++) {
                if (job.id == registeredJobs[x].id) {
                    index = x;
                    break;
                }
            }
            if (index >= 0)
                registeredJobs.splice(index, 1);
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
            var secondsDiff = job.lastRun != undefined ? (new Date().getTime() - job.lastRun.getTime()) / 1000 : 0;
            if (secondsDiff >= job.jobInterval || job.lastRun == undefined) {
                job.onTimerElapsed().finally(function () {
                    job.lastRun = new Date();
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
            unregisterJob: unregisterJob
        });
    }
    appControllers.service('VRTimerService', TimerService);

})(appControllers);