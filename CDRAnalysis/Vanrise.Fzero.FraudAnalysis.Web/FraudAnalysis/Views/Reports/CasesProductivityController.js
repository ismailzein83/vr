CasesProductivityController.$inject = ['$scope','ReportingAPIService', 'StrategyAPIService', 'UserAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'KindEnum', 'StatusEnum'];

function CasesProductivityController($scope, ReportingAPIService, StrategyAPIService, UserAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, KindEnum, StatusEnum) {

    var mainGridAPI;
    var arrMenuAction = [];

    defineScope();
    load();


    function defineScope() {

        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.gridMenuActions = [];

        $scope.strategies = [];

        $scope.casesProductivity = [];

        loadStrategies();

        $scope.selectedStrategies = [];

        $scope.loadMoreData = function () {
            return getData();
        }

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            return retrieveData();
        };
        $scope.searchClicked = function () {
            return retrieveData();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return ReportingAPIService.GetFilteredCasesProductivity(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }

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



    function removeLastComma(strng) {
        var n = strng.lastIndexOf(",");
        var a = strng.substring(0, n)
        return a;
    }

    function retrieveData() {

        var strategiesList = '';

        angular.forEach($scope.selectedStrategies, function (itm) {
            strategiesList = strategiesList + itm.id + ','
        });


        var query = {
            StrategiesList: removeLastComma(strategiesList),
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate
        };

        return mainGridAPI.retrieveData(query);
    }
}
appControllers.controller('FraudAnalysis_CasesProductivityController', CasesProductivityController);
