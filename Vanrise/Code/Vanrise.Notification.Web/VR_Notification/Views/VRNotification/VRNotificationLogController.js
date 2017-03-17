(function (appControllers) {

    "use strict";

    VRNotificationLogController.$inject = ['$scope', 'VRCommon_MasterLogAPIService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService'];

    function VRNotificationLogController($scope, VRCommon_MasterLogAPIService, UtilsService, VRNavigationService, VRUIUtilsService) {

        var viewId;
        var notificationTypeId;

        var vrNotificationTypeSettingsSelectorAPI;
        var vrNotificationTypeSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var searchDirectiveAPI;
        var searchDirectiveAPIReadyDeferred = UtilsService.createPromiseDeferred();

        var bodyDirectiveAPI;
        var bodyDirectiveAPIDirectiveAPIReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isBodyAndSearchLoaded = false;

            $scope.scopeModel.onVRNotificationTypeSettingsSelectorReady = function (api) {
                vrNotificationTypeSettingsSelectorAPI = api;
                vrNotificationTypeSettingsSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onSearchDirectiveReady = function (api) {
                searchDirectiveAPI = api;
                searchDirectiveAPIReadyDeferred.resolve();
            };
            $scope.scopeModel.onBodyDirectiveReady = function (api) {

                console.log("onBodyDirectiveReady-2");

                bodyDirectiveAPI = api;
                bodyDirectiveAPIDirectiveAPIReadyDeferred.resolve();
            };

            $scope.scopeModel.onNotificationTypeSelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    $scope.scopeModel.isBodyAndSearchLoaded = false;
                    $scope.scopeModel.isBodyAndSearchLoaded = true;

                    notificationTypeId = selectedItem.Id;

                    loadSearchDirective();
                    loadBodyDirective();

                    function loadSearchDirective() {
                        var searchDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        var searchDirectivePayload;
                        VRUIUtilsService.callDirectiveLoad(searchDirectiveAPI, searchDirectivePayload, searchDirectiveLoadDeferred);

                        return searchDirectiveLoadDeferred.promise;
                    }
                    function loadBodyDirective() {
                        var bodyDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        var bodyDirectivePayload = {
                            notificationTypeId: notificationTypeId
                        };
                        VRUIUtilsService.callDirectiveLoad(bodyDirectiveAPI, bodyDirectivePayload, bodyDirectiveLoadDeferred);

                        return bodyDirectiveLoadDeferred.promise;
                    }
                }
            };

            $scope.scopeModel.searchClicked = function () {
                var bodyDirectivePayload = {
                    notificationTypeId: notificationTypeId,
                    searchQuery: searchDirectiveAPI.getData()
                };
                buildGridQuery.load(bodyDirectivePayload);
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadNotificationTypeSelector])
                      .catch(function (error) {
                          VRNotificationService.notifyExceptionWithClose(error, $scope);
                      })
                      .finally(function () {
                          $scope.scopeModel.isLoading = false;
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
                };
                VRUIUtilsService.callDirectiveLoad(vrNotificationTypeSettingsSelectorAPI, selectorPayload, selectorLoadDeferred);
            });

            return selectorLoadDeferred.promise;
        }
    }

    appControllers.controller('VR_Notification_VRNotificationLogController', VRNotificationLogController);

})(appControllers);