(function (app) {

    'use strict';

    DataRecordFieldTypeDateTimeSettingDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DateTimeDefaultValueEnum', 'VR_GenericData_DateTimeValidationOperatorEnum', 'VR_GenericData_DateTimeValidationTargetEnum'];

    function DataRecordFieldTypeDateTimeSettingDirective(UtilsService, VRUIUtilsService, VR_GenericData_DateTimeDefaultValueEnum, VR_GenericData_DateTimeValidationOperatorEnum, VR_GenericData_DateTimeValidationTargetEnum) {
        return {
            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                $scope.normalColNum = "4";
                if (ctrl.normalColNum != undefined)
                    $scope.normalColNum = ctrl.normalColNum;

                var runtimeViewTypeSelective = new RuntimeViewTypeSelective($scope, ctrl, $attrs);
                runtimeViewTypeSelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DateTime/Templates/DatetimeFieldTypeSettingTemplate.html"
        };

        function RuntimeViewTypeSelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.defaultValues = [];
                $scope.scopeModel.operators = [];
                $scope.scopeModel.validationTargets = [];
                $scope.scopeModel.validations = [];
                $scope.scopeModel.fields = [];

                $scope.scopeModel.hasValidationChanged = function () {
                    $scope.scopeModel.validations.length = 0;
                };

                $scope.scopeModel.addValidation = function () {
                    extendItemToGrid();
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.validations, deletedItem.tempId, 'tempId');
                    $scope.scopeModel.validations.splice(index, 1);
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.hasValidation && $scope.scopeModel.validations.length == 0)
                        return "At least one Validation must be Added!";

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var defaultValue;
                    var validations;
                    var context;
                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                        context = payload.context;
                    }

                    if (settings != undefined) {
                        defaultValue = settings.DefaultValue;
                        validations = settings.Validations;
                    }

                    loadSelectorsDataSourcesFromEnums();
                    setDefaultValueSelector();
                    loadDateTimeFieldTypes();

                    if (validations != undefined && validations.length > 0) {
                        promises.push(loadValidationsGrid());
                    }

                    function loadSelectorsDataSourcesFromEnums() {
                        $scope.scopeModel.defaultValues = UtilsService.getArrayEnum(VR_GenericData_DateTimeDefaultValueEnum);
                        $scope.scopeModel.operators = UtilsService.getArrayEnum(VR_GenericData_DateTimeValidationOperatorEnum);
                        $scope.scopeModel.validationTargets = UtilsService.getArrayEnum(VR_GenericData_DateTimeValidationTargetEnum);
                    }

                    function setDefaultValueSelector() {
                        if (defaultValue != undefined) {
                            var selectedDefaultValue = UtilsService.getItemByVal($scope.scopeModel.defaultValues, defaultValue, "value");
                            if (selectedDefaultValue != null) {
                                $scope.scopeModel.selectedDefaultValue = selectedDefaultValue;
                            }
                        }
                    }

                    function loadDateTimeFieldTypes() {
                        var allFields = context.getFields();

                        if (allFields == undefined || allFields.length == 0)
                            return;

                        for (var i = 0; i < allFields.length; i++) {
                            var currentField = allFields[i];
                            if (currentField.Type.ConfigId != "b8712417-83ab-4d4b-9ee1-109d20ceb909")
                                continue;

                            $scope.scopeModel.fields.push(currentField);
                        }
                    }

                    function loadValidationsGrid() {
                        $scope.scopeModel.hasValidation = true;
                        var gridLoadPromises = [];

                        gridReadyPromiseDeferred.promise.then(function () {
                            for (var i = 0; i < validations.length; i++) {
                                var currentValidation = validations[i];
                                gridLoadPromises.push(extendItemToGrid(currentValidation));
                            }
                        });

                        return UtilsService.waitMultiplePromises(gridLoadPromises);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    if ($scope.scopeModel.selectedDefaultValue == undefined && !$scope.scopeModel.hasValidation)
                        return undefined;

                    var data = {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.DateTimeRuntimeViewSetting , Vanrise.GenericData.MainExtensions",
                        DefaultValue: $scope.scopeModel.selectedDefaultValue != undefined ? $scope.scopeModel.selectedDefaultValue.value : undefined,
                        Validations: $scope.scopeModel.hasValidation ? getValidationsFromGrid() : undefined
                    };

                    function getValidationsFromGrid() {
                        if ($scope.scopeModel.validations.length == 0)
                            return undefined;

                        var validations = [];
                        for (var i = 0; i < $scope.scopeModel.validations.length; i++) {
                            var currentValidation = $scope.scopeModel.validations[i];
                            validations.push({
                                ValidationTarget: currentValidation.selectedTarget.value,
                                ValidationOperator: currentValidation.selectedOperator.value,
                                FieldName: currentValidation.selectedTarget.showFieldsSelector ? currentValidation.selectedField.FieldName : undefined
                            });
                        }

                        return validations;
                    }

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                }
            }

            function extendItemToGrid(validation) {
                var extendValidationPromises = [];

                var validationDataItem = { tempId: UtilsService.guid() };

                if (validation != undefined) {
                    if (validation.ValidationOperator != undefined) {
                        var selectedOperator = UtilsService.getItemByVal($scope.scopeModel.operators, validation.ValidationOperator, "value");
                        if (selectedOperator != null)
                            validationDataItem.selectedOperator = selectedOperator;
                    }

                    if (validation.ValidationTarget != undefined) {
                        var selectedTarget = UtilsService.getItemByVal($scope.scopeModel.validationTargets, validation.ValidationTarget, "value");
                        if (selectedTarget != null) {
                            validationDataItem.selectedTarget = selectedTarget;

                            if (selectedTarget.showFieldsSelector) {
                                var selectedField = UtilsService.getItemByVal($scope.scopeModel.fields, validation.FieldName, "FieldName");
                                if (selectedField != null)
                                    validationDataItem.selectedField = selectedField;
                            }
                        }
                    }
                }

                validationDataItem.onSelectionTargetChanged = function (selectedTarget) {
                    if (selectedTarget == undefined)
                        return;

                    if (!selectedTarget.showFieldsSelector)
                        validationDataItem.selectedField = undefined;
                };

                $scope.scopeModel.validations.push(validationDataItem);
                return UtilsService.waitMultiplePromises(extendValidationPromises);
            }
        }
    }

    app.directive('vrGenericdataFieldtypeDatetimeSettings', DataRecordFieldTypeDateTimeSettingDirective);

})(app);