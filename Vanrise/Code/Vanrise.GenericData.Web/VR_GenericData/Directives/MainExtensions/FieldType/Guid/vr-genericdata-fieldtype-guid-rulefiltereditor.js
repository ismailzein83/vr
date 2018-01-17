'use strict';
app.directive('vrGenericdataFieldtypeGuidRulefiltereditor', ['VR_GenericData_StringRecordFilterOperatorEnum', 'UtilsService',
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Guid/Templates/GuidFieldTypeRuleFilterEditor.html"
        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload && payload.filterObj) {
                        $scope.guidValue = payload.filterObj.GuidValue;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.GuidRecordFilter, Vanrise.GenericData.Entities",
                        GuidValue: $scope.guidValue

                    };
                };

                api.getExpression = function () {
                    return $scope.guidValue;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

