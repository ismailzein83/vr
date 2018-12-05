'use strict';

app.directive('vrWhsRoutingRouteTechnicalSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteTechnicalSettings/Templates/RouteTechnicalSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var customerTransformationSelectorAPI;
            var customerTransformationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierTransformationSelectorAPI;
            var supplierTransformationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.onCustomerDataTransformationDefinitionReady = function (api) {
                    customerTransformationSelectorAPI = api;
                    customerTransformationReadyPromiseDeferred.resolve();
                };

                $scope.onSupplierDataTransformationDefinitionReady = function (api) {
                    supplierTransformationSelectorAPI = api;
                    supplierTransformationReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var customerTransformationPayload;
                    var supplierTransformationPayload;

                    if (payload != undefined && payload.data != undefined) {
                        if (payload.data.RouteRuleDataTransformation != undefined) {
                            customerTransformationPayload = { selectedIds: payload.data.RouteRuleDataTransformation.CustomerTransformationId };
                            supplierTransformationPayload = { selectedIds: payload.data.RouteRuleDataTransformation.SupplierTransformationId };
                        }

                        if (payload.data.TechnicalPartialRouting != undefined) {
                            $scope.scopeModel.partialRoutesPercentageLimit = payload.data.TechnicalPartialRouting.PartialRoutesPercentageLimit;
                            $scope.scopeModel.partialRoutesUpdateBatchSize = payload.data.TechnicalPartialRouting.PartialRoutesUpdateBatchSize;
                        }
                    }

                    var promises = [];

                    var customerTransformationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    customerTransformationReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(customerTransformationSelectorAPI, customerTransformationPayload, customerTransformationLoadPromiseDeferred);
                    });
                    promises.push(customerTransformationLoadPromiseDeferred.promise);

                    var supplierTransformationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    supplierTransformationReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(supplierTransformationSelectorAPI, supplierTransformationPayload, supplierTransformationLoadPromiseDeferred);
                    });
                    promises.push(supplierTransformationLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Entities.RouteTechnicalSettingData, TOne.WhS.Routing.Entities",
                        RouteRuleDataTransformation: {
                            CustomerTransformationId: customerTransformationSelectorAPI.getSelectedIds(),
                            SupplierTransformationId: supplierTransformationSelectorAPI.getSelectedIds(),
                        },
                        TechnicalPartialRouting: {
                            PartialRoutesPercentageLimit: $scope.scopeModel.partialRoutesPercentageLimit,
                            PartialRoutesUpdateBatchSize: $scope.scopeModel.partialRoutesUpdateBatchSize
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);