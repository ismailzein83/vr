'use strict';

app.directive('retailBeChargingpolicyGrid', ['Retail_BE_ChargingPolicyAPIService', 'Retail_BE_ChargingPolicyService', 'Retail_BE_ServiceTypeService', 'UtilsService', 'VRNotificationService', function (Retail_BE_ChargingPolicyAPIService, Retail_BE_ChargingPolicyService, Retail_BE_ServiceTypeService, UtilsService, VRNotificationService) {
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

            $scope.scopeModel.menuActions = function (dataItem)
            {
                var menuActions = buildCommonMenuActions();
                if (dataItem.menuActions != null) {
                    for (var i = 0; i < dataItem.menuActions.length; i++)
                        menuActions.push(dataItem.menuActions[i]);
                }
                return menuActions;
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_ChargingPolicyAPIService.GetFilteredChargingPolicies(dataRetrievalInput).then(function (response) {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var dataItem = response.Data[i];
                            Retail_BE_ServiceTypeService.defineServiceTypeRuleTabsAndMenuActions(dataItem, gridAPI, buildCriteriaFieldValues(dataItem));
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            $scope.scopeModel.showExpandIcon = function (dataItem) {
                return (dataItem.drillDownExtensionObject != null && dataItem.drillDownExtensionObject.drillDownDirectiveTabs.length > 0);
            };
        }
        function defineAPI() {
            var api = {};

            api.load = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onChargingPolicyAdded = function (addedChargingPolicy) {
                Retail_BE_ServiceTypeService.defineServiceTypeRuleTabsAndMenuActions(addedChargingPolicy, gridAPI, buildCriteriaFieldValues(addedChargingPolicy));
                gridAPI.itemAdded(addedChargingPolicy);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function buildCommonMenuActions() {
            return [{
                name: 'Edit',
                clicked: editChargingPolicy,
                haspermission: hasEditChargingPolicyPermission
            }];
        }
        function buildCriteriaFieldValues(dataItem) {
            return {
                'ChargingPolicy': dataItem.Entity.ChargingPolicyId,
                'ServiceType': dataItem.Entity.ServiceTypeId
            };
        }

        function editChargingPolicy(chargingPolicy)
        {
            var onChargingPolicyUpdated = function (updatedChargingPolicy) {
                Retail_BE_ServiceTypeService.defineServiceTypeRuleTabsAndMenuActions(updatedChargingPolicy, gridAPI, buildCriteriaFieldValues(updatedChargingPolicy));
                gridAPI.itemUpdated(updatedChargingPolicy);
            };

            Retail_BE_ChargingPolicyService.editChargingPolicy(chargingPolicy.Entity.ChargingPolicyId, onChargingPolicyUpdated);
        }
        function hasEditChargingPolicyPermission() {
            return Retail_BE_ChargingPolicyAPIService.HasUpdateChargingPolicyPermission();
        }
    }
}]);
