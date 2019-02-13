

(function (app) {

    'use strict';

    vrTileReportSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function vrTileReportSettings(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FaultCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRTile/Templates/VRTileReportSettingsTemplate.html'

        };
        function FaultCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var tilesGridAPI;
            var tilesGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTilesGridReady = function (api) {
                    tilesGridAPI = api;
                    tilesGridReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {


                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var tiles;
                    if (payload != undefined && payload.selectedValues != undefined && payload.selectedValues.Settings != undefined) {
                        tiles = payload.selectedValues.Settings.VRTiles;
                    }
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);

                    function loadDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                        tilesGridReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = { tiles: tiles };
                            VRUIUtilsService.callDirectiveLoad(tilesGridAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (data) {
                    data.Settings = {
                        $type: "Vanrise.Entities.VRDashboardSettings ,Vanrise.Entities",
                        VRTiles: tilesGridAPI.getData(),
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrCommonTileReportsettings', vrTileReportSettings);

})(app);
