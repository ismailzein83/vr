"use strict";

LinesDetectedController.$inject = ['$scope', 'ReportingAPIService', 'UserAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function LinesDetectedController($scope, ReportingAPIService, UserAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();


    function defineScope() {

        $scope.gridMenuActions = [];

        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.detectedLines = [];


        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return ReportingAPIService.GetFilteredLinesDetected(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }
    }

    function load() {

    }

    function retrieveData() {


        var query = {
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate
        };

        return mainGridAPI.retrieveData(query);
    }
}
appControllers.controller('FraudAnalysis_LinesDetectedController', LinesDetectedController);
