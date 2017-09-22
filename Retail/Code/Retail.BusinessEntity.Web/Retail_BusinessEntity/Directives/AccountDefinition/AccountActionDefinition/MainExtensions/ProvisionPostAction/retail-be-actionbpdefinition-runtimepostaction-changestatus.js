(function (app) {

    'use strict';

    ChangeStatusPostActionDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ChangeStatusPostActionDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ChangeStatusPostAction($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/MainExtensions/ProvisionPostAction/Templates/ChangeStatusPostActionTemplate.html"

        };
        function ChangeStatusPostAction($scope, ctrl, $attrs) {
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
                        $type: "Retail.BusinessEntity.MainExtensions.ChangeStatusPostAction, Retail.BusinessEntity.MainExtensions",
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBeActionbpdefinitionRuntimepostactionChangestatus', ChangeStatusPostActionDirective);

})(app);