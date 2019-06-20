'use strict';
app.directive('vrGenericdataFieldtypeGuidRulefiltereditor', ['VR_GenericData_StringRecordFilterOperatorEnum', 'UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Guid/Templates/GuidFieldTypeRuleFilterEditor.html"

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
                    var promises = [];
                    if (payload) {
                        dataRecordTypeField = payload.dataRecordTypeField;
                        filterObj = payload.filterObj;
                        if (filterObj != undefined) {
                            $scope.selectedFilter = UtilsService.getItemByVal($scope.filters, filterObj.CompareOperator, 'value');
                        }
                        var textFilterLoadDeferred = UtilsService.createPromiseDeferred();

                        textFilterReadyDeferred.promise.then(function () {
                            var textFilterPayload = {
                                fieldType: dataRecordTypeField != undefined ? dataRecordTypeField.Type : undefined,
                                fieldValue: filterObj != undefined ? filterObj.Value : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(textFilterEditorApi, textFilterPayload, textFilterLoadDeferred);
                        });
                        promises.push(textFilterLoadDeferred.promise);
                    }
                    return UtilsService.waitMultiplePromises(promises);

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

