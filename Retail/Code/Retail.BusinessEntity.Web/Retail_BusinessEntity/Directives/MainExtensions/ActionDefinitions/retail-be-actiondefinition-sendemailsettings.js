'use strict';

app.directive('retailBeActiondefinitionSendemailsettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new sendEmailActionDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/ActionDefinitions/Templates/ActionDefinitionSendEmailSettings.html'
        };

        function sendEmailActionDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var mailMessageTypeAPI;
            var mailMessageTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                    mailMessageTypeAPI = api;
                    mailMessageTypeReadyDeferred.resolve();
                };

                defineAPI();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                        extendedSettings = payload.Settings.ExtendedSettings;
                    }
                    var promises = [];

                    var businessEntityDefinitionSelectorLoadPromise = getBusinessEntityDefinitionSelectorLoadPromise();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);


                    function getBusinessEntityDefinitionSelectorLoadPromise() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                },
                                selectedIds: extendedSettings != undefined ? extendedSettings.AccountBEDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }

                    var mailMessageTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    mailMessageTypeReadyDeferred.promise.then(function () {
                        var mailMessageTypePayload;
                        if (extendedSettings != undefined) {
                            mailMessageTypePayload = { selectedIds: extendedSettings.MailMessageTypeId };
                        }
                        VRUIUtilsService.callDirectiveLoad(mailMessageTypeAPI, mailMessageTypePayload, mailMessageTypeLoadDeferred);
                    });
                    promises.push(mailMessageTypeLoadDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountActions.SendEmailActionDefinitionSettings,Retail.BusinessEntity.MainExtensions',
                        AccountBEDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                        MailMessageTypeId: mailMessageTypeAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);