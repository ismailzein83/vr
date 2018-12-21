(function (app) {

    'use strict';

    additionalErrors.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function additionalErrors(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AdditionalErrors($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/AdditionalErrors/AdditionalErrorsTemplate.html"

        };
        function AdditionalErrors($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.className = payload.className;
                        $scope.scopeModel.errorMessages = payload.errorMessages;
                    }
                };

                api.getData = function () {
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonAdditionalerrors', additionalErrors);

})(app);
