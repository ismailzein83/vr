"use strict";

app.directive("retailTelesTelestemplateentitysettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var telesTemplateEntityEntitySettings = new TelesTemplateEntityEntitySettings($scope, ctrl);
            telesTemplateEntityEntitySettings.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Retail_Teles/Directives/TelesTemplateEntity/Templates/TelesTemplateEntityEntitySettings.html"
    };

    function TelesTemplateEntityEntitySettings($scope, ctrl) {

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                }
            
                return UtilsService.waitMultiplePromises(promises);
            };

           

            api.getData = function () {
                return {
                    $type: "Retail.Teles.Entities.TelesTemplateEntitySettings ,Retail.Teles.Entities",
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
    return directiveDefinitionObject;

}]);