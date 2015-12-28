CodePreparationController.$inject = ['$scope','WhS_CodePrep_CodePrepAPIService','WhS_BP_CreateProcessResultEnum','BusinessProcessService','VRUIUtilsService','UtilsService'];

function CodePreparationController($scope, WhS_CodePrep_CodePrepAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcessService, VRUIUtilsService, UtilsService) {
    var sellingNumberPlanDirectiveAPI;
    var sellingNumberPlanReadyPromiseDeferred;

    var currencyDirectiveAPI;
    var currencyReadyPromiseDeferred;

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
        $scope.onSellingNumberPlanSelectorReady = function (api) {
            sellingNumberPlanDirectiveAPI = api;
            var setLoader = function (value) { $scope.isLoadingSellingNumberPlan = value };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sellingNumberPlanDirectiveAPI, undefined, setLoader, sellingNumberPlanReadyPromiseDeferred);
        }
        $scope.upload = function () {
            return WhS_CodePrep_CodePrepAPIService.UploadSaleZonesList($scope.selectedSellingNumberPlan.SellingNumberPlanId, $scope.zoneList.fileId, $scope.effectiveDate,true).then(function (response) {
                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                  return  BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
            });
        }
        $scope.downloadTemplate = function () {
            return WhS_CodePrep_CodePrepAPIService.DownloadImportCodePreparationTemplate().then(function (response) {
                UtilsService.downloadFile(response.data, response.headers);
            });
        }
    }
    function load() {
    }



};



appControllers.controller('WhS_CodePreparation_CodePreparationController', CodePreparationController);