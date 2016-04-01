(function (app) {

    'use strict';

    CDRSourceSelectiveDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'CDRComparison_CDRSourceAPIService', 'UtilsService', 'VRUIUtilsService'];

    function CDRSourceSelectiveDirective(CDRComparison_CDRComparisonAPIService, CDRComparison_CDRSourceAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var cdrSourceSelective = new CDRSourceSelective($scope, ctrl, $attrs);
                cdrSourceSelective.initializeController();
            },
            controllerAs: "sourceCtrl",
            bindToController: true,
            templateUrl: "/Client/Modules/CDRComparison/Directives/Templates/CDRSourceSelectiveTemplate.html"
        };

        function CDRSourceSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var selectorReadyDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            var cdpnNormalizationRuleDirectiveAPI;
            var cdpnNormalizationRuleDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var cgpnNormalizationRuleDirectiveAPI;
            var cgpnNormalizationRuleDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

                $scope.scopeModel.cdpnNormalizationRuleDirectiveReady = function (api) {
                    cdpnNormalizationRuleDirectiveAPI = api;
                    cdpnNormalizationRuleDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.cgpnNormalizationRuleDirectiveReady = function (api) {
                    cgpnNormalizationRuleDirectiveAPI = api;
                    cgpnNormalizationRuleDirectiveReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([selectorReadyDeferred.promise, cdpnNormalizationRuleDirectiveReadyDeferred.promise, cgpnNormalizationRuleDirectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var configId;
                    var normalizationSettings;

                    if (payload != undefined) {
                        configId = payload.ConfigId;
                        directivePayload = payload;
                        normalizationSettings = payload.NormalizationSettings;
                    }

                    extendDirectivePayload();

                    var getCDRSourceTemplateConfigsPromise = getCDRSourceTemplateConfigs();
                    promises.push(getCDRSourceTemplateConfigsPromise);

                    var loadDirectiveDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadDirectiveDeferred.promise);

                    var loadCDPNNormalizationRulePromise = loadCDPNNormalizationRule();
                    promises.push(loadCDPNNormalizationRulePromise);

                    var loadCGPNNormalizationRulePromise = loadCGPNNormalizationRule();
                    promises.push(loadCGPNNormalizationRulePromise);

                    getCDRSourceTemplateConfigsPromise.then(function () {
                        if (configId != undefined) {
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, configId, 'ConfigId');

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

                    function getCDRSourceTemplateConfigs() {
                        return CDRComparison_CDRComparisonAPIService.GetCDRSourceTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                            }
                        });
                    }
                    function loadCDPNNormalizationRule() {
                        var cdpnNormalizationRuleLoadDeferred = UtilsService.createPromiseDeferred();
                        var payload = {
                            FieldToNormalize: 'CDPN'
                        };
                        VRUIUtilsService.callDirectiveLoad(cdpnNormalizationRuleDirectiveAPI, payload, cdpnNormalizationRuleLoadDeferred);
                        return cdpnNormalizationRuleLoadDeferred.promise;
                    }
                    function loadCGPNNormalizationRule() {
                        var cgpnNormalizationRuleLoadDeferred = UtilsService.createPromiseDeferred();
                        var payload = {
                            FieldToNormalize: 'CGPN'
                        };
                        VRUIUtilsService.callDirectiveLoad(cgpnNormalizationRuleDirectiveAPI, payload, cgpnNormalizationRuleLoadDeferred);
                        return cgpnNormalizationRuleLoadDeferred.promise;
                    }
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function extendDirectivePayload() {
                    if (directivePayload == undefined) { directivePayload = {}; }
                    directivePayload.cdrSourceContext = {};
                    directivePayload.cdrSourceContext.readSample = function () {
                        return CDRComparison_CDRSourceAPIService.ReadSample(getData());
                    };
                }

                function getData() {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined) {
                        data = directiveAPI.getData();
                        data.ConfigId = $scope.scopeModel.selectedTemplateConfig.TemplateConfigID;
                        data.NormalizationRules = getNormalizationRules();
                    }
                    return data;

                    function getNormalizationRules() {
                        var normalizationRules = [];

                        var cdpnNormalizationRule = cdpnNormalizationRuleDirectiveAPI.getData();
                        var cgpnNormalizationRule = cgpnNormalizationRuleDirectiveAPI.getData();

                        if (cdpnNormalizationRule != undefined) {
                            normalizationRules.push(cdpnNormalizationRule);
                        }

                        if (cgpnNormalizationRule != undefined) {
                            normalizationRules.push(cgpnNormalizationRule);
                        }

                        return (normalizationRules.length > 0) ? normalizationRules : undefined;
                    }
                }
            }
        }
    }

    app.directive('cdrcomparisonCdrsourceSelective', CDRSourceSelectiveDirective);

})(app);