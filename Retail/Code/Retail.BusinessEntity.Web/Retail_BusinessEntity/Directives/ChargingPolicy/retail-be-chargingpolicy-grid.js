'use strict';

app.directive('retailBeChargingpolicyGrid', ['Retail_BE_ChargingPolicyAPIService', 'Retail_BE_ChargingPolicyService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (Retail_BE_ChargingPolicyAPIService, Retail_BE_ChargingPolicyService, UtilsService, VRUIUtilsService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var chargingPolicyGrid = new ChargingPolicyGrid($scope, ctrl, $attrs);
            chargingPolicyGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ChargingPolicy/Templates/ChargingPolicyGridTemplate.html'
    };

    function ChargingPolicyGrid($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController()
        {
            $scope.scopeModel = {};
            $scope.scopeModel.chargingPolicies = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_ChargingPolicyAPIService.GetFilteredChargingPolicies(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            defineMenuActions();
        }

        function defineAPI() {
            var api = {};

            api.load = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onChargingPolicyAdded = function (addedChargingPolicy) {
                gridAPI.itemAdded(addedChargingPolicy);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editChargingPolicy,
                //haspermission: hasEditChargingPolicyPermission
            });
        }

        function editChargingPolicy(chargingPolicy)
        {
            var onChargingPolicyUpdated = function (updatedChargingPolicy) {
                gridAPI.itemUpdated(updatedChargingPolicy);
            };

            Retail_BE_ChargingPolicyService.editChargingPolicy(chargingPolicy.Entity.ChargingPolicyId, onChargingPolicyUpdated);
        }
        function hasEditChargingPolicyPermission() {
            return Retail_BE_ChargingPolicyAPIService.HasUpdateChargingPolicyPermission();
        }
    }
}]);
