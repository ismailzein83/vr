(function (appControllers) {

    "use strict";

    masterLogManagementController.$inject = ['$scope', '$filter', 'VRCommon_MasterLogService', 'UtilsService'];

    function masterLogManagementController($scope, $filter, VRCommon_MasterLogService, UtilsService) {

        var promises = [];
        var accessibleTabs = [];
        defineScope();
        load();

        function defineScope() {           
          
         

           
        }

        function load() {
            loadAllControls()
           
        }

        function loadAllControls() {

            var allTabs = VRCommon_MasterLogService.getTabsDefinition();
           
            for (var i = 0; i < allTabs.length; i++) {
                var tab = allTabs[i];
                checkTabPermission(tab);
            }

            UtilsService.waitMultiplePromises(promises).then(function () {              
                $scope.drillDownDirectiveTabs = $filter('orderBy')(accessibleTabs, 'rank');
            });

           
            
        }
       
        function checkTabPermission(tab) {
            if (tab.hasPermission == undefined || tab.hasPermission == null) {
                accessibleTabs.push(tab);
                return;
            }
            if (typeof (tab.hasPermission) == 'function' )
             var promise = tab.hasPermission().then(function (isAllowed) {
                 if (isAllowed) {
                    accessibleTabs.push(tab);
                }
             });
             promises.push(promise)
        }
    }

    appControllers.controller('VRCommon_MasterLogManagementController', masterLogManagementController);
})(appControllers);