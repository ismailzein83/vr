"use strict";

app.directive("vrFzeroFraudanalysisFixedprefixesGrid", ["UtilsService", "VRNotificationService", "Fzero_FraudAnalysis_MainService", "Fzero_FraudAnalysis_DefineFixedPrefixesAPIService",
function (UtilsService, VRNotificationService, Fzero_FraudAnalysis_MainService, Fzero_FraudAnalysis_DefineFixedPrefixesAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var defineFixedPrefixesGrid = new DefineFixedPrefixesGrid($scope, ctrl, $attrs);
            defineFixedPrefixesGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/FixedPrefixes/Templates/DefineFixedPrefixesGrid.html"

    };

    function DefineFixedPrefixesGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.fixedPrefixes = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }
                    directiveAPI.onFixedPrefixAdded = function (fixedPrefixObj) {
                        gridAPI.itemAdded(fixedPrefixObj);
                    }
                    directiveAPI.onFixedPrefixUpdated = function (fixedPrefixObj) {
                        gridAPI.itemUpdated(fixedPrefixObj);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Fzero_FraudAnalysis_DefineFixedPrefixesAPIService.GetFilteredFixedPrefixes(dataRetrievalInput)
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
            var defaultMenuActions = [
                        {
                            name: "Edit",
                            clicked: editFixedPrefix,
                        },
                         {
                             name: "Delete",
                             clicked: deleteFixedPrefix,
                         }
            ];

            $scope.gridMenuActions = function (dataItem) {
                return defaultMenuActions;
            }
        }

        function editFixedPrefix(fixedPrefixObj) {
            var onFixedPrefixUpdated = function (fixedPrefix) {
                gridAPI.itemUpdated(fixedPrefix);
            }

            Fzero_FraudAnalysis_MainService.editFixedPrefix(fixedPrefixObj.Entity, onFixedPrefixUpdated);
        }
        function deleteFixedPrefix(fixedPrefixObj) {
            var onFixedPrefixObjDeleted = function (fixedPrefix) {
                gridAPI.itemDeleted(fixedPrefix);
            };

            Fzero_FraudAnalysis_MainService.deleteFixedPrefix($scope, fixedPrefixObj, onFixedPrefixObjDeleted);
        }
    }

    return directiveDefinitionObject;

}]);
