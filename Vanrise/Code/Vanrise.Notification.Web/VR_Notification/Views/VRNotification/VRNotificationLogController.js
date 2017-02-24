(function (appControllers) {

    "use strict";

    VRNotificationLogController.$inject = ['$scope', 'VRCommon_MasterLogAPIService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService'];

    function VRNotificationLogController($scope, VRCommon_MasterLogAPIService, UtilsService, VRNavigationService, VRUIUtilsService) {

        var vrNotificationTypeSettingsSelectorAPI;
        var vrNotificationTypeSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var searchDirectiveAPI;
        var searchDirectiveAPIReadyDeferred = UtilsService.createPromiseDeferred();

        var bodyDirectiveAPI;
        var bodyDirectiveAPIDirectiveAPIReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();
        var viewId;
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }
        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onVRNotificationTypeSettingsSelectorReady = function (api) {
                vrNotificationTypeSettingsSelectorAPI = api;
                vrNotificationTypeSettingsSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSearchDirectiveReady = function (api) {
                searchDirectiveAPI = api;
                searchDirectiveAPIReadyDeferred.resolve();
            };

            $scope.scopeModel.onBodyDirectiveReady = function (api) {
                bodyDirectiveAPI = api;
                bodyDirectiveAPIDirectiveAPIReadyDeferred.resolve();
            };

            $scope.scopeModel.searchClicked = function () {
                var query = buildQuery();
                console.log(query);
                bodyDirectiveAPI.load(query);
            };
        }

        function load() {
            loadAllControls()
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadNotificationTypeSelector])
                  .catch(function (error) {
                      VRNotificationService.notifyExceptionWithClose(error, $scope);
                  })
                 .finally(function () {
                     $scope.isLoading = false;
                 });
        }

        function loadNotificationTypeSelector() {
            var selectorLoadDeferred = UtilsService.createPromiseDeferred();

            vrNotificationTypeSettingsSelectorReadyDeferred.promise.then(function () {

                var selectorPayload = {
                    filter: {
                        Filters: [{
                            $type: "Vanrise.Notification.Business.VRNotificationTypeViewFilter, Vanrise.Notification.Business",
                            ViewId: viewId
                        }]
                    }
                }
                console.log(selectorPayload);
                VRUIUtilsService.callDirectiveLoad(vrNotificationTypeSettingsSelectorAPI, selectorPayload, selectorLoadDeferred);
            });


            return selectorLoadDeferred.promise;
        }

        function loadSearchDirective() {
            var selectorLoadDeferred = UtilsService.createPromiseDeferred();

            searchDirectiveAPIReadyDeferred.promise.then(function () {

                var selectorPayload;
                VRUIUtilsService.callDirectiveLoad(searchDirectiveAPI, selectorPayload, selectorLoadDeferred);
            });


            return selectorLoadDeferred.promise;
        }

        function loadBodyDirective() {
            var selectorLoadDeferred = UtilsService.createPromiseDeferred();

            bodyDirectiveAPIReadyDeferred.promise.then(function () {

                var selectorPayload;
                VRUIUtilsService.callDirectiveLoad(bodyDirectiveAPI, selectorPayload, selectorLoadDeferred);
            });


            return selectorLoadDeferred.promise;
        }

        function buildQuery() {
            return {
                NotificationTypeId: vrNotificationTypeSettingsSelectorAPI.getSelectedIds(),
                SearchQuery: searchDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('VR_Notification_VRNotificationLogController', VRNotificationLogController);
})(appControllers);