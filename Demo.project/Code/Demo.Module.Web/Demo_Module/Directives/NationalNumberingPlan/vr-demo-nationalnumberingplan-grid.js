"use strict";

app.directive("vrDemoNationalnumberingplanGrid", ["UtilsService", "VRNotificationService", "Demo_NationalNumberingPlanAPIService", "Demo_OperatorAccountService", "Demo_NationalNumberingPlanService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_NationalNumberingPlanAPIService, Demo_OperatorAccountService, Demo_NationalNumberingPlanService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var nationalNumberingPlanGrid = new NationalNumberingPlanGrid($scope, ctrl, $attrs);
            nationalNumberingPlanGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/NationalNumberingPlan/Templates/NationalNumberingPlanGridTemplate.html"

    };

    function NationalNumberingPlanGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.nationalNumberingPlans = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "NationalNumberingPlan";
                drillDownDefinition.directive = "vr-demo-nationalnumberingplan-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, nationalNumberingPlanItem) {
                    nationalNumberingPlanItem.operatorAccountGridAPI = directiveAPI;
                    var payload = {
                        query: {
                            NationalNumberingPlansIds: [nationalNumberingPlanItem.Entity.NationalNumberingPlanId]
                        },
                        hideProfileColumn: true
                    };
                    return nationalNumberingPlanItem.operatorAccountGridAPI.loadGrid(payload);
                };
                drillDownDefinitions.push(drillDownDefinition);
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onNationalNumberingPlanAdded = function (nationalNumberingPlanObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(nationalNumberingPlanObject);
                        gridAPI.itemAdded(nationalNumberingPlanObject);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_NationalNumberingPlanAPIService.GetFilteredNationalNumberingPlans(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editNationalNumberingPlan,
            },
                {
                    name: "New Operator Account",
                    clicked: addOperatorAccount
                }
            ];
        }

        function editNationalNumberingPlan(nationalNumberingPlanObj) {
            var onNationalNumberingPlanUpdated = function (nationalNumberingPlan) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(nationalNumberingPlan);
                gridAPI.itemUpdated(nationalNumberingPlan);

            }
            Demo_NationalNumberingPlanService.editNationalNumberingPlan(nationalNumberingPlanObj, onNationalNumberingPlanUpdated);
        }

        function addOperatorAccount(dataItem) {
            gridAPI.expandRow(dataItem);
            var query = {
                NationalNumberingPlansIds: [dataItem.NationalNumberingPlanId],
            }

            var onOperatorAccountAdded = function (operatorAccountObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(operatorAccountObj);
                if (dataItem.operatorAccountGridAPI != undefined)
                    dataItem.operatorAccountGridAPI.onOperatorAccountAdded(operatorAccountObj);
            };
            Demo_OperatorAccountService.addOperatorAccount(onOperatorAccountAdded, dataItem.Entity);
        }

        function deleteNationalNumberingPlan(nationalNumberingPlanObj) {
            var onNationalNumberingPlanDeleted = function () {
                //TODO: This is to refresh the Grid after delete, should be removed when centralized
                retrieveData();
            };

            // Demo_MainService.deleteOperatorAccount(nationalNumberingPlanObj, onNationalNumberingPlanDeleted); to be added in OperatorAccountService
        }
    }

    return directiveDefinitionObject;

}]);