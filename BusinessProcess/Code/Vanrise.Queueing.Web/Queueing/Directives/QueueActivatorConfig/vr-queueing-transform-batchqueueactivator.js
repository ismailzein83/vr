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
                return '/Client/Modules/Queueing/Directives/QueueActivatorConfig/Templates/TransformBatchQueueActivatorTemplate.html';
            }
        };

        function QueueingTransformBatchQueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var selectorAPI;
            var gridAPI;
            var dataGridDataSource;
            var dataTransformationSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataTransformationRecordSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataTransformationDefinitionId;
            $scope.recordTypesWithStages = [];
            $scope.selectedDataRecordStorage = [];
            $scope.transformationRecordTypes = [];
            $scope.showTransformationRecordTypesSelector = false;
            $scope.stagesDataSource = [];
            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };



                $scope.onSelectedDataTransformationChanged = function () {
                    if ($scope.selectedDataTransformation != undefined) {
                        dataTransformationDefinitionId = $scope.selectedDataTransformation.DataTransformationDefinitionId;
                        loadDataTransformationRecordSelector();
                    }
                    else
                        $scope.showTransformationRecordTypesSelector = false;

                }

            }


            $scope.ongridReady = function (api) {
                gridAPI = api;
            };


            function loadDataTransformationRecordSelector() {
                $scope.transformationRecordTypes = [];
                if (dataTransformationDefinitionId != undefined) {
                    VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationDefinitionRecords(dataTransformationDefinitionId).then(function (response) {
                        if (response) {
                            dataGridDataSource = {};
                            dataGridDataSource = response;
                            loadDataGrid();

                            $scope.transformationRecordTypes = [];
                            for (var i = 0; i < response.length; i++) {
                                $scope.transformationRecordTypes.push(response[i]);
                            }
                        }
                        dataTransformationRecordSelectorReadyPromiseDeferred.resolve();
                    })

                    $scope.showTransformationRecordTypesSelector = true;
                }
            }



            function loadDataGrid() {
                var dataItem;
                $scope.recordTypesWithStages = [];
                for (var i = 0; i < dataGridDataSource.length; i++) {
                    dataItem = {};
                    dataItem.RecordName = dataGridDataSource[i].RecordName;
                    dataItem.selectedStages = [];
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


                    if (payload != undefined && payload.DataTransformationDefinitionId != undefined) {
                        selectedId = payload.DataTransformationDefinitionId;

                    }

                    loadSelector();


                    function loadDataGridInEditMode() {
                        for (var i = 0; i < $scope.recordTypesWithStages.length; i++) {
                            $scope.recordTypesWithStages[i].selectedStages = payload.NextStagesRecords[i].NextStages;
                        }
                    }

                    dataTransformationSelectorReadyPromiseDeferred.promise.then(function () {
                        loadDataTransformationRecordSelector();
                        dataTransformationRecordSelectorReadyPromiseDeferred.promise.then(function () {
                            $scope.selectedDataRecordStorage = payload.SourceRecordName;
                            loadDataGridInEditMode();
                        })
                    });


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
                        if ($scope.recordTypesWithStages[i].selectedStages != undefined)
                            nextStages.push($scope.recordTypesWithStages[i].selectedStages);
                        NextStagesRecords.push({ RecordName: $scope.recordTypesWithStages[i].RecordName, NextStages: nextStages });
                    }
                    console.log(NextStagesRecords);
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