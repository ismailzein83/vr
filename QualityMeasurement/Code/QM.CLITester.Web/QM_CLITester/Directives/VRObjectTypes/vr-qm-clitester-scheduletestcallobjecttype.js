(function (app) {

    'use strict';

    ScheduleTestCallObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function ScheduleTestCallObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrScheduleTestCallObjectType = new VRScheduleTestCallObjectType($scope, ctrl, $attrs);
                vrScheduleTestCallObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/QM_CLITester/Directives/VRObjectTypes/Templates/ScheduleTestCallObjectTypeTemplate.html"

        };
        function VRScheduleTestCallObjectType($scope, ctrl, $attrs) {
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
                        $type: "QM.CLITester.Entities.VRObjectTypes.ScheduleTestResultObjectType, QM.CLITester.Entities"
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrQmClitesterscheduletestcallobjecttype', ScheduleTestCallObjectType);

})(app);