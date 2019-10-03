"use strict";

app.directive('retailBillingCharge', ['UtilsService', 'VRUIUtilsService', 'Retail_Billing_ChargeTypeAPIService',
    function (UtilsService, VRUIUtilsService, Retail_Billing_ChargeTypeAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBillingChargeDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingCharge/Templates/RetailBillingChargeTemplate.html'
        };

        function retailBillingChargeDirectiveCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var chargeTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var chargeTypeSelectorDirectiveAPI;
            var retailBillingChargeTypeSettingsExtendedSettings;
            var charge;
            var retailBillingChargeTypeId;

            var directiveReadyPromiseDeferred;
            var directiveAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onChargeTypeSelectorReady = function (api) {
                    chargeTypeSelectorDirectiveAPI = api;
                    chargeTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;

                    if (directiveReadyPromiseDeferred != undefined) {
                        directiveReadyPromiseDeferred.resolve();
                    }
                    else {
                        getRetailBillingChargeType($scope.scopeModel.selectedChargeType.RetailBillingChargeTypeId).then(function () {

                            var setLoader = function (value) {
                                $scope.scopeModel.isDirectiveLoading = value;
                            };
                            var payload = {
                                extendedSettings: retailBillingChargeTypeSettingsExtendedSettings
                            };

                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, undefined);
                        });
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    promises.push(loadChargeTypeSelector());

                    if (payload != undefined) {
                        charge = payload.charge;
                        retailBillingChargeTypeId = payload.RetailBillingChargeTypeId;

                        if (retailBillingChargeTypeId != undefined) {
                            promises.push(getRetailBillingChargeType(retailBillingChargeTypeId));

                            directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(directiveReadyPromiseDeferred.promise);
                        }
                    }

                    return UtilsService.waitPromiseNode({
                        promises: promises,
                        getChildNode: function () {

                            var promises = [];

                            if (retailBillingChargeTypeSettingsExtendedSettings != undefined) {
                                var loadRuntimeDirectivePromise = loadRuntimeDirective();
                                promises.push(loadRuntimeDirectivePromise);
                            }

                            return {
                                promises: promises
                            };
                        }
                    });
                };

                api.getData = function () {
                    var obj = directiveAPI != undefined ? directiveAPI.getData() : {};
                    obj.RetailBillingChargeTypeId = $scope.scopeModel.selectedChargeType.RetailBillingChargeTypeId;

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadChargeTypeSelector() {
                var loadChargeTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                chargeTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    var chargeTypeSelectorPayload;

                    if (retailBillingChargeTypeId != undefined)
                        chargeTypeSelectorPayload = { selectedIds: retailBillingChargeTypeId };

                    VRUIUtilsService.callDirectiveLoad(chargeTypeSelectorDirectiveAPI, chargeTypeSelectorPayload, loadChargeTypeSelectorPromiseDeferred);
                });
                return loadChargeTypeSelectorPromiseDeferred.promise;
            }

            function loadRuntimeDirective() {
                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                directiveReadyPromiseDeferred.promise.then(function () {
                    directiveReadyPromiseDeferred = undefined;
                    var payload = {
                        extendedSettings: retailBillingChargeTypeSettingsExtendedSettings,
                        charge: charge
                    };

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, payload, directiveLoadDeferred);
                });

                return directiveLoadDeferred.promise;
            }

            function getRetailBillingChargeType(retailBillingChargeTypeId) {
                return Retail_Billing_ChargeTypeAPIService.GetRetailBillingChargeType(retailBillingChargeTypeId).then(function (response) {
                    if (response != null) {
                        retailBillingChargeTypeSettingsExtendedSettings = response.Settings != undefined ? response.Settings.ExtendedSettings : undefined;
                    }
                });
            }
        }

        return directiveDefinitionObject;
    }]);