(function (app) {

    'use strict';

    ANumberSupplierCodeGridRuntimeViewDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'WhS_BE_ANumberSupplierCodeService', 'VRDateTimeService'];

    function ANumberSupplierCodeGridRuntimeViewDirective(UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_ANumberSupplierCodeService, VRDateTimeService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ANumberSupplierCodeGridRuntimeViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/ANumber/Runtime/Templates/ANumberSupplierCodeGridRuntimeViewTemplate.html'
        };

        function ANumberSupplierCodeGridRuntimeViewCtor($scope, ctrl) {
            this.initializeController = initializeController;
            var aNumberGroupId;

            var SupplierCodeGridApi;


            var supplierSelectorAPI;
            var supplierReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.effectiveOn = VRDateTimeService.getNowDateTime();

                $scope.scopeModel.searchClicked = function () {
                    return SupplierCodeGridApi.loadGrid(buildGridQuery());
                };

                $scope.scopeModel.addSupplierCodes = function () {
                    var onANumberSupplierCodesAdded = function () {
                        SupplierCodeGridApi.loadGrid(buildGridQuery());
                    };
                    WhS_BE_ANumberSupplierCodeService.addANumberSupplierCodes(aNumberGroupId, onANumberSupplierCodesAdded);
                };
                $scope.scopeModel.onSupplierCodeGridReady = function (api) {
                    SupplierCodeGridApi = api;
                    return SupplierCodeGridApi.loadGrid(buildGridQuery());
                };
                $scope.scopeModel.onSupplierReady = function (api) {
                    supplierSelectorAPI = api;
                    supplierReadyPromiseDeferred.resolve();
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
                    promises.push(loadSupplierSelector());
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function loadSupplierSelector() {
                var supplierLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                supplierReadyPromiseDeferred.promise.then(function () {
                    var directivePayload = { selectifsingleitem: true };
                    VRUIUtilsService.callDirectiveLoad(supplierSelectorAPI, directivePayload, supplierLoadPromiseDeferred);
                });
                return supplierLoadPromiseDeferred.promise;
            }

            function buildGridQuery() {
                return {
                    ANumberGroupId: aNumberGroupId,
                    SupplierIds: supplierSelectorAPI.getSelectedIds(),
                    EffectiveOn: $scope.scopeModel.effectiveOn
                };
            }
        }
    }

    app.directive('whsBeAnumberSuppliercodegridviewRuntime', ANumberSupplierCodeGridRuntimeViewDirective);

})(app);