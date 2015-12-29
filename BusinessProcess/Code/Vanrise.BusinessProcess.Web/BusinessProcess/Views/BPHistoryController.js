(function (appControllers) {

    "use strict";

    bpHistoryController.$inject = ['$scope', 'UtilsService', 'BusinessProcessAPIService', 'DataRetrievalResultTypeEnum', 'BusinessProcessService', 'VRValidationService'];

    function bpHistoryController($scope, UtilsService, BusinessProcessAPIService, DataRetrievalResultTypeEnum, BusinessProcessService, VRValidationService) {

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
                BusinessProcessService.openProcessTracking(dataItem.ProcessInstanceID);
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


        function defineScope() {
            var yesterday = new Date();
            yesterday.setDate(yesterday.getDate() - 1);

            $scope.fromDate = yesterday;
            $scope.toDate = new Date();

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            }


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

    appControllers.controller('BusinessProcess_BPHistoryController', bpHistoryController);

})(appControllers);

