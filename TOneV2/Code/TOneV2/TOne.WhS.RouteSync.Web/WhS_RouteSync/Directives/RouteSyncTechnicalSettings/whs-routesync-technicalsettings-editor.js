'use strict';

app.directive('whsRoutesyncTechnicalsettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/RouteSyncTechnicalSettings/Templates/RouteSyncTechnicalSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            $scope.scopeModel = {};

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.switchInfoGetter = JSON.stringify(payload.data.SwitchInfoGetter);
                    }
                }

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.Entities.RouteSyncTechnicalSettings, TOne.WhS.RouteSync.Entities",
                        SwitchInfoGetter: UtilsService.parseStringToJson($scope.scopeModel.switchInfoGetter)
                    };
                    return data;    
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);