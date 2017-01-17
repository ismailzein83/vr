'use strict';

app.directive('retailVoiceAccountidentificationTemplate', ['UtilsService','VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new accountIdentificationCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Retail_Voice/Directives/AccountIdentification/Templates/VoiceAccountIdentificationTemplate.html'
    };


    function accountIdentificationCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;
        $scope.scopeModel = {};

        var accountIdentificationSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var accountIdentificationSelectorDirectiveApi;

        var directiveReadyPromiseDeferred;
        var directiveApi;

        function initializeController() {

            $scope.scopeModel.onAccountIdentificationSelectorReady = function (api) {
                accountIdentificationSelectorDirectiveApi = api;
                accountIdentificationSelectorReadyPromiseDeferred.resolve();
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
                
                var loadAccountIdentificationPromiseDeferred = UtilsService.createPromiseDeferred();

                accountIdentificationSelectorReadyPromiseDeferred.promise.then(function () {
                    var accountIdentificationPayload;
                    if (payload != undefined) {
                        accountIdentificationPayload = { selectedIds: payload.ConfigId };
                    }
                    VRUIUtilsService.callDirectiveLoad(accountIdentificationSelectorDirectiveApi, accountIdentificationPayload, loadAccountIdentificationPromiseDeferred);
                });
                promises.push(loadAccountIdentificationPromiseDeferred.promise);

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