"use strict";

app.directive("vrWhsDealSupplierrateevaluatorSelective", ["WhS_Deal_VolCommitmentDealAPIService", "UtilsService", "VRUIUtilsService",
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
            var supplierRateEvaluator = new SupplierRateEvaluator(selectiveCtrl, $scope);
            supplierRateEvaluator.initializeController();
        },
        controllerAs: "selectiveCtrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Deal/Directives/DealRateEvaluator/Templates/DealSupplierRateEvaluatorTemplate.html"
    };

    function SupplierRateEvaluator(selectiveCtrl, $scope) {
        this.initializeController = initializeController;
        var directiveReadyDeferred;
        var directiveAPI;
        var evaluatedRate;

        function initializeController() {
            selectiveCtrl.templateConfigs = [];

            selectiveCtrl.onSupplierRateEvaluatorSelectorReady = function (api) {
                defineAPI();
            };
            selectiveCtrl.onDirectiveWrapperReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.isLoadingDirective = value;
                };
                var directivePayload = undefined;
                if (evaluatedRate != undefined)
                    directivePayload =
                   {
                       evaluatedRate: evaluatedRate
                   };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
            };
        }
        function defineAPI() {

            var api = {};
            api.load = function (payload) {
                var promises = [];
                var getsupplierRateEvaluatorConfigurationPromise = getSupplierRateEvaluatorConfiguration();
                promises.push(getsupplierRateEvaluatorConfigurationPromise);

                if (payload != undefined) {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();
                    evaluatedRate = payload.evaluatedRate;
                    var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    directiveReadyDeferred.promise.then(function () {
                        var payload = {
                            evaluatedRate: evaluatedRate
                        };
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, loadDirectivePromiseDeferred);
                    });
                    promises.push(loadDirectivePromiseDeferred.promise);
                }

                function getSupplierRateEvaluatorConfiguration() {
                    return WhS_Deal_VolCommitmentDealAPIService.GetSupplierRateEvaluatorConfigurationTemplateConfigs().then(
                        function (supplierRateEvaluators) {
                            if (supplierRateEvaluators != null) {
                                for (var i = 0; i < supplierRateEvaluators.length; i++) {
                                    selectiveCtrl.templateConfigs.push(supplierRateEvaluators[i]);
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
                var data = null;
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

    }
}]);
