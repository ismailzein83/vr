(function (appControllers) {

    "use strict";
    VRUIUtilsService.$inject = ['VRNotificationService', 'UtilsService'];

    function VRUIUtilsService(VRNotificationService, UtilsService) {

        function loadDirective(scope, directiveAPI, loaderProperty) {
            var promise = directiveAPI.load();
            if (promise != undefined) {
                scope[loaderProperty] = true;
                promise.catch(function (error) {
                    VRNotificationService.notifyException(error, scope);
                }).finally(function () {
                    scope[loaderProperty] = false;
                });
            }
        }

        function callDirectiveLoad(directiveAPI, directiveLoadPayload, loadPromiseDeferred) {
            UtilsService.convertToPromiseIfUndefined(directiveAPI.load(directiveLoadPayload))
                .then(function () {
                    if (loadPromiseDeferred != undefined)
                        loadPromiseDeferred.resolve();
                })
                .catch(function (error) {
                    if (loadPromiseDeferred != undefined)
                        loadPromiseDeferred.reject(error);
                });
        }

        function callDirectiveLoadOrResolvePromise(scope, directiveAPI, directiveLoadPayload, setLoader, loadPromiseDeferred) {
            if (loadPromiseDeferred != undefined)
                loadPromiseDeferred.resolve();
            else {
                setLoader(true);
                UtilsService.convertToPromiseIfUndefined(directiveAPI.load(directiveLoadPayload))
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    }).finally(function () {
                        setLoader(false);
                    });
            }
        }

        function getSettingsFromDirective(scope, directiveAPI, templateProperty) {
            if (scope[templateProperty] != undefined) {
                var settings = directiveAPI.getData();
                settings.ConfigId = scope[templateProperty].TemplateConfigID;
                return settings;
            }
            else
                return null;
        }

        return ({
            loadDirective: loadDirective,
            callDirectiveLoad: callDirectiveLoad,
            callDirectiveLoadOrResolvePromise: callDirectiveLoadOrResolvePromise,
            getSettingsFromDirective: getSettingsFromDirective
        });
    }

    appControllers.service('VRUIUtilsService', VRUIUtilsService);
})(appControllers);