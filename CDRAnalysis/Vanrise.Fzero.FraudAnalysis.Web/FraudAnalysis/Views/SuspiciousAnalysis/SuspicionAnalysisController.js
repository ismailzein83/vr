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
            $scope.Guid = "t_FraudResult" + guid();
            getData();
        };

        $scope.loadMoreData = function () {
            return getData();
        }

        $scope.searchClicked = function () {
            mainGridAPI.clearDataAndContinuePaging();
            $scope.Guid = "t_FraudResult" + guid();
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
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        var suspicionLevelsList = '';

        angular.forEach($scope.selectedSuspicionLevels, function (itm) {
            suspicionLevelsList = suspicionLevelsList + itm.id + ','
        });


        var strategiesList = '';

        angular.forEach($scope.selectedStrategies, function (itm) {
            strategiesList = strategiesList + itm.id + ','
        });

        SuspicionAnalysisAPIService.GetFraudResult(fromDate, toDate, strategiesList.slice(0, -1), suspicionLevelsList.slice(0, -1),fruadResult.SubscriberNumber).then(function (response) {

            var params = {
                dateDay: response.DateDay,
                subscriberNumber: response.SubscriberNumber,
                suspicionLevelName: response.SuspicionLevelName,
                fromDate: $scope.fromDate,
                toDate: $scope.toDate,
                statusId: response.StatusId,
                validTill: response.ValidTill,
                lastOccurance: response.LastOccurance,
                strategyName: response.StrategyName,
                numberofOccurances: response.NumberofOccurances
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Suspicious Number Details & Related Numbers";
                modalScope.onSubscriberCaseUpdated = function (subscriberCase) {

                    mainGridAPI.itemUpdated(subscriberCase);
                }
            };
            VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/SuspiciousAnalysis/test.html", params, settings);
        });
        
    }

    function guid() {
        function _p8(s) {
            var p = (Math.random().toString(16) + "000000000").substr(2, 8);
            return s ? "_" + p.substr(0, 4) + "_" + p.substr(4, 4) : p;
        }
        return _p8() + _p8(true) + _p8(true) + _p8();
    }

    function getData() {

        $scope.isGettingFraudResults = true;

        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        var suspicionLevelsList = '';

        angular.forEach($scope.selectedSuspicionLevels, function (itm) {
            suspicionLevelsList = suspicionLevelsList + itm.id + ','
        });


        var strategiesList = '';

        angular.forEach($scope.selectedStrategies, function (itm) {
            strategiesList = strategiesList + itm.id + ','
        });



        var pageInfo = mainGridAPI.getPageInfo();


        return SuspicionAnalysisAPIService.GetFilteredSuspiciousNumbers($scope.Guid, pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, strategiesList.slice(0, -1), suspicionLevelsList.slice(0, -1)).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.fraudResults.push(itm);
            });
        }).finally(function () {
            $scope.isGettingFraudResults = false;
        });
    }


}
appControllers.controller('FraudAnalysis_SuspicionAnalysisController', SuspicionAnalysisController);
