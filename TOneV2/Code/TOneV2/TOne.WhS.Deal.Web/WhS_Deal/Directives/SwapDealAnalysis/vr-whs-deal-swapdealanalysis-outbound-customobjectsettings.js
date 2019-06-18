(function (app) {

    'use strict';

    outboundCustomObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function outboundCustomObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OutboundCustomObjectTypeConstructor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisOutboundCustomObjectTemplate.html"

        };
        function OutboundCustomObjectTypeConstructor($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.Deal.Business.SwapDealOutboundCustomObjectTypeSettings, TOne.WhS.Deal.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsDealSwapdealanalysisOutboundCustomobjectsettings', outboundCustomObjectType);

})(app);
