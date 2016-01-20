﻿"use strict";

LinesDetectedController.$inject = ['$scope', 'ReportingAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRValidationService'];

function LinesDetectedController($scope, ReportingAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRValidationService) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();

    function defineScope() {


        $scope.gridMenuActions = [];

        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.validateTimeRange = function () {
            return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
        }

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



    function retrieveData() {


        var query = {
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate
        };

        return mainGridAPI.retrieveData(query);
    }


}
appControllers.controller('FraudAnalysis_LinesDetectedController', LinesDetectedController);
