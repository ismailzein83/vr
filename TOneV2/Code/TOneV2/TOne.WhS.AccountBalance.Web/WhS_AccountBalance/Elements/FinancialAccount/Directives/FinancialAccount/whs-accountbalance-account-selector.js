'use strict';

app.directive('whsAccountbalanceAccountSelector', ['UtilsService', function (UtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var accountSelectorCtrl = this;
            var accountSelector = new AccountSelector($scope, accountSelectorCtrl, $attrs);
            accountSelector.initializeController();
        },
        controllerAs: "accountSelectorCtrl",
        bindToController: true,
        template: function (element, attributes) {
            return getTemplate(attributes);
        }
    };

    function AccountSelector($scope, accountSelectorCtrl, $attrs) {

        this.initializeController = initializeController;

        var financialAccountSelectorAPI;
        var financialAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.effectiveOnly = true;
            $scope.scopeModel.financialAccounts = [];

            $scope.scopeModel.onFinancialAccountSelectorReady = function (api) {
                financialAccountSelectorAPI = api;
                financialAccountSelectorReadyDeferred.resolve();
            };

            UtilsService.waitMultiplePromises([financialAccountSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                financialAccountSelectorAPI.clearDataSource();

                var accountTypeId;
                var extendedSettings;

                if (payload != undefined) {
                    accountTypeId = payload.accountTypeId;
                    extendedSettings = payload.extendedSettings;
                }
            };

            api.getData = function () {
            };

            if (accountSelectorCtrl.onReady != null) {
                accountSelectorCtrl.onReady(api);
            }
        }
    }
    function getTemplate(attributes) {
        return '<vr-columns colnum="{{accountSelectorCtrl.normalColNum / 3}}">\
                    <vr-switch label="Effective Only" value="scopeModel.effectiveOnly"></vr-switch>\
                </vr-columns>\
                <vr-columns colnum="{{accountSelectorCtrl.normalColNum}}">\
                    <vr-select on-ready="scopeModel.onFinancialAccountSelectorReady"\
				        label="Financial Accounts"\
				        datasource="scopeModel.financialAccounts"\
				        datavaluefield="FinancialAccountId"\
				        datatextfield="Name"\
				        isrequired="accountSelectorCtrl.isrequired"\
				        hideremoveicon="accountSelectorCtrl.isrequired">\
			        </vr-select>\
                </vr-columns>';
    }
}]);