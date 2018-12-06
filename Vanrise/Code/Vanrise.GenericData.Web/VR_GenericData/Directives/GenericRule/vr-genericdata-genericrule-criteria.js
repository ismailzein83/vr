(function (app) {

    'use strict';

    GenericRuleCriteriaDirective.$inject = ['VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VR_GenericData_GenericUIService'];

    function GenericRuleCriteriaDirective(VR_GenericData_DataRecordFieldAPIService, UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_GenericUIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var obj = new GenericRuleCriteria($scope, ctrl, $attrs);
                obj.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericRule/Templates/GenericRuleCriteriaTemplate.html'
        };

        function GenericRuleCriteria($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accessibility;
            var genericUIObj;

            var loadAllFieldsPromiseDeferred = UtilsService.createPromiseDeferred();

            var criteriaFieldsValues;

            var groupCriteriaFieldSelectedPromise;

            function initializeController() {

                $scope.criteriaDefinitionFields;
                $scope.criteriaGroups = [];
                $scope.remainingCriteriaFields = [];
                $scope.onGroupCriteriaFieldSelectionChanged = function (selectedField) {
                    if (selectedField != undefined) {
                        if (groupCriteriaFieldSelectedPromise != undefined) {
                            groupCriteriaFieldSelectedPromise.resolve();
                        }
                        else {
                            if ($scope.criteriaGroups != undefined && $scope.criteriaGroups.length > 0) {
                                for (var i = 0; i < $scope.criteriaGroups.length; i++) {
                                    var criteriaGroup = $scope.criteriaGroups[i];
                                    if (criteriaGroup.fields != undefined && criteriaGroup.fields.length > 0) {
                                        var field = UtilsService.getItemByVal(criteriaGroup.fields, selectedField.FieldName, 'FieldName');
                                        if (field != null) {
                                            for (var k = 0; k < criteriaGroup.fields.length ; k++) {
                                                var criteriaGroupField = criteriaGroup.fields[k];
                                                if (criteriaGroupField.runtimeEditor != undefined) {
                                                    loadRuntimeEditor(criteriaGroupField);
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
                function loadRuntimeEditor(criteriaGroupField) {
                    criteriaGroupField.runtimeEditor.onReadyPromiseDeferred.promise.then(function () {
                        var payload = {
                            fieldTitle: criteriaGroupField.Title,
                            fieldType: criteriaGroupField.FieldType,
                            fieldValue: undefined,
                            genericUIContext: criteriaGroupField.genericUIContext
                        };

                        VRUIUtilsService.callDirectiveLoad(criteriaGroupField.runtimeEditor.directiveAPI, payload, criteriaGroupField.runtimeEditor.loadPromiseDeferred);
                    });
                }
           
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var genericRuleCriteria;
                    if ($scope.criteriaGroups != undefined && $scope.criteriaGroups.length > 0) {
                        for (var i = 0; i < $scope.criteriaGroups.length; i++) {
                            var criteriaGroup = $scope.criteriaGroups[i];
                            if (criteriaGroup.fields != undefined) {
                                for (var j = 0; j < criteriaGroup.fields.length; j++) {
                                    var fieldGroup = criteriaGroup.fields[j];
                                    var fieldGroupData = fieldGroup.runtimeEditor.directiveAPI.getData();
                                    if (fieldGroupData != undefined) {
                                        if (genericRuleCriteria == undefined) {
                                            genericRuleCriteria = {};
                                            genericRuleCriteria.FieldsValues = {};
                                        }
                                        genericRuleCriteria.FieldsValues[fieldGroup.FieldName] = fieldGroupData;

                                    }
                                }
                            }
                        }
                    }
                    if ($scope.remainingCriteriaFields != undefined && $scope.remainingCriteriaFields.length > 0) {
                        for (var i = 0; i < $scope.remainingCriteriaFields.length; i++) {
                            var remainingCriteriaField = $scope.remainingCriteriaFields[i];
                            var remainingCriteriaFieldValue = remainingCriteriaField.runtimeEditor.directiveAPI.getData();
                            if (remainingCriteriaFieldValue != undefined) {
                                if (genericRuleCriteria == undefined) {
                                    genericRuleCriteria = {};
                                    genericRuleCriteria.FieldsValues = {};
                                }

                                genericRuleCriteria.FieldsValues[remainingCriteriaField.FieldName] = remainingCriteriaFieldValue;

                            }
                        }
                    }

                    return genericRuleCriteria;
                };

                api.load = function (payload) {
                    criteriaFieldsValues = undefined;
                    if (payload == undefined || payload.criteriaDefinitionFields == undefined) {
                        return;
                    }
                    criteriaFieldsValues = payload.criteriaFieldsValues;
                    var criteriaAccessibility = payload.criteriaAccessibility;
                    var criteriaPredefinedData = payload.criteriaPredefinedData;
                    if (criteriaFieldsValues != undefined) {
                        groupCriteriaFieldSelectedPromise = UtilsService.createPromiseDeferred();
                    }

                    loadAllFieldsPromiseDeferred.promise.then(function () {
                        for (var i = 0; i < $scope.criteriaFields.length; i++) {
                            var criteriaField = $scope.criteriaFields[i];
                            var belongToGroup = false;
                            if (payload.criteriaDefinitionGroups != undefined && payload.criteriaDefinitionGroups.length > 0) {
                                for (var j = 0; j < payload.criteriaDefinitionGroups.length; j++) {
                                    var criteriaDefinitionGroup = payload.criteriaDefinitionGroups[j];
                                    var criteriaDefField = UtilsService.getItemByVal(criteriaDefinitionGroup.Fields, criteriaField.FieldName, 'FieldName');
                                    if (criteriaDefField != undefined) {
                                        var criteriaGroup = UtilsService.getItemByVal($scope.criteriaGroups, criteriaDefinitionGroup.GroupId, 'groupId');
                                        if (criteriaGroup != undefined) {
                                            var selectedFieldValue = criteriaFieldsValues != undefined ? criteriaFieldsValues[criteriaField.FieldName] : undefined;
                                            if (selectedFieldValue != undefined) {
                                                criteriaGroup.selectedField = criteriaField;
                                            }
                                            criteriaGroup.fields.push(criteriaField);
                                        } else {
                                            $scope.criteriaGroups.push({
                                                groupId: criteriaDefinitionGroup.GroupId,
                                                groupTitle: criteriaDefinitionGroup.GroupTitle,
                                                fields: [criteriaField],
                                                selectedField: criteriaField
                                            });
                                        }
                                        belongToGroup = true;
                                    }
                                }
                                if (!belongToGroup)
                                    $scope.remainingCriteriaFields.push(criteriaField);
                            } else {
                                $scope.remainingCriteriaFields.push(criteriaField);
                            }
                        }
                    });
                    
                    genericUIObj = VR_GenericData_GenericUIService.createGenericUIObj(payload.criteriaDefinitionFields);
                    $scope.criteriaDefinitionFields = payload.criteriaDefinitionFields;
                    var promises = [];

                    promises.push(loadAllFieldsPromiseDeferred.promise);
                    var loadFieldTypeConfigPromise = VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (allConfigs) {

                        var criteriaFieldsPromises = [];

                        angular.forEach($scope.criteriaDefinitionFields, function (field) {
                            var dataFieldTypeConfig = UtilsService.getItemByVal(allConfigs, field.FieldType.ConfigId, 'ExtensionConfigurationId');
                            field.runtimeEditor = {};
                            field.runtimeEditor.directive = dataFieldTypeConfig.RuntimeEditor;
                            field.runtimeEditor.onReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                            field.runtimeEditor.onDirectiveReady = function (api) {
                                field.runtimeEditor.directiveAPI = api;
                                field.runtimeEditor.onReadyPromiseDeferred.resolve();
                            };

                            field.runtimeEditor.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                            //criteriaFieldsPromises.push(field.runtimeEditor.loadPromiseDeferred.promise);

                            if (criteriaAccessibility != undefined) {
                                var accessibleField = criteriaAccessibility[field.FieldName];
                                if (accessibleField != undefined) {
                                    field.notAccessible = accessibleField.notAccessible;
                                }
                            }

                            field.genericUIContext = genericUIObj.getFieldContext(field);  //VR_GenericData_GenericUIService.buildGenericUIContext($scope.criteriaDefinitionFields, field);

                            field.runtimeEditor.onReadyPromiseDeferred.promise.then(function () {
                                var payload = {
                                    fieldTitle: field.Title,
                                    fieldType: field.FieldType,
                                    fieldValue: (criteriaFieldsValues != undefined) ?
                                        criteriaFieldsValues[field.FieldName] :
                                        (criteriaPredefinedData != undefined ?
                                        criteriaPredefinedData[field.FieldName] : undefined),
                                    genericUIContext: field.genericUIContext
                                };
                                VRUIUtilsService.callDirectiveLoad(field.runtimeEditor.directiveAPI, payload, field.runtimeEditor.loadPromiseDeferred);
                            });
                        });

                        UtilsService.waitMultiplePromises(criteriaFieldsPromises).then(function () {
                            loadAllFieldsPromiseDeferred.resolve();
                        }).catch(function (error) {
                            loadAllFieldsPromiseDeferred.reject(error);
                        });

                        $scope.criteriaFields = payload.criteriaDefinitionFields;

                    });
                    promises.push(loadFieldTypeConfigPromise);

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                            genericUIObj.loadingFinish();
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
        }
    }
    app.directive('vrGenericdataGenericruleCriteria', GenericRuleCriteriaDirective);

})(app);