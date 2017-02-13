'use strict';

app.directive('retailTelesEnterprisebedefinitionEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailBeDefinitionsSettingsEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Retail_Teles/Directives/TelesEnterprises/Templates/TelesEnterpriseBEDefinitionEditorTemplate.html'
        };

        function RetailBeDefinitionsSettingsEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {

                        if (payload.businessEntityDefinitionSettings != undefined) {
                            $scope.scopeModel.switchId = payload.businessEntityDefinitionSettings.SwitchId;
                            $scope.scopeModel.domainId = payload.businessEntityDefinitionSettings.DomainId;

                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.Teles.Business.TelesEnterpriseBEDefinitionSettings, Retail.Teles.Business",
                        DomainId: $scope.scopeModel.domainId,
                        SwitchId: $scope.scopeModel.switchId,
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);