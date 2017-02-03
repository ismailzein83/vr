(function (appControllers) {
    "use strict";

    RingoMNPReportManagementController.$inject = ['$scope', 'Retail_Ringo_RingoReportSheetAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function RingoMNPReportManagementController($scope, Retail_Ringo_RingoReportSheetAPIService, utilsService, vrUIUtilsService, vrNotificationService) {

        defineScope();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.generateReport = function () {
                var filter = {
                    FromDate: $scope.scopeModel.fromDate,
                    ToDate: $scope.scopeModel.toDate
                };
                return Retail_Ringo_RingoReportSheetAPIService.DownloadMNPReport(filter).then(function (response) {
                    utilsService.downloadFile(response.data, response.headers);
                });
            };

        }
    }

    appControllers.controller('Retail_Ringo_MNPReportManagementController', RingoMNPReportManagementController);
})(appControllers);