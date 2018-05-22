'use strict';

app.directive('vrWhsRoutingSalecodematchbyplanstep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SaleCodeMatchByPlanStepCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Routing/Directives/TransformationSteps/SaleCodeMatchStepByPlan/Templates/SaleCodeMatchByPlanStepTemplate.html';
            }
        };

        function SaleCodeMatchByPlanStepCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var stepPayload;

            var effectiveOnDirectiveReadyAPI;
            var effectiveOnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanIdDirectiveReadyAPI;
            var sellingNumberPlanIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cdpnDirectiveReadyAPI;
            var cdpnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cgpnDirectiveReadyAPI;
            var cgpnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleCodeDirectiveReadyAPI;
            var saleCodeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneIdDirectiveReadyAPI;
            var saleZoneIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var originatingSaleCodeDirectiveReadyAPI;
            var originatingSaleCodeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var originatingSaleZoneIdDirectiveReadyAPI;
            var originatingSaleZoneIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEffectiveOnReady = function (api) {
                    effectiveOnDirectiveReadyAPI = api;
                    effectiveOnDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSellingNumberPlanIdReady = function (api) {
                    sellingNumberPlanIdDirectiveReadyAPI = api;
                    sellingNumberPlanIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCDPNReady = function (api) {
                    cdpnDirectiveReadyAPI = api;
                    cdpnDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCGPNReady = function (api) {
                    cgpnDirectiveReadyAPI = api;
                    cgpnDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSaleCodeReady = function (api) {
                    saleCodeDirectiveReadyAPI = api;
                    saleCodeDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSaleZoneIdReady = function (api) {
                    saleZoneIdDirectiveReadyAPI = api;
                    saleZoneIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onOriginatingSaleCodeReady = function (api) {
                    originatingSaleCodeDirectiveReadyAPI = api;
                    originatingSaleCodeDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onOriginatingSaleZoneIdReady = function (api) {
                    originatingSaleZoneIdDirectiveReadyAPI = api;
                    originatingSaleZoneIdDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    stepPayload = payload;

                    //Loading SwitchId Directive
                    var loadEffectiveOnDirectivePromiseDeferred = getloadEffectiveOnDirectivePromiseDeferred();
                    promises.push(loadEffectiveOnDirectivePromiseDeferred.promise);

                    //Loading SellingNumberPlanId Directive
                    var loadSellingNumberPlanIdDirectivePromiseDeferred = getLoadSellingNumberPlanIdDirectivePromiseDeferred();
                    promises.push(loadSellingNumberPlanIdDirectivePromiseDeferred.promise);

                    //Loading CDPN Directive
                    var loadCDPNDirectivePromiseDeferred = getLoadCDPNDirectivePromiseDeferred();
                    promises.push(loadCDPNDirectivePromiseDeferred.promise);

                    //Loading CGPN Directive
                    var loadCGPNDirectivePromiseDeferred = getLoadCGPNDirectivePromiseDeferred();
                    promises.push(loadCGPNDirectivePromiseDeferred.promise);

                    //Loading SaleCode Directive
                    var loadSaleCodeDirectivePromiseDeferred = getLoadSaleCodeDirectivePromiseDeferred();
                    promises.push(loadSaleCodeDirectivePromiseDeferred.promise);

                    //Loading SaleZoneId Directive
                    var loadSaleZoneIdDirectivePromiseDeferred = getLoadSaleZoneIdDirectivePromiseDeferred();
                    promises.push(loadSaleZoneIdDirectivePromiseDeferred.promise);

                    //Loading OriginatingSaleCode Directive
                    var loadOriginatingSaleCodeDirectivePromiseDeferred = getLoadOriginatingSaleCodeDirectivePromiseDeferred();
                    promises.push(loadOriginatingSaleCodeDirectivePromiseDeferred.promise);

                    //Loading OriginatingSaleZoneId Directive
                    var loadOriginatingSaleZoneIdDirectivePromiseDeferred = getLoadOriginatingSaleZoneIdDirectivePromiseDeferred();
                    promises.push(loadOriginatingSaleZoneIdDirectivePromiseDeferred.promise);


                    function getloadEffectiveOnDirectivePromiseDeferred() {
                        var loadEffectiveOnDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        effectiveOnDirectiveReadyPromiseDeferred.promise.then(function () {

                            var effectiveOnPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                effectiveOnPayload.selectedRecords = payload.stepDetails.EffectiveOn;

                            VRUIUtilsService.callDirectiveLoad(switchIdDirectiveReadyAPI, effectiveOnPayload, loadEffectiveOnDirectivePromiseDeferred);
                        });

                        return loadEffectiveOnDirectivePromiseDeferred;
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
                    function getLoadCDPNDirectivePromiseDeferred() {
                        var loadCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        cdpnDirectiveReadyPromiseDeferred.promise.then(function () {

                            var cdpnPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                cdpnPayload.selectedRecords = payload.stepDetails.CDPN;

                            VRUIUtilsService.callDirectiveLoad(cdpnDirectiveReadyAPI, cdpnPayload, loadCDPNDirectivePromiseDeferred);
                        });

                        return loadCDPNDirectivePromiseDeferred;
                    }
                    function getLoadCGPNDirectivePromiseDeferred() {

                        var loadCGPNOutDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        cgpnDirectiveReadyPromiseDeferred.promise.then(function () {

                            var cgpnPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                cgpnPayload.selectedRecords = payload.stepDetails.CGPN;

                            VRUIUtilsService.callDirectiveLoad(cgpnDirectiveReadyAPI, cgpnPayload, loadCGPNOutDirectivePromiseDeferred);
                        });

                        return loadCGPNOutDirectivePromiseDeferred;
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
                    function getLoadOriginatingSaleCodeDirectivePromiseDeferred() {
                        var loadOriginatingSaleCodeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        originatingSaleCodeDirectiveReadyPromiseDeferred.promise.then(function () {

                            var originatingSaleCodePayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                originatingSaleCodePayload.selectedRecords = payload.stepDetails.OriginatingSaleCode;

                            VRUIUtilsService.callDirectiveLoad(originatingSaleCodeDirectiveReadyAPI, originatingSaleCodePayload, loadOriginatingSaleCodeDirectivePromiseDeferred);
                        });

                        return loadOriginatingSaleCodeDirectivePromiseDeferred;
                    }
                    function getLoadOriginatingSaleZoneIdDirectivePromiseDeferred() {
                        var loadOriginatingSaleZoneIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        originatingSaleZoneIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var originatingSaleZoneIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                originatingSaleZoneIdPayload.selectedRecords = payload.stepDetails.OriginatingSaleZoneId;

                            VRUIUtilsService.callDirectiveLoad(originatingSaleZoneIdDirectiveReadyAPI, originatingSaleZoneIdPayload, loadOriginatingSaleZoneIdDirectivePromiseDeferred);
                        });

                        return loadOriginatingSaleZoneIdDirectivePromiseDeferred;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Business.TransformationSteps.SaleCodeMatchByPlanStep, TOne.WhS.Routing.Business.TransformationSteps.",
                        EffectiveOn: effectiveOnDirectiveReadyAPI.getData(),
                        SellingNumberPlanId: sellingNumberPlanIdDirectiveReadyAPI.getData(),
                        CDPN: cdpnDirectiveReadyAPI.getData(),
                        CGPN: cgpnDirectiveReadyAPI.getData(),

                        SaleCode: saleCodeDirectiveReadyAPI.getData(),
                        SaleZoneId: saleZoneIdDirectiveReadyAPI.getData(),
                        OriginatingSaleCode: originatingSaleCodeDirectiveReadyAPI.getData(),
                        OriginatingSaleZoneId: originatingSaleZoneIdDirectiveReadyAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);