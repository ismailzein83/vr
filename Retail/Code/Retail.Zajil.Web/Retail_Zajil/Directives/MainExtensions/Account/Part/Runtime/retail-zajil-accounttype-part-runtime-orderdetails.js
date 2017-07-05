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

        var orderdetails = [];

        this.initializeController = initializeController;

        ctrl.orderdetails = [];

        ctrl.loadMoreOrderDetails = function () {

            loadMoreGridData(ctrl.orderdetails, orderdetails);
        }

        ctrl.addOrderDetails = function () {
            var onOrderDetailAdded = function (orderDetailItem) {
                orderdetails.push(orderDetailItem);
                ctrl.orderdetails.push({
                    Entity: orderDetailItem
                });
            };
            Retail_Zajil_OrderDetailService.addOrderDetail(onOrderDetailAdded);
        };

        ctrl.removeOrderDetail = function (dataItem) {
            var index = orderdetails.indexOf(dataItem.Entity);
            orderdetails.splice(index, 1);
            ctrl.orderdetails.splice(index, 1);
        };

        ctrl.editOrderDetailItem = function (dataItem) {
            var onOrderDetailUpdated = function (updatedOrderDetail) {
                var index = orderdetails.indexOf(dataItem.Entity);
                orderdetails[index] = updatedOrderDetail;
                ctrl.orderdetails[index] = {
                    Entity: updatedOrderDetail
                };
            };
            Retail_Zajil_OrderDetailService.editOrderDetail(onOrderDetailUpdated, dataItem.Entity);
        };



        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            //ctrl.orderdetails = [];
            var api = {};
            var partSettings;
            api.load = function (payload) {
                if (payload != undefined && payload.partSettings != undefined) {
                    orderdetails = payload.partSettings.OrderDetailItems;
                    if (orderdetails != undefined && orderdetails.length > 0) {
                        //gridReadyPromises.push(emptyRateGridReadyDeferred.promise);
                        loadMoreGridData(ctrl.orderdetails, orderdetails);
                    }
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
            if (orderdetails != undefined) {
                for (var i = 0; i < orderdetails.length; i++) {
                    var orderDetail = orderdetails[i];
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

        function loadMoreGridData(gridArray, sourceArray) {
            if (sourceArray == undefined)
                return;
            var pageSize = 10 + gridArray.length;
            if (gridArray.length < sourceArray.length) {
                for (var i = gridArray.length; i < sourceArray.length && i < pageSize; i++) {
                    gridArray.push({
                        Entity: sourceArray[i]
                    });
                }
            }
        }
    }
}]);