'use strict';

app.directive('vrGenericdataFieldtypeBusinessentityRuntimeeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectionmode: '@',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                $scope.scopeModel = {};

                ctrl.selectedvalues;
                if ($attrs.selectionmode == "multiple")
                    ctrl.selectedvalues = [];

                if ($attrs.selectionmode == "dynamic") {
                    $scope.scopeModel.calculatedColNum = ctrl.normalColNum * 4;
                    $scope.scopeModel.showInDynamicMode = true;
                }
                else {
                    $scope.scopeModel.calculatedColNum = ctrl.normalColNum;
                }

                ctrl.datasource = [];
                var ctor = new businessEntityCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'runtimeEditorCtrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var multipleselection = "";

            if (attrs.selectionmode == "dynamic" || attrs.selectionmode == "multiple") {
                multipleselection = "ismultipleselection";
            }
            var showaddbutton = "";

            if (attrs.enableadd != undefined) {
                showaddbutton = ' showaddbutton="true" ';
            }

            if (attrs.selectionmode == "dynamic") {
                return '<span vr-loader="isLoading">'
                    + '<vr-directivewrapper directive="selector.directive" normal-col-num="{{scopeModel.calculatedColNum}}" on-ready="selector.onDirectiveReady" onselectionchanged="selector.onselectionchanged" onselectitem = "selector.onselectitem" '
                    + multipleselection + ' isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>'
                    + '<vr-row removeline> <vr-columns width="fullrow"> <vr-section title="{{scopeModel.fieldTitle}}" ng-if="scopeModel.showInDynamicMode"><vr-directivewrapper directive="dynamic.directive" normal-col-num="{{scopeModel.calculatedColNum}}" on-ready="dynamic.onDirectiveReady" isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>' +
                    '</vr-section> </vr-columns> </vr-row>'
                    + '</span>';
            }
            else {
                return '<span vr-loader="isLoading">'
                    + '<vr-directivewrapper directive="selector.directive" ' + showaddbutton + ' normal-col-num="{{scopeModel.calculatedColNum}}"  on-ready="selector.onDirectiveReady" onselectionchanged="selector.onselectionchanged" onselectitem = "selector.onselectitem" '
                    + multipleselection + ' isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>'
                    + '<vr-section title="{{scopeModel.fieldTitle}}" ng-if="scopeModel.showInDynamicMode"><vr-directivewrapper directive="dynamic.directive" normal-col-num="{{scopeModel.calculatedColNum}}" on-ready="dynamic.onDirectiveReady" isrequired="runtimeEditorCtrl.isrequired"></vr-directivewrapper>' +
                    '</vr-section>'
                    + '</span>';
            }
        }

        function businessEntityCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var fieldName;
            var fieldValue;
            var businessEntityDefinitionId;
            var missingGroupSelectorUIControl;
            var genericContext;
            var isFirstLoad = true;
            var fieldValuesByName = {};
            var dependentFields = [];

            function initializeController() {
                //ctrl.showSelector = ($attrs.selectionmode == "dynamic");

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var fieldType;
                    var genericUIContext;
                    var allFieldValuesByName;
                    var parentFieldValues;

                    if (payload != undefined) {
                        $scope.scopeModel.fieldTitle = payload.fieldTitle;
                        fieldName = payload.fieldName;
                        fieldType = payload.fieldType;
                        fieldValue = payload.fieldValue;
                        genericUIContext = payload.genericUIContext;
                        genericContext = payload.genericContext;
                        allFieldValuesByName = payload.allFieldValuesByName;
                        parentFieldValues = payload.parentFieldValues;
                    }

                    if (fieldType != undefined) {
                        businessEntityDefinitionId = fieldType.BusinessEntityDefinitionId;

                        var data;
                        var hasEmtyRequiredDependentField = false;

                        if (fieldType.DependantFields != null && fieldType.DependantFields != undefined) {
                            dependentFields = fieldType.DependantFields;

                            if (allFieldValuesByName != undefined) {
                                data = evaluateAndApplyFieldState(allFieldValuesByName);
                                hasEmtyRequiredDependentField = data.hasEmtyRequiredDependentField;
                            }
                        }

                        var promises = [];

                        var loadWholeSectionPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadWholeSectionPromiseDeferred.promise);

                        var loadBusinessEntityDefPromiseDeferred = VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(businessEntityDefinitionId);
                        promises.push(loadBusinessEntityDefPromiseDeferred);

                        loadBusinessEntityDefPromiseDeferred.then(function (businessEntityDef) {
                            if (businessEntityDef.Settings != null) {

                                if (businessEntityDef.Settings.GroupSelectorUIControl == null || businessEntityDef.Settings.GroupSelectorUIControl == "") {
                                    $scope.scopeModel.showInDynamicMode = false;
                                    missingGroupSelectorUIControl = true;
                                    $scope.scopeModel.calculatedColNum = ctrl.normalColNum;
                                }

                                var innerSectionPromises = [];
                                if ($attrs.selectionmode == "dynamic" && !missingGroupSelectorUIControl) {
                                    $scope.dynamic = {};
                                    $scope.dynamic.directive = businessEntityDef.Settings.GroupSelectorUIControl;
                                    $scope.dynamic.directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    $scope.dynamic.onDirectiveReady = function (api) {
                                        $scope.dynamic.directiveAPI = api;
                                        $scope.dynamic.directiveReadyPromiseDeferred.resolve();
                                    };

                                    $scope.dynamic.directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                    innerSectionPromises.push($scope.dynamic.directiveLoadPromiseDeferred.promise);

                                    $scope.dynamic.directiveReadyPromiseDeferred.promise.then(function () {

                                        var payload;
                                        if (fieldValue != undefined) {
                                            payload = fieldValue.BusinessEntityGroup;
                                        }
                                        VRUIUtilsService.callDirectiveLoad($scope.dynamic.directiveAPI, payload, $scope.dynamic.directiveLoadPromiseDeferred);
                                    });
                                }
                                else {
                                    if ($scope.selector == undefined) {
                                        $scope.selector = {};
                                        $scope.selector.directive = businessEntityDef.Settings.SelectorUIControl;
                                        $scope.selector.directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    }

                                    $scope.selector.onDirectiveReady = function (api) {
                                        $scope.selector.directiveAPI = api;
                                        $scope.selector.directiveReadyPromiseDeferred.resolve();
                                    };

                                    $scope.selector.onselectionchanged = function (selectedvalue) {
                                        if (isFirstLoad)
                                            return;

                                        if (genericUIContext != undefined && genericUIContext.notifyValueChanged != undefined && typeof (genericUIContext.notifyValueChanged) == "function") {
                                            genericUIContext.notifyValueChanged(selectedvalue);
                                        }

                                        if ($attrs.selectionmode != "dynamic") {
                                            var selectedIds = $scope.selector.directiveAPI.getSelectedIds();
                                            if (ctrl.selectionmode == "single" && selectedIds != undefined)
                                                selectedIds = [selectedIds];

                                            fieldValuesByName[fieldName] = selectedIds;
                                            notifyFieldValueChanged(selectedIds);
                                        }
                                    };

                                    $scope.selector.directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                                    innerSectionPromises.push($scope.selector.directiveLoadPromiseDeferred.promise);

                                    $scope.selector.directiveReadyPromiseDeferred.promise.then(function () {

                                        var payload = {
                                            businessEntityDefinitionId: businessEntityDefinitionId,
                                            fieldTitle: $scope.scopeModel.fieldTitle,
                                            beRuntimeSelectorFilter: fieldType.BERuntimeSelectorFilter,
                                            genericUIContext: genericUIContext,
                                            hasEmtyRequiredDependentField: hasEmtyRequiredDependentField,
                                            filter: data != undefined && data.genericBusinessEntityFilters != undefined ? { FieldFilters: data.genericBusinessEntityFilters } : getFilter()
                                        };

                                        if (fieldValue != undefined) {
                                            payload.selectedIds = ($attrs.selectionmode == "dynamic" && missingGroupSelectorUIControl) ? fieldValue.Values : fieldValue;
                                        }

                                        if (parentFieldValues != undefined && fieldName in parentFieldValues) {
                                            payload.isDisabled = parentFieldValues[fieldName].isDisabled;
                                            if (fieldValue == undefined) {
                                                fieldValue = parentFieldValues[fieldName].value;
                                                payload.hasEmtyRequiredDependentField = false;
                                                payload.selectedIds = fieldValue;
                                            }
                                        }

                                        VRUIUtilsService.callDirectiveLoad($scope.selector.directiveAPI, payload, $scope.selector.directiveLoadPromiseDeferred);
                                    });
                                }

                                UtilsService.waitMultiplePromises(innerSectionPromises).then(function () {
                                    loadWholeSectionPromiseDeferred.resolve();
                                }).catch(function (error) {
                                    loadWholeSectionPromiseDeferred.reject(error);
                                });
                            }
                        });

                        return UtilsService.waitMultiplePromises(promises).then(function () {
                            isFirstLoad = false;
                        });
                    }
                };

                api.getData = function () {
                    var retVal;

                    if (ctrl.selectionmode == "dynamic") {
                        if (missingGroupSelectorUIControl) {
                            var selectedIds = $scope.selector.directiveAPI != undefined ? $scope.selector.directiveAPI.getSelectedIds() : undefined;
                            if (selectedIds != undefined) {
                                retVal = {
                                    $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.StaticValues, Vanrise.GenericData.MainExtensions",
                                    Values: selectedIds
                                };
                            }
                        }
                        else {
                            var directiveData = $scope.dynamic.directiveAPI.getData();
                            if (directiveData != undefined) {
                                retVal = {
                                    $type: "Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues.BusinessEntityValues, Vanrise.GenericData.MainExtensions",
                                    BusinessEntityGroup: directiveData
                                };
                            }
                        }
                    }
                    else if (ctrl.selectionmode == "single" || ctrl.selectionmode == "multiple") {
                        retVal = $scope.selector.directiveAPI != undefined ? $scope.selector.directiveAPI.getSelectedIds() : undefined;
                    }

                    return retVal;
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) { //allFieldValuesByFieldNames { field1: [value1, value2], ...}

                    var directiveAPI = getDirectiveAPI();
                    if (directiveAPI == undefined)
                        return;

                    if (directiveAPI.onFieldValueChanged != undefined && typeof (directiveAPI.onFieldValueChanged) == "function")
                        return directiveAPI.onFieldValueChanged(allFieldValuesByFieldNames);

                    var data = evaluateAndApplyFieldState(allFieldValuesByFieldNames);
                    if (!data.isDependentFieldsChanged)
                        return;

                    if (data.isRequiredDependentFieldDeselected) {
                        directiveAPI.clearDataSource();
                        return;
                    }

                    return reloadDirectiveOnValueChanged(directiveAPI, data);
                };

                //api.setFieldValues = function (fieldValuesByNames) {

                //    var setFieldValuesPromiseDeferred = UtilsService.createPromiseDeferred();

                //    var directiveAPI = getDirectiveAPI();
                //    if (fieldValuesByNames == undefined || !(fieldName in fieldValuesByNames) || directiveAPI == undefined || directiveAPI.setFieldValues == undefined || typeof (directiveAPI.setFieldValues) != "function") {
                //        setFieldValuesPromiseDeferred.resolve();
                //        return setFieldValuesPromiseDeferred.promise;
                //    }

                //    var newFieldValue = fieldValuesByNames[fieldName]; //single value (multiple not supported yet)

                //    var oldFieldValues = directiveAPI.getSelectedIds();
                //    if (typeof (oldFieldValues) != "object" && oldFieldValues != undefined)
                //        oldFieldValues = [oldFieldValues];

                //    var isValueChanged = false;
                //    var valueToSet;

                //    if (newFieldValue == undefined) {
                //        isValueChanged = true;
                //    }
                //    else if (oldFieldValues == undefined) {
                //        isValueChanged = true;
                //        valueToSet = [newFieldValue];
                //    }
                //    else if (oldFieldValues.indexOf(newFieldValue) == -1) {
                //        isValueChanged = true;
                //        valueToSet = oldFieldValues;
                //        valueToSet.push(newFieldValue);
                //    }

                //    if (!isValueChanged) {
                //        setFieldValuesPromiseDeferred.resolve();
                //        return setFieldValuesPromiseDeferred.promise;
                //    }

                //    var loadParentGenericBusinessEntitiesPromise = loadParentGenericBusinessEntities(newFieldValue);

                //    UtilsService.waitMultiplePromises([loadParentGenericBusinessEntitiesPromise, $scope.selector.directiveLoadPromiseDeferred.promise]).then(function () {

                //        fieldValuesByName[fieldName] = valueToSet;

                //        var context = {
                //            isDisabled: true,
                //            filter: getFilter()
                //        };

                //        var innerSetFieldValuesPromise = directiveAPI.setFieldValues(valueToSet, context);
                //        if (innerSetFieldValuesPromise != undefined) {
                //            innerSetFieldValuesPromise.then(function () {
                //                notifyFieldValueChanged(valueToSet).then(function () {
                //                    setFieldValuesPromiseDeferred.resolve();
                //                });
                //            });
                //        }
                //        else {
                //            setFieldValuesPromiseDeferred.resolve();
                //        }
                //    });

                //    return setFieldValuesPromiseDeferred.promise;
                //};

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function notifyFieldValueChanged(valueToSet) {
                var _promises = [];

                if (genericContext != undefined && genericContext.notifyFieldValueChanged != undefined && typeof (genericContext.notifyFieldValueChanged) == "function") {
                    var changedField = { fieldName: fieldName, fieldValues: valueToSet };
                    var notifyFieldValueChangedPromise = genericContext.notifyFieldValueChanged(changedField);
                    _promises.push(notifyFieldValueChangedPromise);
                }

                return UtilsService.waitMultiplePromises(_promises);
            }

            function getFilter() {
                var genericBusinessEntityFilters;

                if (dependentFields == undefined || dependentFields.length == 0 || fieldValuesByName == undefined)
                    return genericBusinessEntityFilters;

                for (var i = 0; i < dependentFields.length; i++) {
                    var currentDependentField = dependentFields[i];
                    var dependentFieldName = currentDependentField.FieldName;

                    if ((dependentFieldName in fieldValuesByName) && fieldValuesByName[dependentFieldName] != undefined) {
                        if (genericBusinessEntityFilters == undefined)
                            genericBusinessEntityFilters = [];
                        genericBusinessEntityFilters.push({ FieldName: currentDependentField.MappedFieldName, FilterValues: fieldValuesByName[dependentFieldName] });
                    }
                }

                return genericBusinessEntityFilters != undefined ? { FieldFilters: genericBusinessEntityFilters } : undefined;
            }

            function getDirectiveAPI() {
                var directiveAPI;

                if (ctrl.selectionmode != "dynamic") {
                    directiveAPI = $scope.selector != undefined ? $scope.selector.directiveAPI : undefined;

                    return directiveAPI;
                }
            }

            function evaluateAndApplyFieldState(allChangedFields) {

                var data = {
                    genericBusinessEntityFilters: undefined,
                    hasEmtyRequiredDependentField: false,
                    isDependentFieldsChanged: false,
                    isDependentFieldDeselected: false,
                    isRequiredDependentFieldDeselected: false
                };

                if (dependentFields == undefined || dependentFields.length == 0 || allChangedFields == undefined)
                    return data;

                for (var i = 0; i < dependentFields.length; i++) {
                    var currentDependentField = dependentFields[i];
                    var dependentFieldName = currentDependentField.FieldName;

                    if (dependentFieldName in allChangedFields) {
                        var oldValues = fieldValuesByName[dependentFieldName];
                        var newValues = allChangedFields[dependentFieldName];

                        if (oldValues != undefined || newValues != undefined) {
                            if (oldValues == undefined || newValues == undefined || oldValues.length != newValues.length) {
                                fieldValuesByName[dependentFieldName] = newValues;
                                data.isDependentFieldsChanged = true;
                            }
                            else {
                                for (var j = 0; j < newValues.length; j++) {
                                    var currentValue = newValues[j];
                                    if (oldValues.indexOf(currentValue) == -1) {
                                        fieldValuesByName[dependentFieldName] = allChangedFields[dependentFieldName];
                                        data.isDependentFieldsChanged = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (fieldValuesByName[dependentFieldName] == undefined) {
                            data.isDependentFieldDeselected = true;
                            if (currentDependentField.IsRequired)
                                data.isRequiredDependentFieldDeselected = true;
                        }
                        else {
                            if (data.genericBusinessEntityFilters == undefined)
                                data.genericBusinessEntityFilters = [];
                            data.genericBusinessEntityFilters.push({ FieldName: currentDependentField.MappedFieldName, FilterValues: fieldValuesByName[dependentFieldName] });
                        }
                    }

                    data.hasEmtyRequiredDependentField = data.hasEmtyRequiredDependentField || (currentDependentField.IsRequired && fieldValuesByName[currentDependentField.FieldName] == undefined);
                }

                return data;
            }

            function reloadDirectiveOnValueChanged(directiveAPI, data) {

                var directiveLoadPayload = {
                    fieldTitle: $scope.scopeModel.fieldTitle,
                    businessEntityDefinitionId: businessEntityDefinitionId,
                    selectedIds: data.isDependentFieldDeselected ? undefined : directiveAPI != undefined ? directiveAPI.getSelectedIds() : fieldValue,
                    filter: data.genericBusinessEntityFilters != undefined ? { FieldFilters: data.genericBusinessEntityFilters } : undefined,
                    hasEmtyRequiredDependentField: data.hasEmtyRequiredDependentField
                };
                var setLoader = function (value) {
                    $scope.isLoading = value;
                };
                return VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directiveLoadPayload, setLoader);
            }

            //function loadParentGenericBusinessEntities(genericBusinessEntityId) {
            //    var loadParentGenericBusinessEntitiesDeferred = UtilsService.createPromiseDeferred();

            //    if (genericBusinessEntityId == undefined || dependentFields == undefined || dependentFields.length == 0 || genericContext == undefined || genericContext.setFieldValues == undefined ||
            //        typeof (genericContext.setFieldValues) != "function") {
            //        loadParentGenericBusinessEntitiesDeferred.resolve();
            //        return loadParentGenericBusinessEntitiesDeferred.promise;
            //    }

            //    VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntity(businessEntityDefinitionId, genericBusinessEntityId).then(function (response) {
            //        if (response) {
            //            var beFieldValues = response.FieldValues;

            //            var promise = setDependentFieldsValues(beFieldValues);
            //            if (promise != undefined) {
            //                promise.then(function () {
            //                    loadParentGenericBusinessEntitiesDeferred.resolve();
            //                });
            //            }
            //            else {
            //                loadParentGenericBusinessEntitiesDeferred.resolve();
            //            }
            //        }
            //    });

            //    function setDependentFieldsValues(beFieldValues) {
            //        if (beFieldValues == undefined)
            //            return;

            //        var fieldsToSet = {};

            //        for (var i = 0; i < dependentFields.length; i++) {
            //            var currentDependentField = dependentFields[i];
            //            var beFieldValue = beFieldValues[currentDependentField.MappedFieldName];
            //            var dependentFieldName = currentDependentField.FieldName;
            //            fieldsToSet[dependentFieldName] = beFieldValue;
            //            fieldValuesByName[dependentFieldName] = beFieldValue != undefined ? [beFieldValue] : undefined;
            //        }

            //        if (Object.keys(fieldsToSet).length == 0)
            //            return;

            //        return genericContext.setFieldValues(fieldsToSet);
            //    }

            //    return loadParentGenericBusinessEntitiesDeferred.promise;
            //}
        }

        return directiveDefinitionObject;
    }]);