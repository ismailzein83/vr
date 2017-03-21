'use strict';

app.directive('vrAccountbalanceAccounttypeSourceBillingtransaction', ['UtilsService', 'VRUIUtilsService','VR_AccountBalance_AccountTypeService',
    function (UtilsService, VRUIUtilsService, VR_AccountBalance_AccountTypeService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var staticFieldSourceSetting = new LiveBalanceSourceSetting($scope, ctrl, $attrs);
                staticFieldSourceSetting.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/MainExtensions/Templates/BillingTransactionSourceSetting.html'
        };

        function LiveBalanceSourceSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var context;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.addBillingTransactionField = function () {
                    var onBillingTransactionFieldAdded = function (source) {
                        ctrl.datasource.push({ Entity: source });
                    };
                    VR_AccountBalance_AccountTypeService.addBillingTransactionField(onBillingTransactionFieldAdded, getContext());
                };

                ctrl.removeBillingTransactionField = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }


            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var billingTransactionFields;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        billingTransactionFields = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            billingTransactionFields.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: " Vanrise.AccountBalance.MainExtensions.AccountBalanceFieldSource.BillingTransactionFieldSourceSetting,  Vanrise.AccountBalance.MainExtensions",
                        BillingTransactionFields:billingTransactionFields
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        var sourceSettingEntity = payload.sourceSettingEntity;
                        if(sourceSettingEntity != undefined && sourceSettingEntity.BillingTransactionFields != undefined)
                        {
                            for (var i = 0; i < sourceSettingEntity.BillingTransactionFields.length; i++) {
                                var billingTransactionField = sourceSettingEntity.BillingTransactionFields[i];
                                ctrl.datasource.push({ Entity: billingTransactionField });
                            }
                        }
                        
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editBillingTransactionField,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editBillingTransactionField(billingTransactionFieldObj) {
                var onBillingTransactionFieldUpdated = function (billingTransactionField) {
                    var index = ctrl.datasource.indexOf(billingTransactionFieldObj);
                    ctrl.datasource[index] = { Entity: billingTransactionField };
                };
                VR_AccountBalance_AccountTypeService.editBillingTransactionField(billingTransactionFieldObj.Entity, onBillingTransactionFieldUpdated, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};

                return currentContext;
            }

        }
    }]);
