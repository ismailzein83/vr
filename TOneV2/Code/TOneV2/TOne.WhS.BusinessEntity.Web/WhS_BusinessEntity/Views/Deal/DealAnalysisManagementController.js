(function (appControllers) {

    'use strict';

    DealAnalysisManagementController.$inject = ['$scope', 'WhS_BE_DealAnalysisService', 'WhS_BE_DealAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function DealAnalysisManagementController($scope, WhS_BE_DealAnalysisService, WhS_BE_DealAPIService, UtilsService, VRUIUtilsService, VRNotificationService) {
     
        var gridAPI;

        defineScope();
        load();

        function defineScope() {


          
            $scope.search = function () {

            };

            $scope.add = function () {
                var onDealAnalysisAdded = function () {

                };
                WhS_BE_DealAnalysisService.addDealAnalysis(onDealAnalysisAdded);
            };

         
        }

        function load() {
            loadAllControls();
        }

        function loadAllControls() {
          
        }

    }

    appControllers.controller('WhS_BE_DealAnalysisManagementController', DealAnalysisManagementController);

})(appControllers);