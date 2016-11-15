(function (appControllers) {

    'use strict';

    CodePreparationController.$inject = ['$scope', 'WhS_CP_CodePrepAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcess_BPInstanceService', 'VRUIUtilsService', 'UtilsService', 'BusinessProcess_BPInstanceAPIService'];

    function CodePreparationController($scope, WhS_CP_CodePrepAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcess_BPInstanceService, VRUIUtilsService, UtilsService, BusinessProcess_BPInstanceAPIService) {
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
            };
            $scope.upload = function () {

                var inputArguments = {
                    $type: "TOne.WhS.CodePreparation.BP.Arguments.CodePreparationInput, TOne.WhS.CodePreparation.BP.Arguments",
                    SellingNumberPlanId: $scope.selectedSellingNumberPlan.SellingNumberPlanId,
                    EffectiveDate: $scope.effectiveDate,
                    FileId: $scope.zoneList.fileId,
                    IsFromExcel: true
                };
                var input = {
                    InputArguments: inputArguments
                };


                return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                        return BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);

                });
            };
            $scope.downloadTemplate = function () {
                return WhS_CP_CodePrepAPIService.DownloadImportCodePreparationTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };
        }
        function load() {
        }
    };

    appControllers.controller('WhS_CP_CodePreparationController', CodePreparationController);

})(appControllers);
