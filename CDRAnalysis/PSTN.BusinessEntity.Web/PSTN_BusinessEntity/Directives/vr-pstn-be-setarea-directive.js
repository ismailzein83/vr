"use strict";

app.directive("vrPstnBeSetarea", ["NormalizationRuleAPIService", "UtilsService", "VRNotificationService", function (NormalizationRuleAPIService, UtilsService, VRNotificationService) {

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

            loadTemplates().then(function () {
                defineAPI();
            });
        }

        // private members

        var directiveAPI;
        var directiveData;

        function defineScope() {

            $scope.templates = [];
            $scope.selectedTemplate = undefined;

            $scope.onDirectiveLoaded = function (api) {
                directiveAPI = api;
                
                if (directiveData != undefined) {
                    directiveAPI.setData(directiveData);
                }
            }
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
                var data = directiveAPI.getData();
                data.ConfigId = $scope.selectedTemplate.TemplateConfigID;

                return data;
            }

            api.setData = function (setAreaSettings) {
                if (setAreaSettings == undefined || setAreaSettings == null)
                    return;

                if (directiveAPI != undefined) {
                    directiveAPI.setData(setAreaSettings);
                }
                else {
                    directiveData = setAreaSettings;
                    $scope.selectedTemplate = UtilsService.getItemByVal($scope.templates, setAreaSettings.ConfigId, "TemplateConfigID");
                }
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }
    }

    return directiveDefinitionObj;

}]);
