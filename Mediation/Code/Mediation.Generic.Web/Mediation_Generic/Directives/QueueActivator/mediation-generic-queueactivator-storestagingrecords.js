(function (app) {

    'use strict';

    QueueActivatorStoreStagingRecords.$inject = ['UtilsService', 'VRUIUtilsService', 'Mediation_Generic_StorageStagingStatusEnum', 'VR_GenericData_DataRecordTypeService', 'VR_GenericData_DataRecordFieldAPIService'];

    function QueueActivatorStoreStagingRecords(UtilsService, VRUIUtilsService, Mediation_Generic_StorageStagingStatusEnum, VR_GenericData_DataRecordTypeService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var queueingStoreStagingRecordsQueueActivatorCtor = new QueueingStoreStagingRecordsQueueActivatorCtor(ctrl, $scope);
                queueingStoreStagingRecordsQueueActivatorCtor.initializeController();
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
                return '/Client/Modules/Mediation_Generic/Directives/QueueActivator/Templates/QueueActivatorStoreStagingRecordsTemplate.html';
            }
        };

        function QueueingStoreStagingRecordsQueueActivatorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var filterObj;
            var dataRecoredId;
            var sessionIdRecordTypeFieldsSelectorAPI;
            var sessionIdRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var timeRecordTypeFieldsSelectorAPI;
            var timeRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.statusMappings = [];

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                };

                $scope.scopeModel.onSessionIdDataRecordTypeFieldsSelectorReady = function (api) {
                    sessionIdRecordTypeFieldsSelectorAPI = api;
                    sessionIdRecordTypeFieldsSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onTimeDataRecordTypeFieldsSelectorReady = function (api) {
                    timeRecordTypeFieldsSelectorAPI = api;
                    timeRecordTypeFieldsSelectorReadyDeferred.resolve();
                };


                $scope.onGridReady = function (api) {
                    gridAPI = api;
                }
                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {

                    var timeField;
                    var sessionIdField;
                    if (payload != undefined && payload.QueueActivator != undefined) {
                        timeField = payload.QueueActivator.EventTimeRecordTypeId;
                        sessionIdField = payload.QueueActivator.SessionRecordTypeId;
                        PrepareStatusMappings(payload.QueueActivator.StatusMappings);
                    }
                    else
                        PrepareStatusMappings(undefined);

                    dataRecoredId = payload.DataRecordTypeId;
                    var promises = [];


                    var sessionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    sessionIdRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {

                        var selectorPayload = {
                            dataRecordTypeId: payload.DataRecordTypeId,
                            selectedIds: sessionIdField
                        };
                        VRUIUtilsService.callDirectiveLoad(sessionIdRecordTypeFieldsSelectorAPI, selectorPayload, sessionSelectorLoadDeferred);
                    });
                    promises.push(sessionSelectorLoadDeferred.promise);


                    var timeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    timeRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {

                        var selectorPayload = {
                            dataRecordTypeId: payload.DataRecordTypeId,
                            selectedIds: timeField
                        };
                        VRUIUtilsService.callDirectiveLoad(timeRecordTypeFieldsSelectorAPI, selectorPayload, timeSelectorLoadDeferred);
                    });
                    promises.push(timeSelectorLoadDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);

                }

                api.getData = function () {
                    return {
                        $type: 'Mediation.Generic.QueueActivators.StoreStagingRecordsQueueActivator, Mediation.Generic.QueueActivators',
                        SessionRecordTypeId: sessionIdRecordTypeFieldsSelectorAPI.getSelectedIds(),
                        EventTimeRecordTypeId: timeRecordTypeFieldsSelectorAPI.getSelectedIds(),
                        StatusMappings: getStatusMappings()
                    };
                }

                return api;
            }

            function getStatusMappings() {
                var mappings = [];
                for (var i = 0; i < $scope.scopeModel.statusMappings.length; i++) {
                    var mapping = $scope.scopeModel.statusMappings[i];
                    mappings.push({
                        Status: mapping.Status,
                        Filters: mapping.FilterObj,
                        FilterExpression: mapping.Expression
                    });
                }
                return mappings;
            }

            function loadFields() {
                var obj = { DataRecordTypeId: dataRecoredId };
                var serializedFilter = UtilsService.serializetoJson(obj);
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(serializedFilter);
            }
            function PrepareStatusMappings(statusMappings) {
                var stagingStatusEnums = UtilsService.getArrayEnum(Mediation_Generic_StorageStagingStatusEnum);
                if (statusMappings != undefined) {
                    for (var i = 0; i < statusMappings.length; i++) {
                        var statusMappingObj = {
                            Status: stagingStatusEnums[i].description,
                            Expression: statusMappings[i].FilterExpression,
                            FilterObj: statusMappings[i].Filters,
                            Id: i
                        };
                        $scope.scopeModel.statusMappings.push(statusMappingObj);
                    }
                }
                else {

                    for (var i = 0; i < stagingStatusEnums.length; i++) {
                        var statusMappingObj = {
                            Status: stagingStatusEnums[i].description,
                            Expression: undefined,
                            FilterObj: undefined,
                            Id: i
                        };

                        $scope.scopeModel.statusMappings.push(statusMappingObj);
                    }
                }
            }
            function addFilter(dataItem) {

                $scope.scopeModel.isLoading = true;
                loadFields().then(function (response) {
                    if (response) {
                        var fields = [];
                        for (var i = 0; i < response.length; i++) {
                            var dataRecordField = response[i];
                            fields.push({
                                FieldName: dataRecordField.Entity.Name,
                                FieldTitle: dataRecordField.Entity.Title,
                                Type: dataRecordField.Entity.Type,
                            });
                        }
                        $scope.scopeModel.isLoading = false;

                        var onDataRecordFieldTypeFilterAdded = function (filter, expression) {
                            var obj = {
                                FilterObj: filter,
                                Expression: expression,
                                Status: dataItem.Status,
                                Id: dataItem.Id
                            };
                            gridAPI.itemUpdated(obj);
                        }
                        VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter(fields, dataItem.FilterObj, onDataRecordFieldTypeFilterAdded);
                    }
                });
            }
            function resetFilter(dataItem) {
                dataItem.Expression = undefined;
                dataItem.FilterObj = null;
            }
            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit Condition",
                    clicked: addFilter
                },
                {
                    name: "Clear Condition",
                    clicked: resetFilter
                }
                ];
            }
        }
    }

    app.directive('mediationGenericQueueactivatorStorestagingrecords', QueueActivatorStoreStagingRecords);

})(app);