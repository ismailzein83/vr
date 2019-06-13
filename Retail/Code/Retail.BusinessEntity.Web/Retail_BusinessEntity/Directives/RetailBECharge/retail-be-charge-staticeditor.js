(function (app) {

    'use strict';

    RetailBEChargeStaticEditor.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function RetailBEChargeStaticEditor(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailBEChargeStaticEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/RetailBECharge/Templates/RetailBEChargeStaticEditorTemplate.html"

        };
        function RetailBEChargeStaticEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var chargeSettingsSelectiveAPI;
            var chargeSettingsSelectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onChargeSettingsSelectiveReady = function (api) {
                    chargeSettingsSelectiveAPI = api;
                    chargeSettingsSelectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    function loadChargeSettingsSelective() {
                        var chargeSettingsSelectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        chargeSettingsSelectiveReadyPromiseDeferred.promise.then(function () {
                            var selectivePayload = {
                                Settings: payload != undefined ? payload.Settings : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(chargeSettingsSelectiveAPI, selectivePayload, chargeSettingsSelectiveLoadPromiseDeferred);

                        });
                        return chargeSettingsSelectiveLoadPromiseDeferred.promise;
                    }

                    var rootPromiseNode = {
                        promises: [loadChargeSettingsSelective()]
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);

                };

                api.setData = function (retailBEChargeEntity) {
                    retailBEChargeEntity.Settings = chargeSettingsSelectiveAPI.getData();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeChargeStaticeditor', RetailBEChargeStaticEditor);

})(app);
