'use strict';
app.directive('vrGenericdataFieldtypeChoicesRulefiltereditor', ['VR_GenericData_ListRecordFilterOperatorEnum', 'UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Choices/Templates/ChoicesFieldTypeRuleFilterEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            var selectedObj;
            var choiceFilterEditorApi;
            var choiceFilterReadyDeferred = UtilsService.createPromiseDeferred();

            var filterObj;
            $scope.onChoiceFilterEditorReady = function (api) {
                choiceFilterEditorApi = api;
                choiceFilterReadyDeferred.resolve();
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

                        var choiceFilterLoadDeferred = UtilsService.createPromiseDeferred();

                        choiceFilterReadyDeferred.promise.then(function () {
                            var payload = { fieldType: selectedObj.Type, fieldValue: filterObj != undefined ? filterObj.Values : null };
                            VRUIUtilsService.callDirectiveLoad(choiceFilterEditorApi, payload, choiceFilterLoadDeferred);
                        });
                        promises.push(choiceFilterLoadDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.NumberListRecordFilter, Vanrise.GenericData.Entities",
                        CompareOperator: $scope.selectedFilter.value,
                        Values: choiceFilterEditorApi.getData()
                    };
                };

                api.getExpression = function () {
                    var ids = choiceFilterEditorApi.getData();
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

