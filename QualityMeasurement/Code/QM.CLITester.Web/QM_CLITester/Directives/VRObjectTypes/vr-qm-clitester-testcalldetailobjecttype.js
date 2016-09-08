(function (app) {

    'use strict';

    TestCallDetailObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function TestCallDetailObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrTestCallDetailObjectType = new VRTestCallDetailObjectType($scope, ctrl, $attrs);
                vrTestCallDetailObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/QM_CLITester/Directives/VRObjectTypes/Templates/TestCallDetailObjectTypeTemplate.html"

        };
        function VRTestCallDetailObjectType($scope, ctrl, $attrs) {
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
                        $type: "QM.CLITester.Entities.VRObjectTypes.TestResultObjectType, QM.CLITester.Entities"
                    }
                    return data;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrQmClitestertestcalldetailobjecttype', TestCallDetailObjectType);

})(app);