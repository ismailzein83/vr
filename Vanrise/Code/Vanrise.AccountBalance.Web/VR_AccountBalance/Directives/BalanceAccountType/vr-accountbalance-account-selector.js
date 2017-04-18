'use strict';

app.directive('vrAccountbalanceAccountSelector', ['VR_AccountBalance_AccountTypeAPIService', 'VRUIUtilsService', 'UtilsService', function (VR_AccountBalance_AccountTypeAPIService, VRUIUtilsService, UtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            onselectionchanged: '=',
            isrequired: "=",
            isdisabled: "=",
            selectedvalues: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new BalanceAccount(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'accountBalanceCtrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function BalanceAccount(accountBalanceCtrl, $scope, attrs) {

        this.initializeController = initializeController;

        var directiveAPI;
        var directiveReadyDeferred;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyDeferred.resolve();
            };
            defineAPI();
           
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                $scope.scopeModel.editor = undefined;
                directiveReadyDeferred = UtilsService.createPromiseDeferred();

                var accountTypeId;
                var accountSelectorContext;

                if (payload != undefined) {
                    accountTypeId = payload.accountTypeId;
                    accountSelectorContext = payload.context;
                }

                var promises = [];
                var accountTypeSettings;
                var extendedSettings;

                var getAccountTypeSettingsPromise = getAccountTypeSettings();
                promises.push(getAccountTypeSettingsPromise);

                var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(directiveLoadDeferred.promise);

                getAccountTypeSettingsPromise.then(function () {
                    if (accountTypeSettings != undefined)
                        extendedSettings = accountTypeSettings.ExtendedSettings;

                    if (extendedSettings != undefined)
                        $scope.scopeModel.editor = extendedSettings.AccountSelector;

                    loadDirective().then(function () {
                        directiveLoadDeferred.resolve();
                    }).catch(function (error) {
                        directiveLoadDeferred.reject(error);
                    });
                });

                function getAccountTypeSettings() {
                    return VR_AccountBalance_AccountTypeAPIService.GetAccountTypeSettings(accountTypeId).then(function (response) {
                        accountTypeSettings = response;
                    });
                }
                function loadDirective() {
                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        var directivePayload = {
                            accountTypeId: accountTypeId,
                            context: accountSelectorContext,
                            extendedSettings: extendedSettings
                        };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    });

                    return directiveLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return directiveAPI.getData();
            };

            if (accountBalanceCtrl.onReady != null)
                accountBalanceCtrl.onReady(api);
        }
    }
    function getTemplate(attrs) {
        var ismultipleselection = "";
        if (attrs.ismultipleselection != undefined) {
            ismultipleselection = "ismultipleselection";
        }
        return '<vr-directivewrapper ng-if="scopeModel.editor != undefined" directive="scopeModel.editor" ' + ismultipleselection + ' on-ready="scopeModel.onDirectiveReady"  normal-col-num="{{accountBalanceCtrl.normalColNum}}" isrequired="accountBalanceCtrl.isrequired"  customvalidate="accountBalanceCtrl.customvalidate"></vr-directivewrapper>';
    }
}]);