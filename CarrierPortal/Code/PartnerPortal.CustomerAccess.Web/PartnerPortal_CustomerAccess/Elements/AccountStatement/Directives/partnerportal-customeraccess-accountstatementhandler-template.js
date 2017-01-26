'use strict';

app.directive('partnerportalCustomeraccessAccountstatementhandlerTemplate', ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new accountStatementHandlerCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/PartnerPortal_CustomerAccess/Elements/AccountStatement/Directives/Templates/AccountSatementHandlerTemplate.html'
    };


    function accountStatementHandlerCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;
        $scope.scopeModel = {};

        var accountStatementHandlerSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var accountStatementHandlerSelectorDirectiveApi;

        var directiveReadyPromiseDeferred;
        var directiveApi;

        function initializeController() {

            $scope.scopeModel.onAccountStatementHandlerSelectorReady = function (api) {
                accountStatementHandlerSelectorDirectiveApi = api;
                accountStatementHandlerSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveApi = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isDirectiveLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveApi, undefined, setLoader, directiveReadyPromiseDeferred);
            };
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var loadAccountStatementHandlerPromiseDeferred = UtilsService.createPromiseDeferred();

                accountStatementHandlerSelectorReadyPromiseDeferred.promise.then(function () {
                    var accountStatementHandlerPayload;
                    if (payload != undefined) {
                        accountStatementHandlerPayload = { selectedIds: payload.ConfigId };
                    }
                    VRUIUtilsService.callDirectiveLoad(accountStatementHandlerSelectorDirectiveApi, accountStatementHandlerPayload, loadAccountStatementHandlerPromiseDeferred);
                });
                promises.push(loadAccountStatementHandlerPromiseDeferred.promise);

                if (payload != undefined) {
                    directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyPromiseDeferred.promise.then(function () {
                        directiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(directiveApi, payload, directiveLoadDeferred);
                    });

                    promises.push(directiveLoadDeferred.promise);
                }

                return UtilsService.waitMultiplePromises(promises);
            };


            api.getData = function () {
                return directiveApi.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };
    }

    return directiveDefinitionObject;
}]);