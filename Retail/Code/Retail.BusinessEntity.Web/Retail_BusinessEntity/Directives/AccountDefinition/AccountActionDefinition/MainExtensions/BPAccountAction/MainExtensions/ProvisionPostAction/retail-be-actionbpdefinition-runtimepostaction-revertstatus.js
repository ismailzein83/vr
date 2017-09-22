(function (app) {

    'use strict';

    RevertStatusPostActionDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function RevertStatusPostActionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RevertStatusPostAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/BPAccountAction/MainExtensions/ProvisionPostAction/Templates/RevertStatusPostActionTemplate.html"

        };
        function RevertStatusPostAction($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.RevertStatusPostAction, Retail.BusinessEntity.MainExtensions",
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeActionbpdefinitionRuntimepostactionRevertstatus', RevertStatusPostActionDirective);

})(app);