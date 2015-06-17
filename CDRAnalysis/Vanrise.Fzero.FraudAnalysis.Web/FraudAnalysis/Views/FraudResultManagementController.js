FraudResultManagementController.$inject = ['$scope', 'FraudResultAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function FraudResultManagementController($scope, FraudResultAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

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



    function detailFraudResult(fruadResult) {
        //var params = {
        //    //strategyId: strategy.Id
        //};

        //var settings = {

        //};

        //settings.onScopeReady = function (modalScope) {
        //    modalScope.title = "Edit Strategy";
        //    modalScope.onStrategyUpdated = function (strategy) {
        //        strategy.IsDefaultText = strategy.IsDefault ? "Default" : "Not Default";
        //        mainGridAPI.itemUpdated(strategy);
        //    };
        //};
        //VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/StrategyEditor.html", params, settings);
    }



    function getData() {
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        

        ////// to be updateddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd
        //var strategyId = $scope.selectedStrategies[0];
        //var selectedSuspicionLevels = $scope.selectedSuspicionLevels;
        ///////////////////////////////////////////////////////////////////////////////////////

        var strategyId = 3;
        var suspicionLevelsList = '3';

        var pageInfo = mainGridAPI.getPageInfo();

        return FraudResultAPIService.GetFilteredSuspiciousNumbers(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, strategyId, suspicionLevelsList).then(function (response) {
            angular.forEach(response, function (itm) {
               
                $scope.fraudResults.push(itm);
            });
        });
    }

   
}
appControllers.controller('FraudAnalysis_FraudResultManagementController', FraudResultManagementController);
