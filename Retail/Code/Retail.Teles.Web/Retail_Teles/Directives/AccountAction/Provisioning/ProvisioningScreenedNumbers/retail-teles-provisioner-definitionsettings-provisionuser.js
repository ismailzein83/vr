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

            var loginNameSelectorAPI;
            var loginNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var firstNameSelectorAPI;
            var firstNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var lastNameSelectorAPI;
            var lastNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();


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
            
                $scope.scopeModel.onLoginNameSelectorReady = function (api) {
                    loginNameSelectorAPI = api;
                    loginNameSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onFirstNameSelectorReady = function (api) {
                    firstNameSelectorAPI = api;
                    firstNameSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onLastNameSelectorReady = function (api) {
                    lastNameSelectorAPI = api;
                    lastNameSelectorReadyDeferred.resolve();
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
                UtilsService.waitMultiplePromises([loginNameSelectorReadyDeferred.promise, firstNameSelectorReadyDeferred.promise, lastNameSelectorReadyDeferred.promise, connectionTypeReadyDeferred.promise, companyTypeReadyDeferred.promise, siteTypeReadyDeferred.promise, userTypeReadyDeferred.promise]).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
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

                    promises.push(loadLoginNameSelector());
                    function loadLoginNameSelector() {
                        var loginNameSelectorPayload;
                        if (provisionerDefinitionSettings != undefined) {
                            loginNameSelectorPayload = { provisionUserSettings: provisionerDefinitionSettings.LoginNameField };
                        }
                        return loginNameSelectorAPI.load(loginNameSelectorPayload);
                    }
                    promises.push(loadFirstNameSelector());
                    function loadFirstNameSelector() {
                        var firstNameSelectorPayload;
                        if (provisionerDefinitionSettings != undefined) {
                            firstNameSelectorPayload = { provisionUserSettings: provisionerDefinitionSettings.FirstNameField };
                        }
                        return firstNameSelectorAPI.load(firstNameSelectorPayload);
                    }
                    promises.push(loadLastNameSelector());
                    function loadLastNameSelector() {
                        var lastNameSelectorPayload;
                        if (provisionerDefinitionSettings != undefined) {
                            lastNameSelectorPayload = { provisionUserSettings: provisionerDefinitionSettings.LastNameField };
                        }
                        return lastNameSelectorAPI.load(lastNameSelectorPayload);
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
                        LastNameField: lastNameSelectorAPI.getSelectedIds(),
                        FirstNameField:firstNameSelectorAPI.getSelectedIds(),
                        LoginNameField:loginNameSelectorAPI.getSelectedIds(),
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