﻿'use strict';

app.directive('vrWhsRoutingRouteTechnicalSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteTechnicalSettings/Templates/RouteTechnicalSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            var customerTransformationSelectorAPI;
            var customerTransformationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierTransformationSelectorAPI;
            var supplierTransformationReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var qualityAnalyticTableSelectorAPI;
            var qualityAnalyticTableReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

                $scope.onQualityAnalyticTableReady = function (api) {
                    qualityAnalyticTableSelectorAPI = api;
                    qualityAnalyticTableReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([qualityAnalyticTableReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var customerTransformationPayload;
                    var supplierTransformationPayload;

                    if (payload != undefined && payload.data != undefined) {
                        customerTransformationPayload = { selectedIds: payload.data.RouteRuleDataTransformation.CustomerTransformationId };
                        supplierTransformationPayload = { selectedIds: payload.data.RouteRuleDataTransformation.SupplierTransformationId };
                        $scope.scopeModel.partialRoutesPercentageLimit = payload.data.RouteRuleDataTransformation.PartialRoutesPercentageLimit;
                        $scope.scopeModel.partialRoutesUpdateBatchSize = payload.data.RouteRuleDataTransformation.PartialRoutesUpdateBatchSize;
                    }

                    var customerTransformationLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    customerTransformationReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(customerTransformationSelectorAPI, customerTransformationPayload, customerTransformationLoadPromiseDeferred);
                        });

                    promises.push(customerTransformationLoadPromiseDeferred.promise);

                    var supplierTransformationLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    supplierTransformationReadyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(supplierTransformationSelectorAPI, supplierTransformationPayload, supplierTransformationLoadPromiseDeferred);
                        });

                    promises.push(supplierTransformationLoadPromiseDeferred.promise);


                    function loadQualityAnalyticTableSelector() {
                        var qualityAnalyticTablePayload;
                        if (payload != undefined && payload.data != undefined && payload.data.TechnicalQualityConfiguration != undefined) {
                            qualityAnalyticTablePayload = {
                                selectedIds: payload.data.TechnicalQualityConfiguration.QualityAnalyticTableId
                            }
                        }
                        return qualityAnalyticTableSelectorAPI.load(qualityAnalyticTablePayload);
                    }
                    promises.push(loadQualityAnalyticTableSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Routing.Entities.RouteTechnicalSettingData, TOne.WhS.Routing.Entities",
                        RouteRuleDataTransformation: {
                            CustomerTransformationId: customerTransformationSelectorAPI.getSelectedIds(),
                            SupplierTransformationId: supplierTransformationSelectorAPI.getSelectedIds(),
                            PartialRoutesPercentageLimit: $scope.scopeModel.partialRoutesPercentageLimit,
                            PartialRoutesUpdateBatchSize: $scope.scopeModel.partialRoutesUpdateBatchSize
                        },
                        TechnicalQualityConfiguration: {
                            QualityAnalyticTableId: qualityAnalyticTableSelectorAPI.getSelectedIds(),
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);