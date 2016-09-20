"use strict";

app.directive("vrPstnBeSetarea", ["NormalizationRuleAPIService", "UtilsService", "VRNotificationService", "VRUIUtilsService", function (NormalizationRuleAPIService, UtilsService, VRNotificationService, VRUIUtilsService) {

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
        var readyPromiseDeferred = UtilsService.createPromiseDeferred();

        function defineScope() {

            $scope.templates = [];
            $scope.selectedTemplate = undefined;

            $scope.onDirectiveReady = function (api) {
                setAreaSettingsDirectiveAPI = api;
                readyPromiseDeferred.resolve();
            }
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                $scope.loadingTemplates = true;

                return NormalizationRuleAPIService.GetNormalizationRuleSetAreaSettingsTemplates()
                    .then(function (response) {
                        angular.forEach(response, function (item) {
                            $scope.templates.push(item);
                        });

                        if (payload != undefined)
                            $scope.selectedTemplate = UtilsService.getItemByVal($scope.templates, payload.ConfigId, "ExtensionConfigurationId");
                        var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        readyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(setAreaSettingsDirectiveAPI, payload, directiveLoadPromiseDeferred);
                        });

                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    })
                    .finally(function () {
                        $scope.loadingTemplates = false;
                    });
            };


            api.getData = function () {
                var data = setAreaSettingsDirectiveAPI.getData();
                data.ConfigId = $scope.selectedTemplate.ExtensionConfigurationId;

                return data;
            };


            api.validateData = function () {
                if (setAreaSettingsDirectiveAPI == undefined)
                    return false;

                return setAreaSettingsDirectiveAPI.validateData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);

        }
    }

    return directiveDefinitionObj;

}]);
