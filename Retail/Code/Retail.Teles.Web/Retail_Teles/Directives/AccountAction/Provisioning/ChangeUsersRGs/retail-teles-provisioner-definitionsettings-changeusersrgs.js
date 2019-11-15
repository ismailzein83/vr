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
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ChangeUsersRGs/Templates/ChangeUsersRGsProvisionerDefinitionSettingsTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            var directiveAPI;
            var directivePromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([directivePromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;
                    return loadDirective();
                };

                api.getData = function getData() {
                    return directiveAPI.getData();
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

            }

            function loadDirective() {
                if (mainPayload != undefined) {
                    mainPayload.classFQTN = "Retail.Teles.Business.ChangeUsersRGsDefinitionSettings, Retail.Teles.Business";
                } else {
                    mainPayload = {
                        classFQTN: "Retail.Teles.Business.ChangeUsersRGsDefinitionSettings, Retail.Teles.Business"
                    };
                }
                return directiveAPI.load(mainPayload);
            }
        }
    }

    app.directive('retailTelesProvisionerDefinitionsettingsChangeusersrgs', ProvisionerDefinitionsettingsDirective);

})(app);