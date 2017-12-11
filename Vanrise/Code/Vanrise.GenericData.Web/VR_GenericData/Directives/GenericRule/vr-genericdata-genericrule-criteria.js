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

            function initializeController() {
                $scope.criteriaDefinitionFields;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var genericRuleCriteria = {};

                    if ($scope.criteriaDefinitionFields != undefined) {
                        genericRuleCriteria.FieldsValues = {};
                        var criteriaValuesExist = false;

                        angular.forEach($scope.criteriaDefinitionFields, function (field) {
                            var fieldData = field.runtimeEditor.directiveAPI.getData();
                            if (fieldData != undefined) {
                                genericRuleCriteria.FieldsValues[field.FieldName] = fieldData;
                                criteriaValuesExist = true;
                            }
                        });

                        if (!criteriaValuesExist)
                            genericRuleCriteria = undefined;
                    }
                    return genericRuleCriteria;
                };

                api.load = function (payload) {
                    if (payload == undefined || payload.criteriaDefinitionFields == undefined) {
                        return;
                    }
                    
                    genericUIObj = VR_GenericData_GenericUIService.createGenericUIObj(payload.criteriaDefinitionFields);
                    $scope.criteriaDefinitionFields = payload.criteriaDefinitionFields;
                    var criteriaFieldsValues = payload.criteriaFieldsValues;
                    var criteriaAccessibility = payload.criteriaAccessibility;
                    var criteriaPredefinedData = payload.criteriaPredefinedData;
                    var promises = [];

                    var loadAllFieldsPromiseDeferred = UtilsService.createPromiseDeferred();
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
                            criteriaFieldsPromises.push(field.runtimeEditor.loadPromiseDeferred.promise);

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

                        $scope.criteriaFields = payload.criteriaDefinitionFields

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