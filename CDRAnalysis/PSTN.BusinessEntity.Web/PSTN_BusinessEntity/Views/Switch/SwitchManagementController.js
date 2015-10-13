SwitchManagementController.$inject = ["$scope", "PSTN_BE_Service", "SwitchAPIService", "BrandAPIService", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchManagementController($scope, PSTN_BE_Service, SwitchAPIService, BrandAPIService, UtilsService, VRNotificationService, VRModalService) {

    var gridAPI;

    defineScope();
    load();

    function defineScope() {

        // filter vars
        $scope.name = undefined;
        $scope.brands = [];
        $scope.selectedBrands = [];
        $scope.areaCode = undefined;

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
                modalScope.title = "Add Switch";

                modalScope.onSwitchAdded = function (switchObj) {
                    gridAPI.itemAdded(switchObj);
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Switch/SwitchEditor.html", null, settings);
        }

        // grid functions
        $scope.gridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SwitchAPIService.GetFilteredSwitches(dataRetrievalInput)
                .then(function (response) {

                    if (response.Data != undefined) {
                        for (var i = 0; i < response.Data.length; i++) {
                            setDataItemExtension(response.Data[i]);
                        }
                    }

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        defineMenuActions();
    }

    function setDataItemExtension(dataItem) {
        var extensionObj = {};

        extensionObj.onTrunkGridReady = function (api) {
            extensionObj.trunkGridAPI = api;

            var query = { SelectedSwitchIds: [dataItem.SwitchId] };
            extensionObj.trunkGridAPI.retrieveData(query);

            extensionObj.onTrunkGridReady = undefined;
        };

        dataItem.extensionObj = extensionObj;
    }

    function load() {
        $scope.isLoadingFilters = true;

        return BrandAPIService.GetBrands()
            .then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.brands.push(item);
                });
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            })
            .finally(function () {
                $scope.isLoadingFilters = false;
            });
    }

    function retrieveData() {
        var query = {
            Name: $scope.name,
            SelectedBrandIds: UtilsService.getPropValuesFromArray($scope.selectedBrands, "BrandId"),
            AreaCode: $scope.areaCode,
        };

        gridAPI.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
            {
                name: "Edit",
                clicked: editSwitch
            },
            {
                name: "Delete",
                clicked: deleteSwitch
            },
            {
                name: "Add Trunk",
                clicked: addTrunk
            }
        ];
    }
    
    function editSwitch(gridObj) {
        var modalSettings = {};

        var parameters = {
            SwitchId: gridObj.SwitchId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Switch: " + gridObj.Name;

            modalScope.onSwitchUpdated = function (switchObj) {
                gridAPI.itemUpdated(switchObj);
            };
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Switch/SwitchEditor.html", parameters, modalSettings);
    }

    function deleteSwitch(gridObj) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return SwitchAPIService.DeleteSwitch(gridObj.SwitchId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Switch", deletionResponse))
                                gridAPI.itemDeleted(gridObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function addTrunk(dataItem) {

        gridAPI.expandRow(dataItem);

        var onTrunkAdded = function (trunkObj) {
            if (dataItem.extensionObj.trunkGridAPI.onTrunkAdded != undefined)
                dataItem.extensionObj.trunkGridAPI.onTrunkAdded(trunkObj);
        }

        PSTN_BE_Service.addTrunk(dataItem.SwitchId, onTrunkAdded);
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchManagementController", SwitchManagementController);
