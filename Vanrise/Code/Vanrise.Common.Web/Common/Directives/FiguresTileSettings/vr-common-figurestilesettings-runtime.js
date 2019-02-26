"use strict";
app.directive("vrCommonFigurestilesettingsRuntime", ["UtilsService", "VRUIUtilsService", "VRCommon_VRTileAPIService",'VRTimerService',
    function (UtilsService, VRUIUtilsService, VRCommon_VRTileAPIService, VRTimerService) {

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
            var figureStyleInput;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tileTitle = ctrl.header;
                $scope.scopeModel.fields = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.scopeModel.fields = [];
                   
                    var promises = [];
                    var definitionSettings;
                    if (payload != undefined) {
                        definitionSettings = payload.definitionSettings;
                        $scope.scopeModel.imgPath = definitionSettings.IconPath;
                        $scope.scopeModel.maxItemPerRow = definitionSettings.MaximumItemsPerRow;
                        if (definitionSettings.AutoRefresh) {
                            if ($scope.jobIds) {
                                VRTimerService.unregisterJobByIds($scope.jobIds);
                                $scope.jobIds.length = 0;
                            }
                        }
                    }


                    if (definitionSettings != undefined) {
                        figureStyleInput = {
                            Queries: definitionSettings.Queries,
                            ItemsToDisplay: definitionSettings.ItemsToDisplay
                        };
                        promises.push(loadFigures()); 
                    }
                    function loadFigures() {
                        $scope.scopeModel.isLoading = true;
                        return getFigureItemsValue().then(function (response) {
                            if (response != undefined) {
                                for (var i = 0, length = response.length; i < length; i++) {
                                    var figureStyle = response[i];
                                    $scope.scopeModel.fields.push({
                                        name: figureStyle.Name,
                                        value: figureStyle.Value,
                                        className: figureStyle.StyleFormatingSettings
                                    });
                                }
                                $scope.scopeModel.url = response.ViewURL;
                            }
                            if (definitionSettings.AutoRefresh)
                                registerAutoRefreshJob(definitionSettings.AutoRefreshInterval);

                        });
                    }
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };

                api.getData = function () {
                };

                function registerAutoRefreshJob(autoRefreshInterval) {
                    VRTimerService.registerJob(onTimerElapsed, $scope, autoRefreshInterval);
                }
                function onTimerElapsed() {
                    return getFigureItemsValue().then(function (response) {
                        if (response != undefined) {
                            for (var i = 0, length = response.length; i < length; i++) {
                                var figureStyle = response[i];
                                for (var j = 0; j < $scope.scopeModel.fields.length; j++) {
                                    var field = $scope.scopeModel.fields[j];
                                    if (figureStyle.Name == field.name) {
                                        $scope.scopeModel.fields[j].value = figureStyle.Value;
                                        $scope.scopeModel.fields[j].className = figureStyle.StyleFormatingSettings;
                                    }
                                }
                            }
                            $scope.scopeModel.url = response.ViewURL;
                        }
                    });
                }
                function getFigureItemsValue() {
                    var promise = UtilsService.createPromiseDeferred();
                    VRCommon_VRTileAPIService.GetFigureItemsValue(figureStyleInput).then(function (response) {
                        promise.resolve(response);
                    }).catch(function (error) {
                        promise.reject(error);
                    });

                    return promise.promise;
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };


        };

        return directiveDefinitionObject;
    }
]);