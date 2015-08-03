(function (appControllers) {

    "use strict";

    bpTrackingModalController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', 'BusinessProcessAPIService', 'BusinessProcessService', 'DataRetrievalResultTypeEnum'];

    function bpTrackingModalController($scope, UtilsService, VRNavigationService, BusinessProcessAPIService, BusinessProcessService, DataRetrievalResultTypeEnum) {

    var mainGridApi, nonClosedStatuses;

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.BPInstanceID = undefined;
        if (parameters !== undefined && parameters !== null) {
            $scope.BPInstanceID = parameters.BPInstanceID;
        }
    }

    function loadSummary() {

        BusinessProcessAPIService.GetBPInstance($scope.BPInstanceID).then(function (response) {

            $scope.process = {
                Title: response.Title,
                Date: response.CreatedTime,
                Status: response.StatusDescription
            };
        });
    }

    function loadFilters() {

        BusinessProcessAPIService.GetTrackingSeverity().then(function (response) {
            for (var i = 0 ; i < response.length ; i++) {
                $scope.trackingSeverity.push(response[i]);
            }
        });
    }

    function loadNonClosedStatuses() {
        nonClosedStatuses = [];
        BusinessProcessAPIService.GetNonClosedStatuses().then(function (response) {
            for (var i = 0 ; i < response.length ; i++) {
                nonClosedStatuses.push(response[i]);
            }
        });
    }

    function defineScope() {
        $scope.lastTrackingId = 0;
        $scope.message = '';
        $scope.trackingSeverity = [];
        $scope.selectedTrackingSeverity = [];
        $scope.close = function () {
            $scope.modalContext.closeModal();
        };

        $scope.getSeverityColor = function (dataItem, colDef) {
            return BusinessProcessService.getSeverityColor(dataItem.Severity);
        };

    }

    function retrieveData() {

        return mainGridApi.retrieveData({
            ProcessInstanceID: $scope.BPInstanceID,
            LastTrackingId: $scope.lastTrackingId
        });
    }

    function defineGrid() {

        $scope.datasource = [];

        $scope.onGridReady = function (api) {
            mainGridApi = api;
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return BusinessProcessAPIService.GetTrackingsByInstanceId(dataRetrievalInput)
            .then(function (response) {

                $scope.lastTrackingId = UtilsService.getPropMaxValueFromArray(response.Data.Tracking, "TrackingId");

                if (dataRetrievalInput.DataRetrievalResultType === DataRetrievalResultTypeEnum.Normal.value) {
                    for (var i = 0, len = response.Data.length; i < len; i++) {
                        response.Data[i].SeverityDescription = BusinessProcessService.getSeverityDescription(response.Data[i].Severity);
                    }
                }
                onResponseReady(response);
            });
        };

    }

    function load() {
        defineScope();
        loadParameters();
        defineGrid();
        loadFilters();
        loadSummary();
        loadNonClosedStatuses();
    }
   
    load();
    
}

    appControllers.controller("BusinessProcess_BPTrackingModalController", bpTrackingModalController);

})(appControllers);