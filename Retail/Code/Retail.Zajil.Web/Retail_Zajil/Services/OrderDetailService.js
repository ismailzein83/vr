(function (app) {

    'use strict';

    OrderDetailService.$inject = ['VRModalService', 'VRNotificationService'];

    function OrderDetailService(VRModalService, VRNotificationService) {
        return {      
            editOrderDetail: editOrderDetail,
            addOrderDetail: addOrderDetail
        };


        function addOrderDetail(onOrderDetailAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOrderDetailAdded = onOrderDetailAdded;
            };

            var parameters = {
            };

            VRModalService.showModal('/Client/Modules/Retail_Zajil/Views/OrderDetail/OrderDetailEditor.html', parameters, modalSettings);
        }

        function editOrderDetail(onOrderDetailUpdated, orderDetailEntity) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onOrderDetailUpdated = onOrderDetailUpdated;
            };

            var parameters = {
                orderDetailEntity: orderDetailEntity
            };

            VRModalService.showModal('/Client/Modules/Retail_Zajil/Views/OrderDetail/OrderDetailEditor.html', parameters, modalSettings);
        }

    }

    app.service('Retail_Zajil_OrderDetailService', OrderDetailService);

})(app);