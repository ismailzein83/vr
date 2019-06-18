(function (app) {

    'use strict';

    inboundCustomObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function inboundCustomObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new inboundCustomObjectTypeConstructor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisInboundCustomObjectTemplate.html"

        };
        function inboundCustomObjectTypeConstructor($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.Deal.Business.SwapDealInboundCustomObjectTypeSettings, TOne.WhS.Deal.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsDealSwapdealanalysisInboundCustomobjectsettings', inboundCustomObjectType);

})(app);
