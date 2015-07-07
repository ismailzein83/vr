SuspicionAnalysisController.$inject = ['$scope', 'StrategyAPIService', 'SuspicionAnalysisAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function SuspicionAnalysisController($scope, StrategyAPIService, SuspicionAnalysisAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    var mainGridAPI;
    var arrMenuAction = [];
    
    defineScope();
    load();

       
   

    function defineScope() {

        var Now = new Date();
        Now.setDate(Now.getDate() + 1);

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;


        $scope.customvalidateFrom = function (fromDate) {
            return validateDates(fromDate, $scope.toDate);
        };
        $scope.customvalidateTo = function (toDate) {
            return validateDates($scope.fromDate, toDate);
        };
        function validateDates(fromDate, toDate) {
            if (fromDate == undefined || toDate == undefined)
                return null;
            var from = new Date(fromDate);
            var to = new Date(toDate);
            if (from.getTime() > to.getTime())
                return "Start should be before end";
            else
                return null;
        }

        $scope.suspicionLevels = [
                        { id: 2, name: 'Suspicious' }, { id: 3, name: 'Highly Suspicious' }, { id: 4, name: 'Fraud' }

        ];

        $scope.strategies = [];

        loadStrategies();

        $scope.selectedSuspicionLevels = [];
        $scope.selectedStrategies = [];

        $scope.gridMenuActions = [];

        $scope.fraudResults = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            //getData();
        };

        $scope.loadMoreData = function () {
           return getData();
        }

        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            return getData();
        };

        $scope.resetClicked = function () {

            var Now = new Date();
            Now.setDate(Now.getDate() + 1);

            var Yesterday = new Date();
            Yesterday.setDate(Yesterday.getDate() - 1);
           

            $scope.fromDate = Yesterday;
            $scope.toDate = Now;
            $scope.selectedStrategies = [];
            $scope.selectedSuspicionLevels = [];
            mainGridAPI.clearDataAndContinuePaging();
        };

        defineMenuActions();
    }

    function load() {
        
    }


    function loadStrategies() {
        return StrategyAPIService.GetAllStrategies().then(function (response) {
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
        var params = {
            dateDay: fruadResult.DateDay,
            subscriberNumber: fruadResult.SubscriberNumber,
            suspicionLevelName: fruadResult.SuspicionLevelName,
            fromDate: $scope.fromDate,
            toDate: $scope.toDate
        };

        var settings = {

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Suspicious Number Details & Related Numbers";
        };
        VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousNumberDetails.html", params, settings);
    }



    function getData() {
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        var strategyId;
        if ($scope.selectedStrategies != undefined && $scope.selectedStrategies.id != undefined)
            strategyId = $scope.selectedStrategies.id;
        else
            strategyId = 0;

        var suspicionLevelsList = '';
        
        angular.forEach($scope.selectedSuspicionLevels, function (itm) {
            suspicionLevelsList = suspicionLevelsList + itm.id + ','
        });
        

        var pageInfo = mainGridAPI.getPageInfo();

        console.log('strategyId')
        console.log(strategyId)
        console.log('suspicionLevelsList.slice(0, -1)')
        console.log(suspicionLevelsList.slice(0, -1))

        //return SuspicionAnalysisAPIService.GetFilteredSuspiciousNumbers(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, null, null).then(function (response) {

            return SuspicionAnalysisAPIService.GetFilteredSuspiciousNumbers(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, strategyId, suspicionLevelsList.slice(0, -1)).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.fraudResults.push(itm);
            });
        });
    }

   
}
appControllers.controller('FraudAnalysis_SuspicionAnalysisController', SuspicionAnalysisController);
