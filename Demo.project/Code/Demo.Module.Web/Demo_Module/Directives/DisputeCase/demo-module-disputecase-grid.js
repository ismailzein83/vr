"use strict";
app.directive("demoModuleDisputecaseGrid", ["VRNotificationService", "Demo_Module_DisputeCaseAPIService", "LabelColorsEnum",
function (VRNotificationService, Demo_Module_DisputeCaseAPIService, LabelColorsEnum) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var buildingGrid = new DisputeCaseGrid($scope, ctrl, $attrs);
            buildingGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/DisputeCase/templates/DisputeCaseGridTemplate.html"
    };
    function DisputeCaseGrid($scope, ctrl, $attrs) {
        var gridAPI;
        this.initializeController = initializeController;
        function initializeController() {
            $scope.buildings = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveAPI());
                }
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onDisputeCaseAdded = function (building) {
                        gridAPI.itemAdded(building);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Demo_Module_DisputeCaseAPIService.GetFilteredDisputeCases(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            defineMenuActions();
        }
        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: function () { }
            }, {
                name: "Invoice Comparison Result",
                clicked: function () { }
            }, {
                name: "CDR Comparison Result",
                clicked: function () { }
            }, {
                name: "Send Email",
                clicked: function () { }

            }];
        }

        $scope.getStatusColor = function (dataItem) {
       

            if (dataItem.Entity.StatusCode === 1) return LabelColorsEnum.New.color;
            if (dataItem.Entity.StatusCode === 2) return LabelColorsEnum.WarningLevel1.color;
            if (dataItem.Entity.StatusCode === 3) return LabelColorsEnum.Processing.color;
            if (dataItem.Entity.StatusCode === 4) return LabelColorsEnum.Success.color;
            if (dataItem.Entity.StatusCode === 5) return LabelColorsEnum.Error.color;
            else  return LabelColorsEnum.Info.color;

        };


    }
    return directiveDefinitionObject;
}]
    );