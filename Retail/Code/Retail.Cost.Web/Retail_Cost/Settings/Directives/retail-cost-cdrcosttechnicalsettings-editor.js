'use strict';

app.directive('retailCostCdrcosttechnicalsettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CostTechnicalSettingsEditor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Cost/Settings/Directives/Templates/CDRCostTechnicalSettingsEditorTemplate.html'
        };

        function CostTechnicalSettingsEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeId;

            var reprocessDefinitionSelectorAPI;
            var reprocessDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

            var chunkTimeSelectorAPI;
            var chunkTimeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectorSelectionChangedDeferred;

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showRecordFilterDirective = false;

                $scope.scopeModel.onReprocessDefinitionSelectorReady = function (api) {
                    reprocessDefinitionSelectorAPI = api;
                    reprocessDefinitionPromiseDeferred.resolve();
                };
                $scope.scopeModel.onChunkTimeSelectorReady = function (api) {
                    chunkTimeSelectorAPI = api;
                    chunkTimeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeSelectionChanged = function (selectedItem) {

                    if (selectedItem != undefined) {
                        dataRecordTypeId = selectedItem.DataRecordTypeId;
                        $scope.scopeModel.showRecordFilterDirective = true;

                        if (dataRecordTypeSelectorSelectionChangedDeferred != undefined) {
                            dataRecordTypeSelectorSelectionChangedDeferred.resolve();
                        }
                        else {
                            VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                                var dataRecordFieldsInfo = response;

                                var recordFilterDirectivePayload = {
                                    context: buildContext(dataRecordFieldsInfo)
                                };
                                var setLoader = function (value) {
                                    $scope.scopeModel.isRecordFilterDirectiveLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, recordFilterDirectiveAPI, recordFilterDirectivePayload, setLoader);
                            });
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var costCDRReprocessDefinitionId;
                    var chunkTime;
                    var filterGroup;

                    if (payload != undefined && payload.data != undefined) {
                        var costTechnicalSettingData = payload.data;

                        if (costTechnicalSettingData != undefined) {
                            costCDRReprocessDefinitionId = costTechnicalSettingData.CostCDRReprocessDefinitionId;
                            chunkTime = costTechnicalSettingData.ChunkTime;
                            dataRecordTypeId = costTechnicalSettingData.DataRecordTypeId;
                            filterGroup = costTechnicalSettingData.FilterGroup;
                            $scope.scopeModel.isCostIncluded = costTechnicalSettingData.IsCostIncluded;
                        }
                    }

                    //Loading ReprocessDefinition selector 
                    var reprocessDefinitionSelectorLoadPromise = getReprocessDefinitionSelectorLoadPromise();
                    promises.push(reprocessDefinitionSelectorLoadPromise);

                    //Loading ChunkTime selector 
                    var chunktimeSelectorLoadPromise = getChunktimeSelectorLoadPromise();
                    promises.push(chunktimeSelectorLoadPromise);

                    //Loading DataRecordType Selector
                    var dataRecordTypeSelectorLoadPromise = getDataRecordTypeSelectorLoadPromise();
                    promises.push(dataRecordTypeSelectorLoadPromise);

                    if (dataRecordTypeId != undefined) {
                        //Loading RecordFilterGroup Directive
                        var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise();
                        promises.push(recordFilterDirectiveLoadPromise);
                    }


                    function getReprocessDefinitionSelectorLoadPromise() {
                        var reprocessDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        reprocessDefinitionPromiseDeferred.promise.then(function () {
                            var reprocessDefinitionSelectorPayload;
                            if (costCDRReprocessDefinitionId != undefined) {
                                reprocessDefinitionSelectorPayload = { selectedIds: costCDRReprocessDefinitionId };
                            }
                            VRUIUtilsService.callDirectiveLoad(reprocessDefinitionSelectorAPI, reprocessDefinitionSelectorPayload, reprocessDefinitionSelectorLoadDeferred);
                        });

                        return reprocessDefinitionSelectorLoadDeferred.promise;
                    }
                    function getChunktimeSelectorLoadPromise() {
                        var chunkTimeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        chunkTimeSelectorReadyDeferred.promise.then(function () {
                            var chunkTimeSelectorPayload;
                            if (chunkTime) {
                                chunkTimeSelectorPayload = { selectedIds: chunkTime };
                            }
                            VRUIUtilsService.callDirectiveLoad(chunkTimeSelectorAPI, chunkTimeSelectorPayload, chunkTimeSelectorLoadDeferred);
                        });

                        return chunkTimeSelectorLoadDeferred.promise;
                    }
                    function getDataRecordTypeSelectorLoadPromise() {
                        if (dataRecordTypeId != undefined)
                            dataRecordTypeSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var dataRecordTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorPromiseDeferred.promise.then(function () {
                            var dataRecordTypeSelectorPayload;
                            if (dataRecordTypeId != undefined) {
                                dataRecordTypeSelectorPayload = {
                                    selectedIds: dataRecordTypeId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadPromiseDeferred);
                        });

                        return dataRecordTypeSelectorLoadPromiseDeferred.promise;
                    }
                    function getRecordFilterDirectiveLoadPromise() {
                        var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([recordFilterDirectiveReadyDeferred.promise, dataRecordTypeSelectorSelectionChangedDeferred.promise]).then(function () {
                            dataRecordTypeSelectorSelectionChangedDeferred = undefined;

                            VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                                var dataRecordFieldsInfo = response;

                                var recordFilterDirectivePayload = {
                                    context: buildContext(dataRecordFieldsInfo)
                                };
                                if (filterGroup != undefined) {
                                    recordFilterDirectivePayload.FilterGroup = filterGroup
                                }
                                VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                            });
                        });

                        return recordFilterDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: 'Retail.Cost.Entities.CDRCostTechnicalSettingData, Retail.Cost.Entities',
                        CostCDRReprocessDefinitionId: reprocessDefinitionSelectorAPI.getSelectedIds(),
                        ChunkTime: chunkTimeSelectorAPI.getSelectedIds(),
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        FilterGroup: recordFilterDirectiveAPI.getData().filterObj,
                        IsCostIncluded: $scope.scopeModel.isCostIncluded
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function buildContext(dataRecordFieldsInfo) {
                var context = {
                    getFields: function () {
                        var fields = [];
                        if (dataRecordFieldsInfo != undefined) {
                            for (var i = 0; i < dataRecordFieldsInfo.length; i++) {
                                var dataRecordField = dataRecordFieldsInfo[i].Entity;

                                fields.push({
                                    FieldName: dataRecordField.Name,
                                    FieldTitle: dataRecordField.Title,
                                    Type: dataRecordField.Type
                                });
                            }
                        }
                        return fields;
                    }
                };
                return context;
            }
        }
    }]);