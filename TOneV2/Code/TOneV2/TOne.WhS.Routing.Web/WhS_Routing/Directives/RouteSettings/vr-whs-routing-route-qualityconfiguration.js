'use strict';
app.directive('vrWhsRoutingRouteQualityconfiguration', ['UtilsService', 'VRModalService','WhS_Routing_QualityConfigurationService',
    function (UtilsService, VRModalService, WhS_Routing_QualityConfigurationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new QualityConfigurationEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RouteSettings/Templates/RouteQualityConfigurationTemplate.html"
        };

        function QualityConfigurationEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;
            var qualityConfigurationGridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0) {
                        return "You Should add at least one settings.";
                    }
                };

                $scope.scopeModel.addQualityConfiguration = function () {

                    var onQualityConfigurationAdded = function (qualityConfiguration) {
                        $scope.scopeModel.datasource.push({ Entity: qualityConfiguration });
                    };
                    WhS_Routing_QualityConfigurationService.addQualityConfiguration(onQualityConfigurationAdded);
                };

                $scope.scopeModel.removeQualityConfiguration = function (dataItem) {
                    var index = $scope.scopeModel.datasource.indexOf(dataItem);
                    $scope.scopeModel.datasource.splice(index, 1);
                }

                defineMenuActions();

                defineAPI();

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.RouteRuleQualityConfigurationList != undefined) {
                        for (var i = 0, length = payload.RouteRuleQualityConfigurationList.length ; i < length ; i++) {
                            var item = payload.RouteRuleQualityConfigurationList[i];
                            $scope.scopeModel.datasource.push({ Entity: item });
                        }
                    }
                };

                api.getData = function () {
                    var qualityConfigurations;
                    if ($scope.scopeModel.datasource != undefined) {
                        qualityConfigurations = [];
                        for (var i = 0, length = $scope.scopeModel.datasource.length; i < length; i++) {
                            var datasourceItem=$scope.scopeModel.datasource[i];
                            qualityConfigurations.push(datasourceItem.Entity);
                        }
                    }
                    return qualityConfigurations;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    return [{
                        name: "Edit",
                        clicked: editQualityConfiguration
                    }];
                };
            }

            function editQualityConfiguration(editQualityConfigurationObject) {
                var onQualityConfigurationUpdated = function (newQualityConfiguration) {
                    var index = $scope.scopeModel.datasource.indexOf(editQualityConfigurationObject);
                    $scope.scopeModel.datasource[index] = { Entity: newQualityConfiguration };
                };

                WhS_Routing_QualityConfigurationService.editQualityConfiguration(editQualityConfigurationObject, onQualityConfigurationUpdated);

            }

        }

        return directiveDefinitionObject;
    }]);