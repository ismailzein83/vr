(function (app) {

    'use strict';

    FaildBatchInfoObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function FaildBatchInfoObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var splObjectType = new FaildBatchInfoObjectTypeObject($scope, ctrl, $attrs);
                splObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Integration/Directives/MainExtensions/VRObjectTypes/Templates/FaildBatchInfoObjectTypeTemplate.html'


        };
        function FaildBatchInfoObjectTypeObject($scope, ctrl, $attrs) {
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
                        $type: "Vanrise.Integration.MainExtensions.FailedBatchInfoObjectType, Vanrise.Integration.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrIntegrationFaildbatchinfoObjecttype', FaildBatchInfoObjectType);

})(app);