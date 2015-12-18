(function (appControllers) {

    "use strict";

    countryUploaderController.$inject = ['$scope', 'VRCommon_CountryAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function countryUploaderController($scope, VRCommon_CountryAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        
        defineScope();
        load();
        function defineScope() {
            $scope.uploadTypes = [
                { value: 1, name: "Add New Countries" },
                { value: 2, name: "Update New Countries" }
            ]

            $scope.downloadTemplate = function () {

                return VRCommon_CountryAPIService.DownloadCountriesTemplate(
                    $scope.selectedUploadType.value
                    ).then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            }

            $scope.uploadCountires = function () {
                var countryFile = {
                    FileId: file.fileId,
                    Type:$scope.selectedUploadType.value
                }
                return VRCommon_CountryAPIService.uploadCountires(countryFile).then(function (response) {
                    VRNotificationService.showInformation(response)
                });
            }
        }

        function load() {
               
          
        }


        
    }

    appControllers.controller('VRCommon_CountryUploaderController', countryUploaderController);
})(appControllers);
