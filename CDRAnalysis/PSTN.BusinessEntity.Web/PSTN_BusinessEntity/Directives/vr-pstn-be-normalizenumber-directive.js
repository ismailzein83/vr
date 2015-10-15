"use strict";

app.directive("vrPstnBeNormalizenumber", [function () {

    var directiveDefinitionObj = {
        restrict: "E",
        scope: {
            configid: "=",
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
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        function initializeController() {
            defineScope();

            load().then(function () {
                defineAPI();
            });
        }

        // private members

        var normalizationRuleAdjustNumberSettingsDirectiveAPI;

        function defineScope() {

            $scope.templates = [];
            $scope.selectedNormalizationRuleAdjustNumberActionSettingsTemplate = undefined;
            $scope.disableAddButton = true;

            $scope.normalizationRuleAdjustNumberActionSettingsList = [];

            $scope.onTemplateChanged = function () {
                $scope.disableAddButton = ($scope.selectedTemplate == undefined);
            }
        }

        function load() {
            $scope.isLoadingTemplates = true;

            NormalizationRuleAPIService.GetNormalizationRuleAdjustNumberActionSettingsTemplates()
                .then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.normalizationRuleAdjustNumberActionSettingsTemplates.push(item);
                    });
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoadingTemplates = false;
                }); 
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                return {
                    $type: "PSTN.BusinessEntity.Entities.Normalization.NormalizationRuleSettings, PSTN.BusinessEntity.Entities",
                    ConfigId: ctrl.configid
                };
            }

            api.setData = function (normalizationRuleSettings) {

            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }
    }

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/PSTN_BusinessEntity/Directives/Templates/NormalizeNumberDirectiveTemplate.html";
    }

    return directiveDefinitionObj;
    
}]);
