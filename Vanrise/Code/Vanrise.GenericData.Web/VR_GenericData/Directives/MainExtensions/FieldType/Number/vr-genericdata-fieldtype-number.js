'use strict';
app.directive('vrGenericdataFieldtypeNumber', ['VR_GenericData_FieldNumberDataTypeEnum', 'UtilsService', function (VR_GenericData_FieldNumberDataTypeEnum, UtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            var ctor = new numberTypeCtor(ctrl, $scope);
            ctor.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {
            return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Number/Templates/NumberFieldTypeTemplate.html';
        }
    };

    function numberTypeCtor(ctrl, $scope) {

        var selectorAPI;

        function initializeController() {
            ctrl.numberDataTypes = [];

            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var dataTypeValue;

                if (payload != undefined) {
                    dataTypeValue = payload.DataType
                }

                selectorAPI.clearDataSource();
                ctrl.numberDataTypes = UtilsService.getArrayEnum(VR_GenericData_FieldNumberDataTypeEnum);

                if (dataTypeValue != undefined) {
                    ctrl.selectedNumberDataType = UtilsService.getItemByVal(ctrl.numberDataTypes, dataTypeValue, 'value');
                }
            };

            api.getData = function () {
                return {
                    $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType, Vanrise.GenericData.MainExtensions",
                    DataType: ctrl.selectedNumberDataType.value
                };
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(api);
            }
        }

        this.initializeController = initializeController;
    }
 }]);