"use strict";

app.directive("vrWhsDealSalerateevaluatorSelective", ["WhS_Deal_VolCommitmentDealAPIService", "UtilsService", "VRUIUtilsService",
function (WhS_Deal_VolCommitmentDealAPIService, UtilsService, VRUIUtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "=",
            onselectionchanged: '=',
            isrequired: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var selectiveCtrl = this;
            selectiveCtrl.customLabel = $attrs.customlabel != undefined ? $attrs.customlabel : "Rate Evaluator";
            var saleRateEvaluator = new SaleRateEvaluator(selectiveCtrl, $scope);
            saleRateEvaluator.initializeController();
        },
        controllerAs: "selectiveCtrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Deal/Directives/DealRateEvaluator/Templates/DealSaleRateEvaluatorTemplate.html"
    };

    function SaleRateEvaluator(selectiveCtrl, $scope) {
        this.initializeController = initializeController;
        var directiveReadyDeferred;
        var evaluatedRate;
        var directiveAPI;
        var selectorAPI;
        var context;

        $scope.scopeModel = {};

        function initializeController() {
            selectiveCtrl.templateConfigs = [];
            //selectiveCtrl.selectedTemplateConfig;

            selectiveCtrl.onSaleRateEvaluatorSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
            selectiveCtrl.onDirectiveWrapperReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    selectiveCtrl.isLoadingDirective = value;
                };
                var directivePayload = {
                    context: getContext()
                };
                if (evaluatedRate != undefined)
                    directivePayload.evaluatedRate = evaluatedRate;
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
            };
        }
        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                // selectorAPI.clearDataSource();
                var promises = [];
                var getsaleRateEvaluatorConfigurationPromise = getSaleRateEvaluatorConfiguration();
                promises.push(getsaleRateEvaluatorConfigurationPromise);

                if (payload != undefined) {
                    context = payload.context;
                    evaluatedRate = payload.evaluatedRate;
                }
                if (evaluatedRate != undefined) {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadDirective());
                }

                function loadDirective() {
                    var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    directiveReadyDeferred.promise.then(function () {
                        var payloadEntity = {
                            evaluatedRate: evaluatedRate,
                            context: getContext()
                        };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadEntity, loadDirectivePromiseDeferred);
                    });
                    return loadDirectivePromiseDeferred.promise;
                }
                
                function getSaleRateEvaluatorConfiguration() {
                    return WhS_Deal_VolCommitmentDealAPIService.GetSaleRateEvaluatorConfigurationTemplateConfigs().then(
                        function (rateEvaluators) {
                            if (rateEvaluators != null) {
                                for (var i = 0; i < rateEvaluators.length; i++) {
                                    selectiveCtrl.templateConfigs.push(rateEvaluators[i]);
                                }
                            }
                            if (evaluatedRate != undefined) {
                                selectiveCtrl.selectedTemplateConfig = UtilsService.getItemByVal(selectiveCtrl.templateConfigs, evaluatedRate.ConfigId, 'ExtensionConfigurationId');
                            }
                        });
                }

                return UtilsService.waitMultiplePromises(promises);
            };
            api.getData = function () {
                var data;
                if (selectiveCtrl.selectedTemplateConfig != undefined && directiveAPI != undefined) {
                    data = directiveAPI.getData();
                    if (data != undefined) {
                        data.ConfigId = selectiveCtrl.selectedTemplateConfig.ExtensionConfigurationId;
                    }
                }
                return data;
            };
            api.getDescription = function () {
                if (directiveAPI != undefined) {
                    return directiveAPI.getDescription();
                }
            };

            if (selectiveCtrl.onReady && typeof selectiveCtrl.onReady == "function")
                selectiveCtrl.onReady(api);
        }
        function getContext() {
            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            return currentContext;
        }
    }
}]);
