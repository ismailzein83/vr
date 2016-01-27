(function (appControllers) {

    'use strict';

    StrategyService.$inject = ['VRModalService'];

    function StrategyService(VRModalService) {
        return {
            addStrategy: addStrategy,
            editStrategy: editStrategy
        };

        function addStrategy(onStrategyAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onStrategyAdded = onStrategyAdded;
            };

            VRModalService.showModal('/Client/Modules/FraudAnalysis/Views/Strategy/StrategyEditor.html', null, modalSettings);
        }

        function editStrategy(strategyId, onStrategyUpdated) {
            var modalParameters = {
                strategyId: strategyId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onStrategyUpdated = onStrategyUpdated;
            };

            VRModalService.showModal("/Client/Modules/FraudAnalysis/Views/Strategy/StrategyEditor.html", modalParameters, modalSettings);
        }
    }

    appControllers.service('CDRAnalysis_FA_StrategyService', StrategyService);

})(appControllers);