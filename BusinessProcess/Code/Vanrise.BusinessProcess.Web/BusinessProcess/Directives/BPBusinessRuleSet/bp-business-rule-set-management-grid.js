"use strict";

app.directive("businessprocessBpBusinessRuleSetManagementGrid", ["UtilsService", "VRNotificationService", "BusinessProcess_BPBusinessRuleSetAPIService", "BusinessProcess_BPBusinessRuleSetService", "VRUIUtilsService", "BusinessProcess_BPSchedulerTaskService",
function (UtilsService, VRNotificationService, BusinessProcess_BPBusinessRuleSetAPIService, BusinessProcess_BPBusinessRuleSetService, VRUIUtilsService, BusinessProcess_BPSchedulerTaskService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var bpBusinessRuleSetManagementGrid = new BPBusinessRuleSetManagementGrid($scope, ctrl, $attrs);
            bpBusinessRuleSetManagementGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPBusinessRuleSet/Templates/BPBusinessRuleSetGridTemplate.html"

    };

    function BPBusinessRuleSetManagementGrid($scope, ctrl, $attrs) {

        var gridAPI;


        this.initializeController = initializeController;

        function initializeController() {

            $scope.bpBusinessRules = [];
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

                    directiveAPI.onBusinessRuleSetAdded = function (businessRuleSetObj) {
                        gridAPI.itemAdded(businessRuleSetObj);
                    }
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return BusinessProcess_BPBusinessRuleSetAPIService.GetFilteredBPBusinessRuleSets(dataRetrievalInput)
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
                name: "Edit", clicked: editBPBusinessRuleSet,
                haspermission: hasEditBusinessRuleSet
            }];
        }

        function hasEditBusinessRuleSet() {
            return BusinessProcess_BPBusinessRuleSetAPIService.HasEditBusinessRuleSet();
        }

        function editBPBusinessRuleSet(bpBusinessRuleSetObj) {
            var onBusinessRuleSetUpdated = function (updatedItem) {
                gridAPI.itemUpdated(updatedItem);
            };
            BusinessProcess_BPBusinessRuleSetService.editBPBusinessRuleSet(bpBusinessRuleSetObj.Entity.BPBusinessRuleSetId, onBusinessRuleSetUpdated);
        };
    }

    return directiveDefinitionObject;

}]);