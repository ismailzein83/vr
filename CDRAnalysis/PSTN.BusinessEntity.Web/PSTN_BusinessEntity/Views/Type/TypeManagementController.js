TypeManagementController.$inject = ["$scope", "TypeAPIService", "VRNotificationService", "VRModalService"];

function TypeManagementController($scope, TypeAPIService, VRNotificationService, VRModalService) {

    var gridAPI = undefined;

    defineScope();
    load();

    function defineScope() {

        // filter vars
        $scope.name = undefined;

        // grid vars
        $scope.types = [];
        $scope.gridMenuActions = [];

        // filter functions
        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.addType = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add Switch Type";

                modalScope.onTypeAdded = function (typeObj) {
                    gridAPI.itemAdded(typeObj);
                };
            };

            VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Type/TypeEditor.html", null, settings);
        }

        // grid functions
        $scope.onGridReady = function (api) {
            gridAPI = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return TypeAPIService.GetFilteredTypes(dataRetrievalInput)
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
                clicked: editType
            },
            {
                name: "Delete",
                clicked: deleteType
            }
        ];
    }

    function editType(gridObj) {
        var modalSettings = {};

        var parameters = {
            TypeId: gridObj.TypeId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Switch Type: " + gridObj.Name;

            modalScope.onTypeUpdated = function (TypeObj) {
                gridAPI.itemUpdated(TypeObj);
            };
        };

        VRModalService.showModal("/Client/Modules/PSTN_BusinessEntity/Views/Type/TypeEditor.html", parameters, modalSettings);
    }

    function deleteType(gridObj) {

        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response == true) {

                    return TypeAPIService.DeleteType(gridObj.TypeId)
                        .then(function (deletionResponse) {
                            if (VRNotificationService.notifyOnItemDeleted("Switch Type", deletionResponse))
                                gridAPI.itemDeleted(gridObj);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller("PSTN_BusinessEntity_TypeManagementController", TypeManagementController);
