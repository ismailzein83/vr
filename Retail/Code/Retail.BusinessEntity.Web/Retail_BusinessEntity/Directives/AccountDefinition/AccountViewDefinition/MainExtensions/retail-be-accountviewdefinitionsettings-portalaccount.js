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

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        name = payload.accountViewDefinitionSettings != undefined ? payload.accountViewDefinitionSettings.Name : undefined;
                        email = payload.accountViewDefinitionSettings != undefined ? payload.accountViewDefinitionSettings.Email : undefined;
                        connectionId = payload.accountViewDefinitionSettings != undefined ? payload.accountViewDefinitionSettings.ConnectionId : undefined;
                    }

                    //Loading Name selector
                    var nameAccountGenericFieldDefinitionSelectorLoadPromise = getNameAccountGenericFieldDefinitionSelectorLoadPromise();
                    promises.push(nameAccountGenericFieldDefinitionSelectorLoadPromise);

                    //Loading Email selector
                    var emailAccountGenericFieldDefinitionSelectorLoadPromise = getEmailAccountGenericFieldDefinitionSelectorLoadPromise();
                    promises.push(emailAccountGenericFieldDefinitionSelectorLoadPromise);

                    //Loading Email selector
                    var connectionSelectorLoadPromise = getConnectionSelectorLoadPromise();
                    promises.push(connectionSelectorLoadPromise);


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

                        nameAccountGenericFieldDefinitionSelectorPromiseDeferred.promise.then(function () {

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

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var nameAccountGenericField = nameAccountGenericFieldDefinitionSelectorAPI.getData();
                    var emailAccountGenericField = emailAccountGenericFieldDefinitionSelectorAPI.getData();

                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.AccountViews.PortalAccount, Retail.BusinessEntity.MainExtensions",
                        Name: nameAccountGenericField != undefined ? nameAccountGenericField.Name : undefined,
                        Email: emailAccountGenericField != undefined ? emailAccountGenericField.Name : undefined,
                        ConnectionId: connectionSelectorAPI.getSelectedIds()
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