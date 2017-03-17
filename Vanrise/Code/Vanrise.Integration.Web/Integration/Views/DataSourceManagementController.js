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
        $scope.DisableAllDataSources = disableAllDataSources;
        $scope.EnableAllDataSources = enableAllDataSources;
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
        var payload = {
            context: getContext(),
            query: query
        };
        return payload;
    }

    function addNewDataSource() {
        var onDataSourceAdded = function (dataSource) {
            gridApi.onDataSourceAdded(dataSource);
        };
        VR_Integration_DataSourceService.addDataSource(onDataSourceAdded);
    }

    function disableAllDataSources() {

        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return VR_Integration_DataSourceAPIService.DisableAllDataSource().then(function (response) {
                    if (response) {
                        $scope.viewAll = false;
                        $scope.searchClicked();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, null);
                });
            }
        });
    }

    function enableAllDataSources() {

        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return VR_Integration_DataSourceAPIService.EnableAllDataSource().then(function (response) {
                    if (response) {
                        $scope.viewAll = true;
                        $scope.searchClicked();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, null);
                });
            }
        });
    }

    function getContext () {
        var context = {
            showEnableAll: function() {
                $scope.viewAll = false;
            },
            showDisableAll: function() {
                $scope.viewAll = true;
            }
        };
        return context;
        };
}

appControllers.controller('Integration_DataSourceManagementController', DataSourceManagementController);