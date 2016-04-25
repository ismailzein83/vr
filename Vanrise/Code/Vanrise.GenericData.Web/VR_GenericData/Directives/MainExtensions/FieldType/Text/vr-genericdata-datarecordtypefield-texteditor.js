'use strict';
app.directive('vrGenericdataDatarecordtypefieldTexteditor', ['VR_GenericData_StringRecordFilterOperatorEnum', 'UtilsService',
    function (VR_GenericData_StringRecordFilterOperatorEnum, UtilsService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Text/Templates/DataRecordTypeFieldTextEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            function initializeController() {
                defineAPI();
            }
            var textFilterEditorApi;
            $scope.onTextFilterEditorReady = function (api) {
                textFilterEditorApi = api;
                textFilterEditorApi.load();
            }
            function defineAPI() {
                var api = {};

                api.load = function () {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_StringRecordFilterOperatorEnum);
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.StringRecordFilter, Vanrise.GenericData.Entities",
                        CompareOperator: $scope.selectedFilter.value,
                        Value: textFilterEditorApi.getData()
                    };
                }

                api.getExpression = function () {
                    return $scope.selectedFilter.description + ' ' + textFilterEditorApi.getData();
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

