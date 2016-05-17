﻿'use strict';
app.directive('vrGenericdataFieldtypeNumberRulefiltereditor', ['VR_GenericData_NumberRecordFilterOperatorEnum', 'UtilsService',
    function (VR_GenericData_NumberRecordFilterOperatorEnum, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Number/Templates/NumberFieldTypeRuleFilterEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            function initializeController() {
                defineAPI();
            }
            var numberFilterEditorApi;
            var filterObj;
            var dataRecordTypeField;

            $scope.onNumberFilterEditorReady = function (api) {
                numberFilterEditorApi = api;
                var payload = { fieldType: dataRecordTypeField != undefined ? dataRecordTypeField.Type : null, fieldValue: filterObj != undefined ? filterObj.Value : null };
                api.load(payload);
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
                    }
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.NumberRecordFilter, Vanrise.GenericData.Entities",
                        CompareOperator: $scope.selectedFilter.value,
                        Value: numberFilterEditorApi.getData()
                    };
                }

                api.getExpression = function () {
                    return $scope.selectedFilter.description + ' ' + numberFilterEditorApi.getData();
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

