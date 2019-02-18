(function (app) {

    'use strict';

    onnetOperatorDirectionServicesType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function onnetOperatorDirectionServicesType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OnNetOperatorDirectionServices($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OnNetOperatorDeclaration/Templates/OnNetOperatorDirectionServicesCustomObjectSettingsTemplate.html"

        };
        function OnNetOperatorDirectionServices($scope, ctrl, $attrs) {
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
                        $type: "Retail.RA.Business.OnNetOperatorDeclarationServicesCustomObjectTypeSettings, Retail.RA.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailRaOnnetoperatordeclarationservicesCustomobjectsettings', onnetOperatorDirectionServicesType);

})(app);
