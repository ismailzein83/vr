'use strict';
app.directive('vrWhsBeDatatransformationSupplierzonerate', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new SupplierZoneRateMatchStepCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/TransformationSteps/SupplierZoneRateStep/Templates/SupplierZoneRateStepTemplate.html';
            }

        };

        function SupplierZoneRateMatchStepCtor(ctrl, $scope) {           

            var supplierIdDirectiveAPI;
            var supplierIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierZoneIdDirectiveAPI;
            var supplierZoneIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveOnDirectiveAPI;
            var effectiveOnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierZoneRateDirectiveAPI;
            var supplierZoneRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onSupplierIdDirectiveReady = function (api) {
                    supplierIdDirectiveAPI = api;
                    supplierIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onSupplierZoneIdDirectiveReady = function (api) {
                    supplierZoneIdDirectiveAPI = api;
                    supplierZoneIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onEffectiveOnDirectiveReady = function (api) {
                    effectiveOnDirectiveAPI = api;
                    effectiveOnDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.onSupplierZoneRateDirectiveReady = function (api) {
                    supplierZoneRateDirectiveAPI = api;
                    supplierZoneRateDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var loadSupplierIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    supplierIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSupplierId;
                        if (payload != undefined) {
                            payloadSupplierId = {};
                            payloadSupplierId.context = payload.context;
                            if (payload.stepDetails != undefined)
                                payloadSupplierId.selectedRecords = payload.stepDetails.SupplierId;
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierIdDirectiveAPI, payloadSupplierId, loadSupplierIdDirectivePromiseDeferred);
                    });

                    promises.push(loadSupplierIdDirectivePromiseDeferred.promise);

                    var loadSupplierZoneIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    supplierZoneIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSupplierZoneId;
                        if (payload != undefined) {
                            payloadSupplierZoneId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadSupplierZoneId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadSupplierZoneId.selectedRecords = payload.stepDetails.SupplierZoneId;
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierZoneIdDirectiveAPI, payloadSupplierZoneId, loadSupplierZoneIdDirectivePromiseDeferred);
                    });

                    promises.push(loadSupplierZoneIdDirectivePromiseDeferred.promise);

                    var loadEffectiveOnDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    effectiveOnDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadEffectiveOn;
                        if (payload != undefined) {
                            payloadEffectiveOn = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadEffectiveOn.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadEffectiveOn.selectedRecords = payload.stepDetails.EffectiveOn;
                        }
                        VRUIUtilsService.callDirectiveLoad(effectiveOnDirectiveAPI, payloadEffectiveOn, loadEffectiveOnDirectivePromiseDeferred);
                    });

                    promises.push(loadEffectiveOnDirectivePromiseDeferred.promise);


                    var loadSupplierZoneRateDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    supplierZoneRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSupplierZoneRate;
                        if (payload != undefined) {
                            payloadSupplierZoneRate = {};
                            payloadSupplierZoneRate.context = payload.context;
                            if (payload.stepDetails != undefined)
                                payloadSupplierZoneRate.selectedRecords = payload.stepDetails.SupplierZoneRate;
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierZoneRateDirectiveAPI, payloadSupplierZoneRate, loadSupplierZoneRateDirectivePromiseDeferred);
                    });

                    promises.push(loadSupplierZoneRateDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.TransformationSteps.SupplierZoneRateStep, TOne.WhS.BusinessEntity.MainExtensions",
                        SupplierId: supplierIdDirectiveAPI != undefined ? supplierIdDirectiveAPI.getData() : undefined,
                        SupplierZoneId: supplierZoneIdDirectiveAPI != undefined ? supplierZoneIdDirectiveAPI.getData() : undefined,
                        EffectiveOn: effectiveOnDirectiveAPI != undefined ? effectiveOnDirectiveAPI.getData() : undefined,
                        SupplierZoneRate: supplierZoneRateDirectiveAPI != undefined ? supplierZoneRateDirectiveAPI.getData() : undefined
                    }
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);