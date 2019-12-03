"use strict";

app.directive("retailBillingDiscountruleTargettypeSettings", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BillingDiscountRuleTargetTypeSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingDiscountRule/TargetType/Templates/DiscountRuleTargetTypeSettingsTemplate.html"
        };

        function BillingDiscountRuleTargetTypeSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var targetRecordTypeSelectorAPI;
            var targetRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTargetRecordTypeSelectorReady = function (api) {
                    targetRecordTypeSelectorAPI = api;
                    targetRecordTypeSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var settings;

                    if (payload != undefined) {
                        var targetTypeEntity = payload.componentType;

                        if (targetTypeEntity != undefined) {
                            settings = targetTypeEntity.Settings;
                            $scope.scopeModel.name = targetTypeEntity.Name;
                        }
                    }

                    function loadTargetRecordTypeSelector() {
                        var targetRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        targetRecordTypeSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                selectedIds: settings != undefined ? settings.TargetRecordTypeId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(targetRecordTypeSelectorAPI, payload, targetRecordTypeSelectorLoadDeferred);
                        });

                        return targetRecordTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: [loadTargetRecordTypeSelector()] });
                };

                api.getData = function () {

                    var settings = {
                        $type: "Retail.Billing.Entities.DiscountRuleTargetTypeSettings, Retail.Billing.Entities",
                        TargetRecordTypeId: targetRecordTypeSelectorAPI.getSelectedIds()
                    };

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: settings
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);