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

            var numberFilterEditorAPI;
            var numberFilterReadyDeferred = UtilsService.createPromiseDeferred();
            var onNumberFilterConditionChangedDeferred;

            function initializeController() {

                $scope.onNumberFilterEditorReady = function (api) {
                    numberFilterEditorAPI = api;
                    numberFilterReadyDeferred.resolve();
                };

                $scope.onFilterSelectionChanged = function (selectedValue) {
                    if (selectedValue == undefined)
                        return;

                    var payload = {
                        filterType: selectedValue
                    };

                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, numberFilterEditorAPI, payload, setLoader, onNumberFilterConditionChangedDeferred);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_NumberRecordFilterOperatorEnum);
                    $scope.selectedFilter = $scope.filters[0];

                    if (payload && payload.filterObj) {
                        filterObj = payload.filterObj;
                        dataRecordTypeField = payload.dataRecordTypeField;
                        $scope.selectedFilter = UtilsService.getItemByVal($scope.filters, payload.filterObj.CompareOperator, 'value');
                        onNumberFilterConditionChangedDeferred = UtilsService.createPromiseDeferred();

                        var promises = [];

                        var numberFilterLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([numberFilterReadyDeferred.promise, onNumberFilterConditionChangedDeferred.promise]).then(function () {
                            onNumberFilterConditionChangedDeferred = undefined;

                            var numberFilterDirectivePayload = { filterType: $scope.selectedFilter };
                            if (payload && payload.filterObj) {
                                numberFilterDirectivePayload.fieldType = dataRecordTypeField != undefined ? dataRecordTypeField.Type : null;
                                numberFilterDirectivePayload.fieldValue = filterObj != undefined ? filterObj.Value : null;
                                numberFilterDirectivePayload.fieldValue2 = filterObj != undefined ? filterObj.Value2 : null;
                                numberFilterDirectivePayload.includeValues = filterObj != undefined ? filterObj.IncludeValues : null;
                            }
                            VRUIUtilsService.callDirectiveLoad(numberFilterEditorAPI, numberFilterDirectivePayload, numberFilterLoadDeferred);
                        });

                        promises.push(numberFilterLoadDeferred.promise);

                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.GenericData.Entities.NumberRecordFilter, Vanrise.GenericData.Entities",
                        CompareOperator: $scope.selectedFilter.value
                    };

                    var numberFilterValues = numberFilterEditorAPI.getData();
                    if (numberFilterValues != undefined) {
                        data.Value = numberFilterValues.value;
                        data.Value2 = numberFilterValues.value2;
                        data.IncludeValues = numberFilterValues.includeValues;
                    }

                    return data;
                };

                api.getExpression = function () {
                    return $scope.selectedFilter.description + ' ' + numberFilterEditorA.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);