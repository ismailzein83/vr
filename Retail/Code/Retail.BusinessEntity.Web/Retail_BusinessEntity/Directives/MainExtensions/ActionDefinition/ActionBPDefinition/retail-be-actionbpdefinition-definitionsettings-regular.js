(function (app) {

    'use strict';

    RegularActionbpdefinitionDefinitionsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function RegularActionbpdefinitionDefinitionsettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var regularActionbpdefinitionDefinitionsettings = new RegularActionbpdefinitionDefinitionsettings($scope, ctrl, $attrs);
                regularActionbpdefinitionDefinitionsettings.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ActionDefinition/ActionBPDefinition/Templates/RegularActionBPDefinitionSettingsTemplate.html"

        };
        function RegularActionbpdefinitionDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var provisionerdefinitionSettingAPI;
            var provisionerdefinitionSettingReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onProvisionerDefinitionSettingReady = function (api) {
            
                    provisionerdefinitionSettingAPI = api;
                    provisionerdefinitionSettingReadyDeferred.resolve();
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                    }
                    var provisionerdefinitionSettingLoadDeferred = UtilsService.createPromiseDeferred();

                    provisionerdefinitionSettingReadyDeferred.promise.then(function () {
                        var definitionSettingPayload = payload != undefined && payload.bpDefinitionSettings != undefined ? { provisionerDefinitionSettings: payload.bpDefinitionSettings.ProvisionerDefinitionSettings } : undefined
                        VRUIUtilsService.callDirectiveLoad(provisionerdefinitionSettingAPI, definitionSettingPayload, provisionerdefinitionSettingLoadDeferred);
                    });
                    return provisionerdefinitionSettingLoadDeferred.promise;
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainActionBPs.Entities.RegularActionBPDefinitionSettings, Retail.BusinessEntity.MainActionBPs.Entities",
                        ProvisionerDefinitionSettings: provisionerdefinitionSettingAPI.getData()
                    }
                    return data;
                }
            }
        }
    }

    app.directive('retailBeActionbpdefinitionDefinitionsettingsRegular', RegularActionbpdefinitionDefinitionsettingsDirective);

})(app);