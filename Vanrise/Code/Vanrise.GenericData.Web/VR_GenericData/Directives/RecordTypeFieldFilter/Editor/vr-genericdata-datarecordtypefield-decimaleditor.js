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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Editor/Templates/DataRecordTypeFieldDecimalEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_NumberRecordFilterOperatorEnum);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

