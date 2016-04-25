'use strict';
app.directive('vrGenericdataDatarecordtypefieldDecimaleditor', ['VR_GenericData_NumberRecordFilterOperatorEnum', 'UtilsService',
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Number/Templates/DataRecordTypeFieldDecimalEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            function initializeController() {
                defineAPI();
            }
            var numberFilterEditorApi;
            $scope.onNumberFilterEditorReady = function (api) {
                numberFilterEditorApi = api;
                api.load();
            }
            function defineAPI() {
                var api = {};

                api.load = function () {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_NumberRecordFilterOperatorEnum);
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

