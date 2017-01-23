'use strict';

app.directive('retailZajilAccounttypePartRuntimeOrderdetails', ["UtilsService", "VRUIUtilsService", "Retail_Zajil_OrderDetailService", function (UtilsService, VRUIUtilsService, Retail_Zajil_OrderDetailService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountTypeOrderDetailsRuntime = new AccountTypeOrderDetailsRuntime($scope, ctrl, $attrs);
            accountTypeOrderDetailsRuntime.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_Zajil/Directives/MainExtensions/Account/Part/Runtime/Templates/AccountTypePartOrderDetailsRuntimeTemplate.html'
    };

    function AccountTypeOrderDetailsRuntime($scope, ctrl, $attrs) {
        ctrl.addOrderDetails = function () {
            var onOrderDetailAdded = function (orderDetailItem) {
                ctrl.orderdetails.push(orderDetailItem);
            };
            Retail_Zajil_OrderDetailService.addOrderDetail(onOrderDetailAdded);
        };

        ctrl.removeOrderDetail = function (dataItem) {
            var index = ctrl.orderdetails.indexOf(dataItem);
            ctrl.orderdetails.splice(index, 1);
        };

        this.initializeController = initializeController;

        function initializeController() {
            defineMenuActions();
            defineAPI();
        }
        function defineAPI() {
            ctrl.orderdetails = [];
            var api = {};
            var partSettings;
            api.load = function (payload) {
                if (payload != undefined && payload.partSettings != undefined) {
                    ctrl.orderdetails = payload.partSettings.OrderDetailItems;
                }

            };
            api.getData = function () {
                var data = buildOrderDetailItemsList();
                return {
                    $type: 'Retail.Zajil.MainExtensions.AccountPartOrderDetail, Retail.Zajil.MainExtensions',
                    OrderDetailItems: data
                };
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function defineMenuActions() {
            ctrl.gridMenuActions = [{
                name: "Edit",
                clicked: editOrderDetailItem,
            }];
        }
        function editOrderDetailItem(orderDetailItem) {
            var onOrderDetailUpdated = function (updatedOrderDetail) {
                var index = ctrl.orderdetails.indexOf(orderDetailItem);
                ctrl.orderdetails[index] = updatedOrderDetail;
            };
            Retail_Zajil_OrderDetailService.editOrderDetail(onOrderDetailUpdated, orderDetailItem);
        }

        function buildOrderDetailItemsList() {
            var tab = [];
            for (vari = 0; i < ctrl.orderdetails.length; i++) {
                tab.push({
                    Charges: ctrl.orderdetails[i].Charges,
                    Payment: ctrl.orderdetails[i].payment,
                    ContractPeriod: ctrl.orderdetails[i].ContractPeriod,
                    ContractRemain: ctrl.orderdetails[i].ContractRemain,
                    ContractDays: ctrl.orderdetails[i].ContractDays,
                    TotalContract: ctrl.orderdetails[i].TotalContract,
                    ChargesYear1: ctrl.orderdetails[i].ChargesYear1,
                    ChargesYear2: ctrl.orderdetails[i].ChargesYear2,
                    ChargesYear3: ctrl.orderdetails[i].ChargesYear3,
                    Installation: ctrl.orderdetails[i].Installation,
                    ThirdParty: ctrl.orderdetails[i].ThirdParty,
                    Discount: ctrl.orderdetails[i].Discount,
                    Achievement: ctrl.orderdetails[i].Achievement
                });
            }

            return tab;
        }
    }
}]);