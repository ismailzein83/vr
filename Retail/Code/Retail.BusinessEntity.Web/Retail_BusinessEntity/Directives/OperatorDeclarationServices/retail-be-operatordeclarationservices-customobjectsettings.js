

(function (app) {

    'use strict';

    operatorDirectionServicesType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function operatorDirectionServicesType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OperatorDirectionServices($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclarationServices/Templates/OperatorDirectionServicesCustomObjectSettingsTemplate.html"

        };
        function OperatorDirectionServices($scope, ctrl, $attrs) {
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
                        $type: "Retail.BusinessEntity.Business.OperatorDirectionServicesCustomObjectTypeSettings, Retail.BusinessEntity.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeOperatordeclarationservicesCustomobjectsettings', operatorDirectionServicesType);

})(app);
