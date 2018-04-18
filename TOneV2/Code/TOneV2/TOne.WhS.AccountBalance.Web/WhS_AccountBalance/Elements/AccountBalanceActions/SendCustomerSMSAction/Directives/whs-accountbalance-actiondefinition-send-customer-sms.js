'use strict';

app.directive('whsAccountbalanceActiondefinitionSendCustomerSms', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SendCustomerSMSActionDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceActions/SendCustomerSMSAction/Directives/Templates/SendCustomerSMSActionDefinition.html'
        };

        function SendCustomerSMSActionDefinition($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var accountSMSMessageTypeSelectorAPI;
            var accountSMSMessageTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var profileSMSMessageTypeSelectorAPI;
            var profileSMSMessageTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var smsSendHanderDirectiveAPI;
            var smsSendHanderDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onAccountSMSMessageTypeSelectorReady = function (api) {
                    accountSMSMessageTypeSelectorAPI = api;
                    accountSMSMessageTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onProfileSMSMessageTypeSelectorReady = function (api) {
                    profileSMSMessageTypeSelectorAPI = api;
                    profileSMSMessageTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onSMSSendHandlerReady = function (api) {
                    smsSendHanderDirectiveAPI = api;
                    smsSendHanderDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(loadAccountSMSMessageTypeSelector());
                    promises.push(loadProfileSMSMessageTypeSelector());
                    promises.push(loadSMSSendHandlerDirective());

                    function loadAccountSMSMessageTypeSelector() {
                        var accountSMSMessageTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        accountSMSMessageTypeSelectorReadyDeferred.promise.then(function () {

                            var accountSMSMessageTypeSelectorPayload;

                            if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined)
                                accountSMSMessageTypeSelectorPayload = {
                                    selectedIds: payload.Settings.ExtendedSettings.AccountSMSMessageTypeId
                                };

                            VRUIUtilsService.callDirectiveLoad(accountSMSMessageTypeSelectorAPI, accountSMSMessageTypeSelectorPayload, accountSMSMessageTypeSelectorLoadDeferred);
                        });
                        return accountSMSMessageTypeSelectorLoadDeferred.promise;
                    }

                    function loadProfileSMSMessageTypeSelector() {
                        var profileSMSMessageTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        profileSMSMessageTypeSelectorReadyDeferred.promise.then(function () {

                            var profileSMSMessageTypeSelectorPayload;

                            if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined)
                                profileSMSMessageTypeSelectorPayload = {
                                    selectedIds: payload.Settings.ExtendedSettings.ProfileSMSMessageTypeId
                                };

                            VRUIUtilsService.callDirectiveLoad(profileSMSMessageTypeSelectorAPI, profileSMSMessageTypeSelectorPayload, profileSMSMessageTypeSelectorLoadDeferred);
                        });
                        return profileSMSMessageTypeSelectorLoadDeferred.promise;
                    }

                    function loadSMSSendHandlerDirective() {
                        var smsSendHandlerDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        smsSendHanderDirectiveReadyDeferred.promise.then(function () {

                            var smsSendHandlerDirectivePayload;

                            if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined)
                                smsSendHandlerDirectivePayload = {
                                    Handler: payload.Settings.ExtendedSettings.Handler
                                };

                            VRUIUtilsService.callDirectiveLoad(smsSendHanderDirectiveAPI, smsSendHandlerDirectivePayload, smsSendHandlerDirectiveLoadDeferred);
                        });
                        return smsSendHandlerDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions.SendCustomerSMSActionDefinition, TOne.WhS.AccountBalance.MainExtensions',
                        AccountSMSMessageTypeId: accountSMSMessageTypeSelectorAPI.getSelectedIds(),
                        ProfileSMSMessageTypeId: profileSMSMessageTypeSelectorAPI.getSelectedIds(),
                        Handler: smsSendHanderDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);