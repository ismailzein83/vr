(function (app) {

    'use strict';

    ProvisionerRuntimesettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerRuntimesettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProvisionerRuntimesettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisionSiteRuntimeSettingsTemplate.html"

        };
        function ProvisionerRuntimesettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var provisionSiteSettingsAPI;
            var provisionSiteSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onProvisionSiteSettingsReady = function (api) {
                    provisionSiteSettingsAPI = api;
                    provisionSiteSettingsReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([provisionSiteSettingsReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var provisionerDefinitionSettings; 
                    var provisionerRuntimeSettings;
                    if (payload != undefined) {
                        provisionerDefinitionSettings = payload.provisionerDefinitionSettings;
                        mainPayload = payload;
                        provisionerRuntimeSettings = payload.provisionerRuntimeSettings;
                    }

                    var promises = [];

                    promises.push(loadProvisionSiteSettings());

                    function loadProvisionSiteSettings() {

                        var provisionSiteSettingsPayload = { showEnterpriseSettings: false };
                        if (provisionerDefinitionSettings != undefined)
                            provisionSiteSettingsPayload.provisionAccountSettings = provisionerDefinitionSettings.Settings;
                        if (provisionerRuntimeSettings != undefined) {
                            provisionSiteSettingsPayload.provisionAccountSettings = provisionerRuntimeSettings.Settings;
                        }
                        return provisionSiteSettingsAPI.load(provisionSiteSettingsPayload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.Teles.Business.Provisioning.ProvisionSiteRuntimeSettings,Retail.Teles.Business",
                        Settings: provisionSiteSettingsAPI.getData()
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerRuntimesettingsProvisionsite', ProvisionerRuntimesettingsDirective);

})(app);