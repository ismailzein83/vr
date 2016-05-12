(function (appControllers) {

    "use strict";
    TimerService.$inject = ['UtilsService'];

    function TimerService(UtilsService) {
        var registeredJobs = [];
        var isGettingData = false;
        var currentIndex = 0;
        function registerJob(callToBeExecuted) {
            var job = {
                id: UtilsService.guid(),
                onTimerElapsed: callToBeExecuted
            };
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
        }, 2000);

        function executeJob(job) {
            job.onTimerElapsed().finally(function () {

                currentIndex++;
                if (currentIndex < registeredJobs.length)
                    executeJob(registeredJobs[currentIndex]);
                else
                    isGettingData = false;
            });
        }

        function registerLowFreqJob(callToBeExecuted)
        {
           return registerJob(callToBeExecuted)
        }

        return ({
            registerJob: registerJob,
            unregisterJob: unregisterJob,
            registerLowFreqJob: registerLowFreqJob
        });
    }
    appControllers.service('VRTimerService', TimerService);

})(appControllers);