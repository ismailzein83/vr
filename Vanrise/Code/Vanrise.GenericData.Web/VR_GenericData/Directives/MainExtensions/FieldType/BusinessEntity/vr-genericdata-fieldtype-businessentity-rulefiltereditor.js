'use strict';
app.directive('vrGenericdataFieldtypeBusinessentityRulefiltereditor', ['VR_GenericData_ListRecordFilterOperatorEnum', 'UtilsService','VRUIUtilsService',
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/BusinessEntity/Templates/BusinessEntityFieldTypeRuleFilterEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            var businessEntityApi;
            var selectedObj;
            var filterObj;
            var businessEntityReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.onBusinessEntityReady = function (api) {
                businessEntityApi = api;
                businessEntityReadyDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_ListRecordFilterOperatorEnum);
                    $scope.selectedFilter = $scope.filters[0];
                    if (payload) {
                        selectedObj = payload.dataRecordTypeField;
                        if (payload.filterObj) {
                            $scope.selectedFilter = UtilsService.getItemByVal($scope.filters, payload.filterObj.CompareOperator, 'value');
                            filterObj = payload.filterObj;
                        }
                        var promises = [];

                        var businessEntityLoadDeferred = UtilsService.createPromiseDeferred();

                        businessEntityReadyDeferred.promise.then(function () {
                            var payload = { fieldType: selectedObj.Type, fieldValue: filterObj != undefined ? filterObj.Values : undefined, fieldTitle: selectedObj.FieldTitle };
                            VRUIUtilsService.callDirectiveLoad(businessEntityApi, payload, businessEntityLoadDeferred);
                        });
                        promises.push(businessEntityLoadDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }


                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.ObjectListRecordFilter, Vanrise.GenericData.Entities",
                        CompareOperator: $scope.selectedFilter.value,
                        Values: businessEntityApi.getData()
                    };
                };

                api.getExpression = function () {
                    var ids = businessEntityApi.getData();
                    var expression = $scope.selectedFilter.description + ' (' + ids[0];
                    if (ids.length > 1) {
                        for (var x = 1; x < ids.length; x++) {
                            expression += ', ' + ids[x];
                        }
                    }
                    expression += ')';
                    return expression;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

