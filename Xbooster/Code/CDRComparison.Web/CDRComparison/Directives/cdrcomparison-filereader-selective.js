(function (app) {

    'use strict';

    FileReaderSelectiveDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'UtilsService', 'VRUIUtilsService'];

    function FileReaderSelectiveDirective(CDRComparison_CDRComparisonAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var fileReaderSelective = new FileReaderSelective($scope, ctrl, $attrs);
                fileReaderSelective.initializeController();
            },
            controllerAs: "fileReaderCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/CDRComparison/Directives/Templates/FileReaderSelectiveTemplate.html"
        };

        function FileReaderSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var configId;

                    if (payload != undefined) {
                        configId = payload.ConfigId;
                        directivePayload = payload;
                    }

                    var getFileReaderTemplateConfigsPromise = getFileReaderTemplateConfigs();
                    promises.push(getFileReaderTemplateConfigsPromise);

                    var loadDirectiveDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadDirectiveDeferred.promise);

                    getFileReaderTemplateConfigsPromise.then(function () {

                        if (configId != undefined) {
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, configId, 'ExtensionConfigurationId');

                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, loadDirectiveDeferred);
                            });
                        }
                        else {
                            loadDirectiveDeferred.resolve();
                        }
                    });

                    return UtilsService.waitMultiplePromises(promises);

                    function getFileReaderTemplateConfigs() {
                        return CDRComparison_CDRComparisonAPIService.GetFileReaderTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                            }
                        });
                    }
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined) {
                        data = directiveAPI.getData();
                        data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                    }
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('cdrcomparisonFilereaderSelective', FileReaderSelectiveDirective);

})(app);