'use strict';

app.directive('vrNpDatatransformationMasterplansalecodematch', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new MasterPlanSaleCodeMatchStepCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return "/Client/Modules/VR_NumberingPlan/Directives/TransformationSteps/MasterPlanSaleCodeMatchStep/Templates/MasterPlanSaleCodeMatchStepTemplate.html";
            }

        };

        function MasterPlanSaleCodeMatchStepCtor(ctrl, $scope) {
            var stepPayload;

            var numberDirectiveReadyAPI;
            var numberDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveOnDirectiveReadyAPI;
            var effectiveOnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleCodeDirectiveReadyAPI;
            var saleCodeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneIdDirectiveReadyAPI;
            var saleZoneIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanIdDirectiveReadyAPI;
            var sellingNumberPlanIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onNumberReady = function (api) {
                    numberDirectiveReadyAPI = api;
                    numberDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onEffectiveOnReady = function (api) {
                    effectiveOnDirectiveReadyAPI = api;
                    effectiveOnDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSaleCodeReady = function (api) {
                    saleCodeDirectiveReadyAPI = api;
                    saleCodeDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSaleZoneIdReady = function (api) {
                    saleZoneIdDirectiveReadyAPI = api;
                    saleZoneIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSellingNumberPlanIdReady = function (api) {
                    sellingNumberPlanIdDirectiveReadyAPI = api;
                    sellingNumberPlanIdDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    stepPayload = payload;

                    //Loading Number Directive
                    var loadNumberDirectivePromiseDeferred = getLoadNumberDirectivePromiseDeferred();
                    promises.push(loadNumberDirectivePromiseDeferred.promise);

                    //Loading EffectiveOn Directive
                    var loadEffectiveOnDirectivePromiseDeferred = getLoadEffectiveOnDirectivePromiseDeferred();
                    promises.push(loadEffectiveOnDirectivePromiseDeferred.promise);

                    //Loading SaleCode Directive
                    var loadSaleCodeDirectivePromiseDeferred = getLoadSaleCodeDirectivePromiseDeferred();
                    promises.push(loadSaleCodeDirectivePromiseDeferred.promise);

                    //Loading SaleZoneId Directive
                    var loadSaleZoneIdDirectivePromiseDeferred = getLoadSaleZoneIdDirectivePromiseDeferred();
                    promises.push(loadSaleZoneIdDirectivePromiseDeferred.promise);

                    //Loading SaleZoneId Directive
                    var loadSellingNumberPlanIdDirectivePromiseDeferred = getLoadSellingNumberPlanIdDirectivePromiseDeferred();
                    promises.push(loadSellingNumberPlanIdDirectivePromiseDeferred.promise);


                    function getLoadNumberDirectivePromiseDeferred() {
                        var loadNumberDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        numberDirectiveReadyPromiseDeferred.promise.then(function () {

                            var numberPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                numberPayload.selectedRecords = payload.stepDetails.Number;

                            VRUIUtilsService.callDirectiveLoad(numberDirectiveReadyAPI, numberPayload, loadNumberDirectivePromiseDeferred);
                        });

                        return loadNumberDirectivePromiseDeferred;
                    }
                    function getLoadEffectiveOnDirectivePromiseDeferred() {
                        var loadEffectiveOnDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        effectiveOnDirectiveReadyPromiseDeferred.promise.then(function () {

                            var effectiveOnPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                effectiveOnPayload.selectedRecords = payload.stepDetails.EffectiveOn;

                            VRUIUtilsService.callDirectiveLoad(effectiveOnDirectiveReadyAPI, effectiveOnPayload, loadEffectiveOnDirectivePromiseDeferred);
                        });

                        return loadEffectiveOnDirectivePromiseDeferred;
                    }
                    function getLoadSaleCodeDirectivePromiseDeferred() {
                        var loadSaleCodeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        saleCodeDirectiveReadyPromiseDeferred.promise.then(function () {

                            var saleCodePayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                saleCodePayload.selectedRecords = payload.stepDetails.SaleCode;

                            VRUIUtilsService.callDirectiveLoad(saleCodeDirectiveReadyAPI, saleCodePayload, loadSaleCodeDirectivePromiseDeferred);
                        });

                        return loadSaleCodeDirectivePromiseDeferred;
                    }
                    function getLoadSaleZoneIdDirectivePromiseDeferred() {
                        var loadSaleZoneIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        saleZoneIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var saleZoneIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                saleZoneIdPayload.selectedRecords = payload.stepDetails.SaleZoneId;

                            VRUIUtilsService.callDirectiveLoad(saleZoneIdDirectiveReadyAPI, saleZoneIdPayload, loadSaleZoneIdDirectivePromiseDeferred);
                        });

                        return loadSaleZoneIdDirectivePromiseDeferred;
                    }
                    function getLoadSellingNumberPlanIdDirectivePromiseDeferred() {
                        var loadSellingNumberPlanIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        sellingNumberPlanIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var sellingNumberPlanIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                sellingNumberPlanIdPayload.selectedRecords = payload.stepDetails.SellingNumberPlanId;

                            VRUIUtilsService.callDirectiveLoad(sellingNumberPlanIdDirectiveReadyAPI, sellingNumberPlanIdPayload, loadSellingNumberPlanIdDirectivePromiseDeferred);
                        });

                        return loadSellingNumberPlanIdDirectivePromiseDeferred;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.NumberingPlan.Business.TransformationSteps.MasterPlanSaleCodeMatchStep, Vanrise.NumberingPlan.Business",
                        Number: numberDirectiveReadyAPI.getData(),
                        EffectiveOn: effectiveOnDirectiveReadyAPI.getData(),
                        SaleCode: saleCodeDirectiveReadyAPI.getData(),
                        SaleZoneId: saleZoneIdDirectiveReadyAPI.getData(),
                        SellingNumberPlanId: sellingNumberPlanIdDirectiveReadyAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);