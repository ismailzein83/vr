(function (app) {

    'use strict';

    VRObjectVariableManagementDirective.$inject = ['VRCommon_ObjectVariableService', 'UtilsService', 'VRNotificationService'];

    function VRObjectVariableManagementDirective(VRCommon_ObjectVariableService, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrObjectVariable = new VRObjectVariable($scope, ctrl);
                vrObjectVariable.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: '/Client/Modules/Common/Directives/VRObjectVariable/Templates/VRObjectVariableManagementTemplate.html'
        };

        function VRObjectVariable($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.objectVariables = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.addObjectVariable = function () {
                    var onObjectVariableAdded = function (addedCriteriaField) {
                        //extendCriteriaFieldObject(addedCriteriaField);
                        ctrl.objectVariables.push(addedCriteriaField);
                        //addPriorityDataItem(addedCriteriaField);
                    };
                    VRCommon_ObjectVariableService.addVRObjectVariable(onObjectVariableAdded);
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var objectVariable;
                    if (payload != undefined && payload.objects != undefined) {
                        for (var i = 0; i < payload.objects.length; i++) {
                            objectVariable = payload.objects[i];
                            ctrl.objectVariables.push(objectVariable);
                        }
                    }
                };

                api.getData = function () {
                    var data;
                    var dataItem

                    if (ctrl.objectVariables.length > 0) {
                        data = {};

                        for (var i = 0; i < ctrl.objectVariables.length; i++) {
                            dataItem = ctrl.objectVariables[i];
                            data[dataItem.ObjectName] = getMappedObjectVariable(dataItem);
                        }
                    }

                    function getMappedObjectVariable(dataItem) {
                        return {
                            ObjectName: dataItem.ObjectName,
                            ObjectType: dataItem.ObjectType
                        };
                    }

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editObjectVariable
                }, {
                    name: 'Delete',
                    clicked: deleteObjectVariable
                }];
            }

            function editObjectVariable(objectVariable) {
                var onObjectVariableUpdated = function (updatedObjectVariable) {
                    //extendCriteriaFieldObject(updatedCriteriaField);
                    var index = UtilsService.getItemIndexByVal(ctrl.objectVariables, objectVariable.ObjectName, 'ObjectName');
                    ctrl.objectVariables[index] = updatedObjectVariable;
                };
                VRCommon_ObjectVariableService.editVRObjectVariable(objectVariable, onObjectVariableUpdated);
            }
            function deleteObjectVariable(objectVariable) {
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        var index = UtilsService.getItemIndexByVal(ctrl.objectVariables, objectVariable.ObjectName, 'ObjectName');
                        ctrl.objectVariables.splice(index, 1);
                    }
                });
            }
        }

    }
    

    app.directive('vrCommonObjectvariableManagement', VRObjectVariableManagementDirective);

})(app);