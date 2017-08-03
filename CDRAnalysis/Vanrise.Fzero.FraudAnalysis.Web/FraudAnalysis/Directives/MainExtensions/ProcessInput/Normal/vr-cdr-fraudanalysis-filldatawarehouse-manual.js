"use strict";

app.directive("vrCdrFraudanalysisFilldatawarehouseManual", ["VRValidationService", function (VRValidationService) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
           return getDirectiveTemplateUrl();
        }
    };
    function getDirectiveTemplateUrl() {
        return "/Client/Modules/FraudAnalysis/Directives/MainExtensions/ProcessInput/Normal/Templates/FillDataWarehouseTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {

       
        this.initializeController = initializeController;

        

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var yesterday = new Date();
            yesterday.setDate(yesterday.getDate() - 1);

            $scope.fromDate = yesterday;
            $scope.toDate = new Date();

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };

            $scope.createProcessInputObjects = [];

           
            var api = {};
            api.getData = function () {
                $scope.createProcessInputObjects.length = 0;

                $scope.createProcessInputObjects.push({
                    InputArguments: {
                        $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.FillDataWarehouseProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                        FromDate: $scope.fromDate,
                        ToDate: $scope.toDate
                    }
                });

                return $scope.createProcessInputObjects;
            };
           
            api.load = function (payload) {

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
