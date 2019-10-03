"use strict";

app.directive("retailBillingChargetypeExtendedsettingsCustomcode", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new BillingChargeTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/ChargeType/Definition/MainExtensions/Templates/CustomCodeChargeTypeTemplate.html"
        };

        function BillingChargeTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;

                    if (payload != undefined)
                        extendedSettings = payload != undefined ? payload.extendedSettings : undefined;

                    $scope.scopeModel.pricingLogic = extendedSettings != undefined ? extendedSettings.PricingLogic : undefined;

                    function loadDataRecordTypeSelector() {
                        var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                selectedIds: extendedSettings != undefined ? extendedSettings.TargetRecordTypeId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
                        });
                        return dataRecordTypeSelectorLoadDeferred.promise;
                    }
                    return UtilsService.waitPromiseNode({ promises: [loadDataRecordTypeSelector()] });
                };

                api.getData = function () {

                    var obj = {
                        $type: "Retail.Billing.MainExtensions.RetailBillingChargeType.RetailBillingCustomCodeChargeType,Retail.Billing.MainExtensions",
                        TargetRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        PricingLogic: $scope.scopeModel.pricingLogic
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);