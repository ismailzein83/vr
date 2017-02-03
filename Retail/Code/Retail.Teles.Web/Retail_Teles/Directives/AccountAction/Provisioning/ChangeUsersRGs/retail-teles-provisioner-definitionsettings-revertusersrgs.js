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
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ChangeUsersRGs/Templates/RevertUsersRGsProvisionerDefinitionSettingsTemplate.html"

        };
        function ProvisionerDefinitionsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;

            function initializeController() {
                $scope.scopeModel = {};
               
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var provisionerDefinitionSettings;
                    if (payload != undefined) {
                        mainPayload = payload;
                        provisionerDefinitionSettings = payload.provisionerDefinitionSettings;
                        if(provisionerDefinitionSettings != undefined)
                        {
                            $scope.scopeModel.actionType = provisionerDefinitionSettings.ActionType;
                            $scope.scopeModel.switchId = provisionerDefinitionSettings.SwitchId;

                        }

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
                        $type: "Retail.Teles.Business.RevertUsersRGsDefinitionSettings,Retail.Teles.Business",
                        ActionType: $scope.scopeModel.actionType,
                        SwitchId: $scope.scopeModel.switchId,
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerDefinitionsettingsRevertusersrgs', ProvisionerDefinitionsettingsDirective);

})(app);