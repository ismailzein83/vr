BPManagementController.$inject = ['$scope', 'UtilsService', 'BusinessProcessAPIService', 'VRModalService',  'DataRetrievalResultTypeEnum', 'BusinessProcessService'];

function BPManagementController($scope, UtilsService, BusinessProcessAPIService, VRModalService,DataRetrievalResultTypeEnum, BusinessProcessService) {

    "use strict";

    var mainGridApi;

    function retrieveData() {

        return mainGridApi.retrieveData({
            DefinitionsId: UtilsService.getPropValuesFromArray($scope.selectedDefinition, "BPDefinitionID"),
            InstanceStatus: UtilsService.getPropValuesFromArray($scope.selectedInstanceStatus, "Value"),
            DateFrom: $scope.fromDate,
            DateTo: $scope.toDate
        });
    }

    function defineGrid() {

        $scope.datasource = [];

        $scope.onGridReady = function (api) {
            mainGridApi = api;
            return retrieveData();
        };

        $scope.onTitleClicked = function (dataItem) {

            VRModalService.showModal('/Client/Modules/BusinessProcess/Views/BPTrackingModal.html', {
                BPInstanceID: dataItem.ProcessInstanceID
            }, {
                onScopeReady: function (modalScope) {
                    modalScope.title = "Tracking";
                }
            });

        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return BusinessProcessAPIService.GetFilteredBProcess(dataRetrievalInput)
            .then(function (response) {
                if (dataRetrievalInput.DataRetrievalResultType === DataRetrievalResultTypeEnum.Normal.value) {
                    for (var i = 0, len = response.Data.length; i < len; i++) {
                        response.Data[i].StatusDescription = BusinessProcessService.getStatusDescription(response.Data[i].Status);
                    }
                }
                onResponseReady(response);
            });
        };

    }

    function getCurrentDate(days) {
        var d = new Date();
        var currDate = d.getDate() + days;
        var currMonth = d.getMonth();
        var currYear = d.getFullYear();
        return new Date(currYear, currMonth, currDate);
    }

    function defineScope() {
        $scope.toDate = getCurrentDate(+1);
        $scope.fromDate = getCurrentDate(-1);
        $scope.definitions = [];
        $scope.selectedDefinition = [];
        $scope.instanceStatus = [];
        $scope.selectedInstanceStatus = [];

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.getStatusColor = function (dataItem, colDef) {
            return BusinessProcessService.getStatusColor(dataItem.Status);
        };

    }

    function loadFilters() {

        BusinessProcessAPIService.GetDefinitions().then(function (response) {

            for (var i = 0, len = response.length; i < len; i++) {
                $scope.definitions.push(response[i]);
            }
        });

        BusinessProcessAPIService.GetStatusList().then(function (response) {
            for (var i = 0, len = response.length; i < len; i++) {
                $scope.instanceStatus.push(response[i]);
            }
        });
    }

    function load() {
        loadFilters();
    }

    defineScope();
    load();
    defineGrid();
}
appControllers.controller('BusinessProcess_BPManagementController', BPManagementController);