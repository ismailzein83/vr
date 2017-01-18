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
                orderDetailObj = parameters.orderDetailEntity;
            }
            isEditMode = (orderDetailObj != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.SaveOrderDetail = function () {
                if (isEditMode) {
                    return updateOrderDetail();
                }
                else {
                    return insertOrderDetail();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
             loadAllControls(); 

            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([ setTitle, loadStaticData]).then(function () {

                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });


                function setTitle() {
                    if (isEditMode && orderDetailObj != undefined)
                        $scope.title = 'Edit OrderDetail';
                    else
                        $scope.title = UtilsService.buildTitleForAddEditor('OrderDetail');
                }

                function loadStaticData() {
                    if (!isEditMode)
                        return;
                    $scope.scopeModel.charges = orderDetailObj.Charges;
                    $scope.scopeModel.payment = orderDetailObj.Payment;
                    $scope.scopeModel.contractPeriod = orderDetailObj.ContractPeriod;
                    $scope.scopeModel.contractRemain = orderDetailObj.ContractRemain
                    $scope.scopeModel.contractDays = orderDetailObj.ContractDays;
                    $scope.scopeModel.totalContract = orderDetailObj.TotalContract;
                    $scope.scopeModel.chargesYear1 = orderDetailObj.ChargesYear1;
                    $scope.scopeModel.chargesYear2 = orderDetailObj.ChargesYear2;
                    $scope.scopeModel.chargesYear3 = orderDetailObj.ChargesYear3;
                    $scope.scopeModel.installation = orderDetailObj.Installation;
                    $scope.scopeModel.thirdParty = orderDetailObj.ThirdParty;
                    $scope.scopeModel.discount = orderDetailObj.Discount;
                    $scope.scopeModel.achievement = orderDetailObj.Achievement;
                }
            }
        }

        function buildOrderDetailObjFromScope() {
            var obj = {               
                Charges:$scope.scopeModel.charges,
                Payment:$scope.scopeModel.payment,
                ContractPeriod:$scope.scopeModel.contractPeriod,                
                ContractRemain:$scope.scopeModel.contractRemain,
                ContractDays:$scope.scopeModel.contractDays,
                TotalContract:$scope.scopeModel.totalContract,
                ChargesYear1:$scope.scopeModel.chargesYear1,
                ChargesYear2:$scope.scopeModel.chargesYear2,
                ChargesYear3:$scope.scopeModel.chargesYear3,
                Installation:$scope.scopeModel.installation,
                ThirdParty:$scope.scopeModel.thirdParty,
                Discount:$scope.scopeModel.discount,
                Achievement: $scope.scopeModel.achievement
            };
            return obj;
        }

        function insertOrderDetail() {
            var obj = buildOrderDetailObjFromScope();
             if ($scope.onOrderDetailAdded != undefined)
                 $scope.onOrderDetailAdded(obj);
             $scope.modalContext.closeModal();   
        }

        function updateOrderDetail() {
            var obj = buildOrderDetailObjFromScope();
            if ($scope.onOrderDetailUpdated != undefined)
                $scope.onOrderDetailUpdated(obj);
            $scope.modalContext.closeModal();
        }

    }

    appControllers.controller('Retail_Zagil_OrderDetailEditorController', OrderDetailEditorController);
})(appControllers);
