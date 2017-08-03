"use strict";

app.directive("vrCdrPstnbeStagingtocdrManual", ["VRValidationService", function (VRValidationService) {
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
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/MainExtensions/ProcessInput/Normal/Templates/StagingToCDRManualTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        this.initializeController = initializeController; 

        function initializeController() {

            var yesterday = new Date();
            yesterday.setDate(yesterday.getDate() - 1);

            $scope.fromDate = yesterday;
            $scope.toDate = new Date();

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };

            $scope.createProcessInputObjects = [];


            defineAPI();
        }

        function defineAPI() {
            
            var api = {};

            api.getData = function () {

                $scope.createProcessInputObjects.length = 0;
                $scope.createProcessInputObjects.push({
                    InputArguments: {
                        $type: "Vanrise.Fzero.CDRImport.BP.Arguments.StagingtoCDRProcessInput, Vanrise.Fzero.CDRImport.BP.Arguments",
                        FromDate: new Date($scope.fromDate),
                        ToDate: new Date($scope.toDate)
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
