"use strict";

app.directive("vrDemoNationalnumberingplanGrid", ["UtilsService", "VRNotificationService", "Demo_NationalNumberingPlanAPIService", "Demo_NationalNumberingPlanService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, Demo_NationalNumberingPlanAPIService, Demo_NationalNumberingPlanService, VRUIUtilsService) {

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
        this.initializeController = initializeController;

        function initializeController() {

            $scope.nationalNumberingPlans = [];
            defineMenuActions();
            $scope.onGridReady = function (api) {
                gridAPI = api;


                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());


                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onNationalNumberingPlanAdded = function (nationalNumberingPlanObject) {
                        gridAPI.itemAdded(nationalNumberingPlanObject);
                    }
                    return directiveAPI;
                }


            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_NationalNumberingPlanAPIService.GetFilteredNationalNumberingPlans(dataRetrievalInput)
                    .then(function (response) {
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
            }
            ];
        }

        function editNationalNumberingPlan(nationalNumberingPlanObj) {
            var onNationalNumberingPlanUpdated = function (nationalNumberingPlan) {
                gridAPI.itemUpdated(nationalNumberingPlan);

            }
            Demo_NationalNumberingPlanService.editNationalNumberingPlan(nationalNumberingPlanObj, onNationalNumberingPlanUpdated);
        }

    }

    return directiveDefinitionObject;

}]);