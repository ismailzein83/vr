﻿(function (app) {

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
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisionUserRuntimeSettingsTemplate.html"

        };
        function ProvisionerRuntimesettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var domainsDirectiveAPI;
            var domainsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedDomainDeferred;

            var enterprisesDirectiveAPI;
            var enterprisesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedEnterpriseDeferred;

            var sitesDirectiveAPI;
            var sitesDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var selectedSiteDeferred;

            var gatewaysDirectiveAPI;
            var gatewaysDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


            var provisionUserSettingsAPI;
            var provisionUserSettingsReadyDeferred = UtilsService.createPromiseDeferred();
            var provisionerDefinitionSettings;

            var accountId;
            var accountBEDefinitionId;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDomainsDirectiveReady = function (api) {
                    domainsDirectiveAPI = api;
                    domainsDirectiveReadyDeferred.resolve();
                };
                $scope.scopeModel.onDomainsSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedDomainDeferred != undefined)
                            selectedDomainDeferred.resolve();
                        else {
                            var payload = {
                                vrConnectionId: provisionerDefinitionSettings.VRConnectionId,
                                filter: {
                                   // AccountBEDefinitionId: accountBEDefinitionId,
                                    TelesDomainId: domainsDirectiveAPI.getSelectedIds(),
                                }
                            };
                            var setLoader = function (value) {
                                $scope.scopeModel.isEnterprisesSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, enterprisesDirectiveAPI, payload, setLoader, undefined);
                            sitesDirectiveAPI.clearDataSource();
                            gatewaysDirectiveAPI.clearDataSource();
                        }

                    }
                };
                $scope.scopeModel.onEnterprisesDirectiveReady = function (api) {
                    enterprisesDirectiveAPI = api;
                    enterprisesDirectiveReadyDeferred.resolve();
                };
                $scope.scopeModel.onEnterprisesSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedEnterpriseDeferred != undefined)
                            selectedEnterpriseDeferred.resolve();
                        else {
                            var payload = {
                                vrConnectionId: provisionerDefinitionSettings.VRConnectionId,
                                enterpriseId: enterprisesDirectiveAPI.getSelectedIds(),
                                filter: {
                                   // AccountBEDefinitionId: accountBEDefinitionId,
                                }
                            };
                            var setLoader = function (value) {
                                $scope.scopeModel.isSitesSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sitesDirectiveAPI, payload, setLoader, undefined);
                            gatewaysDirectiveAPI.clearDataSource();
                        }
                    }
                };
                $scope.scopeModel.onSitesDirectiveReady = function (api) {
                    sitesDirectiveAPI = api;
                    sitesDirectiveReadyDeferred.resolve();
                };
                $scope.scopeModel.onSitesSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedSiteDeferred != undefined)
                            selectedSiteDeferred.resolve();
                        else {
                            var payload = {
                                vrConnectionId: provisionerDefinitionSettings.VRConnectionId,
                                siteId: sitesDirectiveAPI.getSelectedIds(),
                                filter: {
                                   // AccountBEDefinitionId: accountBEDefinitionId,
                                }
                            };
                            var setLoader = function (value) {
                                $scope.scopeModel.isGatewaysSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, gatewaysDirectiveAPI, payload, setLoader, undefined);
                        }

                    }
                };
                $scope.scopeModel.onGatewaysDirectiveReady = function (api) {
                    gatewaysDirectiveAPI = api;
                    gatewaysDirectiveReadyDeferred.resolve();
                };
                $scope.scopeModel.onProvisionUserSettingsReady = function (api) {
                    provisionUserSettingsAPI = api;
                    provisionUserSettingsReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([provisionUserSettingsReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var provisionerRuntimeSettings;
                    if (payload != undefined) {
                        accountId = payload.accountId;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        provisionerDefinitionSettings = payload.provisionerDefinitionSettings;
                        mainPayload = payload;
                        provisionerRuntimeSettings = payload.provisionerRuntimeSettings;
                        if (provisionerRuntimeSettings != undefined) {
                            selectedDomainDeferred = UtilsService.createPromiseDeferred();
                            selectedEnterpriseDeferred = UtilsService.createPromiseDeferred();
                            selectedSiteDeferred = UtilsService.createPromiseDeferred();
                        }
                    }


                    function loadDomainsDirective() {
                        var domainsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        domainsDirectiveReadyDeferred.promise.then(function () {
                            var domainsDirectivePayload;
                            if (provisionerDefinitionSettings != undefined) {
                                domainsDirectivePayload = {
                                    vrConnectionId: provisionerDefinitionSettings.VRConnectionId,
                                    selectedIds: provisionerRuntimeSettings != undefined ? provisionerRuntimeSettings.TelesDomainId : undefined,
                                    filter: {
                                      //  AccountBEDefinitionId: accountBEDefinitionId
                                    }
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(domainsDirectiveAPI, domainsDirectivePayload, domainsDirectiveLoadDeferred);
                        });

                        return domainsDirectiveLoadDeferred.promise;
                    }
                    function loadEnterprisesDirective() {
                        if (provisionerRuntimeSettings != undefined) {
                            var enterprisesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                            UtilsService.waitMultiplePromises([enterprisesDirectiveReadyDeferred.promise, selectedDomainDeferred.promise]).then(function () {
                                selectedDomainDeferred = undefined;
                                var enterprisesDirectivePayload;
                                if (provisionerDefinitionSettings != undefined) {
                                    enterprisesDirectivePayload = {
                                        vrConnectionId: provisionerDefinitionSettings.VRConnectionId,
                                        selectedIds: provisionerRuntimeSettings != undefined ? provisionerRuntimeSettings.TelesEnterpriseId : undefined,
                                        filter: {
                                           // AccountBEDefinitionId: accountBEDefinitionId,
                                            TelesDomainId: provisionerRuntimeSettings != undefined ? provisionerRuntimeSettings.TelesDomainId : undefined,
                                        }
                                    };
                                }
                                VRUIUtilsService.callDirectiveLoad(enterprisesDirectiveAPI, enterprisesDirectivePayload, enterprisesDirectiveLoadDeferred);
                            });

                            return enterprisesDirectiveLoadDeferred.promise;
                        }

                    }
                    function loadSitesDirective() {
                        if (provisionerRuntimeSettings != undefined) {
                            var sitesDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                            UtilsService.waitMultiplePromises([sitesDirectiveReadyDeferred.promise, selectedEnterpriseDeferred.promise]).then(function () {
                                selectedEnterpriseDeferred = undefined;
                                var sitesDirectivePayload;
                                if (provisionerDefinitionSettings != undefined) {
                                    sitesDirectivePayload = {
                                        vrConnectionId: provisionerDefinitionSettings.VRConnectionId,
                                        selectedIds: provisionerRuntimeSettings != undefined ? provisionerRuntimeSettings.TelesSiteId : undefined,
                                        enterpriseId: provisionerRuntimeSettings != undefined ? provisionerRuntimeSettings.TelesEnterpriseId : undefined,
                                        filter: {
                                          //  AccountBEDefinitionId: accountBEDefinitionId,
                                        }
                                    };
                                }
                                VRUIUtilsService.callDirectiveLoad(sitesDirectiveAPI, sitesDirectivePayload, sitesDirectiveLoadDeferred);
                            });

                            return sitesDirectiveLoadDeferred.promise;
                        }

                    }
                    function loadGatwaysDirective() {
                        if (provisionerRuntimeSettings != undefined) {
                            var gatwaysDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                            UtilsService.waitMultiplePromises([gatwaysDirectiveReadyDeferred.promise, selectedSiteDeferred.promise]).then(function () {
                                selectedSiteDeferred = undefined;
                                var gatwaysDirectivePayload;
                                if (provisionerDefinitionSettings != undefined) {
                                    gatwaysDirectivePayload = {
                                        vrConnectionId: provisionerDefinitionSettings.VRConnectionId,
                                        selectedIds: provisionerRuntimeSettings != undefined ? provisionerRuntimeSettings.TelesGatewayId : undefined,
                                        siteId: provisionerRuntimeSettings != undefined ? provisionerRuntimeSettings.TelesGatewayId : undefined,
                                        filter: {
                                           // AccountBEDefinitionId: accountBEDefinitionId,
                                        }
                                    };
                                }
                                VRUIUtilsService.callDirectiveLoad(gatwaysDirectiveAPI, gatwaysDirectivePayload, gatwaysDirectiveLoadDeferred);
                            });

                            return gatwaysDirectiveLoadDeferred.promise;
                        }

                    }


                    function loadProvisionUserSettings() {

                        var provisionUserSettingsPayload = {
                            accountId: accountId,
                            accountBEDefinitionId: accountBEDefinitionId
                        };
                        if (provisionerDefinitionSettings != undefined)
                            provisionUserSettingsPayload.provisionerDefinitionSettings = provisionerDefinitionSettings;
                        if (provisionerRuntimeSettings != undefined) {
                            provisionUserSettingsPayload.provisionerRuntimeSettings = provisionerRuntimeSettings.Settings;
                        }
                        return provisionUserSettingsAPI.load(provisionUserSettingsPayload);
                    }

                    return UtilsService.waitMultipleAsyncOperations([loadDomainsDirective, loadEnterprisesDirective, loadSitesDirective, loadGatwaysDirective, loadProvisionUserSettings]);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.Teles.Business.Provisioning.ProvisionUserRuntimeSettings,Retail.Teles.Business",
                        Settings: provisionUserSettingsAPI.getData(),
                        TelesDomainId: domainsDirectiveAPI.getSelectedIds(),
                        TelesEnterpriseId:enterprisesDirectiveAPI.getSelectedIds(),
                        TelesSiteId: sitesDirectiveAPI.getSelectedIds(),
                        TelesGatewayId: gatewaysDirectiveAPI.getSelectedIds()
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerRuntimesettingsProvisionuser', ProvisionerRuntimesettingsDirective);

})(app);