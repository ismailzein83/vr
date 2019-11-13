//"use strict";

//app.directive("retailBillingDiscountruleTargettypeSettings", ["UtilsService", "VRUIUtilsService",
//    function (UtilsService, VRUIUtilsService) {

//        var directiveDefinitionObject = {

//            restrict: "E",
//            scope:
//            {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;

//                var ctor = new BillingDiscountRuleTargetTypeSettingsCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingDiscountRule/TargetType/Templates/DiscountRuleTargetTypeSettingsTemplate.html"
//        };

//        function BillingDiscountRuleTargetTypeSettingsCtor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var targetRecordTypeSelectorAPI;
//            var targetRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {

//                $scope.scopeModel = {};

//                $scope.scopeModel.onTargetRecordTypeSelectorReady = function (api) {
//                    targetRecordTypeSelectorAPI = api;
//                    targetRecordTypeSelectorReadyDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {

//                    var targetTypeEntity;
//                    var extendedSettings;

//                    if (payload != undefined) {
//                        targetTypeEntity = payload.componentType; 
//                        $scope.scopeModel.name = targetTypeEntity != undefined ? targetTypeEntity.Name : undefined;
//                    }

//                    if (targetTypeEntity != undefined && targetTypeEntity.Settings != undefined) {
//                        extendedSettings = targetTypeEntity.Settings.ExtendedSettings;
//                    }

//                    function loadTargetRecordTypeSelector() {
//                        var targetRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

//                        targetRecordTypeSelectorReadyDeferred.promise.then(function () {
//                            var payload = {
//                                selectedIds: extendedSettings != undefined ? extendedSettings.TargetRecordTypeId : undefined
//                            };
//                            VRUIUtilsService.callDirectiveLoad(targetRecordTypeSelectorAPI, payload, targetRecordTypeSelectorLoadDeferred);
//                        });

//                        return targetRecordTypeSelectorLoadDeferred.promise;
//                    }

//                    return UtilsService.waitPromiseNode({ promises: [loadTargetRecordTypeSelector()] });
//                };

//                api.getData = function () {

//                    var settings = {
//                        $type: "Retail.Billing.Entities.DiscountRuleTargetTypeSettings,Retail.Billing.Entities",
//                        ExtendedSettings: {
//                            TargetRecordTypeId: targetRecordTypeSelectorAPI.getSelectedIds()
//                        }
//                    };

//                    return {
//                        Name: $scope.scopeModel.name,
//                        Settings: settings
//                    };
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }


//        }
//        return directiveDefinitionObject;
//    }
//]);