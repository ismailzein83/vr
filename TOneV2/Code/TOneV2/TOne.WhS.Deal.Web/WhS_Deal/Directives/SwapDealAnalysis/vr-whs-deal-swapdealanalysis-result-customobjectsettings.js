(function (app) {

    'use strict';

    resultCustomObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function resultCustomObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new resultCustomObjectTypeConstructor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisResultCustomObjectTemplate.html"

        };
        function resultCustomObjectTypeConstructor($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.Deal.Business.SwapDealResultCustomObjectTypeSettings, TOne.WhS.Deal.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsDealSwapdealanalysisResultCustomobjectsettings', resultCustomObjectType);

})(app);
