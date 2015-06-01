StrategyManagementController.$inject = ['$scope', 'StrategyAPIService'];


function StrategyManagementController($scope, StrategyAPIService) {

    var mainGridAPI;
    var arrMenuAction = [];
    var stopPaging;
    defineScope();
    load();

    function defineScope() {

        $scope.gridMenuActions = [];

        $scope.strategies = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

        $scope.loadMoreData = function () {
            if (stopPaging)
                return;
            return getData();
        }

        $scope.searchClicked = function () {
            stopPaging = false;
            return getData(true);
        };

        $scope.resetClicked = function () {
            stopPaging = true;
            $scope.name = '';
            $scope.description = '';
            return getData(true);
        };

        $scope.AddNewStrategy = function () {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "New Strategy";
                modalScope.onStrategyAdded = function (strategy) {
                    mainGridAPI.itemAdded(strategy);
                };
            };
            VRModalService.showModal('/Client/Modules/Main/Views/StrategyEditor.html', null, settings);

        }
    }

    function load() {
        function MenuAction(name, width, title, url) {
            this.name = name;
            this.clicked = function (dataItem) {
                var params = {
                    strategyId: dataItem.StrategyId
                };

                var settings = {
                    width: width
                };

                settings.onScopeReady = function (modalScope) {
                    modalScope.title = title;
                    modalScope.onStrategyUpdated = function (strategy) {
                        mainGridAPI.itemUpdated(strategy);
                    };
                };
                VRModalService.showModal(url, params, settings);
            };
        }

        arrMenuAction.push(new MenuAction("Edit", "40%", "Edit Strategy", "/Client/Modules/Main/Views/StrategyEditor.html"));
      
        arrMenuAction.forEach(function (item) {
            $scope.gridMenuActions.push({
                name: item.name,
                clicked: item.clicked
            });

        });

        getData(true);
    }

    function getData(startFromFirstRow) {
        var fromRow;
        if (startFromFirstRow) {
            fromRow = 1;
            $scope.strategies.length = 0;
        }
        else
            fromRow = $scope.strategies.length + 1;
        var toRow = fromRow + 20 - 1;

        var name = $scope.name != undefined ? $scope.name : '';
        var description = $scope.description != undefined ? $scope.description : '';
      


        return StrategyAPIService.GetFilteredStrategies(fromRow, toRow, name, description).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push(itm);
            });
            if (response.length < 20)
                stopPaging = true;
        });
    }
}
appControllers.controller('FraudAnalysis_StrategyManagementController', StrategyManagementController);
