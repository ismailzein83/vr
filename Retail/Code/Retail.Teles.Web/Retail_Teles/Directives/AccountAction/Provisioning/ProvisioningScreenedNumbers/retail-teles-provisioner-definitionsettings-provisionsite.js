(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerDefinitionsettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProvisionerDefinitionsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisionSiteDefinitionSettingsTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var connectionTypeAPI;
            var connectionTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var provisionSiteSettingsAPI;
            var provisionSiteSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
               
                $scope.scopeModel.onConectionTypeReady = function (api) {
                    connectionTypeAPI = api;
                    connectionTypeReadyDeferred.resolve();
                };
            
                $scope.scopeModel.onProvisionSiteSettingsReady = function (api) {
                    provisionSiteSettingsAPI = api;
                    provisionSiteSettingsReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([provisionSiteSettingsReadyDeferred.promise, connectionTypeReadyDeferred.promise]).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var provisionerDefinitionSettings;
                    if (payload != undefined) {
                        mainPayload = payload;
                        provisionerDefinitionSettings = payload.provisionerDefinitionSettings;
                        if (provisionerDefinitionSettings != undefined) {
                            $scope.scopeModel.countryCode = provisionerDefinitionSettings.CountryCode;
                        }
                    }

                    var promises = [];

                    promises.push(loadConectionTypes());

                    function loadConectionTypes() {
                        var connectionTypePayload;
                        if (provisionerDefinitionSettings != undefined) {
                            connectionTypePayload = { selectedIds: provisionerDefinitionSettings.VRConnectionId };
                        }
                        return connectionTypeAPI.load(connectionTypePayload);
                    }

                    promises.push(loadProvisionSiteSettings());

                    function loadProvisionSiteSettings() {
                        var provisionSiteSettingsPayload;
                        if (provisionerDefinitionSettings != undefined) {
                            provisionSiteSettingsPayload = { provisionAccountSettings: provisionerDefinitionSettings.Settings, showEnterpriseSettings: false };
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
                        $type: "Retail.Teles.Business.Provisioning.ProvisionSiteDefinitionSettings,Retail.Teles.Business",
                        VRConnectionId: connectionTypeAPI.getSelectedIds(),
                        Settings: provisionSiteSettingsAPI.getData(),
                        CountryCode: $scope.scopeModel.countryCode,
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerDefinitionsettingsProvisionsite', ProvisionerDefinitionsettingsDirective);

})(app);