"use strict";

app.directive("bpInstanceTestinginserthandler", ["BusinessProcess_BPInstanceAPIService", "BusinessProcess_BPInstanceService", "BusinessProcess_GridMaxSize", "VRTimerService",
    function (BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, BusinessProcess_GridMaxSize, VRTimerService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BPInstanceTestingInsertHandlerCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/MainExtensions/Templates/BPInstanceTestingInsertHandlerTemplate.html"
        };

        function BPInstanceTestingInsertHandlerCtor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.BPInstance.TestingBPInstanceBeforeInsertHandler, Vanrise.BusinessProcess.MainExtensions"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);