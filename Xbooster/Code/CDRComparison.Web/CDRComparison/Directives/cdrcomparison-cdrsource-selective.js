(function (app) {

    'use strict';

    CDRSourceSelectiveDirective.$inject = ['CDRComparison_CDRComparisonAPIService', 'CDRComparison_CDRSourceAPIService', 'CDRComparison_CDRSourceConfigAPIService', 'CDRComparison_CDRSourceTimeUnitEnum', 'UtilsService', 'VRUIUtilsService'];

    function CDRSourceSelectiveDirective(CDRComparison_CDRComparisonAPIService, CDRComparison_CDRSourceAPIService, CDRComparison_CDRSourceConfigAPIService, CDRComparison_CDRSourceTimeUnitEnum, UtilsService, VRUIUtilsService) {
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
                $scope.scopeModel.durationTimeUnits = UtilsService.getArrayEnum(CDRComparison_CDRSourceTimeUnitEnum);
                $scope.scopeModel.selectedDurationTimeUnit = UtilsService.getItemByVal($scope.scopeModel.durationTimeUnits, CDRComparison_CDRSourceTimeUnitEnum.Seconds.value, 'value');

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
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

                    var cdrSourceConfigId;
                    var cdrSource;

                    if (payload != undefined) {
                        cdrSourceConfigId = payload.cdrSourceConfigId;
                    }

                    var getCDRSourceTemplateConfigsPromise = getCDRSourceTemplateConfigs();
                    promises.push(getCDRSourceTemplateConfigsPromise);

                    if (cdrSourceConfigId != undefined) {
                        var loadSelfDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadSelfDeferred.promise);

                        getCDRSourceConfig().then(function () {
                            UtilsService.waitMultipleAsyncOperations([loadDirective, loadNormalizationRules, loadStaticControls]).then(function () {
                                loadSelfDeferred.resolve();
                            }).catch(function (error) {
                                loadSelfDeferred.reject();
                            });
                        });
                    }
                    else {
                        extendDirectivePayload();

                        var loadNormalizationRulesPromise = loadNormalizationRules();
                        promises.push(loadNormalizationRulesPromise);
                    }

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
                    function getCDRSourceConfig() {
                        return CDRComparison_CDRSourceConfigAPIService.GetCDRSourceConfig(cdrSourceConfigId).then(function (cdrSourceConfig) {
                            cdrSource = cdrSourceConfig.CDRSource;
                        });
                    }
                    function loadDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        getCDRSourceTemplateConfigsPromise.then(function () {
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, cdrSource.ConfigId, 'ExtensionConfigurationId');

                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                directivePayload = cdrSource;
                                extendDirectivePayload();
                                VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                            });
                        });

                        return directiveLoadDeferred.promise;
                    }
                    function loadNormalizationRules() {

                        return UtilsService.waitMultipleAsyncOperations([loadCDPNNormalizationRule, loadCGPNNormalizationRule]);

                        function loadCDPNNormalizationRule()
                        {
                            var cdpnNormalizationRuleLoadDeferred = UtilsService.createPromiseDeferred();

                            var payload = { FieldToNormalize: 'CDPN' };

                            if (cdrSource != undefined && cdrSource.NormalizationRules != null) {
                                var cdpnNormalizationRule = UtilsService.getItemByVal(cdrSource.NormalizationRules, 'CDPN', 'FieldToNormalize');
                                if (cdpnNormalizationRule != null)
                                    payload = cdpnNormalizationRule;
                            }

                            VRUIUtilsService.callDirectiveLoad(cdpnNormalizationRuleDirectiveAPI, payload, cdpnNormalizationRuleLoadDeferred);
                            return cdpnNormalizationRuleLoadDeferred.promise;
                        }
                        function loadCGPNNormalizationRule() {
                            var cgpnNormalizationRuleLoadDeferred = UtilsService.createPromiseDeferred();

                            var payload = { FieldToNormalize: 'CGPN' };

                            if (cdrSource != undefined && cdrSource.NormalizationRules != null) {
                                var cgpnNormalizationRule = UtilsService.getItemByVal(cdrSource.NormalizationRules, 'CGPN', 'FieldToNormalize');
                                if (cgpnNormalizationRule != null)
                                    payload = cgpnNormalizationRule;
                            }

                            VRUIUtilsService.callDirectiveLoad(cgpnNormalizationRuleDirectiveAPI, payload, cgpnNormalizationRuleLoadDeferred);
                            return cgpnNormalizationRuleLoadDeferred.promise;
                        }
                    }
                    function loadStaticControls() {
                        $scope.scopeModel.selectedDurationTimeUnit = UtilsService.getItemByVal($scope.scopeModel.durationTimeUnits, cdrSource.DurationTimeUnit, 'value');
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
                        data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        data.NormalizationRules = getNormalizationRules();
                        data.DurationTimeUnit = $scope.scopeModel.selectedDurationTimeUnit.value
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