'use strict';
app.directive('vrGenericdataFieldtypeDatetimeRulefiltereditor', ['VR_GenericData_DateTimeRecordFilterOperatorEnum', 'UtilsService','VRUIUtilsService',
    function (VR_GenericData_DateTimeRecordFilterOperatorEnum, UtilsService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DateTime/Templates/DateTimeFieldTypeRuleFilterEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            var dateFilterEditorApi;
            var dateFilterReadyDeferred = UtilsService.createPromiseDeferred();

            var selectedObj;
            var filterObj;

            function initializeController() {
                defineAPI();
            }

            $scope.onDateFilterEditorReady = function (api) {
                dateFilterEditorApi = api;
                dateFilterReadyDeferred.resolve();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_DateTimeRecordFilterOperatorEnum);
                    $scope.selectedFilter = $scope.filters[0];

                    if (payload) {
                        selectedObj = payload.dataRecordTypeField;
                        if (payload.filterObj) {
                            $scope.selectedFilter = UtilsService.getItemByVal($scope.filters, payload.filterObj.CompareOperator, 'value');
                            filterObj = payload.filterObj;
                        }
                        var promises = [];

                        var dateFilterLoadDeferred = UtilsService.createPromiseDeferred();

                        dateFilterReadyDeferred.promise.then(function () {
                            var payload = { fieldType: selectedObj.Type, fieldValue: filterObj != undefined ? filterObj.Value : null, fieldTitle: 'Date' };
                            VRUIUtilsService.callDirectiveLoad(dateFilterEditorApi, payload, dateFilterLoadDeferred);
                        });
                        promises.push(dateFilterLoadDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }

                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.DateTimeRecordFilter, Vanrise.GenericData.Entities",
                        CompareOperator: $scope.selectedFilter.value,
                        Value: dateFilterEditorApi.getData()
                    };
                };

                api.getExpression = function () {
                    return $scope.selectedFilter.description + ' ' + dateFilterEditorApi.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

