'use strict';
app.directive('vrGenericdataDatarecordtypefieldBusinessentityeditor', ['VR_GenericData_ListRecordFilterOperatorEnum', 'UtilsService',
    function (VR_GenericData_ListRecordFilterOperatorEnum, UtilsService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Editor/Templates/DataRecordTypeFieldBusinessEntityEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            $scope.businessEntitySelector;

            $scope.onBusinessEntitySelectorReady = function (api) {
                api.load();
            }

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_ListRecordFilterOperatorEnum);
                    $scope.businessEntitySelector = payload.selector;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

