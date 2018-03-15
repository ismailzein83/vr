(function (app) {

    'use strict';

    ANumberSaleCodeGridRuntimeViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_ANumberSaleCodeService', 'VRDateTimeService'];

    function ANumberSaleCodeGridRuntimeViewDirective(UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_ANumberSaleCodeService, VRDateTimeService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ANumberSaleCodeGridRuntimeViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/ANumber/Runtime/Templates/ANumberSaleCodeGridRuntimeViewTemplate.html'
        };

        function ANumberSaleCodeGridRuntimeViewCtor($scope, ctrl) {
            this.initializeController = initializeController;
            var aNumberGroupId;

            var saleCodeGridApi;


            var sellingNumberPlanSelectorAPI;
            var sellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.effectiveOn = VRDateTimeService.getNowDateTime();
                $scope.scopeModel.searchClicked = function () {
                    return saleCodeGridApi.loadGrid(buildGridQuery());
                };

                $scope.scopeModel.addSaleCodes = function () {
                    var onANumberSaleCodesAdded = function () {
                        saleCodeGridApi.loadGrid(buildGridQuery());
                    };
                    WhS_BE_ANumberSaleCodeService.addANumberSaleCodes(aNumberGroupId, onANumberSaleCodesAdded);
                };
                $scope.scopeModel.onSaleCodeGridReady = function (api) {
                    saleCodeGridApi = api;
                    return saleCodeGridApi.loadGrid(buildGridQuery());
                };
                $scope.scopeModel.onSellingNumberReady = function (api) {
                    sellingNumberPlanSelectorAPI = api;
                    sellingReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;
                    var promises = [];
                    if (payload != undefined) {
                        aNumberGroupId = payload.genericBusinessEntityId;
                    }
                    promises.push(loadSellingNumberPlanSelector());
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function loadSellingNumberPlanSelector() {
                var sellingLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                sellingReadyPromiseDeferred.promise.then(function () {
                    var directivePayload = { selectifsingleitem: true };
                    VRUIUtilsService.callDirectiveLoad(sellingNumberPlanSelectorAPI, directivePayload, sellingLoadPromiseDeferred);
                });
                return sellingLoadPromiseDeferred.promise;
            }

            function buildGridQuery() {
                return {
                    ANumberGroupId: aNumberGroupId,
                    SellingNumberPlanIds: sellingNumberPlanSelectorAPI.getSelectedIds(),
                    EffectiveOn: $scope.scopeModel.effectiveOn
                };
            }
        }
    }

    app.directive('whsBeAnumberSalecodegridviewRuntime', ANumberSaleCodeGridRuntimeViewDirective);

})(app);