(function (appControllers) {

    "use strict";

    settingsManagementController.$inject = ['$scope', 'VRCommon_SettingsAPIService', 'UtilsService', 'VRNotificationService'];

    function settingsManagementController($scope, VRCommon_SettingsAPIService, UtilsService, VRNotificationService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.settingCategories = [];
            $scope.searchClicked = function () {
                var promises = [];
                for (var x = 0; x < $scope.settingCategories.length; x++) {
                    var currentItem = $scope.settingCategories[x];
                    if (currentItem.api != undefined) {
                        promises.push(search(currentItem));
                    }
                }
                return UtilsService.waitMultiplePromises(promises);
            };
        }

        function load() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadSettingsCategories]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadSettingsCategories() {
            return VRCommon_SettingsAPIService.GetDistinctSettingCategories().then(function (response) {
                $scope.settingCategories.length = 0;
                for (var x = 0; x < response.length; x++) {
                    var currentItem = response[x];
                    buildSettingCategory(currentItem);
                }
            });
        }

        var search = function (currentItem) {
            return currentItem.api.loadGrid(getFilterObject(currentItem.settingName));
        };

        var buildSettingCategory = function (currentItem) {
            var settingCategory = {
                settingName: currentItem,
                onGridReady: function (api) {
                    settingCategory.api = api;
                    api.loadGrid(getFilterObject(currentItem));
                }
            };
            $scope.settingCategories.push(settingCategory);
        };

        function getFilterObject(category) {
            return {
                Name: $scope.name,
                Category: category
            };
        }
    }

    appControllers.controller('VRCommon_SettingsManagementController', settingsManagementController);
})(appControllers);