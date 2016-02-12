(function (app) {

    'use strict';

    QueueingStoreBatchQueueActivator.$inject = ['VR_GenericData_DataTransformationDefinitionAPIService','UtilsService', 'VRUIUtilsService'];

    function QueueingStoreBatchQueueActivator(VR_GenericData_DataTransformationDefinitionAPIService,UtilsService, VRUIUtilsService) {
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
            var dataGridDataSource;
            $scope.transformationRecordTypes = [];
            $scope.showTransformationRecordTypesSelector = false;
            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

               
                
                $scope.onSelectedDataTransformationChanged = function () {
                    if ($scope.selectedDataTransformation != undefined) {
                        $scope.showTransformationRecordTypesSelector = true;
                    }
                    else {
                        $scope.showTransformationRecordTypesSelector = false;
                    }
                    if ($scope.selectedDataTransformation != undefined) {
                        $scope.transformationRecordTypes = [];
                        VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationDefinitionRecords($scope.selectedDataTransformation.DataTransformationDefinitionId).then(function (response) {
                            if (response) {
                                dataGridDataSource = response;
                                for (var i = 0; i < response.length; i++) {
                                    $scope.transformationRecordTypes.push(response[i]);
                                }
                              
                            }

                        })
                    }
                }
            
            }

           
       

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedId;
                    
                    if (payload != undefined) {
                        selectedId = payload.DataTransformationDefinitionId;
                        ctrl.SourceRecordName = payload.SourceRecordName;
                    }

                    return loadSelector();

                    function loadSelector() {
                        var selectorLoadDeferred = UtilsService.createPromiseDeferred();
                        var selectorPayload = { selectedIds: selectedId };
                        VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, selectorLoadDeferred);
                        return selectorLoadDeferred.promise;
                    }
                }

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.QueueActivators.TransformBatchQueueActivator, Vanrise.GenericData.QueueActivators',
                        SourceRecordName: selectorAPI.getSelectedIds()
                    };
                }

                return api;
            }
        }
    }

    app.directive('vrQueueingTransformBatchqueueactivator', QueueingStoreBatchQueueActivator);

})(app);