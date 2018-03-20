'use strict';
app.directive('vrWhsRoutingDatatransformationMatchingsuppliercodematch', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new SupplierCodeMatchStepCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_Routing/Directives/TransformationSteps/MatchingSupplierCodeMatchStep/Templates/MatchingSupplierCodeMatchStepTemplate.html';
            }

        };

        function SupplierCodeMatchStepCtor(ctrl, $scope) {
            var numberDirectiveAPI;
            var numberRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierIdDirectiveAPI;
            var supplierIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierANumberIdDirectiveAPI;
            var supplierANumberIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var effectiveOnDirectiveAPI;
            var effectiveOnDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var matchingSupplierIdDirectiveAPI;
            var matchingSupplierIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierCodeDirectiveAPI;
            var supplierCodeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierZoneIdDirectiveAPI;
            var supplierZoneIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.onNumberDirectiveReady = function (api) {
                    numberDirectiveAPI = api;
                    numberRateDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onSupplierIdDirectiveReady = function (api) {
                    supplierIdDirectiveAPI = api;
                    supplierIdDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onSupplierANumberIdDirectiveReady = function (api) {
                    supplierANumberIdDirectiveAPI = api;
                    supplierANumberIdDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onEffectiveOnDirectiveReady = function (api) {
                    effectiveOnDirectiveAPI = api;
                    effectiveOnDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onMatchingSupplierIdDirectiveReady = function (api) {
                    matchingSupplierIdDirectiveAPI = api;
                    matchingSupplierIdDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onSupplierCodeDirectiveReady = function (api) {
                    supplierCodeDirectiveAPI = api;
                    supplierCodeDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onSupplierZoneIdDirectiveReady = function (api) {
                    supplierZoneIdDirectiveAPI = api;
                    supplierZoneIdDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var loadNumberDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    numberRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadNumber;
                        if (payload != undefined) {
                            payloadNumber = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadNumber.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadNumber.selectedRecords = payload.stepDetails.Number;
                        }
                        VRUIUtilsService.callDirectiveLoad(numberDirectiveAPI, payloadNumber, loadNumberDirectivePromiseDeferred);
                    });

                    promises.push(loadNumberDirectivePromiseDeferred.promise);


                    var loadSupplierIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    supplierIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSupplierId;
                        if (payload != undefined) {
                            payloadSupplierId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadSupplierId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadSupplierId.selectedRecords = payload.stepDetails.SupplierId;
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierIdDirectiveAPI, payloadSupplierId, loadSupplierIdDirectivePromiseDeferred);
                    });

                    promises.push(loadSupplierIdDirectivePromiseDeferred.promise);

                    var loadSupplierANumberIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    supplierANumberIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSupplierANumberId;
                        if (payload != undefined) {
                            payloadSupplierANumberId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadSupplierANumberId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadSupplierANumberId.selectedRecords = payload.stepDetails.SupplierANumberId;
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierANumberIdDirectiveAPI, payloadSupplierANumberId, loadSupplierANumberIdDirectivePromiseDeferred);
                    });

                    promises.push(loadSupplierANumberIdDirectivePromiseDeferred.promise);

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


                    var loadMatchingSupplierIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    matchingSupplierIdDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadMatchingSupplierId;
                        if (payload != undefined) {
                            payloadMatchingSupplierId = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadMatchingSupplierId.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadMatchingSupplierId.selectedRecords = payload.stepDetails.MatchingSupplierId;
                        }
                        VRUIUtilsService.callDirectiveLoad(matchingSupplierIdDirectiveAPI, payloadMatchingSupplierId, loadMatchingSupplierIdDirectivePromiseDeferred);
                    });

                    promises.push(loadMatchingSupplierIdDirectivePromiseDeferred.promise);


                    var loadSupplierCodeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    supplierCodeDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSupplierCode;
                        if (payload != undefined) {
                            payloadSupplierCode = {};
                            if (payload != undefined && payload.context != undefined)
                                payloadSupplierCode.context = payload.context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadSupplierCode.selectedRecords = payload.stepDetails.SupplierCode;
                        }
                        VRUIUtilsService.callDirectiveLoad(supplierCodeDirectiveAPI, payloadSupplierCode, loadSupplierCodeDirectivePromiseDeferred);
                    });

                    promises.push(loadSupplierCodeDirectivePromiseDeferred.promise);


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
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.Routing.Business.TransformationSteps.MatchingSupplierCodeMatchStep, TOne.WhS.Routing.Business",
                        Number: numberDirectiveAPI != undefined ? numberDirectiveAPI.getData() : undefined,
                        SupplierId: supplierIdDirectiveAPI != undefined ? supplierIdDirectiveAPI.getData() : undefined,
                        SupplierANumberId: supplierANumberIdDirectiveAPI != undefined ? supplierANumberIdDirectiveAPI.getData() : undefined,
                        EffectiveOn: effectiveOnDirectiveAPI != undefined ? effectiveOnDirectiveAPI.getData() : undefined,
                        MatchingSupplierId: matchingSupplierIdDirectiveAPI != undefined ? matchingSupplierIdDirectiveAPI.getData() : undefined,
                        SupplierCode: supplierCodeDirectiveAPI != undefined ? supplierCodeDirectiveAPI.getData() : undefined,
                        SupplierZoneId: supplierZoneIdDirectiveAPI != undefined ? supplierZoneIdDirectiveAPI.getData() : undefined
                    };
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