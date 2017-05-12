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

        ctrl.editOrderDetailItem = function (dataItem) {
            var onOrderDetailUpdated = function (updatedOrderDetail) {
                var index = ctrl.orderdetails.indexOf(dataItem);
                ctrl.orderdetails[index] = updatedOrderDetail;
            };
            Retail_Zajil_OrderDetailService.editOrderDetail(onOrderDetailUpdated, dataItem);
        };

        this.initializeController = initializeController;

        function initializeController() {
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

        function buildOrderDetailItemsList() {
            var tab = [];
            if (ctrl.orderdetails != undefined)
            {
                for (var i = 0; i < ctrl.orderdetails.length; i++) {
                    var orderDetail = ctrl.orderdetails[i];
                    tab.push({
                        OrderId: orderDetail.OrderId,
                        Charges: orderDetail.Charges,
                        Payment: orderDetail.Payment,
                        ContractPeriod: orderDetail.ContractPeriod,
                        ContractRemain: orderDetail.ContractRemain,
                        ContractDays: orderDetail.ContractDays,
                        TotalContract: orderDetail.TotalContract,
                        ChargesYear1: orderDetail.ChargesYear1,
                        ChargesYear2: orderDetail.ChargesYear2,
                        ChargesYear3: orderDetail.ChargesYear3,
                        Installation: orderDetail.Installation,
                        ThirdParty: orderDetail.ThirdParty,
                        Discount: orderDetail.Discount,
                        Achievement: orderDetail.Achievement,
                        ParentSo: orderDetail.ParentSo,
                        ChildSo: orderDetail.ChildSo,
                        SalesAgent: orderDetail.SalesAgent,
                        Type: orderDetail.Type,
                        ConfirmDate: orderDetail.ConfirmDate,
                        CloseDate: orderDetail.CloseDate,
                        Remarks: orderDetail.Remarks
                    });
                }
            }
            return tab;
        }
    }
}]);