(function (appControllers) {

    "use strict";

    //PromiseDebugService.$inject = [];

    function PromiseDebugService() {

        var registeredPromises = [];

        function registerPromise(promiseObj) {

            registeredPromises.push(promiseObj);

            registredVerbose(promiseObj.name, "color: blue");

            promiseObj.promise.then(function () {
                var pendingPromisesName = [];
                var resolvedPromises = [];
                var pendingPromises = [];

                if (registeredPromises.length > 0) {
                    for (var i = 0; i < registeredPromises.length; i++) {
                        var currentPromiseObj = registeredPromises[i];
                        if (currentPromiseObj.promise["$$state"].status == 0) {
                            pendingPromisesName.push(currentPromiseObj.name);
                            pendingPromises.push(currentPromiseObj);
                        }
                        else {
                            resolvedPromises.push(currentPromiseObj.name);
                        }
                    }
                }

                registeredPromises = pendingPromises;

                if (pendingPromisesName.length == 0) {
                    allResolvedVerbose(resolvedPromises, "color: green");
                }
                else {
                    resolvedVerbose(resolvedPromises, pendingPromisesName, "color: green", "color: red");
                }
            });
        }

        function registredVerbose(registredPromiseName, color) {
            console.debug("Registred: %c" + registredPromiseName, color);
        }

        function resolvedVerbose(resolvedPromisesName, pendingPromisesName, resolvedColor, pendingColor) {
            console.debug("Resolved: %c" + resolvedPromisesName + "  %c|  Pending: %c" + pendingPromisesName, resolvedColor, "color: black", pendingColor);
        }

        function allResolvedVerbose(lastResolvedPromiseName, color) {
            console.debug("%cResolved: " + lastResolvedPromiseName + "  |  No Pending Promises", color);
        }

        return ({
            registerPromise: registerPromise
        });
    }
    appControllers.service('VRPromiseDebugService', PromiseDebugService);

})(appControllers);