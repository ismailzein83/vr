'use strict';

app.directive('retailVoiceInternationalidentificationTemplate', ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new internationalIdentificationCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Retail_Voice/Directives/InternationalIdentification/Templates/VoiceInternationalIdentificationTemplate.html'
    };


    function internationalIdentificationCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;
        $scope.scopeModel = {};

        var internationalIdentificationSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var internationalIdentificationSelectorDirectiveApi;

        var directiveReadyPromiseDeferred;
        var directiveApi;

        function initializeController() {

            $scope.scopeModel.onInternationalIdentificationSelectorReady = function (api) {
                internationalIdentificationSelectorDirectiveApi = api;
                internationalIdentificationSelectorReadyPromiseDeferred.resolve();
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

                var loadInternationalIdentificationPromiseDeferred = UtilsService.createPromiseDeferred();

                internationalIdentificationSelectorReadyPromiseDeferred.promise.then(function () {
                    var internationalIdentificationPayload;
                    if (payload != undefined) {
                        internationalIdentificationPayload = { selectedIds: payload.ConfigId };
                    }
                    VRUIUtilsService.callDirectiveLoad(internationalIdentificationSelectorDirectiveApi, internationalIdentificationPayload, loadInternationalIdentificationPromiseDeferred);
                });
                promises.push(loadInternationalIdentificationPromiseDeferred.promise);

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