(function (app) {

    'use strict';

    VRObjectVariableManagementDirective.$inject = ['VRCommon_VRObjectVariableService', 'VRCommon_VRObjectTypeDefinitionAPIService', 'UtilsService', 'VRNotificationService'];

    function VRObjectVariableManagementDirective(VRCommon_VRObjectVariableService, VRCommon_VRObjectTypeDefinitionAPIService, UtilsService, VRNotificationService) {
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
            var objectTypeConfigs;
            var VRObjectTypeDefinitionsInfo; // used to map VRObjectTypeId to VRObjectTypeName for the grid ObjectType column 

            function initializeController() {
                ctrl.objectVariables = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.onAddObjectVariable = function () {
                    var onObjectVariableAdded = function (addedObjectVariable) {
                        extendObjectVariableObject(addedObjectVariable);
                        ctrl.objectVariables.push(addedObjectVariable);
                    };
                    VRCommon_VRObjectVariableService.addVRObjectVariable(ctrl.objectVariables, onObjectVariableAdded);
                };
                ctrl.onRemoveObjectVariable = function (objectVariable) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.objectVariables, objectVariable.ObjectName, 'ObjectName');
                            ctrl.objectVariables.splice(index, 1);
                        }
                    });
                }

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    return loadVRObjectTypeDefinitionsInfo().then(function () {
                        var objectVariable;
                        if (payload != undefined && payload.objects != undefined) {
                            for (var i = 0; i < payload.objects.length; i++) {
                                objectVariable = payload.objects[i];
                                extendObjectVariableObject(objectVariable);
                                ctrl.objectVariables.push(objectVariable);
                            }
                        }
                    });

                    function loadVRObjectTypeDefinitionsInfo() {
                        var filter;

                        return VRCommon_VRObjectTypeDefinitionAPIService.GetVRObjectTypeDefinitionsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                            VRObjectTypeDefinitionsInfo = [];
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) 
                                    VRObjectTypeDefinitionsInfo.push(response[i]);
                            }
                        });
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
                            VRObjectTypeDefinitionId: dataItem.VRObjectTypeDefinitionId
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
                }];
            }

            function editObjectVariable(objectVariable) {
                var onObjectVariableUpdated = function (updatedObjectVariable) {
                    var index = UtilsService.getItemIndexByVal(ctrl.objectVariables, objectVariable.ObjectName, 'ObjectName');
                    extendObjectVariableObject(updatedObjectVariable);
                    ctrl.objectVariables[index] = updatedObjectVariable;
                };
                VRCommon_VRObjectVariableService.editVRObjectVariable(objectVariable, ctrl.objectVariables, onObjectVariableUpdated);
            }
            function extendObjectVariableObject(objectVariable) {

                if (VRObjectTypeDefinitionsInfo != undefined) {
                    var VRObjectTypeDefinitionInfo;
                    for (var i = 0; i < VRObjectTypeDefinitionsInfo.length; i++) {
                        VRObjectTypeDefinitionInfo = VRObjectTypeDefinitionsInfo[i];
                        if (VRObjectTypeDefinitionInfo.VRObjectTypeDefinitionId == objectVariable.VRObjectTypeDefinitionId) {
                            objectVariable.ObjectTypeName = VRObjectTypeDefinitionInfo.Name;
                            return;
                        }
                    }
                }
            }
        }

    }
    

    app.directive('vrCommonObjectvariableManagement', VRObjectVariableManagementDirective);

})(app);