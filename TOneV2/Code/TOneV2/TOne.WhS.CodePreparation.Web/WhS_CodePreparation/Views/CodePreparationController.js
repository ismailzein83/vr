CodePreparationController.$inject = ['$scope', 'WhS_BE_SellingNumberPlanAPIService','WhS_CodePrep_CodePrepAPIService','WhS_BP_CreateProcessResultEnum','BusinessProcessService'];

function CodePreparationController($scope, WhS_BE_SellingNumberPlanAPIService, WhS_CodePrep_CodePrepAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcessService) {
    defineScope();
    loadParameters();
    load();
    function loadParameters() {
    }
    function defineScope() {
        $scope.sellingNumberPlans = [];
        $scope.selectedSellingNumberPlan;
        $scope.effectiveDate = new Date();
        $scope.zoneList;
        $scope.upload = function () {
         return WhS_CodePrep_CodePrepAPIService.UploadSaleZonesList($scope.selectedSellingNumberPlan.SellingNumberPlanId, $scope.zoneList.fileId, $scope.effectiveDate).then(function (response) {
                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                  return  BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
            });
        }
    }
    function load() {
        loadSellingNumberPlans();
    }
    function loadSellingNumberPlans() {
        $scope.isInitializing = true;
        return WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
            for (var i = 0; i < response.length; i++)
                $scope.sellingNumberPlans.push(response[i]);
        }).finally(function () {
            $scope.isInitializing = false;
        });
    }


};



appControllers.controller('WhS_CodePreparation_CodePreparationController', CodePreparationController);