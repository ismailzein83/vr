'use strict';

app.directive('vrGenericdataCompositerecordconditiondefinitiongroup', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_CompositeRecordConditionDefinitionService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_CompositeRecordConditionDefinitionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new CompositeRecordConditionDefinitionGroupCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/CompositeRecordCondition/Definition/Templates/CompositeRecordConditionDefinitionGroupTemplate.html'
        };

        function CompositeRecordConditionDefinitionGroupCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            var conditionRecordNames = [];
            var conditionRecordTitles = [];

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                $scope.scopeModel.isGridValid = function () {
                    if ($scope.scopeModel.datasource.length < 1)
                        return 'Grid should contains at least one item';
                    return null;
                };

                $scope.scopeModel.addCompositeRecordConditionDefinition = function () {
                    var onCompositeRecordConditionDefinitionAdded = function (addedCompositeRecordConditionDefinition) {
                        $scope.scopeModel.datasource.push({ Entity: addedCompositeRecordConditionDefinition });
                    };

                    getConditionRecordNamesAndTitles();

                    VR_GenericData_CompositeRecordConditionDefinitionService.addCompositeRecordConditionDefinition(onCompositeRecordConditionDefinitionAdded, conditionRecordNames, conditionRecordTitles);
                };

                $scope.scopeModel.removeCompositeRecordConditionDefinition = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Entity, 'Entity');
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.datasource.length = 0;

                    var compositeRecordConditionDefinitionGroup;

                    if (payload != undefined) {
                        if (payload.compositeRecordConditionDefinitionGroup != undefined &&
                            payload.compositeRecordConditionDefinitionGroup.CompositeRecordFilterDefinitions != undefined)
                            compositeRecordConditionDefinitionGroup = payload.compositeRecordConditionDefinitionGroup.CompositeRecordFilterDefinitions;
                    }

                    if (compositeRecordConditionDefinitionGroup != undefined) {
                        for (var i = 0; i < compositeRecordConditionDefinitionGroup.length; i++) {
                            $scope.scopeModel.datasource.push({ Entity: compositeRecordConditionDefinitionGroup[i] });
                        }
                    }
                };

                api.getData = function () {

                    function getCompositeRecordConditionDefinitionGroup() {
                        var compositeRecordConditions = [];
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var currentItem = $scope.scopeModel.datasource[i].Entity;
                            compositeRecordConditions.push({
                                Name: currentItem.Name,
                                Title: currentItem.Title,
                                Settings: currentItem.Settings
                            });
                        }
                        return compositeRecordConditions;
                    }

                    return {
                        CompositeRecordFilterDefinitions: getCompositeRecordConditionDefinitionGroup()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editCompositeRecordConditionDefinition
                }];
            }

            function editCompositeRecordConditionDefinition(compositeRecordConditionDefinition) {
                var onCompositeRecordConditionDefinitionUpdated = function (updatedCompositeRecordConditionDefinition) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, compositeRecordConditionDefinition.Entity, 'Entity');
                    $scope.scopeModel.datasource[index] = { Entity: updatedCompositeRecordConditionDefinition };
                };

                getConditionRecordNamesAndTitles();

                VR_GenericData_CompositeRecordConditionDefinitionService.editCompositeRecordConditionDefinition(onCompositeRecordConditionDefinitionUpdated,
                    compositeRecordConditionDefinition.Entity, conditionRecordNames, conditionRecordTitles);
            }

            function getConditionRecordNamesAndTitles() {
                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                    var currentEntity = $scope.scopeModel.datasource[i].Entity;
                    conditionRecordNames.push(currentEntity.Name);
                    conditionRecordTitles.push(currentEntity.Title);
                }
            }
        }
    }]);