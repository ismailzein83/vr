(function (appControllers) {

    "use strict";

    HttpProxyMethodService.$inject = ['VRModalService'];
    function HttpProxyMethodService(VRModalService) {

        function addHttpProxyMethod(onHttpProxyMethodAdded, context) {

            var parameters = {
                context: context
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onHttpProxyMethodAdded = onHttpProxyMethodAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Directives/VRNamespace/MainExtensions/Templates/HttpProxyMethodEditor.html', parameters, settings);
        }

        function editHttpProxyMethod(onHttpProxyMethodUpdated, httpProxyMethodEntity, context) {

            var parameters = {
                httpProxyMethodEntity: httpProxyMethodEntity,
                context: context};

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onHttpProxyMethodUpdated = onHttpProxyMethodUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Directives/VRNamespace/MainExtensions/Templates/HttpProxyMethodEditor.html', parameters, settings);
        }

        return {
            addHttpProxyMethod: addHttpProxyMethod,
            editHttpProxyMethod: editHttpProxyMethod
        };
    }

    appControllers.service('VRCommon_HttpProxyMethodService', HttpProxyMethodService);

})(appControllers);