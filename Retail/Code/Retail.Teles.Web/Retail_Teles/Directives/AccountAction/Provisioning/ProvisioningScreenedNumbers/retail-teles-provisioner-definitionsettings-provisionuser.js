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
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisionUserDefinitionSettingsTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var connectionTypeAPI;
            var connectionTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var provisionUserSettingsAPI;
            var provisionUserSettingsReadyDeferred = UtilsService.createPromiseDeferred();
            var companyTypeAPI;
            var companyTypeReadyDeferred = UtilsService.createPromiseDeferred();
            var siteTypeAPI;
            var siteTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var userTypeAPI;
            var userTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
               
                $scope.scopeModel.onConectionTypeReady = function (api) {
                    connectionTypeAPI = api;
                    connectionTypeReadyDeferred.resolve();
                };
            
                $scope.scopeModel.onProvisionUserSettingsReady = function (api) {
                    provisionUserSettingsAPI = api;
                    provisionUserSettingsReadyDeferred.resolve();
                };
                $scope.scopeModel.onCompanyAccountTypeSelectorReady = function (api) {
                    companyTypeAPI = api;
                    companyTypeReadyDeferred.resolve();
                };
                $scope.scopeModel.onSiteAccountTypeSelectorReady = function (api) {
                    siteTypeAPI = api;
                    siteTypeReadyDeferred.resolve();
                };
                $scope.scopeModel.onUserAccountTypeSelectorReady = function (api) {
                    userTypeAPI = api;
                    userTypeReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([provisionUserSettingsReadyDeferred.promise, connectionTypeReadyDeferred.promise, companyTypeReadyDeferred.promise,siteTypeReadyDeferred.promise, userTypeReadyDeferred.promise]).then(function () {
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

                    promises.push(loadProvisionUserSettings());
                    function loadProvisionUserSettings() {
                        var provisionUserSettingsPayload;
                        if (provisionerDefinitionSettings != undefined) {
                            provisionUserSettingsPayload = { provisionUserSettings: provisionerDefinitionSettings.Settings };
                        }
                        return provisionUserSettingsAPI.load(provisionUserSettingsPayload);
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

                    promises.push(loadUserTypes());
                    function loadUserTypes() {
                        var userTypePayload;
                        if (provisionerDefinitionSettings != undefined) {
                            userTypePayload = { selectedIds: provisionerDefinitionSettings.UserTypeId };
                        }
                        return userTypeAPI.load(userTypePayload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.Teles.Business.Provisioning.ProvisionUserDefinitionSettings,Retail.Teles.Business",
                        VRConnectionId: connectionTypeAPI.getSelectedIds(),
                        Settings: provisionUserSettingsAPI.getData(),
                        CountryCode: $scope.scopeModel.countryCode,
                        CompanyTypeId: companyTypeAPI.getSelectedIds(),
                        SiteTypeId: siteTypeAPI.getSelectedIds(),
                        UserTypeId: userTypeAPI.getSelectedIds(),
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerDefinitionsettingsProvisionuser', ProvisionerDefinitionsettingsDirective);

})(app);