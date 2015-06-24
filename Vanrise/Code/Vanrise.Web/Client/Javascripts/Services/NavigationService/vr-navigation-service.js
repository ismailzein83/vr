'use strict';

app.service('VRNavigationService', function ($location, $routeParams, UtilsService) {

    var replaceText = "[[*~^]]";

    return ({
        goto: goto,
        getParameters: getParameters,
        setParameters: setParameters
    });

    function goto(url, parameters) {
        if (parameters != undefined) {
            var serializedParameters = UtilsService.replaceAll(JSON.stringify(parameters), "/", replaceText);
            url = '/viewwithparams' + url + '/' + serializedParameters;
        }
        else
            url = '/view' + url;
        $location.path(url).replace();
    }

    function getParameters(scope) {
        if (scope.modalContext != undefined)//if the view is in Modal Dialog
            return scope.modalContext.parameters;
        else {
            var serializedParams = $routeParams.params;
            if (serializedParams != undefined)
                return JSON.parse(UtilsService.replaceAll(serializedParams, replaceText, "/"));
            else
                return undefined;
        }

    }

    //if the view is in Modal Dialog
    function setParameters(scope, parameters) {
        if (scope.modalContext != undefined)
            scope.modalContext.parameters = parameters;
    }
});

