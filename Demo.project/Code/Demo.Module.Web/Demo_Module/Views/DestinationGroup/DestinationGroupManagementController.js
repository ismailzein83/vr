(function (appControllers) {

    "use strict";

    destinationGroupManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'Demo_DestinationGroupService', 'VRValidationService', 'Demo_DestinationGroupAPIService'];

    function destinationGroupManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, Demo_DestinationGroupService, VRValidationService, Demo_DestinationGroupAPIService) {
        var gridAPI;
        var destinationGroupDirectiveAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.groupTypeTemplates = [];
            $scope.selectedGroupTypeTemplates = [];

            $scope.validateDateRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewDestinationGroup = AddNewDestinationGroup;

            function getFilterObject() {

                var selectedGroupTypeTemplateIds = [];

                angular.forEach($scope.selectedGroupTypeTemplates, function (item) {
                    selectedGroupTypeTemplateIds.push(item.TemplateConfigID);
                });

                var data = {
                    DestinationTypes: selectedGroupTypeTemplateIds
                };

                return data;
            }
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDestinationTypes])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }

        function loadDestinationTypes() {
            return Demo_DestinationGroupAPIService.GetGroupTypeTemplates().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.groupTypeTemplates.push(item);
                });
            });
        }


        function AddNewDestinationGroup() {
            var onDestinationGroupAdded = function (destinationGroupObj) {
                gridAPI.onDestinationGroupAdded(destinationGroupObj);
            };

            Demo_DestinationGroupService.addDestinationGroup(onDestinationGroupAdded);
        }

    }

    appControllers.controller('Demo_DestinationGroupManagementController', destinationGroupManagementController);
})(appControllers);