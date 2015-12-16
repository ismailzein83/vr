(function (appControllers) {

    "use strict";

    defineCDRFieldsEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_CDRProcessing_DefineCDRFieldsAPIService', 'VRValidationService'];

    function defineCDRFieldsEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_CDRProcessing_DefineCDRFieldsAPIService, VRValidationService) {

        var isEditMode;
        var ruleId;

        var customerRuleEntity;
        loadParameters();
        defineScope();
        load();
        function loadParameters() {
            
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                ruleId = parameters.RuleId
            }
            isEditMode = (ruleId != undefined);
        }
        function defineScope() {
            $scope.SaveCDRField = function () {
                if (isEditMode) {
                    return updateCustomerRule();
                }
                else {
                    return insertCustomerRule();
                }
            };
            $scope.close = function () {
                $scope.modalContext.closeModal()
            };
         
        }

        function load() {
            $scope.isLoading = true;
            if (isEditMode) {
                $scope.title = UtilsService.buildTitleForUpdateEditor("Customer Identification Rule");

                loadAllControls();
                       
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor("Customer Identification Rule");
                loadAllControls();
            }

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadFilterBySection])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }
      
        function loadFilterBySection() {
          
        }

      

        function buildCustomerRuleObjectObjFromScope() {
            
           
           
            return customerRule;
        }

        function insertCustomerRule() {

            var customerRuleObject = buildCustomerRuleObjectObjFromScope();
            return WhS_CDRProcessing_DefineCDRFieldsAPIService.AddCDRField(customerRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("CDR Field", response)) {
                    if ($scope.onCDRFieldAdded != undefined)
                        $scope.onCDRFieldAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });

        }

        function updateCustomerRule() {
            var customerRuleObject = buildCustomerRuleObjectObjFromScope();
            customerRuleObject.RuleId = ruleId;
            WhS_CDRProcessing_DefineCDRFieldsAPIService.UpdateCDRField(customerRuleObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("CDR Field", response)) {
                    if ($scope.onCDRFieldUpdated != undefined)
                        $scope.onCDRFieldUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

    }

    appControllers.controller('WhS_CDRProcessing_DefineCDRFieldsEditorController', defineCDRFieldsEditorController);
})(appControllers);
