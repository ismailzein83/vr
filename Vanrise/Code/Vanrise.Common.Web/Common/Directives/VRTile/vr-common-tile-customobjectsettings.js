

(function (app) {

    'use strict';

    vrTileReport.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function vrTileReport(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FaultCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRTile/Templates/VRTileCustomObjectSettingsTemplate.html'

        };
        function FaultCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.Entities.VRTileCustomObjectTypeSettings,Vanrise.Entities"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonTileCustomobjectsettings', vrTileReport);

})(app);
