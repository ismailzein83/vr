'use strict';
app.directive('vrGenericdataFieldtypeTextRulefiltereditor', ['VR_GenericData_StringRecordFilterOperatorEnum', 'UtilsService','VRUIUtilsService',
    function (VR_GenericData_StringRecordFilterOperatorEnum, UtilsService, VRUIUtilsService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Text/Templates/TextFieldTypeRuleFilterEditor.html"

        };

        function recordTypeFieldItemEditorCtor(ctrl, $scope, $attrs) {
            function initializeController() {
                defineAPI();
            }
            var textFilterEditorApi;
            var textFilterReadyDeferred = UtilsService.createPromiseDeferred();
            var filterObj;
            var dataRecordTypeField;
            $scope.onTextFilterEditorReady = function (api) {
                textFilterEditorApi = api;
                textFilterReadyDeferred.resolve();
            };
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.filters = UtilsService.getArrayEnum(VR_GenericData_StringRecordFilterOperatorEnum);
                    $scope.selectedFilter = $scope.filters[0];
                    if (payload && payload.filterObj) {
                        filterObj = payload.filterObj;
                        dataRecordTypeField = payload.dataRecordTypeField;
                        $scope.selectedFilter = UtilsService.getItemByVal($scope.filters, payload.filterObj.CompareOperator, 'value');
                        var promises = [];

                        var textFilterLoadDeferred = UtilsService.createPromiseDeferred();

                        textFilterReadyDeferred.promise.then(function () {
                            var payload = { fieldType: dataRecordTypeField != undefined ? dataRecordTypeField.Type : null, fieldValue: filterObj != undefined ? filterObj.Value : null };
                            VRUIUtilsService.callDirectiveLoad(textFilterEditorApi, payload, textFilterLoadDeferred);
                        });
                        promises.push(textFilterLoadDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.StringRecordFilter, Vanrise.GenericData.Entities",
                        CompareOperator: $scope.selectedFilter.value,
                        Value: textFilterEditorApi.getData()
                    };
                };

                api.getExpression = function () {
                    return $scope.selectedFilter.description + ' ' + textFilterEditorApi.getData();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

