'use strict';

app.directive('vrGenericdataFieldtypeDatetimeRulefiltereditor', ['VR_GenericData_DateTimeRecordFilterOperatorEnum', 'VR_GenericData_DateTimeRecordFilterComparisonPartEnum', 'UtilsService', 'VRUIUtilsService', 'VRDateTimeService',
    function (VR_GenericData_DateTimeRecordFilterOperatorEnum, VR_GenericData_DateTimeRecordFilterComparisonPartEnum, UtilsService, VRUIUtilsService, VRDateTimeService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DateTime/Templates/DateTimeFieldTypeRuleFilterEditor.html"
        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var selectedDataRecordField;
            var filterObj;

            var onFilterSelectionChangedDeferred = UtilsService.createPromiseDeferred();
            var onComparisonPartSelectionChangedDeferred = UtilsService.createPromiseDeferred();

            var dateFilterEditorAPI;
            var dateFilterReadyDeferred = UtilsService.createPromiseDeferred();

            var date2FilterEditorAPI;
            var date2FilterReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.filters = [];
                $scope.comparisonParts = [];
                $scope.isDateFilterRecompiling = true;
                $scope.isDate2FilterRecompiling = true;

                $scope.onDateFilterEditorReady = function (api) {
                    dateFilterEditorAPI = api;

                    var dateFilterPayload = {
                        fieldType: selectedDataRecordField != undefined ? selectedDataRecordField.Type : undefined,
                        fieldValue: filterObj != undefined ? filterObj.Value : undefined,
                        fieldTitle: ($scope.selectedFilter != undefined && $scope.selectedFilter.showSecondDateTimePicker) ? 'From' :
                                     isTimeComparisonPart($scope.selectedComparisonPart) ? 'Time' : 'Date'
                    };
                    var setLoader = function (value) {
                        $scope.isDateFilterEditorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dateFilterEditorAPI, dateFilterPayload, setLoader, dateFilterReadyDeferred);
                };
                $scope.onDate2FilterEditorReady = function (api) {
                    date2FilterEditorAPI = api;

                    var date2FilterPayload = {
                        fieldType: selectedDataRecordField != undefined ? selectedDataRecordField.Type : undefined,
                        fieldValue: filterObj != undefined ? filterObj.Value2 : undefined,
                        fieldTitle: 'To'
                    };
                    var setLoader = function (value) {
                        $scope.isDate2FilterEditorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, date2FilterEditorAPI, date2FilterPayload, setLoader, date2FilterReadyDeferred);
                };

                $scope.onFilterSelectionChanged = function (selectedFilter) {

                    if (selectedFilter != undefined) {
                        if (onFilterSelectionChangedDeferred != undefined) {
                            onFilterSelectionChangedDeferred.resolve();
                        }
                        else if (selectedFilter.showSecondDateTimePicker) {
                            dateFilterEditorAPI.setLabel("From");

                            var date2FilterPayload = {
                                fieldType: selectedDataRecordField != undefined ? selectedDataRecordField.Type : undefined,
                                fieldValue: undefined,
                                fieldTitle: 'To'
                            };
                            var setLoader = function (value) {
                                setTimeout(function () {
                                    $scope.isDate2FilterEditorLoading = value;
                                }, 0);
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, date2FilterEditorAPI, date2FilterPayload, setLoader);
                        }
                        else {
                            var label = isTimeComparisonPart($scope.selectedComparisonPart) ? 'Time' : 'Date';
                            dateFilterEditorAPI.setLabel(label);
                        }
                    }
                };
                $scope.onComparisonPartSelectionChanged = function (selectedComparisonPart) {

                    if (selectedComparisonPart != undefined) {
                        selectedDataRecordField.Type.DataType = selectedComparisonPart.value;

                        if (onComparisonPartSelectionChangedDeferred != undefined) {
                            onComparisonPartSelectionChangedDeferred.resolve();
                        } else {
                            $scope.isDateFilterRecompiling = false;
                            $scope.isDate2FilterRecompiling = false;

                            setTimeout(function () {
                                $scope.isDateFilterRecompiling = true;
                                $scope.isDate2FilterRecompiling = true;
                            }, 0);
                        }
                    }
                };

                $scope.validateDates = function () {

                    if (dateFilterEditorAPI == undefined || date2FilterEditorAPI == undefined)
                        return null;

                    var value = dateFilterEditorAPI.getData();
                    var value2 = date2FilterEditorAPI.getData();

                    if ($scope.selectedFilter == undefined || !$scope.selectedFilter.showSecondDateTimePicker)
                        return null;

                    if (value == undefined || value2 == undefined)
                        return null;

                    if (isTimeComparisonPart($scope.selectedComparisonPart)) {

                        if (value2.Hour > value.Hour)
                            return null;

                        if (value2.Hour < value.Hour)
                            return 'Invalid Time Range';

                        //value2.Hour = value.Hour
                        if (value2.Minute > value.Minute)
                            return null;

                        if (value2.Minute < value.Minute)
                            return 'Invalid Time Range';

                        //value2.Minute = value.Minute
                        if (value2.Second > value.Second)
                            return null;

                        if (value2.Second < value.Second)
                            return 'Invalid Time Range';

                        //value2.Second = value.Second
                        if (value2.Millisecond > value.Millisecond)
                            return null;

                        if (value2.Millisecond < value.Millisecond)
                            return 'Invalid Time Range';
                    }
                    else {
                        if (value > value2)
                            return 'Invalid Time Range';
                    }

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_DateTimeRecordFilterOperatorEnum);
                    $scope.selectedFilter = $scope.filters[0];

                    if (payload) {
                        selectedDataRecordField = JSON.parse(JSON.stringify(payload.dataRecordTypeField)); //cloning DataRecordTypeField
                        filterObj = payload.filterObj;

                        if (filterObj) {
                            $scope.selectedFilter = UtilsService.getItemByVal($scope.filters, filterObj.CompareOperator, 'value');
                        }

                        var comparisonPart = filterObj != undefined ? filterObj.ComparisonPart : undefined;
                        loadComparisonPartSelector(selectedDataRecordField, comparisonPart);

                        var dateFilterLoadDeferred = UtilsService.createPromiseDeferred();
                        var date2FilterLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([dateFilterReadyDeferred.promise, date2FilterReadyDeferred.promise, onFilterSelectionChangedDeferred.promise, onComparisonPartSelectionChangedDeferred.promise]).then(function () {
                            dateFilterReadyDeferred = date2FilterReadyDeferred = onFilterSelectionChangedDeferred = onComparisonPartSelectionChangedDeferred = undefined;

                            //Loading DateFilter Directive
                            var dateFilterPayload = {
                                fieldType: selectedDataRecordField.Type,
                                fieldValue: filterObj != undefined ? filterObj.Value : undefined,
                                fieldTitle: ($scope.selectedFilter != undefined && $scope.selectedFilter.showSecondDateTimePicker) ? 'From' :
                                             isTimeComparisonPart($scope.selectedComparisonPart) ? 'Time' : 'Date'
                            };
                            VRUIUtilsService.callDirectiveLoad(dateFilterEditorAPI, dateFilterPayload, dateFilterLoadDeferred);

                            //Loading Date2Filter Directive
                            var date2FilterPayload = {
                                fieldType: selectedDataRecordField.Type,
                                fieldValue: filterObj != undefined ? filterObj.Value2 : undefined,
                                fieldTitle: 'To'
                            };
                            VRUIUtilsService.callDirectiveLoad(date2FilterEditorAPI, date2FilterPayload, date2FilterLoadDeferred);
                        });

                        return UtilsService.waitMultiplePromises([dateFilterLoadDeferred.promise, date2FilterLoadDeferred.promise]);
                    }
                };

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Entities.DateTimeRecordFilter, Vanrise.GenericData.Entities",
                        ComparisonPart: $scope.selectedComparisonPart.value,
                        CompareOperator: $scope.selectedFilter.value,
                        Value: dateFilterEditorAPI.getData(),
                        Value2: $scope.selectedFilter.showSecondDateTimePicker ? date2FilterEditorAPI.getData() : undefined
                    };

                    return obj;
                };

                api.getExpression = function () {

                    if ($scope.selectedFilter.value.showSecondDateTimePicker) {
                        return $scope.selectedFilter.description + ' ' + dateFilterEditorAPI.getData();
                    }
                    else {
                        return $scope.selectedFilter.description + ' ' + dateFilterEditorAPI.getData() + ' and ' + date2FilterEditorAPI.getData();
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadComparisonPartSelector(dataRecordTypeField, selectedComparisonPart) {
                if (dataRecordTypeField.Type == undefined || dataRecordTypeField.Type.DataType == undefined)
                    return;

                switch (dataRecordTypeField.Type.DataType) {
                    case 0: $scope.comparisonParts = UtilsService.getArrayEnum(VR_GenericData_DateTimeRecordFilterComparisonPartEnum); break;
                    case 1: $scope.comparisonParts.push(VR_GenericData_DateTimeRecordFilterComparisonPartEnum.Time); break;
                    case 2: $scope.comparisonParts.push(VR_GenericData_DateTimeRecordFilterComparisonPartEnum.Date); break;
                    case 3: $scope.comparisonParts.push(VR_GenericData_DateTimeRecordFilterComparisonPartEnum.YearMonth); break;
                    case 4: $scope.comparisonParts.push(VR_GenericData_DateTimeRecordFilterComparisonPartEnum.YearWeek); break;
                    case 5: $scope.comparisonParts.push(VR_GenericData_DateTimeRecordFilterComparisonPartEnum.Hour); break;
                }

                if (selectedComparisonPart != undefined) {
                    $scope.selectedComparisonPart = UtilsService.getItemByVal($scope.comparisonParts, selectedComparisonPart, 'value');
                }
                else {
                    $scope.selectedComparisonPart = $scope.comparisonParts[0];
                }
            }

            function isTimeComparisonPart(selectedComparisonPart) {
                if (selectedComparisonPart == VR_GenericData_DateTimeRecordFilterComparisonPartEnum.Time)
                    return true;

                if (selectedComparisonPart == VR_GenericData_DateTimeRecordFilterComparisonPartEnum.Hour)
                    return true;

                return false;
            }
        }

        return directiveDefinitionObject;
    }]);

