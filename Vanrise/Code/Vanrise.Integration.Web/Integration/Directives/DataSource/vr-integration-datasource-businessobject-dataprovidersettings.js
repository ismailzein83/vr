"use strict";
app.directive("vrIntegrationDatasourceBusinessobjectDataprovidersettings", [
function () {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var businessObject = new BusinessObject($scope, ctrl, $attrs);
            businessObject.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/Integration/Directives/DataSource/Templates/DataSourceBusinessObjectDataProviderSettingsTemplate.html'
    };


    function BusinessObject($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.Integration.MainExtensions.BusinessObjectDataStore.DataSourceBusinessObjectDataProviderSettings, Vanrise.Integration.MainExtensions"
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}
]);