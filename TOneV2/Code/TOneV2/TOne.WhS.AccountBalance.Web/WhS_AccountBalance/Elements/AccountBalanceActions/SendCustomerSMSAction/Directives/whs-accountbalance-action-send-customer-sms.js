'use strict';

app.directive('whsAccountbalanceActionSendCustomerSms', ['UtilsService', 'VRUIUtilsService',
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
            var ctor = new SendCustomerSMSAction($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceActions/SendCustomerSMSAction/Directives/Templates/SendCustomerSMSAction.html'
    };

    function SendCustomerSMSAction($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var accountSMSMessageTemplateSelectorAPI;
        var accountSMSMessageTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var profileSMSMessageTemplateSelectorAPI;
        var profileSMSMessageTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountSMSMessageTemplateSelectorReady = function (api) {
                accountSMSMessageTemplateSelectorAPI = api;
                accountSMSMessageTemplateSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onProfileSMSMessageTemplateSelectorReady = function (api) {
                profileSMSMessageTemplateSelectorAPI = api;
                profileSMSMessageTemplateSelectorReadyDeferred.resolve();
            };
            defineAPI();
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var promises = [];
               
                promises.push(loadAccountSMSMessageTemplateSelector());
                promises.push(loadProfileSMSMessageTemplateSelector());

                function loadAccountSMSMessageTemplateSelector() {
                    var accountSMSMessageTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    accountSMSMessageTemplateSelectorReadyDeferred.promise.then(function () {

                        var accountSMSMessageTemplateSelectorPayload;

                        if (payload != undefined && payload.selectedVRActionDefinition != undefined && payload.selectedVRActionDefinition.Settings != undefined && payload.selectedVRActionDefinition.Settings.ExtendedSettings != undefined)
                            accountSMSMessageTemplateSelectorPayload = {
                                filter: payload.selectedVRActionDefinition.Settings.ExtendedSettings.AccountSMSMessageTypeId
                            };

                        if(payload != undefined && payload.vrActionEntity != undefined)
                            accountSMSMessageTemplateSelectorPayload.selectedIds = payload.vrActionEntity.AccountSMSTemplateId;

                        VRUIUtilsService.callDirectiveLoad(accountSMSMessageTemplateSelectorAPI, accountSMSMessageTemplateSelectorPayload, accountSMSMessageTemplateSelectorLoadDeferred);
                    });
                    return accountSMSMessageTemplateSelectorLoadDeferred.promise;
                }

                function loadProfileSMSMessageTemplateSelector() {
                    var profileSMSMessageTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    profileSMSMessageTemplateSelectorReadyDeferred.promise.then(function () {

                        var profileSMSMessageTemplateSelectorPayload;

                        if (payload != undefined && payload.selectedVRActionDefinition != undefined && payload.selectedVRActionDefinition.Settings != undefined && payload.selectedVRActionDefinition.Settings.ExtendedSettings != undefined)
                            profileSMSMessageTemplateSelectorPayload = {
                                filter: payload.selectedVRActionDefinition.Settings.ExtendedSettings.ProfileSMSMessageTypeId
                            };

                        if (payload != undefined && payload.vrActionEntity != undefined)
                            profileSMSMessageTemplateSelectorPayload.selectedIds = payload.vrActionEntity.ProfileSMSTemplateId;

                        VRUIUtilsService.callDirectiveLoad(profileSMSMessageTemplateSelectorAPI, profileSMSMessageTemplateSelectorPayload, profileSMSMessageTemplateSelectorLoadDeferred);
                    });
                    return profileSMSMessageTemplateSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.AccountBalance.MainExtensions.VRBalanceAlertActions.SendCustomerSMSAction, TOne.WhS.AccountBalance.MainExtensions',
                    ActionName: 'Send Customer SMS',
                    AccountSMSTemplateId: accountSMSMessageTemplateSelectorAPI.getSelectedIds(),
                    ProfileSMSTemplateId: profileSMSMessageTemplateSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);