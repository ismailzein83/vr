'use strict';

app.directive('vrWhsBeGetcdpnsforzonematchstep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new GetCDPNsForZoneMatchStepCtor(ctrl, $scope);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/MappingSteps/GetCDPNsForZoneMatch/Templates/GetCDPNsForZoneMatchStepTemplate.html';
            }
        };

        function GetCDPNsForZoneMatchStepCtor(ctrl, $scope) {
            var stepPayload;

            var switchIdDirectiveReadyAPI;
            var switchIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cdpnNormalizedDirectiveReadyAPI;
            var cdpnNormalizedDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cdpnInDirectiveReadyAPI;
            var cdpnInDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cdpnOutDirectiveReadyAPI;
            var cdpnOutDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleZoneCDPNDirectiveReadyAPI;
            var saleZoneCDPNDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierZoneCDPNDirectiveReadyAPI;
            var supplierZoneCDPNDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSwitchIdReady = function (api) {
                    switchIdDirectiveReadyAPI = api;
                    switchIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCDPNNormalizedReady = function (api) {
                    cdpnNormalizedDirectiveReadyAPI = api;
                    cdpnNormalizedDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCDPNInReady = function (api) {
                    cdpnInDirectiveReadyAPI = api;
                    cdpnInDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCDPNOutReady = function (api) {
                    cdpnOutDirectiveReadyAPI = api;
                    cdpnOutDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSaleZoneCDPNReady = function (api) {
                    saleZoneCDPNDirectiveReadyAPI = api;
                    saleZoneCDPNDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierZoneCDPNReady = function (api) {
                    supplierZoneCDPNDirectiveReadyAPI = api;
                    supplierZoneCDPNDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    stepPayload = payload;

                    //Loading SwitchId Directive
                    var loadSwitchIdDirectivePromiseDeferred = getLoadSwitchIdDirectivePromiseDeferred();
                    promises.push(loadSwitchIdDirectivePromiseDeferred.promise);

                    //Loading CDPNNormalized Directive
                    var loadCDPNNormalizedDirectivePromiseDeferred = getLoadCDPNNormalizedDirectivePromiseDeferred();
                    promises.push(loadCDPNNormalizedDirectivePromiseDeferred.promise);

                    //Loading CDPNIn Directive
                    var loadCDPNInDirectivePromiseDeferred = getLoadCDPNInDirectivePromiseDeferred();
                    promises.push(loadCDPNInDirectivePromiseDeferred.promise);

                    //Loading CDPNOut Directive
                    var loadCDPNOutDirectivePromiseDeferred = getLoadCDPNOutDirectivePromiseDeferred();
                    promises.push(loadCDPNOutDirectivePromiseDeferred.promise);

                    //Loading SaleZoneCDPN Directive
                    var loadSaleZoneCDPNDirectivePromiseDeferred = getLoadSaleZoneCDPNDirectivePromiseDeferred();
                    promises.push(loadSaleZoneCDPNDirectivePromiseDeferred.promise);

                    //Loading SupplierZoneCDPN Directive
                    var loadSupplierZoneCDPNDirectivePromiseDeferred = getLoadSupplierZoneCDPNDirectivePromiseDeferred();
                    promises.push(loadSupplierZoneCDPNDirectivePromiseDeferred.promise);


                    function getLoadSwitchIdDirectivePromiseDeferred() {
                        var loadSwitchIdDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        switchIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var switchIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                switchIdPayload.selectedRecords = payload.stepDetails.SwitchId;

                            VRUIUtilsService.callDirectiveLoad(switchIdDirectiveReadyAPI, switchIdPayload, loadSwitchIdDirectivePromiseDeferred);
                        });

                        return loadSwitchIdDirectivePromiseDeferred;
                    }
                    function getLoadCDPNNormalizedDirectivePromiseDeferred() {
                        var loadCDPNNormalizedDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        cdpnNormalizedDirectiveReadyPromiseDeferred.promise.then(function () {

                            var cdpnNormalizedPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                cdpnNormalizedPayload.selectedRecords = payload.stepDetails.CDPNNormalized;

                            VRUIUtilsService.callDirectiveLoad(cdpnNormalizedDirectiveReadyAPI, cdpnNormalizedPayload, loadCDPNNormalizedDirectivePromiseDeferred);
                        });

                        return loadCDPNNormalizedDirectivePromiseDeferred;
                    }
                    function getLoadCDPNInDirectivePromiseDeferred() {
                        var loadCDPNInDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        cdpnInDirectiveReadyPromiseDeferred.promise.then(function () {

                            var cdpnInPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                cdpnInPayload.selectedRecords = payload.stepDetails.CDPNIn;

                            VRUIUtilsService.callDirectiveLoad(cdpnInDirectiveReadyAPI, cdpnInPayload, loadCDPNInDirectivePromiseDeferred);
                        });

                        return loadCDPNInDirectivePromiseDeferred;
                    }
                    function getLoadCDPNOutDirectivePromiseDeferred() {
                        var loadCDPNOutDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        cdpnOutDirectiveReadyPromiseDeferred.promise.then(function () {

                            var cdpnOutPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                cdpnOutPayload.selectedRecords = payload.stepDetails.CDPNOut;

                            VRUIUtilsService.callDirectiveLoad(cdpnOutDirectiveReadyAPI, cdpnOutPayload, loadCDPNOutDirectivePromiseDeferred);
                        });

                        return loadCDPNOutDirectivePromiseDeferred;
                    }

                    function getLoadSaleZoneCDPNDirectivePromiseDeferred() {
                        var loadSaleZoneCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        saleZoneCDPNDirectiveReadyPromiseDeferred.promise.then(function () {

                            var saleZoneCDPNPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                saleZoneCDPNPayload.selectedRecords = payload.stepDetails.SaleZoneCDPN;

                            VRUIUtilsService.callDirectiveLoad(saleZoneCDPNDirectiveReadyAPI, saleZoneCDPNPayload, loadSaleZoneCDPNDirectivePromiseDeferred);
                        });

                        return loadSaleZoneCDPNDirectivePromiseDeferred;
                    }
                    function getLoadSupplierZoneCDPNDirectivePromiseDeferred() {
                        var loadSupplierZoneCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        supplierZoneCDPNDirectiveReadyPromiseDeferred.promise.then(function () {

                            var supplierZoneCDPNPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                supplierZoneCDPNPayload.selectedRecords = payload.stepDetails.SupplierZoneCDPN;

                            VRUIUtilsService.callDirectiveLoad(supplierZoneCDPNDirectiveReadyAPI, supplierZoneCDPNPayload, loadSupplierZoneCDPNDirectivePromiseDeferred);
                        });

                        return loadSupplierZoneCDPNDirectivePromiseDeferred;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.MappingSteps.GetCDPNsForZoneMatchStep, TOne.WhS.BusinessEntity.MainExtensions",
                        SwitchId: switchIdDirectiveReadyAPI.getData(),
                        CDPNNormalized: cdpnNormalizedDirectiveReadyAPI.getData(),
                        CDPNIn: cdpnInDirectiveReadyAPI.getData(),
                        CDPNOut: cdpnOutDirectiveReadyAPI.getData(),

                        SaleZoneCDPN: saleZoneCDPNDirectiveReadyAPI.getData(),
                        SupplierZoneCDPN: supplierZoneCDPNDirectiveReadyAPI.getData(),
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);