DataSourceManagementController.$inject = ['$scope', 'VR_Integration_DataSourceAPIService', 'VR_Integration_DataSourceService', "UtilsService", 'VRNotificationService'];

function DataSourceManagementController($scope, VR_Integration_DataSourceAPIService, VR_Integration_DataSourceService, UtilsService, VRNotificationService) {
    var gridApi;

    defineScope();
    load();

    function defineScope() {
        $scope.adapterTypes = [];
        $scope.selectedAdapterTypes = [];
        $scope.statuses = [
            { value: true, description: "Enabled" },
            { value: false, description: "Disabled" }
        ];
        $scope.selectedStatuses = [];

        $scope.searchClicked = function () {
            return gridApi.loadGrid(getGridQuery());
        };

        $scope.gridReady = function (api) {
            gridApi = api;
            gridApi.loadGrid(getGridQuery());
        };
        $scope.AddNewDataSource = addNewDataSource;

        $scope.hasAddDataSource = function () {
            return VR_Integration_DataSourceAPIService.HasAddDataSource();
        };
    }

    function load() {
        $scope.isLoading= true;
        loadAllControls();
       
    }

    function loadAllControls() {
        return VR_Integration_DataSourceAPIService.GetDataSourceAdapterTypes()
           .then(function (responseArray) {
               angular.forEach(responseArray, function (item) {
                   $scope.adapterTypes.push(item);
               });
           })
           .catch(function (error) {
               VRNotificationService.notifyException(error, $scope);
               $scope.isLoading = false;
           })
           .finally(function () {
               $scope.isLoading = false;
           });
    }

    function getGridQuery() {
        var query = {
            Name: $scope.name,
            AdapterTypeIDs: UtilsService.getPropValuesFromArray($scope.selectedAdapterTypes, "ExtensionConfigurationId"),
            IsEnabled: ($scope.selectedStatuses.length == 1) ? $scope.selectedStatuses[0].value : null
        };
        return query;
    }

    function addNewDataSource() {
        var onDataSourceAdded = function (dataSource) {
            gridApi.onDataSourceAdded(dataSource);
        };
        VR_Integration_DataSourceService.addDataSource(onDataSourceAdded);
    }

  
}

appControllers.controller('Integration_DataSourceManagementController', DataSourceManagementController);