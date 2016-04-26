'use strict';
app.directive('vrGenericdataFieldtypeTextRulefiltereditor', ['VR_GenericData_StringRecordFilterOperatorEnum', 'UtilsService',
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Text/Templates/TextFieldTypeRuleFilterEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            function initializeController() {
                defineAPI();
            }
            var textFilterEditorApi;
            var filterObj;
            var dataRecordTypeField;
            $scope.onTextFilterEditorReady = function (api) {
                textFilterEditorApi = api;
                var payload = { fieldType: dataRecordTypeField != undefined ? dataRecordTypeField.Entity.Type : null, fieldValue: filterObj != undefined ? filterObj.Value : null };
                textFilterEditorApi.load(payload);
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_StringRecordFilterOperatorEnum);
                    $scope.selectedFilter = $scope.filters[0];
                    if (payload && payload.filterObj) {
                        filterObj = payload.filterObj;
                        dataRecordTypeField = payload.dataRecordTypeField;
                        $scope.selectedFilter = UtilsService.getItemByVal($scope.filters, payload.filterObj.CompareOperator, 'value');
                    }
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

