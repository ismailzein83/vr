'use strict';

app.directive('whsAccountbalanceActionCustomerSendemail', ['UtilsService', function (UtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var sendMailCustomerAction = new SendMailCustomerAction($scope, ctrl, $attrs);
            sendMailCustomerAction.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceActions/SendCustomerEmailAction/Directives/Templates/SendCustomerEmailAction.html'
    };

    function SendMailCustomerAction($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var accountMailMessageTemplateSelectorAPI;
        var accountMailMessageTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var profileMailMessageTemplateSelectorAPI;
        var profileMailMessageTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountMailMessageTemplateSelectorReady = function (api) {
                accountMailMessageTemplateSelectorAPI = api;
                accountMailMessageTemplateSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onProfileMailMessageTemplateSelectorReady = function (api) {
                profileMailMessageTemplateSelectorAPI = api;
                profileMailMessageTemplateSelectorReadyDeferred.resolve();
            };

            UtilsService.waitMultiplePromises([accountMailMessageTemplateSelectorReadyDeferred.promise, profileMailMessageTemplateSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                var extendedSettings;

                if (payload != undefined && payload.selectedVRActionDefinition != undefined && payload.selectedVRActionDefinition.Settings != null) {
                    extendedSettings = payload.selectedVRActionDefinition.Settings.ExtendedSettings;
                }
                var promises = [];
                
                promises.push(loadAccountMailMessageTemplateSelector());
                promises.push(loadProfileMailMessageTemplateSelector());

                function loadAccountMailMessageTemplateSelector() {
                    var accountMailMessageTemplatePayload = {
                        filter: {
                            VRMailMessageTypeId: extendedSettings.AccountMessageTypeId
                        },
                        selectedIds: payload.vrActionEntity != undefined ? payload.vrActionEntity.AccountMailTemplateId : undefined

                    };
                    return accountMailMessageTemplateSelectorAPI.load(accountMailMessageTemplatePayload);
                }
                function loadProfileMailMessageTemplateSelector() {
                    var profileMailMessageTemplatePayload = {
                        filter: {
                            VRMailMessageTypeId: extendedSettings.ProfileMessageTypeId
                        },
                        selectedIds: payload.vrActionEntity != undefined ? payload.vrActionEntity.ProfileMailTemplateId : undefined
                    };
                    return profileMailMessageTemplateSelectorAPI.load(profileMailMessageTemplatePayload);
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.AccountBalance.MainExtensions.SendCustomerEmailAction, TOne.WhS.AccountBalance.MainExtensions',
                    ActionName: 'Customer Send Email',
                    AccountMailTemplateId: accountMailMessageTemplateSelectorAPI.getSelectedIds(),
                    ProfileMailTemplateId: profileMailMessageTemplateSelectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);