(function (appControllers) {

    'use strict';

    DefaultController.$inject = ['$rootScope', '$scope', 'VRNavigationService', 'UISettingsService'];

    function DefaultController($rootScope, $scope, VRNavigationService, UISettingsService) {

        if ($rootScope.moduleFilter) {
            $rootScope.hideToogleIcon = true;
            $rootScope.toogledStatuts(false);
        }

        if (UISettingsService.getDefaultPageURl() && (!$rootScope.toTilesView || !$rootScope.showApplicationTiles)) {
            window.location.href = UISettingsService.getDefaultPageURl();
        }
             $rootScope.selectedMenu = null;
        $rootScope.openMenuTile = function (item) {
            $rootScope.selectedtile = null;
            $rootScope.selectedMenu = null;
            $rootScope.goToModulePage(item);
        };
    }

    appControllers.controller('VR_Sec_DefaultController', DefaultController);

})(appControllers);