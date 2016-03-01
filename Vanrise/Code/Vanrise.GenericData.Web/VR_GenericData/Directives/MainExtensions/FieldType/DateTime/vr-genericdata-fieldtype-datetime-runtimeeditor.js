'use strict';
app.directive('vrGenericdataFieldtypeDatetimeRuntimeeditor', ['UtilsService', 'VR_GenericData_DateTimeDataTypeEnum', function (UtilsService, VR_GenericData_DateTimeDataTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectionmode: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            $scope.scopeModel = {};

            var ctor = new dateTimeCtor(ctrl, $scope, $attrs);
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
        template: function (element, attrs) {
            return getDirectiveTemplate(attrs);
        }
    };

    function dateTimeCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            $scope.scopeModel.value;

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var fieldType;
                var fieldValue;

                if (payload != undefined) {
                    $scope.scopeModel.label = payload.fieldTitle;
                    fieldType = payload.fieldType;
                    fieldValue = payload.fieldValue;
                    var dataTypes = UtilsService.getArrayEnum(VR_GenericData_DateTimeDataTypeEnum);
                    $scope.scopeModel.fieldType = UtilsService.getItemByVal(dataTypes, fieldType.DataType, 'value');
                }
            
                if (fieldValue != undefined) {
                     $scope.scopeModel.value = fieldValue;
                }
            }

            api.getData = function () {
                var retVal = $scope.scopeModel.value;

                return retVal;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }

    function getDirectiveTemplate(attrs) {
        return '<vr-columns colnum="{{ctrl.normalColNum}}" ng-if="scopeModel.fieldType !=undefined && scopeModel.label != undefined ">'
                     + '<vr-label>{{scopeModel.label}}</vr-label>'
                    + '<vr-directivewrapper  directive="\'vr-datetimepicker\'" on-ready="scopeModel.onDateTimeReady" type="{{scopeModel.fieldType.type}}" value="scopeModel.value"></vr-directivewrapper>'
                + '</vr-columns>';
    }

    return directiveDefinitionObject;
}]);

