'use strict';

app.directive('retailRingoAccountpartRuntimeEntitiesinfo', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeEntitiesInfoPartRuntime = new AccountTypeEntitiesInfoPartRuntime($scope, ctrl, $attrs);
            accountTypeEntitiesInfoPartRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_Ringo/Directives/MainExtensions/AccountParts/Runtime/Templates/AccountTypeEntitiesInfoPartRuntimeTemplate.html'
    };

    function AccountTypeEntitiesInfoPartRuntime($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var agentDirectiveApi;
        var agentReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var posDirectiveApi;
        var posReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var distributorDirectiveApi;
        var distributorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var mainPayload;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAgentSelectorDirectiveReady = function (api) {
                agentDirectiveApi = api;
                agentReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onPointOfSaleSelectorDirectiveReady = function (api) {
                posDirectiveApi = api;
                posReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDistributorSelectorDirectiveReady = function (api) {
                distributorDirectiveApi = api;
                distributorReadyPromiseDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                mainPayload = payload;
                return loadSelectors();
            };

            api.getData = function () {
                return {
                    $type: 'Retail.Ringo.MainExtensions.AccountParts.AccountPartEntitiesInfo,Retail.Ringo.MainExtensions',
                    PosId: posDirectiveApi.getSelectedIds(),
                    AgentId: agentDirectiveApi.getSelectedIds(),
                    DistributorId: distributorDirectiveApi.getSelectedIds()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadSelectors() {
            var promises = [];



            var loadAgentPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadAgentPromiseDeferred.promise);


            var agentPayload = {
                AccountBEDefinitionId: "2F57CDE2-033E-48F9-BD33-03113D77C9AC"
            };

            if (mainPayload != undefined && mainPayload.partSettings != undefined && mainPayload.partSettings.AgentId != undefined) {
                agentPayload.selectedIds = mainPayload.partSettings.AgentId;
            }

            agentReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(agentDirectiveApi, agentPayload, loadAgentPromiseDeferred);
            });


            var loadPosPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadPosPromiseDeferred.promise);


            var posPayload = {
                AccountBEDefinitionId: "B2B211BC-ECE0-4C8E-9B40-7958D05FF753"
            };
            if (mainPayload != undefined && mainPayload.partSettings != undefined && mainPayload.partSettings.PosId != undefined) {
                posPayload.selectedIds = mainPayload.partSettings.PosId;
            }

            posReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(posDirectiveApi, posPayload, loadPosPromiseDeferred);
            });


            var loadDistributorPromiseDeferred = UtilsService.createPromiseDeferred();
            promises.push(loadDistributorPromiseDeferred.promise);


            var distributorload = {
                AccountBEDefinitionId: "A24B5BD3-1FD8-4C5B-BA2F-E09C1F369D88"
            };
            if (mainPayload != undefined && mainPayload.partSettings != undefined && mainPayload.partSettings.DistributorId != undefined) {

                distributorload.selectedIds = mainPayload.partSettings.DistributorId;
            }

            distributorReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(distributorDirectiveApi,
                    distributorload,
                    loadDistributorPromiseDeferred);
            });

            return UtilsService.waitMultiplePromises(promises);
        }
    }
}]);