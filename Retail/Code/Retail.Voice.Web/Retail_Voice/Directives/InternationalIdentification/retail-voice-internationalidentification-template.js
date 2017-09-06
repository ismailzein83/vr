'use strict';

app.directive('retailVoiceInternationalidentificationTemplate', ['UtilsService', 'VRUIUtilsService', 'Retail_Voice_InternationalNumberIdentificationEnum', 
    function (UtilsService, VRUIUtilsService, Retail_Voice_InternationalNumberIdentificationEnum) {

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
            controllerAs: 'internationalCtrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Retail_Voice/Directives/InternationalIdentification/Templates/VoiceInternationalIdentificationTemplate.html'
        };

        function internationalIdentificationCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var internationalIdentificationSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var internationalIdentificationSelectorDirectiveApi;

            var directiveReadyPromiseDeferred;
            var directiveApi;

            function initializeController() {
                $scope.scopeModel = {};

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

                    $scope.scopeModel.internationalIdentificationNumber = UtilsService.getArrayEnum(Retail_Voice_InternationalNumberIdentificationEnum);
                    $scope.scopeModel.selectedInternationalIdentificationNumber = payload != undefined ?
                        UtilsService.getItemByVal($scope.scopeModel.internationalIdentificationNumber, payload.InternationalNumberIdentification, 'value') : undefined

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

                    var directiveWrapperData = directiveApi.getData();
                    if (directiveWrapperData == undefined)
                        return;

                    directiveWrapperData.InternationalNumberIdentification = $scope.scopeModel.selectedInternationalIdentificationNumber.value;
                    return directiveWrapperData;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
        }

        return directiveDefinitionObject;
    }]);