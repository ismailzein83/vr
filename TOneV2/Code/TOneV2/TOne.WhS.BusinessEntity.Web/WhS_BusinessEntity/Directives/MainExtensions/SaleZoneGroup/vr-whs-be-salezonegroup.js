﻿'use strict';
app.directive('vrWhsBeSalezonegroup', ['UtilsService', '$compile', 'WhS_BE_SaleZoneAPIService', 'VRNotificationService', 'VRUIUtilsService',
function (UtilsService, $compile, WhS_BE_SaleZoneAPIService, VRNotificationService, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new beSaleZoneGroup(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/SaleZoneGroup/Templates/SaleZoneTemplate.html"

    };


    function beSaleZoneGroup(ctrl, $scope, $attrs) {

        var payloadFilter;
        var saleZoneGroupDirectiveAPI;
        var saleZoneGroupDirectiveReadyPromiseDeferred;

        function initializeController() {
            $scope.onSaleZoneGroupDirectiveReady = function (api) {
                saleZoneGroupDirectiveAPI = api;
                var setLoader = function (value) { $scope.isLoadingSaleZoneGroupDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneGroupDirectiveAPI, payloadFilter, setLoader, saleZoneGroupDirectiveReadyPromiseDeferred);
            }

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                $scope.saleZoneGroupTemplates = [];
                var saleZoneConfigId;
                var saleZoneGroupPayload;
                if (payload != undefined) {
                    payloadFilter = {
                        saleZoneFilterSettings: payload.saleZoneFilterSettings,
                        sellingNumberPlanId: payload.sellingNumberPlanId != undefined ? payload.sellingNumberPlanId : undefined,
                    }

                    saleZoneGroupPayload = {
                        sellingNumberPlanId: payload.sellingNumberPlanId != undefined ? payload.sellingNumberPlanId : undefined,
                        saleZoneFilterSettings: payload.saleZoneFilterSettings,
                        saleZoneGroupSettings: payload.saleZoneGroupSettings != undefined ? payload.saleZoneGroupSettings : payload
                    }
                    saleZoneConfigId = payload.saleZoneGroupSettings != undefined ? payload.saleZoneGroupSettings.ConfigId : payload.ConfigId;
                }
                var promises = [];

                var loadSaleZoneGroupTemplatesPromise = WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {

                        if (!payload.saleZoneFilterSettings || !payload.saleZoneFilterSettings.RoutingProductId || !item.Settings || !item.Settings.IsHiddenInRP) {
                            $scope.saleZoneGroupTemplates.push(item);
                        }
                    });

                    if (saleZoneConfigId != undefined)
                        $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, saleZoneConfigId, "TemplateConfigID");

                });
                promises.push(loadSaleZoneGroupTemplatesPromise);

                if (saleZoneConfigId != undefined) {
                    saleZoneGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var saleZoneGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(saleZoneGroupDirectiveLoadPromiseDeferred.promise);

                    saleZoneGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                        saleZoneGroupDirectiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(saleZoneGroupDirectiveAPI, saleZoneGroupPayload, saleZoneGroupDirectiveLoadPromiseDeferred);
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            api.getData = function () {
                var saleZoneGroupSettings;
                if ($scope.selectedSaleZoneGroupTemplate != undefined) {
                    if (saleZoneGroupDirectiveAPI != undefined) {
                        saleZoneGroupSettings = saleZoneGroupDirectiveAPI.getData();
                        saleZoneGroupSettings.ConfigId = $scope.selectedSaleZoneGroupTemplate.TemplateConfigID;
                    }
                }
                return saleZoneGroupSettings;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);