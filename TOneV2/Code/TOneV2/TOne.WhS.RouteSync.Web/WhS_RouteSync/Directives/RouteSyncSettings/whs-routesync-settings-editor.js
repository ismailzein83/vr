'use strict';

app.directive('whsRoutesyncSettingsEditor', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/WhS_RouteSync/Directives/RouteSyncSettings/Templates/RouteSyncSettingsTemplate.html"
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
                        $scope.scopeModel.routeBatchSize = payload.data.RouteSyncProcess.RouteBatchSize;
                    }
                };

                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.RouteSync.Entities.RouteSyncSettings, TOne.WhS.RouteSync.Entities",
                        RouteSyncProcess: { RouteBatchSize: $scope.scopeModel.routeBatchSize }
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);