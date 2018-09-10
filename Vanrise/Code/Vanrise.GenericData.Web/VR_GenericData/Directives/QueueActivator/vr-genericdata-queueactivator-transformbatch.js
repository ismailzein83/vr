(function (app) {

    'use strict';

    QueueActivatorTransformBatch.$inject = ['VR_GenericData_DataTransformationDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function QueueActivatorTransformBatch(VR_GenericData_DataTransformationDefinitionAPIService, UtilsService, VRUIUtilsService) {
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
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/QueueActivator/Templates/QueueActivatorTransformBatchTemplate.html';
            }
        };

        function QueueingTransformBatchQueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var dataTransformationSelectorAPI;
            var gridAPI;
            var receivedQueueActivator;
            var isEditMode;
            var dataTransformationRecordSelectorPromise;
            var dataTransformationDefinitionId;
            var dataRecordTypeId;
            var secondtimeChanged = false;
            var existingStages;
            var transformationRecords = {};

            $scope.nextRecords = [];
            $scope.selectedTransformationRecord = [];
            $scope.transformationRecords = [];
            $scope.showTransformationRecordSelector = false;
            $scope.scopeModal = {};

            function initializeController() {
                var isTriggered = false;
                ctrl.onDataTransformationSelectorReady = function (api) {
                    if (isTriggered)
                        return;
                    isTriggered = true;
                    dataTransformationSelectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };



                $scope.onSelectedDataTransformationChanged = function (value) {

                    if (value != undefined) {
                        dataTransformationDefinitionId = value.DataTransformationDefinitionId;
                        if (dataTransformationRecordSelectorPromise != undefined) {
                            dataTransformationRecordSelectorPromise.resolve();

                        }
                        else {
                            $scope.scopeModal.isLoadingTransformActivatorSection = true;
                            loadDataTransformationRecordSelector().then(function () {
                                loadNextRecordsSection();
                                $scope.scopeModal.isLoadingTransformActivatorSection = false;
                            })
                        }
                    }
                    else
                        $scope.showTransformationRecordSelector = false;
                };

            }


            $scope.ongridReady = function (api) {
                gridAPI = api;
            };

            function loadDataTransformationRecordSelector() {
                if (dataTransformationDefinitionId != undefined) {
                    $scope.showTransformationRecordSelector = true;
                    return VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationDefinitionRecords(dataTransformationDefinitionId).then(function (transformationRecordsResponse) {
                        var arrayTransformationRecords = [];
                        var transformationRecord;
                        $scope.nextRecords.length = 0;
                        $scope.transformationRecords.length = 0;
                        for (var i = 0; i < transformationRecordsResponse.length; i++) {
                            transformationRecord = transformationRecordsResponse[i];
                            if (transformationRecord.DataRecordTypeId == dataRecordTypeId && transformationRecord.IsArray) {
                                $scope.transformationRecords.push(transformationRecord);
                            }
                            if (transformationRecord.IsArray) {
                                arrayTransformationRecords.push(transformationRecord);
                            }
                        }
                        transformationRecords = arrayTransformationRecords;
                    })
                }
            }


            function loadNextRecordsSection() {
                var dataItem;
                var transformationRecord;
                $scope.nextRecords.length = 0;
                if (transformationRecords != undefined) {
                    for (var i = 0; i < transformationRecords.length; i++) {
                        transformationRecord = transformationRecords[i];
                        dataItem = {};
                        dataItem.RecordName = transformationRecord.RecordName;
                        dataItem.nextStages = [];
                        for (var j = 0; j < existingStages.length; j++) {
                            if (existingStages[j].DataRecordTypeId == transformationRecord.DataRecordTypeId)
                                dataItem.nextStages.push({ stageName: existingStages[j].stageName });
                        }
                        dataItem.selectedStages = transformationRecord.NextStages != undefined ? transformationRecord.NextStages : [];
                        $scope.nextRecords.push(dataItem);
                    }
                }
                if (isEditMode) {
                    var selectedRecord;
                    if ($scope.nextRecords != undefined) {
                        for (var i = 0; i < $scope.nextRecords.length; i++) {
                            if (receivedQueueActivator.NextStagesRecords != undefined) {
                                selectedRecord = UtilsService.getItemByVal(receivedQueueActivator.NextStagesRecords, $scope.nextRecords[i].RecordName, "RecordName");
                                if (selectedRecord != undefined && selectedRecord.NextStages != undefined) {
                                    for (var j = 0; j < selectedRecord.NextStages.length; j++) {
                                        var selectedStage = UtilsService.getItemByVal($scope.nextRecords[i].nextStages, selectedRecord.NextStages[j], "stageName");
                                        if (selectedStage != null)
                                            $scope.nextRecords[i].selectedStages.push(selectedStage);
                                    }
                                }
                            }
                        }
                    }
                }

            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var existingDataTransformationId;
                    var promises = [];
                    if (payload != undefined && payload.ExistingStages != undefined) {
                        existingStages = payload.ExistingStages;
                    }

                    if (payload != undefined && payload.DataRecordTypeId != undefined)
                        dataRecordTypeId = payload.DataRecordTypeId;


                    if (payload != undefined && payload.QueueActivator != undefined) {
                        isEditMode = true;
                        receivedQueueActivator = payload.QueueActivator;
                        existingDataTransformationId = payload.QueueActivator.DataTransformationDefinitionId;
                        dataTransformationRecordSelectorPromise = UtilsService.createPromiseDeferred();
                        promises.push(dataTransformationRecordSelectorPromise.promise);
                    }

                    var loadDataTransformationSelectorPromise = loadDataTransformationSelector();
                    promises.push(loadDataTransformationSelectorPromise);


                    loadDataTransformationSelectorPromise.then(function () {
                        if (payload != undefined && payload.QueueActivator != undefined) {
                            dataTransformationRecordSelectorPromise.promise.then(function () {
                                dataTransformationRecordSelectorPromise = undefined;
                                var dataTransformationRecordSelectorPromise = loadDataTransformationRecordSelector();
                                promises.push(dataTransformationRecordSelectorPromise);
                                dataTransformationRecordSelectorPromise.then(function () {
                                    var selectedValue = UtilsService.getItemByVal($scope.transformationRecords, payload.QueueActivator.SourceRecordName, "RecordName");
                                    $scope.selectedTransformationRecord = selectedValue;
                                    loadNextRecordsSection();
                                });
                            });
                        }

                    });

                    return UtilsService.waitMultiplePromises(promises);


                    function loadDataTransformationSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload = { selectedIds: existingDataTransformationId };
                        VRUIUtilsService.callDirectiveLoad(dataTransformationSelectorAPI, selectorPayload, selectorLoadDeferred);
                        return selectorLoadDeferred.promise;
                    }
                };


                api.getData = function () {
                    var NextStagesRecords = [];
                    var selectedNextStages;
                    for (var i = 0; i < $scope.nextRecords.length; i++) {
                        selectedNextStages = [];
                        var nextStage = $scope.nextRecords[i];
                        if (nextStage.selectedStages.length > 0) {
                            for (var j = 0; j < nextStage.selectedStages.length; j++) {
                                selectedNextStages.push(nextStage.selectedStages[j].stageName);
                            }

                            NextStagesRecords.push({ RecordName: nextStage.RecordName, NextStages: selectedNextStages });
                        }
                    }
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.TransformBatchQueueActivator, Vanrise.GenericData.QueueActivators',
                        DataTransformationDefinitionId: dataTransformationSelectorAPI.getSelectedIds(),
                        SourceRecordName: $scope.selectedTransformationRecord.RecordName,
                        NextStagesRecords: NextStagesRecords.length > 0 ? NextStagesRecords : undefined
                    };
                };

                return api;
            }
        }
    }

    app.directive('vrGenericdataQueueactivatorTransformbatch', QueueActivatorTransformBatch);

})(app);