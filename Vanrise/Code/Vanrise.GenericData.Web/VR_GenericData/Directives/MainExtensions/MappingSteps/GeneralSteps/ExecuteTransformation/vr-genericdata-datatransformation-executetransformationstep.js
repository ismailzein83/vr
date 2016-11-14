'use strict';
app.directive('vrGenericdataDatatransformationExecutetransformationstep', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService','VR_GenericData_DataTransformationDefinitionAPIService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService, VR_GenericData_DataTransformationDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new ExecuteTransformationStepCtor(ctrl, $scope);
                ctor.initializeController();

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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/ExecuteTransformation/Templates/ExecuteTransformationStepTemplate.html';
            }

        };

        function ExecuteTransformationStepCtor(ctrl, $scope) {
            var dataTransformationSelectorDirectiveReadyAPI;
            var dataTransformationSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var mainPayload;
            function initializeController() {
                $scope.recordsMapping = [];
                $scope.onDataTransformationSelectorDirectiveReady = function (api) {
                    dataTransformationSelectorDirectiveReadyAPI = api;
                    dataTransformationSelectorReadyPromiseDeferred.resolve();
                };

                $scope.onDataTransformationSelectionChanged = function () {

                    if (dataTransformationSelectorDirectiveReadyAPI != undefined && dataTransformationSelectorDirectiveReadyAPI.getSelectedIds() != undefined) {
                        $scope.isLoadingMappingData = true;
                        VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationDefinition(dataTransformationSelectorDirectiveReadyAPI.getSelectedIds()).then(function (response) {
                            if (response && response.RecordTypes) {
                                $scope.recordsMapping.length = 0;
                                for (var i = 0; i < response.RecordTypes.length; i++) {
                                    var record = response.RecordTypes[i];
                                    var filterItem = {
                                        Record: record,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    var payload;
                                    if (mainPayload != undefined && mainPayload.RecordsMapping != undefined)
                                        payload = mainPayload.RecordsMapping[i] != undefined ? mainPayload.RecordsMapping[i].Value : undefined;
                                    addFilterItemToGrid(filterItem, payload);
                                }
                            }
                        }).finally(function () {
                            $scope.isLoadingMappingData = false;
                        });
                    }
                };

              
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined)
                        mainPayload = payload.stepDetails;
                    var promises = [];
                    var loadDataTransformationSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                    dataTransformationSelectorReadyPromiseDeferred.promise.then(function () {
                        var payloadSelector;
                        if (payload != undefined && payload.stepDetails != undefined)
                            payloadSelector = {
                                selectedIds: payload.stepDetails.DataTransformationId
                            };
                        VRUIUtilsService.callDirectiveLoad(dataTransformationSelectorDirectiveReadyAPI, payloadSelector, loadDataTransformationSelectorPromiseDeferred);
                    });
                    promises.push(loadDataTransformationSelectorPromiseDeferred.promise);

                    if (payload != undefined && payload.stepDetails != undefined && payload.stepDetails.DataTransformationId != undefined) {
                        VR_GenericData_DataTransformationDefinitionAPIService.GetDataTransformationDefinition(payload.stepDetails.DataTransformationId).then(function (response) {
                            if (response && response.RecordTypes) {
                                $scope.recordsMapping.length = 0;
                                for (var i = 0; i < response.RecordTypes.length; i++) {
                                    var record = response.RecordTypes[i];
                                    var filterItem = {
                                        Record: record,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    promises.push(filterItem.loadPromiseDeferred.promise);
                                    var payloadData;
                                    if (payload.stepDetails.RecordsMapping != undefined)
                                        payloadData = payload.stepDetails.RecordsMapping[i] != undefined ? mainPayload.RecordsMapping[i].Value : undefined;
                                    addFilterItemToGrid(filterItem, payloadData);
                                }
                            }
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var recordsMapping = [];
                    if ($scope.recordsMapping.length > 0) {
                        for (var i = 0; i < $scope.recordsMapping.length; i++) {
                            var recordMapping = $scope.recordsMapping[i];
                            if (recordMapping.directiveAPI != undefined && recordMapping.directiveAPI.getData() != undefined) {
                                recordsMapping.push({
                                    RecordName: recordMapping.FieldName,
                                    Value: recordMapping.directiveAPI != undefined ? recordMapping.directiveAPI.getData() : undefined
                                });
                            }

                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.ExecuteTransformationStep, Vanrise.GenericData.Transformation.MainExtensions",
                        DataTransformationId: dataTransformationSelectorDirectiveReadyAPI != undefined ? dataTransformationSelectorDirectiveReadyAPI.getSelectedIds() : undefined,
                        RecordsMapping: recordsMapping
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function addFilterItemToGrid(filterItem, payload) {
                var dataItem = {
                    FieldName: filterItem.Record.RecordName,
                    Title: filterItem.Record.RecordName
                };
                var dataItemPayload = {};

                if (mainPayload != undefined) {
                    dataItemPayload.context = mainPayload.context;

                }
                if (payload != undefined)
                    dataItemPayload.selectedRecords = payload;
                dataItem.onSourceMappingReady = function (api) {
                    dataItem.directiveAPI = api;
                    filterItem.readyPromiseDeferred.resolve();
                };

                filterItem.readyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, filterItem.loadPromiseDeferred);
                    });

                $scope.recordsMapping.push(dataItem);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);