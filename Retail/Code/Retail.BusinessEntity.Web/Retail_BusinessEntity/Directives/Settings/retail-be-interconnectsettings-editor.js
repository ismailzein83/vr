(function (app) {

    'use strict';

    InterconnectSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function InterconnectSettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InterconnectSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Settings/Templates/InterconnectSettingsEditorTemplate.html"
        };


        function InterconnectSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    if (payload != undefined && payload.data != undefined)
                    {
                        $scope.scopeModel.localOperatorName = payload.data.LocalOperatorName;
                        $scope.scopeModel.localOperatorID = payload.data.LocalOperatorID;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Entities.InterconnectSettings,Retail.BusinessEntity.Entities",
                        LocalOperatorName: $scope.scopeModel.localOperatorName,
                        LocalOperatorID : $scope.scopeModel.localOperatorID
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeInterconnectsettingsEditor', InterconnectSettingsDirective);
})(app);