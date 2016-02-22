(function (app) {

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
            var nextStages = {};

            $scope.nextStages = [];
            $scope.selectedTransformationRecordType = [];
            $scope.transformationRecordTypes = [];
            $scope.showTransformationRecordTypesSelector = false;
            $scope.stages = [];

            function initializeController() {
                var isTriggered = false;
                ctrl.onSelectorReady = function (api) {
                    if (isTriggered)
                        return;
                    isTriggered = true;
                    dataTransformationSelectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };



                $scope.onSelectedDataTransformationChanged = function () {

                    if ($scope.selectedDataTransformation != undefined) {
                        dataTransformationDefinitionId = $scope.selectedDataTransformation.DataTransformationDefinitionId;
                        if (isEditMode) {
                            dataTransformationRecordSelectorPromise.resolve();

                        }
                        else {
                            var dataTransformationRecordSelectorPromisee = loadDataTransformationRecordSelector();
                            dataTransformationRecordSelectorPromisee.then(function () {
                                loadDataGrid();
                            })
                        }
                    }
                    else
                        $scope.showTransformationRecordTypesSelector = false;
                }

            }


            $scope.ongridReady = function (api) {
                gridAPI = api;
            };

            function loadDataTransformationRecordSelector() {
                if (dataTransformationDefinitionId != undefined) {
                    $scope.showTransformationRecordTypesSelector = true;
                    return VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationDefinitionRecords(dataTransformationDefinitionId).then(function (response) {
                        var filteredResponse = [];
                        $scope.nextStages.length = 0;
                        $scope.transformationRecordTypes.length = 0;
                        for (var i = 0; i < response.length; i++) {
                            if (response[i].DataRecordTypeId == dataRecordTypeId && response[i].IsArray) {
                                $scope.transformationRecordTypes.push(response[i]);
                            }
                            if (response[i].IsArray) {
                                filteredResponse.push(response[i]);
                            }
                        }
                        nextStages = filteredResponse;
                    })
                }
            }


            function loadDataGrid() {
                var dataItem;
                $scope.nextStages.length = 0;
                for (var i = 0; i < nextStages.length; i++) {
                    dataItem = {};
                    dataItem.RecordName = nextStages[i].RecordName;
                    dataItem.existingStages = [];
                    for (var j = 0; j < existingStages.length; j++) {
                        if (existingStages[j].DataRecordTypeId == nextStages[i].DataRecordTypeId )
                            dataItem.existingStages.push({ stageName: existingStages[j].stageName });
                    }
                    dataItem.selectedStages = nextStages[i].NextStages != undefined ? nextStages[i].NextStages : [];
                    $scope.nextStages.push(dataItem);
                }

                if (isEditMode) {
                    for (var i = 0; i < $scope.nextStages.length; i++) {

                        for (var j = 0; j < receivedQueueActivator.NextStagesRecords.length; j++) {
                            if ($scope.nextStages[i].RecordName == receivedQueueActivator.NextStagesRecords[j].RecordName) {
                                for (var k = 0; k < receivedQueueActivator.NextStagesRecords[j].NextStages.length; k++) {
                                    var selectedValue = UtilsService.getItemByVal($scope.nextStages[i].existingStages, receivedQueueActivator.NextStagesRecords[j].NextStages[k], "stageName");
                                    if (selectedValue != null)
                                        $scope.nextStages[i].selectedStages.push(selectedValue);
                                }
                            }
                        }
                    }
                }

            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedId;
                    var promises = [];
                    if (payload != undefined && payload.ExistingStages != undefined) {
                        $scope.stages.length = 0;
                        existingStages = payload.ExistingStages;
                    }

                    if (payload != undefined && payload.DataRecordTypeId != undefined)
                        dataRecordTypeId = payload.DataRecordTypeId


                    if (payload != undefined && payload.QueueActivator != undefined) {
                        isEditMode = true;
                        receivedQueueActivator = payload.QueueActivator;
                        selectedId = payload.QueueActivator.DataTransformationDefinitionId;
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
                                    var selectedValue = UtilsService.getItemByVal($scope.transformationRecordTypes, payload.QueueActivator.SourceRecordName, "RecordName");
                                    $scope.selectedTransformationRecordType = selectedValue;
                                    loadDataGrid();
                                })
                            });
                        }

                    })

                    return UtilsService.waitMultiplePromises(promises);


                    function loadDataTransformationSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload = { selectedIds: selectedId };
                        VRUIUtilsService.callDirectiveLoad(dataTransformationSelectorAPI, selectorPayload, selectorLoadDeferred);
                        return selectorLoadDeferred.promise;
                    }
                }


                api.getData = function () {
                    var NextStagesRecords = [];
                    var selectedNextStages;
                    for (var i = 0; i < $scope.nextStages.length; i++) {
                        nextStages = [];
                        if ($scope.nextStages[i].selectedStages.length > 0) {
                            for (var j = 0; j < $scope.nextStages[i].selectedStages.length; j++) {

                                selectedNextStages.push($scope.nextStages[i].selectedStages[j].stageName);
                            }

                            NextStagesRecords.push({ RecordName: $scope.nextStages[i].RecordName, NextStages: selectedNextStages });
                        }
                    }
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.TransformBatchQueueActivator, Vanrise.GenericData.QueueActivators',
                        DataTransformationDefinitionId: dataTransformationSelectorAPI.getSelectedIds(),
                        SourceRecordName: $scope.selectedTransformationRecordType.RecordName,
                        NextStagesRecords: NextStagesRecords.length > 0 ? NextStagesRecords : undefined
                    };
                }

                return api;
            }
        }
    }

    app.directive('vrGenericdataQueueactivatorTransformbatch', QueueingStoreBatchQueueActivator);

})(app);