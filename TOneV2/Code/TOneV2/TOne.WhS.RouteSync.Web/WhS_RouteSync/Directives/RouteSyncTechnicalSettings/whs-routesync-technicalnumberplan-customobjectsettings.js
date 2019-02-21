﻿(function (app) {
    'use strict';
    technicalNumberPlanType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function technicalNumberPlanType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TechnicalNumberPlanCustomObjectType($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/RouteSyncTechnicalSettings/Templates/TechnicalNumberPlanCustomObjectSettingsTemplate.html"
        };
        function TechnicalNumberPlanCustomObjectType($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.RouteSync.Business.TechnicalNumberPlanCustomObjectTypeSettings, TOne.WhS.RouteSync.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsRoutesyncTechnicalnumberplanCustomobjectsettings', technicalNumberPlanType);

})(app);
