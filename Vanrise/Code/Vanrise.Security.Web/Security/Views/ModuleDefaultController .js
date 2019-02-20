(function (appControllers) {

    'use strict';

    ModuleDefaultController.$inject = ['$rootScope', '$scope', 'VRNavigationService', 'UtilsService'];

    function ModuleDefaultController($rootScope, $scope, VRNavigationService, UtilsService) {
        if ($rootScope.moduleFilter) {
            $rootScope.hideToogleIcon = false;           
        }
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != undefined) {
            var moduleId = parameters.moduleId;
            $rootScope.seledtedModuleId = moduleId;            
        }        
        $rootScope.setSelectedMenuTile();

        $scope.moduleFilter = function (item) {
            return item.MenuType == 0 && item.RenderedAsView == false;
        };
        $scope.pageFilter = function (item) {
            return item.MenuType == 1 ||  item.RenderedAsView == true;
        };
        if ($rootScope.selectedtile && $rootScope.selectedtile.DefaultURL && ($rootScope.toModuleTilesView==false || $rootScope.showModuleTiles ==  false)) {
            if ($rootScope.moduleFilter) {
                $rootScope.hideToogleIcon = false;
            }
            window.location.href = $rootScope.selectedtile.DefaultURL;
        }
      
        $rootScope.openMenuNode = function (item) {
            if (item.Childs && item.Childs.length > 0) {
                $rootScope.selectedtile = item;
                $rootScope.goToModulePage(item);
            }
            else
                window.location.href = item.Location;
        };
    }

    appControllers.controller('VR_Sec_ModuleDefaultController', ModuleDefaultController);

})(appControllers);