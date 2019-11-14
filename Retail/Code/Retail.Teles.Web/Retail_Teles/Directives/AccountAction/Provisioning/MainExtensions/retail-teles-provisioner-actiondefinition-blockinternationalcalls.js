(function (app) {

    'use strict';

    ProvisionerDefinitionsettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerDefinitionsettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProvisionerDefinitionsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/MainExtensions/Templates/BlockInternationalCallsActionDefinitionTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var directiveAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    defineAPI();
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    var directivePayload = payload != undefined ? payload.Settings : undefined;

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadPromiseDeferred);

                    return directiveLoadPromiseDeferred.promise;
                };

                api.getData = function getData() {
                    var data = {
                        $type: "Retail.Teles.Business.UserRGActionDefinition, Retail.Teles.Business"
                    };
                    directiveAPI.setData(data);
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

            }
        }
    }

    app.directive('retailTelesProvisionerActiondefinitionBlockinternationalcalls', ProvisionerDefinitionsettingsDirective);

})(app);