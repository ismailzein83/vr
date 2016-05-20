(function (appControllers) {

    "use strict";

    masterLogManagementController.$inject = ['$scope', '$filter', 'VRCommon_MasterLogService'];

    function masterLogManagementController($scope, $filter, VRCommon_MasterLogService) {


        defineScope();
        load();

        function defineScope() {
            //var tabs = VRCommon_MasterLogService.getTabsDefinition();
            //console.log(tabs);
            //tabs = $filter('orderBy')(tabs, 'title');
            //console.log(tabs);
            $scope.drillDownDirectiveTabs = $filter('orderBy')(VRCommon_MasterLogService.getTabsDefinition(), 'title');// VRCommon_MasterLogService.getTabsDefinition();


           
        }

        function load() {
            
        }

        function loadAllControls() {
            
        }

    }

    appControllers.controller('VRCommon_MasterLogManagementController', masterLogManagementController);
})(appControllers);