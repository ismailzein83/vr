"use strict";
               
app.directive("retailTelesTelestemplateentitydefinitionsettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var telesTemplateEntityEntityDefinitionSetting = new TelesTemplateEntityEntityDefinitionSettingDirective($scope, ctrl);
            telesTemplateEntityEntityDefinitionSetting.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_Teles/Directives/TelesTemplateEntity/Definition/Templates/TelesTemplateEntityEntityDefinitionSettingDirective.html"
    };

    function TelesTemplateEntityEntityDefinitionSettingDirective($scope, ctrl) {

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: "Retail.Teles.Entities.TelesTemplateEntityDefinitionSettings ,Retail.Teles.Entities",
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
    return directiveDefinitionObject;

}]);