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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/BusinessEntity/Templates/DataRecordTypeFieldBusinessEntityEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            var businessEntityApi;
            var selectedObj;
            var filterObj;
            $scope.onBusinessEntityReady = function (api) {
                var payload = { fieldType: selectedObj.Entity.Type, fieldValue: filterObj != undefined ? filterObj.Values : null };
                api.load(payload);
                businessEntityApi = api;
            }

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_ListRecordFilterOperatorEnum);
                    if (payload) {
                        selectedObj = payload.dataRecordTypeField;
                        if (payload.filterObj) {
                            $scope.selectedFilter = UtilsService.getItemByVal($scope.filters, payload.filterObj.CompareOperator, 'value');
                            filterObj = payload.filterObj;
                        }
                    }
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.NumberListRecordFilter, Vanrise.GenericData.Entities",
                        CompareOperator: $scope.selectedFilter.value,
                        Values: businessEntityApi.getData().BusinessEntityIds
                    };
                }

                api.getExpression = function () {
                    var ids = businessEntityApi.getData().BusinessEntityIds;
                    var expression = $scope.selectedFilter.description + ' (' + ids[0];
                    if (ids.length > 1) {
                        for (var x = 1; x < ids.length; x++) {
                            expression += ', ' + ids[x];
                        }
                    }
                    expression += ')';
                    return expression;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

