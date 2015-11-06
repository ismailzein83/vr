'use strict';
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


        var saleZoneGroupDirectiveAPI;
        var saleZoneGroupDirectiveReadyPromiseDeferred;

        function initializeController() {
            $scope.onSaleZoneGroupDirectiveReady = function (api) {
                saleZoneGroupDirectiveAPI = api;

                var setLoader = function (value) { $scope.isLoadingSaleZoneGroupDirective = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneGroupDirectiveAPI, undefined, setLoader, saleZoneGroupDirectiveReadyPromiseDeferred);
            }

            defineAPI();
        }

        function defineAPI() {
            var api = {};

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

            api.load = function (payload) {
                $scope.saleZoneGroupTemplates = [];
                var saleZoneConfigId;
                var saleZoneGroupSettings = { filter: {} };

                if (payload != undefined) {
                    if (payload.SaleZoneGroupSettings != null) {
                        saleZoneConfigId = payload.SaleZoneGroupSettings.ConfigId;
                        saleZoneGroupSettings.filter.SaleZoneFilterSettings = payload.SaleZoneGroupSettings;
                    }
                }
                var promises = [];
                var loadSaleZoneGroupTemplatesPromise = WhS_BE_SaleZoneAPIService.GetSaleZoneGroupTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.saleZoneGroupTemplates.push(item);
                    });

                    if (saleZoneConfigId != undefined)
                        $scope.selectedSaleZoneGroupTemplate = UtilsService.getItemByVal($scope.saleZoneGroupTemplates, saleZoneConfigId, "TemplateConfigID")
                });
                promises.push(loadSaleZoneGroupTemplatesPromise);

                if (saleZoneGroupSettings.filter.SaleZoneFilterSettings != undefined) {
                    saleZoneGroupDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var saleZoneGroupDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(saleZoneGroupDirectiveLoadPromiseDeferred.promise);

                    saleZoneGroupDirectiveReadyPromiseDeferred.promise.then(function () {
                        saleZoneGroupDirectiveReadyPromiseDeferred = undefined;
                        VRUIUtilsService.callDirectiveLoad(saleZoneGroupDirectiveAPI, saleZoneGroupSettings, saleZoneGroupDirectiveLoadPromiseDeferred);
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);