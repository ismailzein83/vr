(function (app) {

    'use strict';

    ServicePackageVolumeItemDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ServicePackageVolumeItemDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var servicePackageVolumeItem = new ServicePackageVolumeItem($scope, ctrl, $attrs);
                servicePackageVolumeItem.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Package/PackageItem/Templates/ServicePackageVolumeItemTemplate.html"

        };
        function ServicePackageVolumeItem($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.ServicePackageItem.ServicePackageVolumeItem,Retail.BusinessEntity.MainExtensions",
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBePackagePackageitemVolume', ServicePackageVolumeItemDirective);

})(app);