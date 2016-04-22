'use strict';
app.directive('vrGenericdataDatarecordtypefieldBooleditor', ['VR_GenericData_StringRecordFilterOperatorEnum', 'UtilsService',
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Editor/Templates/DataRecordTypeFieldBoolEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_StringRecordFilterOperatorEnum);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

