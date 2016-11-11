'use strict';
app.directive('vrGenericdataFieldtypeBooleanRulefiltereditor', ['VR_GenericData_StringRecordFilterOperatorEnum', 'UtilsService',
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Boolean/Templates/BooleanFieldTypeRuleFilterEditor.html"
        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload && payload.filterObj) {
                        $scope.isTrue = payload.filterObj.IsTrue;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.BooleanRecordFilter, Vanrise.GenericData.Entities",
                        IsTrue: $scope.isTrue

                    };
                };

                api.getExpression = function () {
                    return $scope.isTrue ? 'Is True' : 'Is False';
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

