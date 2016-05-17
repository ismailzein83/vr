'use strict';
app.directive('vrGenericdataDatarecordtypefieldRulefilter', ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_ConditionEnum',
    function (VR_GenericData_DataRecordTypeAPIService, UtilsService, VRUIUtilsService, VR_GenericData_ConditionEnum) {

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
            $scope.scopeModel = {};
            $scope.scopeModel.dataRecordTypeField;
            var dataRecordTypeFieldEditorApi;

            var context;
            var filterObj;



            function initializeController() {
               
                $scope.scopeModel.onDataRecordTypeFieldEditorReady = function (api) {
                    dataRecordTypeFieldEditorApi = api;
                    var payload = { dataRecordTypeField: $scope.scopeModel.dataRecordTypeField, filterObj: filterObj };
                    api.load(payload);
                }

                $scope.scopeModel.onDataRecordTypeFieldFilterReady = function (api) {
                    dataRecordTypeFieldEditorApi = api;
                    api.load();
                }
                $scope.scopeModel.osnDataRecordTypeFieldSelectionChanged = function()
                {
                    if (context != undefined && $scope.scopeModel.dataRecordTypeField !=undefined)
                    {
                        $scope.scopeModel.ruleFilterEditor = context.getRuleEditor($scope.scopeModel.dataRecordTypeField.Type.ConfigId);
                    }
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                   
                    if (payload != undefined) {
                        $scope.scopeModel.conditions = UtilsService.getArrayEnum(VR_GenericData_ConditionEnum);
                        $scope.scopeModel.selectedCondition = $scope.scopeModel.conditions[0];
                        context = payload.context;
                        $scope.scopeModel.datasource = context.getFields();
                        $scope.scopeModel.dataRecordTypeField = $scope.scopeModel.datasource[0];
                        filterObj = payload.filterObj;

                        if (filterObj) {
                            $scope.scopeModel.selectedCondition = UtilsService.getItemByVal($scope.scopeModel.conditions, filterObj.$type, 'type');
                            if (!$scope.scopeModel.selectedCondition) {
                                $scope.scopeModel.selectedCondition = UtilsService.getItemByVal($scope.scopeModel.conditions, '', 'type');
                            }
                            $scope.scopeModel.dataRecordTypeField = UtilsService.getItemByVal($scope.scopeModel.datasource, filterObj.FieldName, 'FieldName');
                        }
                    }
                }

                api.getData = function () {
                    var obj = dataRecordTypeFieldEditorApi.getData();
                    obj.FieldName = $scope.scopeModel.dataRecordTypeField.FieldName;
                    return obj;
                }

                api.getExpression = function () {
                    return $scope.scopeModel.dataRecordTypeField.FieldTitle + ' ' + dataRecordTypeFieldEditorApi.getExpression();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

