'use strict';

app.directive('vrWhsBeSalezonegroupAllexcept', ['WhS_BE_SaleZoneAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SaleZoneAPIService, WhS_BE_SellingNumberPlanAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                sellingnumberplanid: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new allExceptCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService);
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
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SaleZoneGroup/Templates/SelectiveSaleZonesDirectiveTemplate.html';
            }
        };

        function allExceptCtor(ctrl, $scope, WhS_BE_SaleZoneAPIService) {
            this.initializeController = initializeController;

            var saleZoneSelectorAPI;
            var saleZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var sellingNumberPlanParameter;

            function initializeController() {
                $scope.onSaleZoneSelectorReady = function (api) {
                    saleZoneSelectorAPI = api;
                    saleZoneSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var sellingNumberPlanId;
                    var saleZoneGroupSettings;
                    var saleZoneFilterSettings;

                    if (payload != undefined) {
                        sellingNumberPlanId = payload.sellingNumberPlanId;
                        saleZoneGroupSettings = payload.saleZoneGroupSettings;
                        saleZoneFilterSettings = payload.saleZoneFilterSettings;
                    }

                    var promises = [];
                    promises.push(loadSaleZoneSelector());

                    function loadSaleZoneSelector() {
                        var loadSaleZoneSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        saleZoneSelectorReadyDeferred.promise.then(function () {
                            var sellingNumberPlanId = sellingNumberPlanId != undefined ? sellingNumberPlanId : saleZoneGroupSettings != undefined ? saleZoneGroupSettings.SellingNumberPlanId : undefined;

                            var saleZonePayload = {
                                filter: { SaleZoneFilterSettings: saleZoneFilterSettings != undefined ? saleZoneFilterSettings : undefined },
                                sellingNumberPlanId: sellingNumberPlanId,
                                selectedIds: saleZoneGroupSettings != undefined ? saleZoneGroupSettings.ZoneIds : undefined,
                                showSellingNumberPlanIfMultiple: true
                            };
                            VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, saleZonePayload, loadSaleZoneSelectorPromiseDeferred);
                        });

                        return loadSaleZoneSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.AllExceptSaleZoneGroup, TOne.WhS.BusinessEntity.MainExtensions",
                        SellingNumberPlanId: saleZoneSelectorAPI.getSellingNumberPlanId(),
                        ZoneIds: saleZoneSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);