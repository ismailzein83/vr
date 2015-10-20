﻿"use strict";

app.directive("vrPstnBeSetarea", ["NormalizationRuleAPIService", "UtilsService", "VRNotificationService", function (NormalizationRuleAPIService, UtilsService, VRNotificationService) {

    var directiveDefinitionObj = {
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
        templateUrl: "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/SetAreaDirectiveTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        // public members

        this.initializeController = initializeController;

        function initializeController() {
            defineScope();
            defineAPI();
        }

        // private members

        var setAreaSettingsDirectiveAPI;
        var setAreaSettings;

        function defineScope() {

            $scope.templates = [];
            $scope.selectedTemplate = undefined;

            $scope.onDirectiveLoaded = function (api) {
                setAreaSettingsDirectiveAPI = api;
                
                if (setAreaSettings != undefined) {
                    setAreaSettingsDirectiveAPI.setData(setAreaSettings);
                }
            }
        }

        function defineAPI() {
            var api = {};

            api.load = function () {
                return loadTemplates();
            };

            api.getData = function () {
                var data = setAreaSettingsDirectiveAPI.getData();
                data.ConfigId = $scope.selectedTemplate.TemplateConfigID;

                return data;
            }

            api.setData = function (data) {
                if (setAreaSettingsDirectiveAPI != undefined) {
                    setAreaSettingsDirectiveAPI.setData(setAreaSettings);
                }
                else {
                    setAreaSettings = data;
                    $scope.selectedTemplate = UtilsService.getItemByVal($scope.templates, data.ConfigId, "TemplateConfigID");
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);

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
        }
    }

    return directiveDefinitionObj;

}]);
