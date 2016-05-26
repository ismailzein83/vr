'use strict';
app.directive('vrWhsBeSalezoneMasterplanSelector', ['WhS_BE_SellingNumberPlanAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SellingNumberPlanAPIService, UtilsService, VRUIUtilsService) {

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
            var label = "Sale Zone Master Plan";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Sale Zone Master Plan";
                multipleselection = "ismultipleselection";
            }

            return '<vr-whs-be-salezone-selector on-ready="scopeModal.onSaleZoneSelectorReady" selectedvalues="scopeModal.selectedSaleZones" normal-col-num="{{ctrl.normalColNum}}" ismultipleselection>'
            + '</vr-whs-be-salezone-selector>'
        }


        function saleZoneMasterPlanCtor(saleZoneSelectorCtrl, $scope, attrs) {

            var saleZoneDirectiveAPI;
            var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanId = undefined;

            function initializeController() {
                $scope.scopeModal = {};
                $scope.scopeModal.selectedSaleZones = [];
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
                    var promiseDeferredLoadSelector = UtilsService.createPromiseDeferred();
                    var promises = [];
                    promises.push(promiseDeferredLoadSelector.promise);

                    var masterPlanPromise = GetMasterPlan().then(function () {
                        loadSaleZoneSelectorSelector().then(function () {
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

                function loadSaleZoneSelectorSelector() {

                    var saleZonePlanLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    saleZoneReadyPromiseDeferred.promise
    .then(function () {
        var directivePayload = {
            sellingNumberPlanId: $scope.scopeModal.SellingNumberPlanId
        }

        VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, directivePayload, saleZonePlanLoadPromiseDeferred);
    });
                    return saleZonePlanLoadPromiseDeferred.promise;
                }

                function GetMasterPlan() {

                    return WhS_BE_SellingNumberPlanAPIService.GetMasterSellingNumberPlan().then(function (response) {
                        $scope.scopeModal.SellingNumberPlanId = response.SellingNumberPlanId;
                    });
                }

                if (saleZoneSelectorCtrl.onReady != null)
                    saleZoneSelectorCtrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);