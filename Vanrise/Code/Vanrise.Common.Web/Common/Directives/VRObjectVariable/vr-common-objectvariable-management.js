(function (app) {

    'use strict';

    VRObjectVariableManagementDirective.$inject = ['VRCommon_ObjectVariableService', 'VRCommon_VRObjectTypeAPIService', 'UtilsService', 'VRNotificationService'];

    function VRObjectVariableManagementDirective(VRCommon_ObjectVariableService, VRCommon_VRObjectTypeAPIService, UtilsService, VRNotificationService) {
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

            function initializeController() {
                ctrl.objectVariables = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.addObjectVariable = function () {
                    var onObjectVariableAdded = function (addedObjectVariable) {
                        extendObjectVariableObject(addedObjectVariable);
                        ctrl.objectVariables.push(addedObjectVariable);
                    };
                    VRCommon_ObjectVariableService.addVRObjectVariable(ctrl.objectVariables, onObjectVariableAdded);
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    return loadObjectTypeConfigs().then(function () {
                        var objectVariable;
                        if (payload != undefined && payload.objects != undefined) {
                            for (var i = 0; i < payload.objects.length; i++) {
                                objectVariable = payload.objects[i];
                                extendObjectVariableObject(objectVariable);
                                ctrl.objectVariables.push(objectVariable);
                            }
                        }
                    });

                    function loadObjectTypeConfigs() {
                        return VRCommon_VRObjectTypeAPIService.GetObjectTypeExtensionConfigs().then(function (response) {
                            if (response != null) {
                                objectTypeConfigs = [];
                                for (var i = 0; i < response.length; i++) {
                                    objectTypeConfigs.push(response[i]);
                                }
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
                    var index = UtilsService.getItemIndexByVal(ctrl.objectVariables, objectVariable.ObjectName, 'ObjectName');
                    ctrl.objectVariables[index] = updatedObjectVariable;
                };
                VRCommon_ObjectVariableService.editVRObjectVariable(objectVariable, ctrl.objectVariables, onObjectVariableUpdated);
            }
            function deleteObjectVariable(objectVariable) {
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        var index = UtilsService.getItemIndexByVal(ctrl.objectVariables, objectVariable.ObjectName, 'ObjectName');
                        ctrl.objectVariables.splice(index, 1);
                    }
                });
            }
            function extendObjectVariableObject(objectVariable) {
                var dataRecordTypeConfigObject = UtilsService.getItemByVal(objectTypeConfigs, objectVariable.ObjectType.ConfigId, 'ExtensionConfigurationId');
                if (dataRecordTypeConfigObject != null) {
                    objectVariable.DataRecordType = dataRecordTypeConfigObject.Title;
                }
            }
        }

    }
    

    app.directive('vrCommonObjectvariableManagement', VRObjectVariableManagementDirective);

})(app);