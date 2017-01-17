(function (appControllers) {

    "use strict";

    OrderDetailEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function OrderDetailEditorController($scope, UtilsService, VRNotificationService, VRNavigationService) {

        var isEditMode;
        var orderDetailObj;
        var exitingOrderDetails;
        loadParameters();
        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                orderDetailObj = parameters.orderDetailObjValue;
            }
            isEditMode = (orderDetailObj != undefined);
        }

        function defineScope() {
            $scope.scopeModal = {};

            $scope.scopeModal.SaveOrderDetail = function () {
                if (isEditMode) {
                    return updateOrderDetail();
                }
                else {
                    return insertOrderDetail();
                }
            };
            $scope.scopeModal.close = function () {
                $scope.modalContext.closeModal();
            };

       
        }

        function load() {
            $scope.scopeModal.isLoading = true;
             loadAllControls(); 

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([ setTitle, loadStaticData]).then(function () {

                }).finally(function () {
                    $scope.scopeModal.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


                function setTitle() {
                    if (isEditMode && orderDetailObjValue != undefined)
                        $scope.title = 'Edit OrderDetail';
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('OrderDetail');
                }

                function loadStaticData() {
                    $scope.scopeModal.OrderDetailValue = orderDetailObjValue;
                }


            }
        }

        function buildOrderDetailObjFromScope() {
            var obj = {

            };
            return obj;
        }

        function insertOrderDetail() {
            var orderDetailObj = buildOrderDetailObjFromScope();
             if ($scope.onOrderDetailAdded != undefined)
                 $scope.onOrderDetailAdded(orderDetailObj);
             $scope.modalContext.closeModal();   
        }

        function updateOrderDetail() {
            var OrderDetail = buildOrderDetailObjFromScope();
            if ($scope.onOrderDetailUpdated != undefined)
                $scope.onOrderDetailUpdated(orderDetailObj);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('Retail_Zagil_OrderDetailEditorController', OrderDetailEditorController);
})(appControllers);
