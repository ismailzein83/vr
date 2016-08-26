'use strict';

app.directive('vrSecMailmessagetemplatesettings', ['VR_Sec_MailMessageTemplateSettingsEnum', 'UtilsService', 'VRUIUtilsService',
    function (VR_Sec_MailMessageTemplateSettingsEnum, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var mailMessageTemplateSettings = new MailMessageTemplateSettings(ctrl, $scope, $attrs);
                mailMessageTemplateSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/Settings/Templates/MailMessageTemplateSettingsTemplate.html"
        };

        function MailMessageTemplateSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var addUserSelectorReadyAPI;
            var addUserSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var resetPasswordSelectorReadyAPI;
            var resetPasswordSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var forgotPasswordSelectorReadyAPI;
            var forgotPasswordSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};

            $scope.scopeModel.onAddUserSelectorReady = function (api) {
                addUserSelectorReadyAPI = api;
                addUserSelectorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onResetPasswordSelectorReady = function (api) {
                resetPasswordSelectorReadyAPI = api;
                resetPasswordSelectorReadyPromiseDeferred.resolve();
            }
            $scope.scopeModel.onForgotPasswordSelectorReady = function (api) {
                forgotPasswordSelectorReadyAPI = api;
                forgotPasswordSelectorReadyPromiseDeferred.resolve();
            }

            function initializeController() {

                var promises = [addUserSelectorReadyPromiseDeferred.promise, resetPasswordSelectorReadyPromiseDeferred.promise, forgotPasswordSelectorReadyPromiseDeferred.promise];

                UtilsService.waitMultiplePromises(promises).then(function(){
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var newUserId;
                    var resetPasswordId;
                    var forgotPasswordId;

                    if (payload != undefined) {
                        newUserId = payload.NewUserId;
                        resetPasswordId = payload.ResetPasswordId;
                        forgotPasswordId = payload.ForgotPasswordId;
                    }


                    //Loading Add User Selector
                    var addUserSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var addUserPayload = {
                        selectedIds: newUserId,
                        filter: { VRMailMessageTypeId: VR_Sec_MailMessageTemplateSettingsEnum.NewUserMailMessageTypeId.value }
                    }
                    VRUIUtilsService.callDirectiveLoad(addUserSelectorReadyAPI, addUserPayload, addUserSelectorLoadPromiseDeferred);
                    promises.push(addUserSelectorLoadPromiseDeferred.promise);

                    //Loading Reset Password Selector
                    var resetPasswordSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var resetPasswordPayload = {
                        selectedIds: resetPasswordId,
                        filter: { VRMailMessageTypeId: VR_Sec_MailMessageTemplateSettingsEnum.ResetPasswordMailMessageTypeId.value }
                    }
                    VRUIUtilsService.callDirectiveLoad(resetPasswordSelectorReadyAPI, resetPasswordPayload, resetPasswordSelectorLoadPromiseDeferred);
                    promises.push(resetPasswordSelectorLoadPromiseDeferred.promise);

                    //Loading Forgot Password Selector
                    var forgotPasswordSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var forgotPasswordPayload = {
                        selectedIds: forgotPasswordId,
                        filter: { VRMailMessageTypeId: VR_Sec_MailMessageTemplateSettingsEnum.ForgotPasswordMailMessageTypeId.value }
                    }
                    VRUIUtilsService.callDirectiveLoad(forgotPasswordSelectorReadyAPI, forgotPasswordPayload, forgotPasswordSelectorLoadPromiseDeferred);
                    promises.push(forgotPasswordSelectorLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        NewUserId: addUserSelectorReadyAPI.getSelectedIds(),
                        ResetPasswordId: resetPasswordSelectorReadyAPI.getSelectedIds(),
                        ForgotPasswordId: forgotPasswordSelectorReadyAPI.getSelectedIds(),
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);