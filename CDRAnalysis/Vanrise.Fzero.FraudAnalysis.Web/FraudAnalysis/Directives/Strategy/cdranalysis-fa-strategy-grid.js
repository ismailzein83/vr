(function (app) {

    'use strict';

    StrategyGridDirective.$inject = ['StrategyAPIService', 'CDRAnalysis_FA_StrategyService', 'VRNotificationService'];

    function StrategyGridDirective(StrategyAPIService, CDRAnalysis_FA_StrategyService, VRNotificationService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var strategyGrid = new StrategyGrid($scope, ctrl, $attrs);
                strategyGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/FraudAnalysis/Directives/Strategy/Templates/StrategyGridTemplate.html'
        };

        function StrategyGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.strategies = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady && typeof ctrl.onReady == 'function')
                        ctrl.onReady(getDirectiveAPI());
                };

                ctrl.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return StrategyAPIService.GetFilteredStrategies(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    permissions: 'Root/Strategy Module:Edit',
                    clicked: editStrategy
                }];
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.loadGrid = function (query) {
                    return gridAPI.retrieveData(query);
                };

                directiveAPI.onStrategyAdded = function (addedStrategy) {
                    gridAPI.itemAdded(addedStrategy);
                };

                return directiveAPI;
            }

            function editStrategy(strategy) {
                var onStrategyUpdated = function (updatedStrategy) {
                    gridAPI.itemUpdated(updatedStrategy);
                };
                CDRAnalysis_FA_StrategyService.editStrategy(strategy.Entity.Id, onStrategyUpdated);
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('cdranalysisFaStrategyGrid', StrategyGridDirective);

})(app);
