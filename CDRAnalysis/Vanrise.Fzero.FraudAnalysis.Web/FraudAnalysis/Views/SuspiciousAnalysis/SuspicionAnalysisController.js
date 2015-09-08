"use strict";

SuspicionAnalysisController.$inject = ['$scope', 'StrategyAPIService', 'CaseManagementAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'CaseStatusEnum', 'SuspicionLevelsEnum'];

function SuspicionAnalysisController($scope, StrategyAPIService, CaseManagementAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, CaseStatusEnum, SuspicionLevelsEnum) {

    var mainGridAPI;

    defineScope();
    load();

    function defineScope() {
        $scope.statuses = [];
        angular.forEach(CaseStatusEnum, function (status) {
            $scope.statuses.push({ id: status.id, name: status.name })
        });


        $scope.selectedStatus=[];
        $scope.selectedStatus.push(UtilsService.getItemByVal($scope.statuses, 1, "id"));

        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.suspicionLevels = [];
        angular.forEach(SuspicionLevelsEnum, function (level) {
            $scope.suspicionLevels.push({ id: level.id, name: level.name })
        });

      

        $scope.strategies = [];

        loadStrategies();

        $scope.selectedSuspicionLevels = [];
        $scope.selectedStrategies = [];

        $scope.gridMenuActions = [];

        $scope.fraudResults = [];


        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            return retrieveData();
        };
        $scope.searchClicked = function () {
            return retrieveData();
        }


        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CaseManagementAPIService.GetFilteredSuspiciousNumbers(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }



        defineMenuActions();
    }


    function removeLastComma(strng) {
        var n = strng.lastIndexOf(",");
        var a = strng.substring(0, n)
        return a;
    }


    function retrieveData() {

        var suspicionLevelsList = '';

        angular.forEach($scope.selectedSuspicionLevels, function (itm) {
            suspicionLevelsList = suspicionLevelsList + itm.id + ','
        });


        var strategiesList = '';

        angular.forEach($scope.selectedStrategies, function (itm) {
            strategiesList = strategiesList + itm.id + ','
        });


        var caseStatusesList = '';

        angular.forEach($scope.selectedStatus, function (itm) {
            caseStatusesList = caseStatusesList + itm.id + ','
        });



        var query = {
            AccountNumber : $scope.accountNumber,
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate,
            SuspicionLevelsList: removeLastComma(suspicionLevelsList),
            StrategiesList: removeLastComma(strategiesList),
            CaseStatusesList: removeLastComma(caseStatusesList)
        };

      
        return mainGridAPI.retrieveData(query);
    }

    function load() {

    }


    function loadStrategies() {
        var periodId = 0; // all periods
        var isEnabled = ''; // all enabled and disabled
        return StrategyAPIService.GetStrategies(periodId, isEnabled).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name });
            });
        });
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Details",
            clicked: detailFraudResult
        }];
    }



    function detailFraudResult(fruadResult) {
        
        var suspicionLevelsList = '';

        angular.forEach($scope.selectedSuspicionLevels, function (itm) {
            suspicionLevelsList = suspicionLevelsList + itm.id + ','
        });


        var strategiesList = '';

        angular.forEach($scope.selectedStrategies, function (itm) {
            strategiesList = strategiesList + itm.id + ','
        });

        var params = {
            fromDate: $scope.fromDate,
            toDate: $scope.toDate,
            strategiesList: strategiesList,
            suspicionLevelsList: suspicionLevelsList,
            accountNumber: fruadResult.AccountNumber
        };

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Suspicious Number Details";
            modalScope.onAccountCaseUpdated = function (accountCase) {

                mainGridAPI.itemUpdated(accountCase);
            }
        };
        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousAnalysis/SuspiciousNumberDetails.html", params, settings);
        
    }


}
appControllers.controller('FraudAnalysis_SuspicionAnalysisController', SuspicionAnalysisController);
