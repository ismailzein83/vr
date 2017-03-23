(function (appControllers) {

    "use strict";

    VRNotificationLogController.$inject = ['$scope', 'VRCommon_MasterLogAPIService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService'];

    function VRNotificationLogController($scope, VRCommon_MasterLogAPIService, UtilsService, VRNavigationService, VRUIUtilsService) {

        var viewId;
        var notificationTypeId;

        var notificationTypeSettingsSelectorAPI;
        var notificationTypeSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var notificationStatusSelectorAPI;
        var notificationStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var searchDirectiveAPI;
        var searchDirectiveAPIReadyDeferred = UtilsService.createPromiseDeferred();

        var bodyDirectiveAPI;
        var bodyDirectiveAPIReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.isNotificationTypeSettingSelected = false;

            $scope.scopeModel.onVRNotificationTypeSettingsSelectorReady = function (api) {
                notificationTypeSettingsSelectorAPI = api;
                notificationTypeSettingsSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onNotificationStatusSelectorReady = function (api) {
                notificationStatusSelectorAPI = api;
                notificationStatusSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onSearchDirectiveReady = function (api) {
                searchDirectiveAPI = api;
                searchDirectiveAPIReadyDeferred.resolve();
            };
            $scope.scopeModel.onBodyDirectiveReady = function (api) {
                bodyDirectiveAPI = api;
                bodyDirectiveAPIReadyDeferred.resolve();
            };

            $scope.scopeModel.onNotificationTypeSelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    $scope.scopeModel.isNotificationTypeSettingSelected = true;
                    notificationTypeId = selectedItem.Id;

                    loadSearchDirective();
                    loadBodyDirective();

                    function loadSearchDirective() {
                        var searchDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        searchDirectiveAPIReadyDeferred.promise.then(function () {
                            var searchDirectivePayload = {
                                notificationTypeId: notificationTypeId,
                                context: buildSearchEditorContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(searchDirectiveAPI, searchDirectivePayload, searchDirectiveLoadDeferred);
                        });

                        return searchDirectiveLoadDeferred.promise;
                    }
                    function loadBodyDirective() {
                        var bodyDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        bodyDirectiveAPIReadyDeferred.promise.then(function () {
                            var bodyDirectivePayload = {
                                notificationTypeId: notificationTypeId,
                                query: buildBodyDirectiveQuery()
                            };
                            VRUIUtilsService.callDirectiveLoad(bodyDirectiveAPI, bodyDirectivePayload, bodyDirectiveLoadDeferred);
                        });

                        return bodyDirectiveLoadDeferred.promise;
                    }
                }
            };

            $scope.scopeModel.searchClicked = function () {
                var bodyDirectivePayload = {
                    notificationTypeId: notificationTypeId,
                    query: buildBodyDirectiveQuery(),
                    extendedQuery: searchDirectiveAPI.getData()
                };
                bodyDirectiveAPI.load(bodyDirectivePayload);
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadNotificationTypeSelector, loadNotificationStatusSelector])
                      .catch(function (error) {
                          VRNotificationService.notifyExceptionWithClose(error, $scope);
                      })
                      .finally(function () {
                          $scope.scopeModel.isLoading = false;
                      });
        }
        function loadNotificationTypeSelector() {
            var notificationTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            notificationTypeSettingsSelectorReadyDeferred.promise.then(function () {

                var notificationTypeSelectorPayload = {
                    filter: {
                        Filters: [{
                            $type: "Vanrise.Notification.Business.VRNotificationTypeViewFilter, Vanrise.Notification.Business",
                            ViewId: viewId
                        }]
                    }
                };
                VRUIUtilsService.callDirectiveLoad(notificationTypeSettingsSelectorAPI, notificationTypeSelectorPayload, notificationTypeSelectorLoadDeferred);
            });

            return notificationTypeSelectorLoadDeferred.promise;
        }
        function loadNotificationStatusSelector() {
            var notificationStatusSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            notificationStatusSelectorReadyDeferred.promise.then(function () {

                var notificationStatusSelectorPayload;
                VRUIUtilsService.callDirectiveLoad(notificationStatusSelectorAPI, notificationStatusSelectorPayload, notificationStatusSelectorLoadDeferred);
            });

            return notificationStatusSelectorLoadDeferred.promise;
        }

        function buildSearchEditorContext() {
            var context = {
                isNotificationTypeSettingSelected: function () {
                    return $scope.scopeModel.isNotificationTypeSettingSelected;
                },
                isAdvancedTabSelected: function () {
                    return $scope.advancedSelected;
                }
            }
            return context;
        }

        function buildBodyDirectiveQuery() {
            return {
                Description: $scope.scopeModel.description,
                StatusIds: notificationStatusSelectorAPI.getSelectedIds(),
                AlertLevelIds: undefined
            };
        }
    }

    appControllers.controller('VR_Notification_VRNotificationLogController', VRNotificationLogController);

})(appControllers);