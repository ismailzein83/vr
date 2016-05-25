(function (appControllers) {

    "use strict";

    masterLogManagementController.$inject = ['$scope', '$filter', 'VRCommon_MasterLogService'];

    function masterLogManagementController($scope, $filter, VRCommon_MasterLogService) {


        defineScope();
        load();

        function defineScope() {           
            VRCommon_MasterLogService.getTabsDefinition().then(function (response) {
                $scope.drillDownDirectiveTabs = $filter('orderBy')(response, 'title');
            });
         

           
        }

        function load() {
            
        }

        function loadAllControls() {
            
        }

    }

    appControllers.controller('VRCommon_MasterLogManagementController', masterLogManagementController);
})(appControllers);