'use strict';
app.directive('vrGenericdataDatarecordtypefieldDatetimeeditor', ['VR_GenericData_DateTimeRecordFilterOperatorEnum', 'UtilsService',
    function (VR_GenericData_DateTimeRecordFilterOperatorEnum, UtilsService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Editor/Templates/DataRecordTypeFieldDateTimeEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            $scope.date;
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_DateTimeRecordFilterOperatorEnum);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

