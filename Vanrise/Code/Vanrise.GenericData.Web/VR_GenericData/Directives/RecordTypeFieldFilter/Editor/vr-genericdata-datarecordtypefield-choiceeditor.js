'use strict';
app.directive('vrGenericdataDatarecordtypefieldChoiceeditor', ['VR_GenericData_ListRecordFilterOperatorEnum', 'UtilsService', 'VRUIUtilsService',
    function (VR_GenericData_ListRecordFilterOperatorEnum, UtilsService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Editor/Templates/DataRecordTypeFieldChoiceEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            var selectedObj;


            $scope.onChoiceFilterEditorReady = function (api) {
                var payload = { fieldType: { Choices: selectedObj.values } };
                api.load(payload);
            }

            
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_ListRecordFilterOperatorEnum);
                    selectedObj = payload;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

