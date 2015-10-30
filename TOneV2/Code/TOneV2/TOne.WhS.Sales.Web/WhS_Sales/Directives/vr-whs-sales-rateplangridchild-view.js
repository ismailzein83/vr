"use strict";

app.directive("vrWhsSalesRateplangridchildView", ["UtilsService", function (UtilsService) {

    var directiveDefinitionObj = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope) {
            var ctrl = this;

            var directiveController = new DirectiveController($scope, ctrl);
            directiveController.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "Client/Modules/WhS_Sales/Directives/Templates/RatePlanGridChildViewTemplate.html"
    };

    function DirectiveController($scope, ctrl) {
        this.initializeController = initializeController;

        function initializeController() {
            $scope.choices = [
                { title: "Choice 1", editor: "directive", loaded: false },
                { title: "Choice 2", editor: "directive", loaded: false },
                { title: "Choice 3", editor: "directive", loaded: false }
            ];

            $scope.selectedChoiceIndex = 0;
            $scope.selectedChoice = $scope.choices[0];

            $scope.onChoiceSelectionChanged = function () {
                if ($scope.selectedChoiceIndex != undefined) {
                    $scope.selectedChoice = $scope.choices[$scope.selectedChoiceIndex];
                    
                    if (!$scope.selectedChoice.loaded) {
                        $scope.selectedChoice.loaded = true;
                        console.log("loaded");
                    }
                }
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                ctrl.onReady(buildDirectiveAPI());
            }

            function buildDirectiveAPI() {
                var api = {};

                api.setHeader = function (newHeader) {
                    $scope.header = newHeader;
                };

                return api;
            }
        }
    }

    return directiveDefinitionObj;

}]);
