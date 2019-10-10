
"use strict";

//PromiseDebugService.$inject = [];

function PromiseDebugService() {

    var registeredPromises = [];

    function registerPromise(promiseObj) {
        var promiseDebuggerState = getDebuggerState();
        if (promiseDebuggerState == null || !promiseDebuggerState || promiseDebuggerState == "false")
            return;

        if (promiseObj.name == undefined) {
            var createPromiseLocation = getCreatePromiseLocation();
            if (createPromiseLocation == undefined)
                return;

            promiseObj.name = "Promise(" + createPromiseLocation + ")";
        }

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

    function enableDebugger() {
        sessionStorage.setItem("VRPromiseDebuggerState", true);
        return "Promise Debugger Enabled! Check Verbose/Debug Message Level to monitor promises.";
    }

    function disableDebugger() {
        sessionStorage.setItem("VRPromiseDebuggerState", false);
        return "Promise Debugger Disabled!";
    }

    function disableLoader() {
        $('.loading-circles').css({display: 'none'});
        $('.divLoading').removeClass('divLoading');
        return "Loader Disabled!";
    }

    function getDebuggerState() {
        return sessionStorage.getItem("VRPromiseDebuggerState");
    }

    function getCreatePromiseLocation() {
        var stackString = Error().stack.toString();
        var indexOfModulePath = stackString.search("/Modules/");
        if (indexOfModulePath == -1)
            return undefined;

        var modulePath = stackString.substring(indexOfModulePath);
        var indexOfParantheseOrNewLine = modulePath.indexOf(")");  // For Chrome
        if (indexOfParantheseOrNewLine == -1) {
            indexOfParantheseOrNewLine = modulePath.indexOf("\n"); // For Firefox
            if (indexOfParantheseOrNewLine == -1)
                return undefined;
        }

        modulePath = modulePath.substring(0, indexOfParantheseOrNewLine);
        var lastIndexOfSlash = modulePath.lastIndexOf("/");
        var lastIndexOf2Points = modulePath.lastIndexOf(":");

        if (lastIndexOfSlash == -1 || lastIndexOf2Points == -1)
            return undefined;

        modulePath = modulePath.substring(lastIndexOfSlash + 1, lastIndexOf2Points);
        return modulePath;
    }

    //function getCreatePromiseLocation() {
    //    var stackString = Error().stack.toString();
    //    var indexOfModulePath = stackString.search("/Modules/");
    //    if (indexOfModulePath == -1)
    //        return undefined;

    //    var indexOfStartParanth = getHTTPIndex(stackString, indexOfModulePath);

    //    var modulePath = stackString.substring(indexOfStartParanth);
    //    var indexOfParantheseOrNewLine = modulePath.indexOf(")");  // For Chrome
    //    if (indexOfParantheseOrNewLine == -1) {
    //        indexOfParantheseOrNewLine = modulePath.indexOf("\n"); // For Firefox
    //        if (indexOfParantheseOrNewLine == -1)
    //            return undefined;
    //    }

    //    modulePath = modulePath.substring(0, indexOfParantheseOrNewLine);

    //    return modulePath;
    //}

    //function getHTTPIndex(stackString, index) {
    //    if (stackString.length < index) {
    //        return undefined;
    //    }

    //    for (var i = index; i > 1; i--) {
    //        if (stackString[i] == "(" || stackString[i] == "@" || (stackString[i] == "t" && stackString[i - 1] == "a"))
    //            return i + 1;
    //    }

    //    return undefined;
    //}

    function registredVerbose(registredPromiseName, color) {
        console.debug("Registred: %c" + registredPromiseName, color);
    }

    function resolvedVerbose(resolvedPromisesName, pendingPromisesName, resolvedColor, pendingColor) {
        console.debug("Resolved: %c" + resolvedPromisesName + "  %c|  Pending: %c" + pendingPromisesName.join(", "), resolvedColor, "color: black", pendingColor);
    }

    function allResolvedVerbose(lastResolvedPromiseName, color) {
        console.debug("%cResolved: " + lastResolvedPromiseName + "  |  No Pending Promises", color);
    }

    return ({
        registerPromise: registerPromise,
        enableDebugger: enableDebugger,
        disableDebugger: disableDebugger,
        disableLoader: disableLoader
    });
}

appControllers.service('VRPromiseDebugService', PromiseDebugService);