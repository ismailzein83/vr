'use strict';

app.directive('vrSecSecurityproviderFinduserRemoteprovider', ['UtilsService', 'VR_Sec_SecurityProviderAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_Sec_SecurityProviderAPIService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RemoteproviderCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/MainExtensions/SecurityProvider/FindUser/Templates/RemoteProviderTemplate.html"
        };

        function RemoteproviderCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var emailSelectorAPI;
            var emailSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var emailSelectorSelectedDeferred;

            var context;

            var connectionId;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEmailSelectorReady = function (api) {
                    emailSelectorAPI = api;
                    emailSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onEmailSelectorChanged = function (selectedObject) {
                    if (selectedObject != undefined) {
                        if (emailSelectorSelectedDeferred != undefined) {
                            emailSelectorSelectedDeferred.resolve();
                        }
                        else {
                            if (context != undefined && context.fillUserInfo != undefined && typeof (context.fillUserInfo) == "function") {
                                var userInfo = { name: selectedObject.Name, description: selectedObject.Description };
                                context.fillUserInfo(userInfo);
                            }
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var securityProviderId = payload.securityProviderId;
                    var email = payload.email;
                    context = payload.context;

                    var rootPromiseNode = {
                        promises: [getSecurityProviderPromise(), emailSelectorReadyDeferred.promise],
                        getChildNode: function () {
                            var loadEmailSelectorPromise = getEmailSelectorLoadPromise();
                            return {
                                promises: [loadEmailSelectorPromise]
                            };
                        }
                    };

                    var getEmailSelectorLoadPromise = function () {
                        var emailSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        var emailSelectorPayload = { connectionId: connectionId };
                        if (email != undefined) {
                            emailSelectorSelectedDeferred = UtilsService.createPromiseDeferred();
                            emailSelectorPayload.selectedIds = email;
                        }
                        VRUIUtilsService.callDirectiveLoad(emailSelectorAPI, emailSelectorPayload, emailSelectorLoadPromiseDeferred);
                        return emailSelectorLoadPromiseDeferred.promise;
                    };

                    function getSecurityProviderPromise() {
                        return VR_Sec_SecurityProviderAPIService.GetSecurityProviderbyId(securityProviderId).then(function (response) {
                            connectionId = response.Settings.ExtendedSettings.VRConnectionId;
                        });
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };


                api.getData = function () {
                    return {
                        email: emailSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }
]);