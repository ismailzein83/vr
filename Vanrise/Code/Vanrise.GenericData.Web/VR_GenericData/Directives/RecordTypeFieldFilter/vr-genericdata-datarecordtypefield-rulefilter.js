'use strict';
app.directive('vrGenericdataDatarecordtypefieldRulefilter', ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_GenericData_DataRecordTypeAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new recordTypeFieldRuleFilterCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Templates/DataRecordTypeFieldRuleFilter.html"

        };

        function recordTypeFieldRuleFilterCtor(ctrl, $scope, $attrs) {
            $scope.dataRecordTypeField;

            var dataRecordTypeId;
            var context;
            $scope.onDataRecordTypeFieldEditorReady = function (api) {
                api.load($scope.dataRecordTypeField);
            }
            $scope.onSelectorReady = function (api) {
                var payload = { dataRecordTypeId: dataRecordTypeId };
                api.load(payload);
            }

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        dataRecordTypeId = payload.dataRecordTypeId;
                        context = payload.context;
                        $scope.datasource = context.getFields();
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

