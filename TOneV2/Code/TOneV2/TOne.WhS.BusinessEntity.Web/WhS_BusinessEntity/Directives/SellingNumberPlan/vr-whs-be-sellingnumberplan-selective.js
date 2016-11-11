'use strict';
app.directive('vrWhsBeSellingnumberplanSelective', ['WhS_BE_SaleZoneAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SaleZoneAPIService, WhS_BE_SellingNumberPlanAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new selectiveCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SellingNumberPlan/Templates/SelectiveSellingNumberPlansDirectiveTemplate.html"

        };

        function selectiveCtor(ctrl, $scope) {

            var sellingNumberPlanDirectiveAPI;
            var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanParameter;

            function initializeController() {

                $scope.onSellingNumberPlanDirectiveReady = function (api) {
                    sellingNumberPlanDirectiveAPI = api;
                    sellingNumberPlanReadyPromiseDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var sellingNumberPlanIds;

                    if (payload && payload.saleZoneGroupSettings) {
                        sellingNumberPlanIds = payload.saleZoneGroupSettings.SellingNumberPlanIds;
                    }


                    var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadSellingNumberPlanPromiseDeferred.promise);

                    sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                        var sellingNumberPlanPayload;

                        if (sellingNumberPlanIds != undefined) {
                            sellingNumberPlanPayload = {
                                selectedIds: sellingNumberPlanIds
                            };
                        }

                        VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, sellingNumberPlanPayload, loadSellingNumberPlanPromiseDeferred);
                    });


                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SellingNumberPlan.SelectiveSellingNumberPlan, TOne.WhS.BusinessEntity.MainExtensions",
                        SellingNumberPlanIds: sellingNumberPlanDirectiveAPI.getSelectedIds(),
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);