SwitchManagementController.$inject = ["$scope", "SwitchAPIService", "DataSourceAPIService", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchManagementController($scope, SwitchAPIService, DataSourceAPIService, UtilsService, VRNotificationService, VRModalService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {

        // filter vars
        $scope.name = undefined;
        $scope.types = [];
        $scope.selectedTypes = [];
        $scope.areaCode = undefined;
        $scope.dataSources = [];
        $scope.selectedDataSources = [];

        // grid vars
        $scope.switches = [];
        $scope.gridMenuActions = [];

        // filter functions
        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.addSwitch = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add a Switch";

                modalScope.onSwitchAdded = function (switchObject) {

                    switchObject.TypeDescription = (switchObject.TypeID != null) ?
                        UtilsService.getItemByVal($scope.types, switchObject.TypeID, "ID").Name : null;

                    switchObject.DataSourceName = UtilsService.getItemByVal($scope.dataSources, switchObject.DataSourceID, "DataSourceId").Name;

                    gridAPI.itemAdded(switchObject);
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchEditor.html", null, settings);
        }

        // grid functions
        $scope.gridReady = function (api) {
            gridAPI = api;

            if ($scope.types.length > 0 && $scope.dataSources.length > 0) // types and data sources are loaded
                return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SwitchAPIService.GetFilteredSwitches(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {
                        item.TypeDescription = UtilsService.getItemByVal($scope.types, item.TypeID, "ID").Name;
                        item.DataSourceName = UtilsService.getItemByVal($scope.dataSources, item.DataSourceID, "DataSourceId").Name;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        defineMenuActions();
    }

    function load() {
        $scope.isLoadingFilters = true;

        return UtilsService.waitMultipleAsyncOperations([loadSwitchTypes, loadDataSources])
            .then(function () {

                if (gridAPI != undefined)
                    return retrieveData();
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingFilters = false;
            });
    }

    function loadSwitchTypes() {
        return SwitchAPIService.GetSwitchTypes()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.types.push(item);
                });
            });
    }
    
    function loadDataSources() {
        return DataSourceAPIService.GetDataSources()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.dataSources.push(item);
                });
            });
    }

    function retrieveData() {
        var query = {
            Name: $scope.name,
            SelectedTypeIDs: UtilsService.getPropValuesFromArray($scope.selectedTypes, "ID"),
            AreaCode: $scope.areaCode,
            SelectedDataSourceIDs: UtilsService.getPropValuesFromArray($scope.selectedDataSources, "DataSourceId")
        };

        gridAPI.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editSwitch
        }];
    }
    
    function editSwitch(gridObject) {
        var modalSettings = {};

        var parameters = {
            SwitchID: gridObject.ID
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Switch: " + gridObject.Name;

            modalScope.onSwitchUpdated = function (switchObject) {
                
                switchObject.TypeDescription = (switchObject.TypeID != null) ?
                    UtilsService.getItemByVal($scope.types, switchObject.TypeID, "ID").Name : null;

                switchObject.DataSourceName = UtilsService.getItemByVal($scope.dataSources, switchObject.DataSourceID, "DataSourceId").Name;

                gridAPI.itemUpdated(switchObject);
            };
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/SwitchEditor.html", parameters, modalSettings);
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchManagementController", SwitchManagementController);
