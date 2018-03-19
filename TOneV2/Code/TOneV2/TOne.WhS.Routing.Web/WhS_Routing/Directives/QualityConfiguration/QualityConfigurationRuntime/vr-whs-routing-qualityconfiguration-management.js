'use strict';

app.directive('vrWhsRoutingQualityconfigurationManagement', ['UtilsService', 'VRModalService', 'WhS_Routing_QualityConfigurationService',
    function (UtilsService, VRModalService, WhS_Routing_QualityConfigurationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new QualityConfigurationManagementCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Routing/Directives/QualityConfiguration/QualityConfigurationRuntime/Templates/QualityConfigurationManagementTemplate.html"
        };

        function QualityConfigurationManagementCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.isValid = function () {

                    if ($scope.scopeModel.datasource != undefined && $scope.scopeModel.datasource.length > 0) {
                        var defaultCount = 0;
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var item = $scope.scopeModel.datasource[i];
                            if (item.Entity.IsDefault) {
                                defaultCount++;
                            }
                        }
                        if (defaultCount == 0)
                            return "At least one default settings should be added.";
                        if (defaultCount > 1)
                            return "Only one default settings is permitted.";
                    } else {
                        return "At least one settings should be added.";
                    }
                    return null;
                };

                $scope.scopeModel.addQualityConfiguration = function () {
                    var onQualityConfigurationAdded = function (qualityConfiguration) {
                        if ($scope.scopeModel.datasource.length == 0)
                            qualityConfiguration.IsDefault = true;
                        else
                            qualityConfiguration.IsDefault = false;

                        qualityConfiguration.IsActive = true;
                        $scope.scopeModel.datasource.push({ Entity: qualityConfiguration });
                    };

                    WhS_Routing_QualityConfigurationService.addRouteRuleQualityConfiguration(onQualityConfigurationAdded, getQualityConfigurationNames());
                };

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
                            var datasourceItem = $scope.scopeModel.datasource[i];
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
                    var actions = [{
                        name: "Edit",
                        clicked: editQualityConfiguration
                    }];

                    if (!dataItem.Entity.IsDefault) {
                        actions.push({
                            name: "Set Default",
                            clicked: setDefault
                        },
                        {
                            name: "Delete",
                            clicked: removeQualityConfiguration
                        });
                    }

                    if (!dataItem.Entity.IsActive) {
                        actions.push({
                            name: "Enable",
                            clicked: setActive
                        });
                    }
                    else if (!dataItem.Entity.IsDefault) {
                        actions.push({
                            name: "Disable",
                            clicked: setDisable
                        });
                    }

                    return actions;
                };
            }

            function editQualityConfiguration(editQualityConfigurationObject) {
                var onQualityConfigurationUpdated = function (newQualityConfiguration) {
                    var index = $scope.scopeModel.datasource.indexOf(editQualityConfigurationObject);
                    newQualityConfiguration.IsActive = editQualityConfigurationObject.Entity.IsActive;
                    newQualityConfiguration.IsDefault = editQualityConfigurationObject.Entity.IsDefault;
                    $scope.scopeModel.datasource[index] = { Entity: newQualityConfiguration };
                };

                WhS_Routing_QualityConfigurationService.editRouteRuleQualityConfiguration(onQualityConfigurationUpdated, editQualityConfigurationObject.Entity, getQualityConfigurationNames());
            }

            function removeQualityConfiguration(removedQualityConfigurationObject) {
                var index = $scope.scopeModel.datasource.indexOf(removedQualityConfigurationObject);
                $scope.scopeModel.datasource.splice(index, 1);
            }

            function setDefault(defaultQualityConfigurationObject) {
                for (var i = 0, length = $scope.scopeModel.datasource.length; i < length; i++) {
                    var entity = $scope.scopeModel.datasource[i].Entity;
                    if (entity.IsDefault) {
                        entity.IsDefault = false;
                        $scope.scopeModel.datasource[i] = { Entity: entity };
                        break;
                    }
                }

                var index = $scope.scopeModel.datasource.indexOf(defaultQualityConfigurationObject);
                defaultQualityConfigurationObject.Entity.IsActive = true;
                defaultQualityConfigurationObject.Entity.IsDefault = true;
                $scope.scopeModel.datasource[index] = { Entity: defaultQualityConfigurationObject.Entity };
            }

            function setActive(activeQualityConfigurationObject) {
                var index = $scope.scopeModel.datasource.indexOf(activeQualityConfigurationObject);
                activeQualityConfigurationObject.Entity.IsActive = true;
                $scope.scopeModel.datasource[index] = { Entity: activeQualityConfigurationObject.Entity };
            }

            function setDisable(activeQualityConfigurationObject) {
                var index = $scope.scopeModel.datasource.indexOf(activeQualityConfigurationObject);
                activeQualityConfigurationObject.Entity.IsActive = false;
                $scope.scopeModel.datasource[index] = { Entity: activeQualityConfigurationObject.Entity };
            }

            function getQualityConfigurationNames() {
                if ($scope.scopeModel.datasource == undefined)
                    return;

                var qualityConfigurationNames = [];

                for (var index = 0; index < $scope.scopeModel.datasource.length; index++) {
                    var qualityConfigurationEntity = $scope.scopeModel.datasource[index].Entity;
                    if (qualityConfigurationEntity && qualityConfigurationEntity.Name)
                        qualityConfigurationNames.push(qualityConfigurationEntity.Name);
                }

                return qualityConfigurationNames;
            }
        }

        return directiveDefinitionObject;
    }]);