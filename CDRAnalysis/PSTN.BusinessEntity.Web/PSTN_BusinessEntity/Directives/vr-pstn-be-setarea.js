"use strict";

app.directive("vrPstnBeSetArea", ["NormalizationRuleAPIService", "VRNotificationService", function (NormalizationRuleAPIService, VRNotificationService) {

    var directiveDefinitionObj = {
        restrict: "E",
        scope: {
            onloaded: "="
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
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/SetAreaDirectiveTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        // public members

        this.initializeController = initializeController;

        function initializeController() {
            defineScope();

            loadActionTemplates().then(function () {
                defineAPI();
            });
        }

        // private members

        var directiveAPI;

        function defineScope() {

            $scope.templates = [];
            $scope.selectedTemplate = undefined;
        }

        function loadTemplates() {
            $scope.loadingTemplates = true;

            return NormalizationRuleAPIService.GetNormalizationRuleSetAreaSettingsTemplates()
                .then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.templates.push(item);
                    });
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.loadingTemplates = false;
                });
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "PSTN.BusinessEntity.Entities.NormalizationRuleSetAreaSettings, PSTN.BusinessEntity.Entities",
                    Data: directiveAPI.getData()
                };
            }

            api.setData = function (setAreaSettings) {
                if (setAreaSettings == undefined || setAreaSettings == null)
                    return;

                directiveAPI.setData(setAreaSettings);
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }
    }

    return directiveDefinitionObj;

}]);
