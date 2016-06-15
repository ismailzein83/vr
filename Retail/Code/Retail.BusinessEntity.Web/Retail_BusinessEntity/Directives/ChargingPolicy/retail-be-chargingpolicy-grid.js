'use strict';

app.directive('retailBeChargingpolicyGrid', ['Retail_BE_ChargingPolicyAPIService', 'Retail_BE_ChargingPolicyService', 'VR_GenericData_GenericRule', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (Retail_BE_ChargingPolicyAPIService, Retail_BE_ChargingPolicyService, VR_GenericData_GenericRule, UtilsService, VRUIUtilsService, VRNotificationService) {
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

            $scope.scopeModel.menuActions = function (dataItem) {
                var menuActions = buildCommonMenuActions();

                if (dataItem.RuleDefinitions == null)
                    return menuActions;

                for (var i = 0; i < dataItem.RuleDefinitions.length; i++) {
                    var ruleDefinition = dataItem.RuleDefinitions[i];
                    var menuAction = buildMenuAction(ruleDefinition);
                    menuActions.push(menuAction);
                }

                function buildMenuAction(ruleDefinition) {
                    var menuAction = {};
                    menuAction.name = 'Add ' + ruleDefinition.Title;
                    menuAction.clicked = function (dataItem) {
                        var onGenericRuleAdded = function (addedGenericRule) {
                            dataItem['ruleGrid' + ruleDefinition.RuleDefinitionId + 'API'].onGenericRuleAdded(dataItem);
                        };
                        var preDefinedData = {
                            criteriaFieldsValues: {
                                'ChargingPolicy': { Values: [dataItem.Entity.ChargingPolicyId] },
                                'ServiceType': { Values: [dataItem.Entity.ServiceTypeId] }
                            }
                        };
                        var accessibility = {
                            criteriaAccessibility: {
                                'ChargingPolicy': { notAccessible: true },
                                'ServiceType': { notAccessible: true }
                            },
                            settingNotAccessible: false
                        };
                        VR_GenericData_GenericRule.addGenericRule(ruleDefinition.RuleDefinitionId, onGenericRuleAdded, preDefinedData, accessibility);
                    };
                    return menuAction;
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
                            var drillDownTabs = buildDrillDownTabs(dataItem.RuleDefinitions);
                            var drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(drillDownTabs, gridAPI, $scope.scopeModel.menuActions);
                            drillDownManager.setDrillDownExtensionObject(dataItem);
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };
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

        function buildCommonMenuActions() {
            return [{
                name: 'Edit',
                clicked: editChargingPolicy,
                haspermission: hasEditChargingPolicyPermission
            }];
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

        function buildDrillDownTabs(ruleDefinitions) {
            var drillDownTabs = [];

            if (ruleDefinitions == null)
                return drillDownTabs;

            for (var i = 0; i < ruleDefinitions.length; i++) {
                var drillDownTab = buildDrillDownTab(ruleDefinitions[i]);
                drillDownTabs.push(drillDownTab);
            }

            function buildDrillDownTab(ruleDefinition) {
                var drillDownTab = {};

                drillDownTab.title = ruleDefinition.Title;
                drillDownTab.directive = 'vr-genericdata-genericrule-grid';

                drillDownTab.loadDirective = function (directiveAPI, dataItem) {
                    var propertyName = 'ruleGrid' + ruleDefinition.RuleDefinitionId + 'API';
                    dataItem[propertyName] = directiveAPI;
                    var ruleGridQuery = { RuleDefinitionId: ruleDefinition.RuleDefinitionId };
                    return directiveAPI.loadGrid(ruleGridQuery);
                };

                return drillDownTab;
            }

            return drillDownTabs;
        }
    }
}]);
