FraudResultManagementController.$inject = ['$scope', 'FraudResultAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function FraudResultManagementController($scope, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    var mainGridAPI;
    var arrMenuAction = [];
    
    defineScope();
    load();

       
   

    function defineScope() {

        $scope.gridMenuActions = [];

        $scope.fraudResults = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            getData();
        };

        $scope.loadMoreData = function () {
           return getData();
        }

        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        };

        $scope.resetClicked = function () {            
            $scope.fromDate = '';
            $scope.toDate = '';
            $scope.selectedStrategies = [];
            $scope.selectedSuspicionLevels = [];
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        };

        defineMenuActions();
    }

    function load() {
        
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Details",
            clicked: detailFraudResult
        }];
    }

    function getData() {
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        

        ////// to be updateddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd
        //var strategyId = $scope.selectedStrategies[0];
        //var selectedSuspicionLevels = $scope.selectedSuspicionLevels;
        ///////////////////////////////////////////////////////////////////////////////////////

        var strategyId = 3;
        var selectedSuspicionLevels = '3';

        var pageInfo = mainGridAPI.getPageInfo();

        return FraudResultAPIService.GetFilteredSuspiciousNumbers(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, strategyId, suspicionList).then(function (response) {
            angular.forEach(response, function (itm) {
               
                $scope.fraudResults.push(itm);
            });
        });
    }

   
}
appControllers.controller('FraudAnalysis_FraudResultManagementController', FraudResultManagementController);
