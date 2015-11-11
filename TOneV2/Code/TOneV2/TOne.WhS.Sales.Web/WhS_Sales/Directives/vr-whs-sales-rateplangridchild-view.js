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

        var choice1DirectiveAPI;

        function initializeController() {
            defineChoices();
            load();

            $scope.onChoiceSelectionChanged = function () {
                if ($scope.selectedChoiceIndex != undefined) {
                    $scope.choices[$scope.selectedChoiceIndex].isLoaded = true;
                }
            };
            
            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                ctrl.onReady(buildDirectiveAPI());
            }

            function buildDirectiveAPI() {
                var api = {};
                return api;
            }

            function load() {

            }

            function defineChoices() {
                $scope.choices = [
                    { title: "Supplier", editor: "vr-whs-be-carrieraccount-selector", isLoaded: true, isSelected: true },
                    { title: "Carrier Profile", editor: "vr-whs-be-carrierprofile-selector", isLoaded: false, isSelected: false },
                    { title: "Country", editor: "vr-common-country-selector", isLoaded: false, isSelected: false }
                ];

                for (var i = 0; i < $scope.choices.length; i++) {
                    defineDirectiveProperties($scope.choices[i]);
                }
            }

            function defineDirectiveProperties(choice) {
                choice.directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                choice.onDirectiveReady = function (api) {
                    choice.directiveAPI = api;
                    choice.directiveReadyPromiseDeferred.resolve();
                    delete choice.onDirectiveReady;
                };

                choice.loadDirective = function () {
                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    choice.directiveReadyPromiseDeferred.promise.then(function () {
                        var directivePayload = {
                            filter: {},
                            selectedIds: undefined
                        };

                        VRUIUtilsService.callDirectiveLoad(choice.directiveAPI, directivePayload, directiveLoadPromiseDeferred);
                    });

                    return directiveLoadPromiseDeferred.promise;
                };
            }
        }
    }

    return directiveDefinitionObj;

}]);
