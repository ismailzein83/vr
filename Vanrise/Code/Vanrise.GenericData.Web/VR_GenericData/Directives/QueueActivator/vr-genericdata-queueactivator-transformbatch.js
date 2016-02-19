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

            var selectorAPI;
            var gridAPI;
            var dataGridDataSource;
            var dataTransformationSelectorReadyPromiseDeferred;
            var dataTransformationRecordSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataTransformationDefinitionId;
            var dataRecordTypeId;
            var secondtimeChanged = false;
            var stagesDataSource;

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
                        var filteredResponse = [];

                        $scope.recordTypesWithStages.length=0;
                        $scope.transformationRecordTypes.length = 0;
                        $scope.selectedDataRecordStorage = [];
                       
                        for (var i = 0; i < response.length; i++) {
                            if (response[i].DataRecordTypeId == dataRecordTypeId && response[i].IsArray) {
                                $scope.transformationRecordTypes.push(response[i]);
                            }
                            if (response[i].IsArray) {
                             filteredResponse.push(response[i]);
                            }
                        }
                        loadDataGrid(filteredResponse);
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
                    dataItem.stagesDataSource = [];
                    for (var j = 0; j < stagesDataSource.length; j++) {
                        if (stagesDataSource[j].DataRecordTypeId == response[i].DataRecordTypeId)
                            dataItem.stagesDataSource.push({ stageName: stagesDataSource[j].stageName });
                    }
                    dataItem.selectedStages =response[i].NextStages!=undefined? response[i].NextStages: [];
                    $scope.recordTypesWithStages.push(dataItem);
                }

            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedId;
                    var promises=[];
                    if (payload != undefined && payload.ExistingStages != undefined) {
                        $scope.stagesDataSource = [];
                        stagesDataSource = payload.ExistingStages;
                    }

                    if(payload != undefined && payload.DataRecordTypeId != undefined)
                        dataRecordTypeId=payload.DataRecordTypeId

                    if (payload != undefined && payload.QueueActivator != undefined) {
                        selectedId = payload.QueueActivator.DataTransformationDefinitionId;
                        dataTransformationSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    }

                   var loadSelectorPromise=loadSelector();
                   promises.push(loadSelectorPromise);

                    if (payload != undefined && payload.QueueActivator != undefined) {
                        dataTransformationSelectorReadyPromiseDeferred.promise.then(function () {
                            dataTransformationRecordSelectorReadyPromiseDeferred.promise.then(function () {
                                    var selectedValue = UtilsService.getItemByVal($scope.transformationRecordTypes, payload.QueueActivator.SourceRecordName, "RecordName");
                                    $scope.selectedDataRecordStorage = selectedValue;
                                    loadDataGridInEditMode();
                            })
                        });
                    }

                    return UtilsService.waitMultiplePromises(promises);


                    function loadDataGridInEditMode() {
                        for (var i = 0; i < $scope.recordTypesWithStages.length; i++) {
                       
                                for (var j = 0; j < payload.QueueActivator.NextStagesRecords.length; j++) {
                                    if ($scope.recordTypesWithStages[i].RecordName == payload.QueueActivator.NextStagesRecords[j].RecordName) {
                                        for (var k = 0; k < payload.QueueActivator.NextStagesRecords[j].NextStages.length;k++)
                                        var selectedValue = UtilsService.getItemByVal($scope.recordTypesWithStages[i].stagesDataSource, payload.QueueActivator.NextStagesRecords[j].NextStages[k], "stageName");
                                        if (selectedValue != null)
                                            $scope.recordTypesWithStages[i].selectedStages.push(selectedValue);
                                    }
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
                        if ($scope.recordTypesWithStages[i].selectedStages.length > 0) {
                            for (var j = 0; j < $scope.recordTypesWithStages[i].selectedStages.length; j++) {

                                nextStages.push($scope.recordTypesWithStages[i].selectedStages[j].stageName);
                            }

                            NextStagesRecords.push({ RecordName: $scope.recordTypesWithStages[i].RecordName, NextStages: nextStages });
                        }
                    }
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.TransformBatchQueueActivator, Vanrise.GenericData.QueueActivators',
                        DataTransformationDefinitionId: selectorAPI.getSelectedIds(),
                        SourceRecordName: $scope.selectedDataRecordStorage.RecordName,
                        NextStagesRecords: NextStagesRecords.length>0 ? NextStagesRecords:undefined
                    };
                }

                return api;
            }
        }
    }

    app.directive('vrGenericdataQueueactivatorTransformbatch', QueueingStoreBatchQueueActivator);

})(app);