"use strict";
dynamicPagePreviewController.$inject = ['$scope', 'VRUIUtilsService', 'VRNavigationService'];

function dynamicPagePreviewController($scope, VRUIUtilsService, VRNavigationService) {
    var viewId;
    var previewDirectiveAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null && parameters.viewId != undefined) {
            viewId = parameters.viewId;
        }
    }

    function defineScope() {
        $scope.onPreviewAPIReady = function (api) {
            previewDirectiveAPI = api;

            var payload = {
                viewId: viewId
            };
            var setLoader = function (value) {
                $scope.isLoadingPreviewDirective = value;
            };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, previewDirectiveAPI, payload, setLoader);

        };
    }

    function load() {

    }
}
appControllers.controller('VR_Sec_DynamicPagePreviewController', dynamicPagePreviewController);