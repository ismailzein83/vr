"use strict";

app.directive('vrWhsCodepreparationCodesgrid', ['VRNotificationService', 'VRUIUtilsService', 'WhS_CodePrep_CodePrepAPIService',
function (VRNotificationService, VRUIUtilsService, WhS_CodePrep_CodePrepAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var ctor = new saleCodesGrid($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CodePreparation/Directives/CodePreparation/Templates/CodePreparationSaleCodesGridTemplate.html"

    };

    function saleCodesGrid($scope, ctrl, $attrs) {
        var gridAPI;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.salecodes = [];
            $scope.ZoneName;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        $scope.ZoneName = query.ZoneName;
                        return gridAPI.retrieveData(query);
                    }

                    directiveAPI.onCodeAdded = function (codeItemObject) {
                        gridAPI.itemAdded(codeItemObject);
                    }

                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_CodePrep_CodePrepAPIService.GetCodeItems(dataRetrievalInput)
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
                name: "Move",
                clicked: moveCode,
            }
            ];
        }

        function moveCode() { }

        
    }

    return directiveDefinitionObject;

}]);
