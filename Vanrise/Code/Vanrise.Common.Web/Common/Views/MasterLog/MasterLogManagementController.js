(function (appControllers) {

    "use strict";

    masterLogManagementController.$inject = ['$scope', 'VRCommon_MasterLogService'];

    function masterLogManagementController($scope, VRCommon_MasterLogService) {


        defineScope();
        load();

        function defineScope() {
            $scope.drillDownDirectiveTabs = VRCommon_MasterLogService.getTabsDefinition();


           
        }

        function load() {
            
        }

        function loadAllControls() {
            
        }

    }

    appControllers.controller('VRCommon_MasterLogManagementController', masterLogManagementController);
})(appControllers);