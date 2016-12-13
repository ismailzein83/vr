(function (appControllers) {

    "use strict";

    masterLogManagementController.$inject = ['$scope', 'VRCommon_MasterLogAPIService', 'UtilsService', 'VRNavigationService'];

    function masterLogManagementController($scope, VRCommon_MasterLogAPIService, UtilsService, VRNavigationService) {
        loadParameters();
        defineScope();
        load();
        var viewId;
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != null) {
                viewId = parameters.viewId;
            }
        }
        function defineScope() {
            $scope.drillDownDirectiveTabs = [];
        }

        function load() {
            loadAllControls()
        }

        function loadAllControls() {
            $scope.isLoading = true;
            VRCommon_MasterLogAPIService.GetMasterLogDirectives(viewId).then(function (response) {
                if (response.length > 0) {
                    for (var i = 0 ; i < response.length; i++) {
                        var obj = response[i];
                        var tabDefinition = {
                            title: obj.Title,
                            directive: obj.Directive,
                            loadDirective: function (directiveAPI) {
                                return directiveAPI.load();
                            }
                        }
                        $scope.drillDownDirectiveTabs.push(tabDefinition);
                    }
                }
            }).finally(function () {
                    $scope.isLoading = false;
            });
        }
       
        
    }

    appControllers.controller('VRCommon_MasterLogManagementController', masterLogManagementController);
})(appControllers);