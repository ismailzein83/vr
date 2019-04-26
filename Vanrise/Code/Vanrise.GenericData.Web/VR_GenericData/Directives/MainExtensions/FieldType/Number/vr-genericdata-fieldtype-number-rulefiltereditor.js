'use strict';

app.directive('vrGenericdataFieldtypeNumberRulefiltereditor', ['VR_GenericData_NumberRecordFilterOperatorEnum', 'UtilsService', 'VRUIUtilsService',
    function (VR_GenericData_NumberRecordFilterOperatorEnum, UtilsService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Number/Templates/NumberFieldTypeRuleFilterEditor.html"
        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var filterObj;
            var dataRecordTypeField;

            var onFilterSelectionChangedDeferred = UtilsService.createPromiseDeferred();

            var numberFilterEditorAPI;
            var numberFilterReadyDeferred = UtilsService.createPromiseDeferred();

            var number2FilterEditorAPI;
            var number2FilterReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.filters = UtilsService.getArrayEnum(VR_GenericData_NumberRecordFilterOperatorEnum);

                $scope.onNumberFilterEditorReady = function (api) {
                    numberFilterEditorAPI = api;
                    numberFilterReadyDeferred.resolve();
                };

                $scope.onNumber2FilterEditorReady = function (api) {
                    number2FilterEditorAPI = api;
                    number2FilterReadyDeferred.resolve();
                };

                $scope.onFilterSelectionChanged = function (selectedValue) {
                    if (selectedValue == undefined)
                        return;

                    if (onFilterSelectionChangedDeferred != undefined) {
                        onFilterSelectionChangedDeferred.resolve();
                    }
                    else if ($scope.selectedFilter.showSecondNumberField) {
                        numberFilterEditorAPI.setLabel("From Number");

                        var payload = {
                            fieldType: dataRecordTypeField != undefined ? dataRecordTypeField.Type : null,
                            fieldTitle: 'To Number'
                        };
                        var setLoader = function (value) {
                            setTimeout(function () {
                                $scope.isNumber2FilterEditorLoading = value;
                            }, 0);
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, number2FilterEditorAPI, payload, setLoader);
                    }
                    else {
                        numberFilterEditorAPI.setLabel("");
                    }
                };

                $scope.validateNumbers = function () {

                    if (numberFilterEditorAPI == undefined || number2FilterEditorAPI == undefined)
                        return null;

                    if ($scope.selectedFilter == undefined || !$scope.selectedFilter.showSecondNumberField)
                        return null;

                    if (numberFilterEditorAPI.getData() > number2FilterEditorAPI.getData())
                        return 'To Number must be greater than From Number';
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.includeValues = true;
                    $scope.selectedFilter = $scope.filters[0];

                    if (payload) {
                        filterObj = payload.filterObj;
                        dataRecordTypeField = payload.dataRecordTypeField;

                        if (filterObj) {
                            $scope.selectedFilter = UtilsService.getItemByVal($scope.filters, filterObj.CompareOperator, 'value');
                            $scope.scopeModel.includeValues = filterObj.IncludeValues;
                        }

                        var numberFilterLoadDeferred = UtilsService.createPromiseDeferred();
                        var number2FilterLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([numberFilterReadyDeferred.promise, number2FilterReadyDeferred.promise, onFilterSelectionChangedDeferred.promise]).then(function () {
                            onFilterSelectionChangedDeferred = undefined;

                            numberFilterReadyDeferred.promise.then(function () {

                                var numberFilterPayload = {
                                    fieldType: dataRecordTypeField != undefined ? dataRecordTypeField.Type : null,
                                    fieldValue: filterObj != undefined ? filterObj.Value : undefined,
                                    fieldTitle: $scope.selectedFilter.showSecondNumberField ? 'From Number' : ''
                                };
                                VRUIUtilsService.callDirectiveLoad(numberFilterEditorAPI, numberFilterPayload, numberFilterLoadDeferred);
                            });

                            number2FilterReadyDeferred.promise.then(function () {

                                var number2FilterPayload = {
                                    fieldType: dataRecordTypeField != undefined ? dataRecordTypeField.Type : null,
                                    fieldValue: filterObj != undefined ? filterObj.Value2 : undefined,
                                    fieldTitle: 'To Number'
                                };
                                VRUIUtilsService.callDirectiveLoad(number2FilterEditorAPI, number2FilterPayload, number2FilterLoadDeferred);
                            });
                        });

                        return UtilsService.waitMultiplePromises([numberFilterLoadDeferred.promise, number2FilterLoadDeferred.promise]);
                    }
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.GenericData.Entities.NumberRecordFilter, Vanrise.GenericData.Entities",
                        CompareOperator: $scope.selectedFilter.value,
                        Value: numberFilterEditorAPI.getData(),
                        Value2: $scope.selectedFilter.showSecondNumberField ? number2FilterEditorAPI.getData() : undefined,
                        IncludeValues: $scope.scopeModel.includeValues
                    };
                    return data;
                };

                api.getExpression = function () {
                    if ($scope.selectedFilter.showSecondNumberField)
                        return $scope.selectedFilter.description + ' ' + numberFilterEditorAPI.getData() + ' and ' + number2FilterEditorAPI.getData();
                    else
                        return $scope.selectedFilter.description + ' ' + numberFilterEditorAPI.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);