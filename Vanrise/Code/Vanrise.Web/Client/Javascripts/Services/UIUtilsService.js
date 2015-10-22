(function (appControllers) {

    "use strict";
    VRUIUtilsService.$inject = ['VRNotificationService'];

    function VRUIUtilsService(VRNotificationService) {

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
            getSettingsFromDirective: getSettingsFromDirective
        });
    }

    appControllers.service('VRUIUtilsService', VRUIUtilsService);
})(appControllers);