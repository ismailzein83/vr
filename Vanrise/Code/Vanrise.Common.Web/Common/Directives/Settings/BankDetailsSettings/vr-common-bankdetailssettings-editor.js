'use strict';

app.directive('vrCommonBankdetailssettingsEditor', ['UtilsService', 'VRUIUtilsService','VRCommon_BankDetailService',
    function (UtilsService, VRUIUtilsService, VRCommon_BankDetailService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BankDetailsSettingsEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/Settings/BankDetailsSettings/Templates/BankDetailsSettingsTemplate.html"
        };

        function BankDetailsSettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one action.";
                };
                ctrl.addBankDetail = function () {
                    var onBankDetailAdded = function (bankDetail) {
                        ctrl.datasource.push({ Entity: bankDetail });
                    };

                    VRCommon_BankDetailService.addBankDetail(onBankDetailAdded, ctrl.datasource);
                };
                ctrl.removeBankDetail = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var bankDetailsSettingsPayload;
                    if (payload != undefined && payload.data != undefined) {
                        bankDetailsSettingsPayload = payload.data;
                    }
                    if (bankDetailsSettingsPayload != undefined && bankDetailsSettingsPayload.BankDetails != undefined) {
                        for (var i = 0; i < bankDetailsSettingsPayload.BankDetails.length; i++) {
                            var bankDetail = bankDetailsSettingsPayload.BankDetails[i];
                            ctrl.datasource.push({ Entity: bankDetail });
                        }
                    }
                };

                api.getData = function () {
                    var bankDetails;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        bankDetails = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            bankDetails.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: "Vanrise.Entities.BankDetailsSettings, Vanrise.Entities",
                        BankDetails: bankDetails
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }
            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editBankDetail,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }
            function editBankDetail(bankDetailObj) {
                var onBankDetailUpdated = function (bankDetail) {
                    var index = ctrl.datasource.indexOf(bankDetailObj);
                    ctrl.datasource[index] = { Entity: bankDetail };
                };
                VRCommon_BankDetailService.editBankDetail(bankDetailObj.Entity, onBankDetailUpdated, ctrl.datasource);
            }
        }
    }]);