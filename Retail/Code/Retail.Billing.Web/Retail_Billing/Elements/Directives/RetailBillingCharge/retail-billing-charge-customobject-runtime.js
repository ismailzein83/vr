"use strict";

app.directive('retailBillingChargeCustomobjectRuntime', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailBillingChargeCustomObjectRuntimeDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingCharge/Templates/RetailBillingChargeCustomObjectRuntimeTemplate.html'
        };

        function retailBillingChargeCustomObjectRuntimeDirectiveCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var chargeTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var chargeDirectiveAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onChargeDirectiveReady = function (api) {
                    chargeDirectiveAPI = api;
                    chargeTypeSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var charge;
                    var title;
                    var targetRecordTypeId;

                    if (payload != undefined) {
                        charge = payload.fieldValue;
                        title = payload.fieldTitle;

                        var fieldType = payload.fieldType;

                        if (fieldType != undefined && fieldType.Settings != undefined)
                            targetRecordTypeId = fieldType.Settings.TargetRecordTypeId;
                    }

                    $scope.scopeModel.isPayloadReady = true;

                    promises.push(loadChargeDirective());

                    function loadChargeDirective() {
                        var loadChargeTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        chargeTypeSelectorReadyPromiseDeferred.promise.then(function () {
                            var chargeDirectivePayload;

                            if (payload != undefined)
                                chargeDirectivePayload = {
                                    charge: charge,
                                    title: title,
                                    targetRecordTypeId: targetRecordTypeId
                                };
                            VRUIUtilsService.callDirectiveLoad(chargeDirectiveAPI, chargeDirectivePayload, loadChargeTypeSelectorPromiseDeferred);
                        });
                        return loadChargeTypeSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({
                        promises: promises
                    });
                };

                api.getData = function () {
                    return chargeDirectiveAPI.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
        }

        return directiveDefinitionObject;
    }]);