StrategyManagementController.$inject = ['$scope', 'StrategyAPIService'];


function StrategyManagementController($scope, StrategyAPIService) {

    var mainGridAPI;
    var arrMenuAction = [];
   

    defineScope();
    load();

    function defineScope() {

        $scope.gridMenuActions = [];

        $scope.strategies = [];

        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };

       

        $scope.searchClicked = function () {
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
        return StrategyAPIService.GetAllStrategies().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push(itm);
            });
        });
    }
}
appControllers.controller('StrategyManagementController', StrategyManagementController);