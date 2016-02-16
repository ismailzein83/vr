﻿(function (app) {

    'use strict';

    QueueingStoreBatchQueueActivator.$inject = ['VR_GenericData_DataTransformationDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function QueueingStoreBatchQueueActivator(VR_GenericData_DataTransformationDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var queueingTransformBatchQueueActivatorCtor = new QueueingTransformBatchQueueActivatorCtor(ctrl, $scope);
                queueingTransformBatchQueueActivatorCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Queueing/Directives/QueueActivatorConfig/Templates/TransformBatchQueueActivatorTemplate.html';
            }
        };

        function QueueingTransformBatchQueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var selectorAPI;
            var gridAPI;
            var dataGridDataSource;
            var dataTransformationSelectorReadyPromiseDeferred;
            var dataTransformationRecordSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataTransformationDefinitionId;
            var secondtimeChanged = false;

            $scope.recordTypesWithStages = [];
            $scope.selectedDataRecordStorage = [];
            $scope.transformationRecordTypes = [];
            $scope.showTransformationRecordTypesSelector = false;
            $scope.stagesDataSource = [];

            function initializeController() {
                var isTriggered = false;
                ctrl.onSelectorReady = function (api) {
                    if (isTriggered)
                        return;
                    isTriggered = true;
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };



                $scope.onSelectedDataTransformationChanged = function () {

                    if (secondtimeChanged)
                    {
                        if ($scope.selectedDataTransformation != undefined) {
                            dataTransformationDefinitionId = $scope.selectedDataTransformation.DataTransformationDefinitionId;
                            loadDataTransformationRecordSelector();
                        }
                        else
                            $scope.showTransformationRecordTypesSelector = false;
                        secondtimeChanged = false;
                    }
                    else
                    {
                        secondtimeChanged = true;
                    }
                 
                   
                }

            }

            $scope.ongridReady = function (api) {
                gridAPI = api;
            };

            function loadDataTransformationRecordSelector() {
                if (dataTransformationDefinitionId != undefined) {
                    VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationDefinitionRecords(dataTransformationDefinitionId).then(function (response) {
                        $scope.recordTypesWithStages.length=0;
                        $scope.transformationRecordTypes.length = 0;
                        $scope.selectedDataRecordStorage = [];
                        dataGridDataSource = response;
                        loadDataGrid(dataGridDataSource);
                        for (var i = 0; i < response.length; i++) {
                            $scope.transformationRecordTypes.push(response[i]);
                        }
                        dataTransformationRecordSelectorReadyPromiseDeferred.resolve();
                    })

                    $scope.showTransformationRecordTypesSelector = true;
                }
            }



            function loadDataGrid(response) {
                var dataItem;
                $scope.recordTypesWithStages.length = 0;
                for (var i = 0; i < response.length; i++) {
                    dataItem = {};
                    dataItem.RecordName = response[i].RecordName;
                    dataItem.selectedStages =response[i].NextStages!=undefined? response[i].NextStages: [];
                    $scope.recordTypesWithStages.push(dataItem);
                }

            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedId;

                    if (payload != undefined && payload.stagesDataSource != undefined) {
                        $scope.stagesDataSource = [];
                        $scope.stagesDataSource = payload.stagesDataSource;
                    }


                    if (payload != undefined && payload.QueueActivator != undefined) {
                        selectedId = payload.QueueActivator.DataTransformationDefinitionId;
                        dataTransformationSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    }

                    loadSelector();

                    if (payload != undefined && payload.QueueActivator != undefined) {
                        dataTransformationSelectorReadyPromiseDeferred.promise.then(function () {
                            dataTransformationRecordSelectorReadyPromiseDeferred.promise.then(function () {
                                if (payload != undefined && payload.QueueActivator != undefined) {
                                    var selectedValue = UtilsService.getItemByVal($scope.transformationRecordTypes, payload.QueueActivator.SourceRecordName, "RecordName");
                                    $scope.selectedDataRecordStorage = selectedValue;
                                    loadDataGridInEditMode();
                                }
                            })
                        });
                    }

                    function loadDataGridInEditMode() {
                        for (var i = 0; i < $scope.recordTypesWithStages.length; i++) {
                            for (var j = 0; j < payload.QueueActivator.NextStagesRecords[i].NextStages.length; j++) {
                                var selectedValue = UtilsService.getItemByVal($scope.stagesDataSource, payload.QueueActivator.NextStagesRecords[i].NextStages[j], "stageName");
                                if (selectedValue != null)
                                    $scope.recordTypesWithStages[i].selectedStages.push(selectedValue);
                            }
                        }
                    }


                    function loadSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload = { selectedIds: selectedId };
                        VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
                        if (selectedId != undefined)
                            dataTransformationSelectorReadyPromiseDeferred.resolve();
                        return selectorLoadDeferred.promise;
                    }
                }


                api.getData = function () {
                    var NextStagesRecords = [];
                    var nextStages;
                    for (var i = 0; i < $scope.recordTypesWithStages.length; i++) {
                        nextStages = [];
                        for (var j = 0; j < $scope.recordTypesWithStages[i].selectedStages.length; j++) {

                            nextStages.push($scope.recordTypesWithStages[i].selectedStages[j].stageName);
                        }

                        NextStagesRecords.push({ RecordName: $scope.recordTypesWithStages[i].RecordName, NextStages: nextStages });
                    }
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.TransformBatchQueueActivator, Vanrise.GenericData.QueueActivators',
                        DataTransformationDefinitionId: selectorAPI.getSelectedIds(),
                        SourceRecordName: $scope.selectedDataRecordStorage.RecordName,
                        NextStagesRecords: NextStagesRecords
                    };
                }

                return api;
            }
        }
    }

    app.directive('vrQueueingTransformBatchqueueactivator', QueueingStoreBatchQueueActivator);

})(app);