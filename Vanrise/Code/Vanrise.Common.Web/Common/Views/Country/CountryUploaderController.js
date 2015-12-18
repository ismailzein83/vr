(function (appControllers) {

    "use strict";

    countryUploaderController.$inject = ['$scope', 'VRCommon_CountryAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function countryUploaderController($scope, VRCommon_CountryAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        
        defineScope();
        load();
        function defineScope() {
            $scope.downloadTemplate = function () {

                return VRCommon_CountryAPIService.DownloadCountriesTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            }

            $scope.uploadCountires = function () {                
                return VRCommon_CountryAPIService.UploadCountries($scope.file.fileId).then(function (response) {
                    VRNotificationService.showInformation(response)
                });
            }
        }

        function load() {
               
          
        }


        
    }

    appControllers.controller('VRCommon_CountryUploaderController', countryUploaderController);
})(appControllers);
