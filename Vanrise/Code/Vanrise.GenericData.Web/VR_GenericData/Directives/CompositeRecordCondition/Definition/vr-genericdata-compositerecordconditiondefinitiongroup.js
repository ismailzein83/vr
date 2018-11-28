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

                    VR_GenericData_CompositeRecordConditionDefinitionService.addCompositeRecordConditionDefinition(onCompositeRecordConditionDefinitionAdded);
                };

                $scope.scopeModel.removeCompositeRecordConditionDefinition = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Entity.CompositeRecordConditionDefinitionId, 'Entity.CompositeRecordConditionDefinitionId');
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.datasource.length = 0;

                    console.log(payload);
                    //if (payload != undefined) {
                       
                    //}
                };

                api.getData = function () {
                    return {
                        CompositeRecordConditionDefinitionGroup: getCompositeRecordConditionDefinitionGroup()
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
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, compositeRecordConditionDefinition.Entity.CompositeRecordConditionDefinitionId, 'Entity.CompositeRecordConditionDefinitionId');
                    $scope.scopeModel.datasource[index] = { Entity: updatedCompositeRecordConditionDefinition };
                };
                VR_GenericData_CompositeRecordConditionDefinitionService.editCompositeRecordConditionDefinition(onCompositeRecordConditionDefinitionUpdated, compositeRecordConditionDefinition.Entity);
            }

            function getCompositeRecordConditionDefinitionGroup() {
                var compositeRecordConditions = [];
                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                    var currentItem = $scope.scopeModel.datasource[i].Entity;
                    compositeRecordConditions.push({
                        CompositeRecordConditionDefinitionId: currentItem.CompositeRecordConditionDefinitionId,
                        Name: currentItem.Name,
                        Settings: currentItem.Settings
                    });
                }
                return compositeRecordConditions;
            }
        }
    }]);