(function (appControllers) {

    'use strict';

    VoucherCardsActivatorController.$inject = ['$scope', 'VR_Voucher_VoucherCardsAPIService', 'VR_Voucher_VoucherCardsGenerationAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function VoucherCardsActivatorController($scope, VR_Voucher_VoucherCardsAPIService, VR_Voucher_VoucherCardsGenerationAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

       
        var voucherCardsGenerationId;
        var inactiveCards;
        var totalnumberofcards;


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                voucherCardsGenerationId = parameters.genericBusinessEntityId;
            } 
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.activate = function () {
                return activate();
        };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
    }

        function load() {
            $scope.isLoading = true;            
            getGetVoucherCardsGeneration().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });   
        }

        function getGetVoucherCardsGeneration() {
            return VR_Voucher_VoucherCardsGenerationAPIService.GetVoucherCardsGeneration(voucherCardsGenerationId).then(function (response) {
                totalnumberofcards = response.NumberOfCards;
                if (response.InactiveCards != undefined)
                { inactiveCards = response.InactiveCards; }
                else inactiveCards = totalnumberofcards;
                            });
        }

       

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false; $scope.scopeModel.ready = true;
              });
        }
        function setTitle() {
                  $scope.title = "Voucher Cards Activator";           
        }
        function loadStaticData() {
            $scope.scopeModel.inactiveCards = inactiveCards;
            $scope.scopeModel.numberofcards = inactiveCards;

        }
        function buildActivateVoucherCardsActivationInputObjFromScope() {
            var VoucherCardsActivationInput = {
                    VoucherCardsGenerationId: voucherCardsGenerationId,
                    Numberofcards: $scope.scopeModel.numberofcards
            };
            return VoucherCardsActivationInput;
        }
        function activate() {
            $scope.isLoading = true;
            var VoucherCardsActivationInput = buildActivateVoucherCardsActivationInputObjFromScope();
            
            return VR_Voucher_VoucherCardsAPIService.ActivateVoucherCards(VoucherCardsActivationInput).then(function (response) {
                if (response.Result == 0) {
                    if ($scope.onGenericBEUpdated != undefined) {
                        $scope.onGenericBEUpdated(response.UpdatedObject);
                    }
                }
                else VRNotificationService.notifyException(error, $scope);
                    $scope.modalContext.closeModal();
            
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
        });
    }

   }

    appControllers.controller('VR_Voucher_VoucherCardsActivatorController', VoucherCardsActivatorController);
})(appControllers);