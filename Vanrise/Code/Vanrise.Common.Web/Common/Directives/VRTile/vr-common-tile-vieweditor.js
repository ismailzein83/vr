"use strict";

app.directive("vrCommonTileVieweditor", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ViewEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/VRTile/Templates/VRTileViewEditor.html"
        };
        function ViewEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
          
            var vrTilesDirectiveApi;
            var vrTilesDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
           
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onVRTilesDirectiveReady = function (api) {
                    vrTilesDirectiveApi = api;
                    vrTilesDirectivePromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([vrTilesDirectivePromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                    }
                    promises.push(loadVRTilesDirective());

                    function loadVRTilesDirective() {
                        var vrTilesPayload;
                        if (payload != undefined && payload.VRTileViewData != undefined) {
                            vrTilesPayload = { tiles: payload.VRTileViewData.VRTiles };
                        };
                        return vrTilesDirectiveApi.load(vrTilesPayload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Common.Business.VRTileViewSettings, Vanrise.Common.Business",
                        VRTileViewData: {
                            VRTiles: vrTilesDirectiveApi.getData(),
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);