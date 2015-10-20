SwitchBrandManagementController.$inject = ["$scope", "SwitchBrandAPIService", "UtilsService", "VRNotificationService", "VRModalService"];

function SwitchBrandManagementController($scope, SwitchBrandAPIService, UtilsService, VRNotificationService, VRModalService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {

        // filter vars
        $scope.name = undefined;

        // grid vars
        $scope.brands = [];
        $scope.gridMenuActions = [];

        // filter functions
        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.addBrand = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Switch Brand");

                modalScope.onBrandAdded = function (BrandObj) {
                    gridAPI.itemAdded(BrandObj);
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchBrandEditor.html", null, settings);
        }

        // grid functions
        $scope.onGridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return SwitchBrandAPIService.GetFilteredBrands(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }

        defineMenuActions();
    }

    function load() {
        
    }

    function retrieveData() {
        var query = {
            Name: $scope.name
        };

        gridAPI.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
            {
                name: "Edit",
                clicked: editBrand
            },
            {
                name: "Delete",
                clicked: deleteBrand
            }
        ];
    }

    function editBrand(gridObj) {
        var modalSettings = {};

        var parameters = {
            BrandId: gridObj.BrandId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor("Switch Brand", gridObj.Name);

            modalScope.onBrandUpdated = function (BrandObj) {
                gridAPI.itemUpdated(BrandObj);
            };
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/NetworkInfrastructure/SwitchBrandEditor.html", parameters, modalSettings);
    }

    function deleteBrand(gridObj) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return SwitchBrandAPIService.DeleteBrand(gridObj.BrandId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Switch Brand", deletionResponse))
                                gridAPI.itemDeleted(gridObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller("PSTN_BusinessEntity_SwitchBrandManagementController", SwitchBrandManagementController);
