'use strict';

app.directive('vrWhsBeGetcdpnsforidentificationstep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GetCDPNsForIdentificationStepCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/MappingSteps/GetCDPNsForIdentification/Templates/GetCDPNsForIdentificationStepTemplate.html';
            }
        };

        function GetCDPNsForIdentificationStepCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var stepPayload;

            var switchIdDirectiveReadyAPI;
            var switchIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var inputCDPNDirectiveReadyAPI;
            var inputCDPNDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cdpnInDirectiveReadyAPI;
            var cdpnInDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var cdpnOutDirectiveReadyAPI;
            var cdpnOutDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var customerCDPNDirectiveReadyAPI;
            var customerCDPNDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierCDPNDirectiveReadyAPI;
            var supplierCDPNDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var outputCDPNDirectiveReadyAPI;
            var outputCDPNDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSwitchIdReady = function (api) {
                    switchIdDirectiveReadyAPI = api;
                    switchIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onInputCDPNReady = function (api) {
                    inputCDPNDirectiveReadyAPI = api;
                    inputCDPNDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCDPNInReady = function (api) {
                    cdpnInDirectiveReadyAPI = api;
                    cdpnInDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCDPNOutReady = function (api) {
                    cdpnOutDirectiveReadyAPI = api;
                    cdpnOutDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCustomerCDPNReady = function (api) {
                    customerCDPNDirectiveReadyAPI = api;
                    customerCDPNDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSupplierCDPNReady = function (api) {
                    supplierCDPNDirectiveReadyAPI = api;
                    supplierCDPNDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onOutputCDPNReady = function (api) {
                    outputCDPNDirectiveReadyAPI = api;
                    outputCDPNDirectiveReadyPromiseDeferred.resolve();
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

                    //Loading InputCDPN Directive
                    var loadInputCDPNDirectivePromiseDeferred = getLoadInputCDPNDirectivePromiseDeferred();
                    promises.push(loadInputCDPNDirectivePromiseDeferred.promise);

                    //Loading CDPNIn Directive
                    var loadCDPNInDirectivePromiseDeferred = getLoadCDPNInDirectivePromiseDeferred();
                    promises.push(loadCDPNInDirectivePromiseDeferred.promise);

                    //Loading CDPNOut Directive
                    var loadCDPNOutDirectivePromiseDeferred = getLoadCDPNOutDirectivePromiseDeferred();
                    promises.push(loadCDPNOutDirectivePromiseDeferred.promise);

                    //Loading CustomerCDPN Directive
                    var loadCustomerCDPNDirectivePromiseDeferred = getLoadCustomerCDPNDirectivePromiseDeferred();
                    promises.push(loadCustomerCDPNDirectivePromiseDeferred.promise);

                    //Loading SupplierCDPN Directive
                    var loadSupplierCDPNDirectivePromiseDeferred = getLoadSupplierCDPNDirectivePromiseDeferred();
                    promises.push(loadSupplierCDPNDirectivePromiseDeferred.promise);

                    //Loading OutputCDPN Directive
                    var loadOutputCDPNDirectivePromiseDeferred = getLoadOutputCDPNDirectivePromiseDeferred();
                    promises.push(loadOutputCDPNDirectivePromiseDeferred.promise);


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
                    function getLoadInputCDPNDirectivePromiseDeferred() {
                        var loadInputCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        inputCDPNDirectiveReadyPromiseDeferred.promise.then(function () {

                            var inputCDPNPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                inputCDPNPayload.selectedRecords = payload.stepDetails.InputCDPN;

                            VRUIUtilsService.callDirectiveLoad(inputCDPNDirectiveReadyAPI, inputCDPNPayload, loadInputCDPNDirectivePromiseDeferred);
                        });

                        return loadInputCDPNDirectivePromiseDeferred;
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

                    function getLoadCustomerCDPNDirectivePromiseDeferred() {
                        var loadCustomerCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        customerCDPNDirectiveReadyPromiseDeferred.promise.then(function () {

                            var customerCDPNPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                customerCDPNPayload.selectedRecords = payload.stepDetails.CustomerCDPN;

                            VRUIUtilsService.callDirectiveLoad(customerCDPNDirectiveReadyAPI, customerCDPNPayload, loadCustomerCDPNDirectivePromiseDeferred);
                        });

                        return loadCustomerCDPNDirectivePromiseDeferred;
                    }
                    function getLoadSupplierCDPNDirectivePromiseDeferred() {
                        var loadSupplierCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        supplierCDPNDirectiveReadyPromiseDeferred.promise.then(function () {

                            var supplierCDPNPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                supplierCDPNPayload.selectedRecords = payload.stepDetails.SupplierCDPN;

                            VRUIUtilsService.callDirectiveLoad(supplierCDPNDirectiveReadyAPI, supplierCDPNPayload, loadSupplierCDPNDirectivePromiseDeferred);
                        });

                        return loadSupplierCDPNDirectivePromiseDeferred;
                    }
                    function getLoadOutputCDPNDirectivePromiseDeferred() {
                        var loadOutputCDPNDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        outputCDPNDirectiveReadyPromiseDeferred.promise.then(function () {

                            var outputCDPNPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                outputCDPNPayload.selectedRecords = payload.stepDetails.OutputCDPN;

                            VRUIUtilsService.callDirectiveLoad(outputCDPNDirectiveReadyAPI, outputCDPNPayload, loadOutputCDPNDirectivePromiseDeferred);
                        });

                        return loadOutputCDPNDirectivePromiseDeferred;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.MappingSteps.GetCDPNsForIdentificationStep, TOne.WhS.BusinessEntity.MainExtensions",
                        SwitchId: switchIdDirectiveReadyAPI.getData(),
                        InputCDPN: inputCDPNDirectiveReadyAPI.getData(),
                        CDPNIn: cdpnInDirectiveReadyAPI.getData(),
                        CDPNOut: cdpnOutDirectiveReadyAPI.getData(),

                        CustomerCDPN: customerCDPNDirectiveReadyAPI.getData(),
                        SupplierCDPN: supplierCDPNDirectiveReadyAPI.getData(),
                        OutputCDPN: outputCDPNDirectiveReadyAPI.getData(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);