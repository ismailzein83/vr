"use strict";

app.directive('retailBillingCharge', ['UtilsService', 'VRUIUtilsService', 'Retail_Billing_ChargeTypeAPIService',
    function (UtilsService, VRUIUtilsService, Retail_Billing_ChargeTypeAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '=',
                customlabel: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBillingChargeDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingCharge/Templates/RetailBillingChargeTemplate.html'
        };

        function retailBillingChargeDirectiveCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var chargeTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var chargeTypeSelectorDirectiveAPI;
            var retailBillingChargeTypeSettingsExtendedSettings;
            var charge;
            var targetRecordTypeId;
            var title;
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
                    if (payload != undefined) {
                        charge = payload.charge;
                        title = payload.title;
                        targetRecordTypeId = payload.targetRecordTypeId;

                        if (charge != undefined)
                            retailBillingChargeTypeId = charge.RetailBillingChargeTypeId;
                    }

                    var promises = [];
                    promises.push(loadChargeTypeSelector(payload));

                    if (retailBillingChargeTypeId != undefined) {
                        promises.push(getRetailBillingChargeType(retailBillingChargeTypeId));

                        directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(directiveReadyPromiseDeferred.promise);
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

                    if ($scope.scopeModel.selectedChargeType != undefined) {
                        obj.RetailBillingChargeTypeId = $scope.scopeModel.selectedChargeType.RetailBillingChargeTypeId;
                        return obj;
                    }
                    return undefined;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadChargeTypeSelector(payload) {
                var loadChargeTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                chargeTypeSelectorReadyPromiseDeferred.promise.then(function () {

                    var chargeTypeSelectorPayload;
                    if (payload != undefined)
                        chargeTypeSelectorPayload = {
                            selectedIds: retailBillingChargeTypeId,
                            targetRecordTypeId: targetRecordTypeId,
                            title: title
                        };

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