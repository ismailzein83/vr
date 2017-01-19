(function (appControllers) {

    'use strict';

    TQIEditor.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

    function TQIEditor($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                
            }
        }
        function defineScope() {

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
                loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
            });
        }

        function setTitle() {
            $scope.title = 'TQI Methods';
        }
      
       
    }

    appControllers.controller('WhS_Sales_TQIEditor', TQIEditor);

})(appControllers);