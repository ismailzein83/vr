(function (appControllers) {

    "use strict";

    masterLogManagementController.$inject = ['$scope', 'VRCommon_MasterLogService'];

    function masterLogManagementController($scope, VRCommon_MasterLogService) {


        defineScope();
        load();

        function defineScope() {
            // 
            $scope.drillDownDirectiveTabs = VRCommon_MasterLogService.getTabsDefinition();

            //    [
            //    {
            //        title: "Data Source Log",
            //        directive: "vr-integration-log-search",
            //        loadDirective : function (directiveAPI) {
            //             return directiveAPI.load();
            //        }

            //    },
            //    {
            //        title: "Log Entry",
            //        directive: "vr-log-entry-search",
            //        loadDirective: function (directiveAPI) {
            //            return directiveAPI.load();
            //        }

            //    }
            //]
        }

        function load() {
            
        }

        function loadAllControls() {
            
        }

    }

    appControllers.controller('VRCommon_MasterLogManagementController', masterLogManagementController);
})(appControllers);