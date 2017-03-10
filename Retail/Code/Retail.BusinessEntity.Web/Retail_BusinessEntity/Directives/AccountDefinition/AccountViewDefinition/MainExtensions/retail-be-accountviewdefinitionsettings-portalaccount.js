"use strict";

app.directive("retailBeAccountviewdefinitionsettingsPortalaccount", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PortalAccountViewDefinitionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/MainExtensions/Templates/PortalAccountViewSettingsTemplate.html"
        };

        function PortalAccountViewDefinitionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var nameAccountGenericFieldDefinitionSelectorAPI;
            var nameAccountGenericFieldDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var emailAccountGenericFieldDefinitionSelectorAPI;
            var emailAccountGenericFieldDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var connectionSelectorAPI;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var configurePermissionAPI;
            var configurePermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var resetPasswordPermissionAPI;
            var resetPasswordPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onNameAccountGenericFieldDefinitionSelectorReady = function (api) {
                    nameAccountGenericFieldDefinitionSelectorAPI = api;
                    nameAccountGenericFieldDefinitionSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onEmailAccountGenericFieldDefinitionSelectorReady = function (api) {
                    emailAccountGenericFieldDefinitionSelectorAPI = api;
                    emailAccountGenericFieldDefinitionSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorAPI = api;
                    connectionSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;
                    viewPermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onConfigureRequiredPermissionReady = function (api) {
                    configurePermissionAPI = api;
                    configurePermissionReadyDeferred.resolve();
                };
                $scope.scopeModel.onResetPasswordRequiredPermissionReady = function (api) {
                    resetPasswordPermissionAPI = api;
                    resetPasswordPermissionReadyDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionId;
                    var name;
                    var email;
                    var connectionId;
                    var tenantId;
                    var security;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;

                        if (payload.accountViewDefinitionSettings != undefined) {
                            name = payload.accountViewDefinitionSettings.AccountNameMappingField;
                            email = payload.accountViewDefinitionSettings.AccountEmailMappingField;
                            connectionId = payload.accountViewDefinitionSettings.ConnectionId;
                            tenantId = payload.accountViewDefinitionSettings.TenantId;
                            security = payload.accountViewDefinitionSettings.Security || undefined;
                        }
                    }

                    //Loading Name selector
                    var nameAccountGenericFieldDefinitionSelectorLoadPromise = getNameAccountGenericFieldDefinitionSelectorLoadPromise();
                    promises.push(nameAccountGenericFieldDefinitionSelectorLoadPromise);

                    //Loading Email selector
                    var emailAccountGenericFieldDefinitionSelectorLoadPromise = getEmailAccountGenericFieldDefinitionSelectorLoadPromise();
                    promises.push(emailAccountGenericFieldDefinitionSelectorLoadPromise);

                    //Loading ConnectionId selector
                    var connectionSelectorLoadPromise = getConnectionSelectorLoadPromise();
                    promises.push(connectionSelectorLoadPromise);

                    var loadViewRequiredPermissionPromise = loadViewRequiredPermission();
                    promises.push(loadViewRequiredPermissionPromise);

                    var loadConfigureRequiredPermissionPromise = loadConfigureRequiredPermission();
                    promises.push(loadConfigureRequiredPermissionPromise);

                    var loadResetPasswordRequiredPermissionPromise = loadResetPasswordRequiredPermission();
                    promises.push(loadResetPasswordRequiredPermissionPromise);


                    //Loading TenantId
                    $scope.scopeModel.tenantId = tenantId != undefined ? tenantId : 1;

                     
                    function getNameAccountGenericFieldDefinitionSelectorLoadPromise() {
                        var nameAccountGenericFieldDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        nameAccountGenericFieldDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var nameAccountGenericFieldDefinitionSelectorPayload = {
                                accountBEDefinitionId: accountBEDefinitionId
                            };
                            if (name != undefined) {
                                nameAccountGenericFieldDefinitionSelectorPayload.genericFieldDefinition = { Name: name }
                            };
                            VRUIUtilsService.callDirectiveLoad(nameAccountGenericFieldDefinitionSelectorAPI, nameAccountGenericFieldDefinitionSelectorPayload, nameAccountGenericFieldDefinitionSelectorLoadDeferred);
                        });

                        return nameAccountGenericFieldDefinitionSelectorLoadDeferred.promise;
                    }
                    function getEmailAccountGenericFieldDefinitionSelectorLoadPromise() {
                        var emailAccountGenericFieldDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        nameAccountGenericFieldDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var emailAccountGenericFieldDefinitionSelectorPayload = {
                                accountBEDefinitionId: accountBEDefinitionId
                            };
                            if (email != undefined) {
                                emailAccountGenericFieldDefinitionSelectorPayload.genericFieldDefinition = { Name: email }
                            };
                            VRUIUtilsService.callDirectiveLoad(emailAccountGenericFieldDefinitionSelectorAPI, emailAccountGenericFieldDefinitionSelectorPayload, emailAccountGenericFieldDefinitionSelectorLoadDeferred);
                        });

                        return emailAccountGenericFieldDefinitionSelectorLoadDeferred.promise;
                    }
                    function getConnectionSelectorLoadPromise() {
                        var connectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        connectionSelectorPromiseDeferred.promise.then(function () {

                            var connectionSelectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.Common.Business.VRInterAppRestConnectionFilter, Vanrise.Common.Business"
                                    }]
                                },
                                accountBEDefinitionId: accountBEDefinitionId
                            };
                            if (connectionId != undefined) {
                                connectionSelectorPayload.selectedIds = connectionId;
                            };
                            VRUIUtilsService.callDirectiveLoad(connectionSelectorAPI, connectionSelectorPayload, connectionSelectorLoadDeferred);
                        });

                        return connectionSelectorLoadDeferred.promise;
                    }
                    function loadViewRequiredPermission() {
                        var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: security &&  security.ViewRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, dataPayload, viewSettingPermissionLoadDeferred);
                        });
                        return viewSettingPermissionLoadDeferred.promise;
                    }


                    function loadConfigureRequiredPermission() {
                        var congfigurePermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        configurePermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: security && security.ConfigureRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(configurePermissionAPI, dataPayload, congfigurePermissionLoadDeferred);
                        });
                        return congfigurePermissionLoadDeferred.promise;
                    }

                    function loadResetPasswordRequiredPermission() {
                        var resetPasswordPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        resetPasswordPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: security &&  security.ResetPasswordRequiredPermission || undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(resetPasswordPermissionAPI, dataPayload, resetPasswordPermissionLoadDeferred);
                        });
                        return resetPasswordPermissionLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var nameAccountGenericField = nameAccountGenericFieldDefinitionSelectorAPI.getData();
                    var emailAccountGenericField = emailAccountGenericFieldDefinitionSelectorAPI.getData();

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.PortalAccount.PortalAccount, Retail.BusinessEntity.MainExtensions",
                        AccountNameMappingField: nameAccountGenericField != undefined ? nameAccountGenericField.Name : undefined,
                        AccountEmailMappingField: emailAccountGenericField != undefined ? emailAccountGenericField.Name : undefined,
                        ConnectionId: connectionSelectorAPI.getSelectedIds(),
                        TenantId: $scope.scopeModel.tenantId,
                        Security: {
                            ViewRequiredPermission :viewPermissionAPI.getData(),
                            ConfigureRequiredPermission: configurePermissionAPI.getData(),
                            ResetPasswordRequiredPermission: resetPasswordPermissionAPI.getData()
                        }
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);