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
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/RetailBECharge/Templates/RetailBEChargeEntityDirectiveTemplate.html"

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
                                settings: payload != undefined && payload.fieldValue != undefined ? payload.fieldValue.Settings : undefined,
                                title: payload != undefined ? payload.fieldTitle : undefined
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

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Entities.RetailBEChargeEntity, Retail.BusinessEntity.Entities",
                        Settings: chargeSettingsSelectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeChargeEntitydirective', RetailBEChargeStaticEditor);

})(app);
