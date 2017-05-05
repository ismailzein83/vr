"use strict";
VRTileManagementController.$inject = ['$scope', 'VRUIUtilsService', 'VRNavigationService','VR_Sec_ViewAPIService'];

function VRTileManagementController($scope, VRUIUtilsService, VRNavigationService, VR_Sec_ViewAPIService) {
    var viewId;

    var viewEntity;

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
        $scope.scopeModel = {};
        $scope.scopeModel.vrTiles = [];
        //$scope.scopeModel.onPreviewAPIReady = function (api) {
        //    previewDirectiveAPI = api;
        //    var payload = {
        //        viewId: viewId
        //    };
        //    var setLoader = function (value) {
        //        $scope.isLoadingPreviewDirective = value;
        //    };
        //    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, previewDirectiveAPI, payload, setLoader);
        //};
    }

    function load() {
        $scope.scopeModel.isLoading = true;
        getView().then(function () {
            loadAllControls();
        }).catch(function () {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.scopeModel.isLoading = false;
        });
    }

    function loadAllControls() {

        function loadVRTiles() {
            var promises = [];
            console.log(viewEntity);
            return UtilsService.waitMultiplePromises(promises);
        }

        return UtilsService.waitMultipleAsyncOperations([loadVRTiles]).then(function () {
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        });

    }

    function getView() {
        return VR_Sec_ViewAPIService.GetView(viewId).then(function (viewEntityObj) {
            viewEntity = viewEntityObj;
        });
    }
}
appControllers.controller('VRCommon_VRTileManagementController', VRTileManagementController);