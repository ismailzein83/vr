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
            var dataRecordTypeFieldEditorApi;

            var dataRecordTypeId;
            var context;
            var filterObj;

            $scope.onDataRecordTypeFieldEditorReady = function (api) {
                dataRecordTypeFieldEditorApi = api;
                var payload = { dataRecordTypeField: $scope.dataRecordTypeField, filterObj: filterObj };
                api.load(payload);
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
                        filterObj = payload.filterObj;

                        if (filterObj) {
                            $scope.dataRecordTypeField = UtilsService.getItemByVal($scope.datasource, filterObj.FieldName, 'Entity.Name');
                        }
                    }
                }

                api.getData = function () {
                    var obj = dataRecordTypeFieldEditorApi.getData();
                    obj.FieldName = $scope.dataRecordTypeField.Entity.Name;
                    return obj;
                }

                api.getExpression = function () {
                    return $scope.dataRecordTypeField.Entity.Name + ' ' + dataRecordTypeFieldEditorApi.getExpression();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

