'use strict';
app.directive('vrAccountbalanceAccountSelector', ['VR_AccountBalance_AccountTypeAPIService', 'VRUIUtilsService', 'UtilsService',
function (VR_AccountBalance_AccountTypeAPIService, VRUIUtilsService, UtilsService) {

        var directiveDefinitionObject = {
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


        function getTemplate(attrs) {
            var ismultipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                ismultipleselection = "ismultipleselection";
            }
            return '<vr-directivewrapper ng-if="scopeModel.editor != undefined" directive="scopeModel.editor" ' + ismultipleselection + ' on-ready="scopeModel.onDirectiveReady"  normal-col-num="{{accountBalanceCtrl.normalColNum}}" isrequired="accountBalanceCtrl.isrequired"  customvalidate="accountBalanceCtrl.customvalidate"></vr-directivewrapper>';
        }

        function BalanceAccount(accountBalanceCtrl, $scope, attrs) {

            var directiveAPI;
            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            $scope.scopeModel = {};
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyPromiseDeferred.resolve();
            };

            this.initializeController = initializeController;

            function initializeController() {
                if (accountBalanceCtrl.onReady != undefined && typeof (accountBalanceCtrl.onReady) == 'function') {
                    accountBalanceCtrl.onReady(defineAPI());
                }
            }

            function defineAPI() {
                var api = {};
                

                api.load = function (payload) {
                    $scope.scopeModel.editor = undefined ;
                    var accountTypeId;

                    if (payload != undefined) {
                        accountTypeId = payload.accountTypeId;
                    }
                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    VR_AccountBalance_AccountTypeAPIService.GetAccountTypeSettings(accountTypeId).then(function (response) {
                        $scope.scopeModel.accountTypeSettings = response;
                        if ($scope.scopeModel.accountTypeSettings != undefined && $scope.scopeModel.accountTypeSettings.ExtendedSettings != undefined) {

                            $scope.scopeModel.editor = $scope.scopeModel.accountTypeSettings.ExtendedSettings.AccountSelector;
                            directiveReadyPromiseDeferred.promise.then(function () {
                                var directivePayload = response.ExtendedSettings;
                                UtilsService.convertToPromiseIfUndefined(directiveAPI.load(directivePayload)).then(function () {
                                    directiveLoadPromiseDeferred.resolve();
                                }).catch(function (error) {
                                    directiveLoadPromiseDeferred.reject(error);
                                });
                            });
                        }
                    });
                    return directiveLoadPromiseDeferred.promise;

                };

                api.getData = function () {
                    return directiveAPI.getData();
                };

                if (accountBalanceCtrl.onReady != null)
                    accountBalanceCtrl.onReady(api);

                return api;
            }
        }

        return directiveDefinitionObject;

 }]);