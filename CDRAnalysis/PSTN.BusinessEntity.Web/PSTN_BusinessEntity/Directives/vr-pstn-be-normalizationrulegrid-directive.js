"use strict"

app.directive("vrPstnBeNormalizationrulegrid", ["PSTN_BE_Service", "NormalizationRuleAPIService", "VRNotificationService", function (PSTN_BE_Service, NormalizationRuleAPIService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var normalizationRuleGrid = new NormalizationRuleGrid($scope, ctrl);
            normalizationRuleGrid.defineScope();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/NormalizationRuleGridTemplate.html"
    };


    function NormalizationRuleGrid($scope, ctrl) {

        var gridAPI;
        this.defineScope = defineScope;

        function defineScope() {

            $scope.normalizationRules = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};

                    directiveAPI.retrieveData = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onNormalizationRuleAdded = function (trunkObject) {
                        gridAPI.itemAdded(trunkObject);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                return NormalizationRuleAPIService.GetFilteredNormalizationRules(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function editNormalizationRule(dataItem) {
            
            var onNormalizationRuleUpdated = function (normalizationRuleObject) {
                gridAPI.itemUpdated(normalizationRuleObject);
            }

            PSTN_BE_Service.editNormalizationRule(dataItem, onNormalizationRuleUpdated);
        }

        function deleteNormalizationRule(dataItem) {

        }

        function defineMenuActions() {

            $scope.gridMenuActions = [
               {
                   name: "Edit",
                   clicked: editNormalizationRule
               },
               {
                   name: "Delete",
                   clicked: deleteNormalizationRule
               }
            ];
        }
    }

    return directiveDefinitionObject;
}]);