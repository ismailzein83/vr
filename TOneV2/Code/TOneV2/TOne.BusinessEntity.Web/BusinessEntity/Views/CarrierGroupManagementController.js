(function (appControllers) {

    "use strict";

    CarrierGroupManagementController.$inject = ['$scope', 'CarrierGroupAPIService', 'VRModalService'];
    function CarrierGroupManagementController($scope, CarrierGroupAPIService, VRModalService) {

        var treeAPI;
        var mainGridApi;

        var beListReady = false;
        function retrieveData() {
            if (mainGridApi)
                return mainGridApi.retrieveData({
                    GroupId: $scope.currentNode.EntityId,
                    WithDescendants: false
                });
        }

        function defineGrid() {

            $scope.datasource = [];

            $scope.onGridReady = function (api) {
                mainGridApi = api;
                return retrieveData();
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return CarrierGroupAPIService.GetCarrierAccountsByGroup(dataRetrievalInput)
                .then(function (response) {

                    onResponseReady(response);
                });
            };
        }


        function defineScope() {

            $scope.beList = [];

            $scope.currentNode = {
                EntityId: 0
            };

            $scope.treeReady = function (api) {
                treeAPI = api;
            }

            $scope.treeValueChanged = function () {
                if (angular.isObject($scope.currentNode)) {
                    return retrieveData();
                }
            }

            $scope.CarrierAccountsDataSource = [];

            $scope.addNewGroup = addGroup;
            $scope.editGroup = editGroup;
        }

        function load() {

            $scope.isGettingData = true;

            loadTree().finally(function () {
                $scope.isGettingData = false;
                treeAPI.refreshTree($scope.beList);
            });
        }

        function loadTree() {

            return CarrierGroupAPIService.GetEntityNodes()
               .then(function (response) {
                   $scope.beList = response;
                   beListReady = true;
               });
        }

        defineScope();
        load();
        defineGrid();

        function addGroup() {
            var settings = {
                useModalTemplate: true,
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Add Carrier Group";
                modalScope.onTreeAdded = function () {
                    load();
                    $scope.currentNode = undefined;
                };
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierGroupEditor.html', null, settings);
        }

        function editGroup() {
            var settings = {
                useModalTemplate: true,
            };

            var parameters = {
                carrierGroupId: $scope.currentNode.EntityId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Edit Carrier Group";
                modalScope.onTreeUpdated = function () {
                    load();
                    $scope.currentNode = undefined;
                };
            };

            VRModalService.showModal('/Client/Modules/BusinessEntity/Views/CarrierGroupEditor.html', parameters, settings);
        }
    }
    appControllers.controller('Carrier_CarrierGroupManagementController', CarrierGroupManagementController);

})(appControllers);