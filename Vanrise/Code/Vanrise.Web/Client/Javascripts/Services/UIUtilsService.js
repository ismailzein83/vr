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

        return ({
            loadDirective: loadDirective
        });
    }

    appControllers.service('VRUIUtilsService', VRUIUtilsService);
})(appControllers);