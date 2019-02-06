"use strict";
app.directive("vrCommonFigurestilesettingsRuntime", ["UtilsService", "VRUIUtilsService", "VRCommon_VRTileAPIService",
    function (UtilsService, VRUIUtilsService, VRCommon_VRTileAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "=",
                header: '=',
                index: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FigurestilesettingsRuntime($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Common/Directives/FiguresTileSettings/Templates/FiguresTileSettingsRuntimeTemplate.html'
        };
        function FigurestilesettingsRuntime($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tileTitle = ctrl.header;
                $scope.scopeModel.fields = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var figureStyleInput;
                    var promises = [];
                    var definitionSettings;
                    if (payload != undefined) {
                        definitionSettings = payload.definitionSettings;
                    }
                    if (definitionSettings != undefined) {
                        figureStyleInput = {
                            Queries : definitionSettings.Queries,
                            ItemsToDisplay: definitionSettings.ItemsToDisplay
                        };
                        promises.push(loadLiveBalance());
                    }
                    function loadLiveBalance() {
                        return VRCommon_VRTileAPIService.GetFigureItemsValue(figureStyleInput).then(function (response) {
                            if (response != undefined) {
                                    for (var i = 0, length = response.length; i < length; i++) {
                                        var figureStyle = response[i];
                                        $scope.scopeModel.fields.push({
                                            name: figureStyle.Name,
                                            value: figureStyle.Value
                                        });
                                    }
                                $scope.scopeModel.url = response.ViewURL;
                            }
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);