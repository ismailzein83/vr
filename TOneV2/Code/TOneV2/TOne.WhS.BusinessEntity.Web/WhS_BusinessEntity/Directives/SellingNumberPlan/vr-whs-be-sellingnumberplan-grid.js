﻿"use strict";

app.directive("vrWhsBeSellingnumberplanGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SellingNumberPlanAPIService", "WhS_BE_SellingNumberPlanService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, WhS_BE_SellingNumberPlanAPIService, WhS_BE_SellingNumberPlanService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var sellingNumberPlanGrid = new SellingNumberPlanGrid($scope, ctrl, $attrs);
            sellingNumberPlanGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SellingNumberPlan/Templates/SellingNumberPlanGridTemplate.html"

    };

    function SellingNumberPlanGrid($scope, ctrl) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.sellingnumberplans = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = WhS_BE_SellingNumberPlanService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onSellingNumberPlanAdded = function (sellingNumberPlanObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(sellingNumberPlanObject);
                        gridAPI.itemAdded(sellingNumberPlanObject);
                    }


                    directiveAPI.onSellingNumberPlanUpdated = function (sellingNumberPlanObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(sellingNumberPlanObject);
                        gridAPI.itemUpdated(sellingNumberPlanObject);
                    }
                    return directiveAPI;
                }
            };
            defineMenuActions();
        }
        
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return WhS_BE_SellingNumberPlanAPIService.GetFilteredSellingNumberPlans(dataRetrievalInput)
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
       
        function defineMenuActions() {

            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editSellingNumberPlan
            }];
        }

        function editSellingNumberPlan(sellingNumberPlanObj) {
            var onSellingNumberPlanAdded = function (sellingNumberPlanObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(sellingNumberPlanObj);
                gridAPI.itemUpdated(sellingNumberPlanObj);
            }

            WhS_BE_SellingNumberPlanService.editSellingNumberPlan(sellingNumberPlanObj.Entity.SellingNumberPlanId, onSellingNumberPlanAdded);
        }
    }

   
    
    return directiveDefinitionObject;

}]);