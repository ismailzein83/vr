'use strict';

app.directive('vrNpSalezoneMasterplanSelector', ['Vr_NP_SellingNumberPlanAPIService', 'Vr_NP_SaleZoneAPIService', 'UtilsService', 'VRUIUtilsService',
    function (Vr_NP_SellingNumberPlanAPIService, Vr_NP_SaleZoneAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var selector = new saleZoneMasterPlanCtor(ctrl, $scope, $attrs);
                selector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            template: function (element, attrs) {
                return getBeSaleZoneTemplate(attrs);
            }
        };

        function getBeSaleZoneTemplate(attrs) {

            return '<vr-np-salezone-selector on-ready="scopeModal.onSaleZoneSelectorReady" selectedvalues="ctrl.selectedvalues" normal-col-num="{{ctrl.normalColNum}}" ismultipleselection>' +
                   '</vr-np-salezone-selector>';
        }

        function saleZoneMasterPlanCtor(saleZoneSelectorCtrl, $scope, attrs) {
            this.initializeController = initializeController;

            var sellingNumberPlanId;
            var npSalezoneSelectorCustomLabel;

            var saleZoneDirectiveAPI;
            var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModal = {};
                $scope.scopeModal.SellingNumberPlanId;

                $scope.scopeModal.onSaleZoneSelectorReady = function (api) {
                    saleZoneDirectiveAPI = api;
                    saleZoneReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;

                        if (payload.fieldTitle != undefined){
                            npSalezoneSelectorCustomLabel = payload.fieldTitle;
                        }
                    }

                    var promises = [];

                    var promiseDeferredLoadSelector = UtilsService.createPromiseDeferred();
                    promises.push(promiseDeferredLoadSelector.promise);

                    var masterPlanPromise = GetMasterPlan().then(function () {
                        loadSaleZoneSelectorSelector(selectedIds).then(function () {
                            promiseDeferredLoadSelector.resolve();
                        }).catch(function (error) {
                            promiseDeferredLoadSelector.reject(error);
                        });
                    }).catch(function (error) {
                        promiseDeferredLoadSelector.reject(error);
                    });

                    promises.push(masterPlanPromise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SaleZoneId', attrs, saleZoneSelectorCtrl);
                };

                if (saleZoneSelectorCtrl.onReady != null)
                    saleZoneSelectorCtrl.onReady(api);

            }

            function loadSaleZoneSelectorSelector(selectedIds) {
                var saleZonePlanLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                saleZoneReadyPromiseDeferred.promise.then(function () {

                    var directivePayload = {
                        sellingNumberPlanId: $scope.scopeModal.SellingNumberPlanId,
                        selectedIds: selectedIds
                    };
                    if (npSalezoneSelectorCustomLabel != undefined)
                        directivePayload.customLabel = npSalezoneSelectorCustomLabel;

                    VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, directivePayload, saleZonePlanLoadPromiseDeferred);
                });

                return saleZonePlanLoadPromiseDeferred.promise;
            }

            function GetMasterPlan() {
                return Vr_NP_SellingNumberPlanAPIService.GetMasterSellingNumberPlan().then(function (response) {
                    $scope.scopeModal.SellingNumberPlanId = response.SellingNumberPlanId;
                });
            }
        }

        return directiveDefinitionObject;
    }]);