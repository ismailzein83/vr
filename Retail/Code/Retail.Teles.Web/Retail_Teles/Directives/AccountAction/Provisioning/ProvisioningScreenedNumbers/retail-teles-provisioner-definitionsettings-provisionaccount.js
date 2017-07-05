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
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisionAccountDefinitionSettingsTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var connectionTypeAPI;
            var connectionTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var provisionAccountSettingsAPI;
            var provisionAccountSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            var companyTypeAPI;
            var companyTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var siteTypeAPI;
            var siteTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
               
                $scope.scopeModel.onConectionTypeReady = function (api) {
                    connectionTypeAPI = api;
                    connectionTypeReadyDeferred.resolve();
                };
            
                $scope.scopeModel.onProvisionAccountSettingsReady = function (api) {
                    provisionAccountSettingsAPI = api;
                    provisionAccountSettingsReadyDeferred.resolve();
                };
                $scope.scopeModel.onCompanyAccountTypeSelectorReady = function (api) {
                    companyTypeAPI = api;
                    companyTypeReadyDeferred.resolve();
                };

                $scope.scopeModel.onSiteAccountTypeSelectorReady = function (api) {
                    siteTypeAPI = api;
                    siteTypeReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([provisionAccountSettingsReadyDeferred.promise, connectionTypeReadyDeferred.promise, companyTypeReadyDeferred.promise, siteTypeReadyDeferred.promise]).then(function () {
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

                    promises.push(loadProvisionAccountSettings());

                    function loadProvisionAccountSettings() {
                        var provisionAccountSettingsPayload;
                        if (provisionerDefinitionSettings != undefined) {
                            provisionAccountSettingsPayload = { provisionAccountSettings: provisionerDefinitionSettings.Settings, showEnterpriseSettings: true };
                        }
                        return provisionAccountSettingsAPI.load(provisionAccountSettingsPayload);
                    }
                    promises.push(loadCompanyTypes());

                    function loadCompanyTypes() {
                        var companyTypePayload;
                        if (provisionerDefinitionSettings != undefined) {
                            companyTypePayload = { selectedIds: provisionerDefinitionSettings.CompanyTypeId };
                        }
                        return companyTypeAPI.load(companyTypePayload);
                    }

                    promises.push(loadSiteTypes());

                    function loadSiteTypes() {
                        var siteTypePayload;
                        if (provisionerDefinitionSettings != undefined) {
                            siteTypePayload = { selectedIds: provisionerDefinitionSettings.SiteTypeId };
                        }
                        return siteTypeAPI.load(siteTypePayload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.Teles.Business.Provisioning.ProvisionAccountDefinitionSettings,Retail.Teles.Business",
                        VRConnectionId: connectionTypeAPI.getSelectedIds(),
                        Settings:provisionAccountSettingsAPI.getData(),
                        CountryCode: $scope.scopeModel.countryCode,
                        CompanyTypeId: companyTypeAPI.getSelectedIds(),
                        SiteTypeId: siteTypeAPI.getSelectedIds(),
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerDefinitionsettingsProvisionaccount', ProvisionerDefinitionsettingsDirective);

})(app);