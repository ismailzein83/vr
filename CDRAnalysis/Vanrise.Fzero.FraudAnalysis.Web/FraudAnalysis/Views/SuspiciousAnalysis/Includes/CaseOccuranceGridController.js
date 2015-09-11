"use strict";

CaseOccuranceGridController.$inject = ["$scope", "CaseManagementAPIService", "SuspicionLevelEnum", "SuspicionOccuranceStatusEnum", "UtilsService", "VRNotificationService"];

function CaseOccuranceGridController($scope, CaseManagementAPIService, SuspicionLevelEnum, SuspicionOccuranceStatusEnum, UtilsService, VRNotificationService) {

    var gridApi = undefined;

    defineScope();
    load();

    function defineScope() {

        $scope.details = [];

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

            return CaseManagementAPIService.GetFilteredDetailsByCaseID(dataRetrievalInput)
                .then(function (response) {

                    angular.forEach(response.Data, function (item) {

                        var suspicionLevel = UtilsService.getEnum(SuspicionLevelEnum, "value", item.SuspicionLevelID);
                        item.SuspicionLevelDescription = suspicionLevel.description;

                        var detailStatus = UtilsService.getEnum(SuspicionOccuranceStatusEnum, "value", item.SuspicionOccuranceStatus);
                        item.SuspicionOccuranceStatusDescription = detailStatus.description;
                    });

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        }
    }

    function load() {
    }

    function retrieveData() {

        var query = {
            AccountNumber: $scope.dataItem.AccountNumber,
            CaseID: $scope.dataItem.CaseID
        };

        return gridApi.retrieveData(query);
    }
}

appControllers.controller("FraudAnalysis_CaseOccuranceGridController", CaseOccuranceGridController);