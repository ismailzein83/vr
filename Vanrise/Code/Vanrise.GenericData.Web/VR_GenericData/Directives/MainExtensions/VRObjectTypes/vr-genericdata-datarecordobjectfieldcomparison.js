(function (app) {

    'use strict';

    VRDataRecordObjectFieldComparison.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService', 'VR_GenericData_DataRecordFieldComparisonOperatorEnum'];

    function VRDataRecordObjectFieldComparison(UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_DataRecordFieldComparisonOperatorEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DataRecordObjectFieldCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/VRObjectTypes/Templates/VRDataRecordObjectFieldComparisonTemplate.html"

        };
        function DataRecordObjectFieldCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var sourceDataRecordTypeFieldSelectorAPI;
            var sourceDataRecordTypeFieldSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

            var targetDataRecordTypeFieldSelectorAPI;
            var targetDataRecordTypeFieldSelectorAPIReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSourceDataRecordTypeFieldSelectorReady = function (api) {
                    sourceDataRecordTypeFieldSelectorAPI = api;
                    sourceDataRecordTypeFieldSelectorAPIReadyDeferred.resolve();
                };

                $scope.scopeModel.onTargetDataRecordTypeFieldSelectorReady = function (api) {
                    targetDataRecordTypeFieldSelectorAPI = api;
                    targetDataRecordTypeFieldSelectorAPIReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var dataRecordTypeId;
                    var objectPropertyEvaluator, sourceFieldName, targetFieldName, operator;

                    if (payload != undefined) {
                        dataRecordTypeId = payload.objectType != undefined ? payload.objectType.RecordTypeId : undefined;
                        objectPropertyEvaluator = payload.objectPropertyEvaluator != undefined ? payload.objectPropertyEvaluator : undefined;

                        if (objectPropertyEvaluator != undefined) {
                            sourceFieldName = objectPropertyEvaluator.SourceFieldName,
                            targetFieldName = objectPropertyEvaluator.TargetFieldName,
                            operator = objectPropertyEvaluator.Operator;
                        }
                    }

                    //Loading SourceDataRecordTypeField selector
                    var sourceDataRecordTypeFieldSelectorLoadPromise = getSourceDataRecordTypeFieldSelectorLoadPromise();
                    promises.push(sourceDataRecordTypeFieldSelectorLoadPromise);

                    //Loading TargetDataRecordTypeField selector
                    var targetDataRecordTypeFieldSelectorLoadPromise = getTargetDataRecordTypeFieldSelectorLoadPromise();
                    promises.push(targetDataRecordTypeFieldSelectorLoadPromise);

                    //Loading Condition selector
                    $scope.scopeModel.conditions = UtilsService.getArrayEnum(VR_GenericData_DataRecordFieldComparisonOperatorEnum);
                    if (operator != undefined) {
                        $scope.scopeModel.selectedCondition = UtilsService.getItemByVal($scope.scopeModel.conditions, operator, 'value');
                    }


                    function getSourceDataRecordTypeFieldSelectorLoadPromise() {

                        var sourceDataRecordObjectTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        sourceDataRecordTypeFieldSelectorAPIReadyDeferred.promise.then(function () {

                            var sourceDataRecordObjectTypeSelectorPayload = {
                                dataRecordTypeId: dataRecordTypeId
                            };
                            if (sourceFieldName != undefined) {
                                sourceDataRecordObjectTypeSelectorPayload.selectedIds = sourceFieldName;
                            }
                            VRUIUtilsService.callDirectiveLoad(sourceDataRecordTypeFieldSelectorAPI, sourceDataRecordObjectTypeSelectorPayload, sourceDataRecordObjectTypeSelectorLoadDeferred);
                        });

                        return sourceDataRecordObjectTypeSelectorLoadDeferred.promise;
                    }
                    function getTargetDataRecordTypeFieldSelectorLoadPromise() {

                        var targetDataRecordObjectTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        targetDataRecordTypeFieldSelectorAPIReadyDeferred.promise.then(function () {

                            var targetDataRecordObjectTypeSelectorPayload = {
                                dataRecordTypeId: dataRecordTypeId
                            };
                            if (targetFieldName != undefined) {
                                targetDataRecordObjectTypeSelectorPayload.selectedIds = targetFieldName;
                            }
                            VRUIUtilsService.callDirectiveLoad(targetDataRecordTypeFieldSelectorAPI, targetDataRecordObjectTypeSelectorPayload, targetDataRecordObjectTypeSelectorLoadDeferred);
                        });

                        return targetDataRecordObjectTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises)
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldComparisonPropertyEvaluator, Vanrise.GenericData.MainExtensions",
                        SourceFieldName: sourceDataRecordTypeFieldSelectorAPI.getSelectedIds(),
                        TargetFieldName: targetDataRecordTypeFieldSelectorAPI.getSelectedIds(),
                        Operator: $scope.scopeModel.selectedCondition.value
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordobjectfieldcomparison', VRDataRecordObjectFieldComparison);

})(app);