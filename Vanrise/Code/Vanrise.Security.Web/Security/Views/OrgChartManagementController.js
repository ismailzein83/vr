OrgChartManagementController.$inject = ['$scope', 'OrgChartAPIService', 'VRModalService', 'VRNotificationService'];

function OrgChartManagementController($scope, OrgChartAPIService, VRModalService, VRNotificationService) {

    var gridApi;

    defineScope();
    load();

    function defineScope() {
        $scope.orgCharts = [];
        $scope.gridMenuActions = [];

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return OrgChartAPIService.GetFilteredOrgCharts(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        }

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.addOrgChart = function () {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = 'New Organizational Chart';
                modalScope.onOrgChartAdded = function (orgChart) {
                    gridApi.itemAdded(orgChart);
                };
            };

            VRModalService.showModal('/Client/Modules/Security/Views/OrgChartEditor.html', null, settings);
        }

        defineMenuActions();
    }

    function load() {

    }

    function retrieveData() {
        var query = {
            Name: $scope.name
        };

        return gridApi.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [
            {
                name: 'Edit',
                clicked: editOrgChart,
            },
            {
                name: 'Delete',
                clicked: deleteOrgChart,
            }
        ];
    }

    function editOrgChart(orgChartObject) {
        var modalSettings = {};
        
        var parameters = {
            orgChartId: orgChartObject.Id,
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = 'Edit Organizational Chart: ' + orgChartObject.Name;
            modalScope.onOrgChartUpdated = function (orgChart) {
                gridApi.itemUpdated(orgChart);
            };
        };

        VRModalService.showModal('/Client/Modules/Security/Views/OrgChartEditor.html', parameters, modalSettings);
    }

    function deleteOrgChart(orgChartObject) {
        var message = 'Do you want to delete ' + orgChartObject.Name + '?';

        VRNotificationService.showConfirmation(message)
            .then(function (response) {
                if (response == true) {

                    return OrgChartAPIService.DeleteOrgChart(orgChartObject.Id)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Org Chart", deletionResponse);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller('Security_OrgChartManagementController', OrgChartManagementController);
