"use strict";

app.directive("vrGenericdataSummarygroupingcolumnsGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_KeyFieldMappingService",
    function (UtilsService, VRNotificationService, VR_GenericData_KeyFieldMappingService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var summaryGroupingColumns = new SummaryGroupingColumns($scope, ctrl, $attrs);
                summaryGroupingColumns.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/SummaryTransformationDefinition/Templates/SummaryGroupingColumnsManagement.html"

        };

        function SummaryGroupingColumns($scope, ctrl, $attrs) {
            var rawDataRecordTypeId;
            var summaryDataRecordTypeId;
            var gridAPI;
            this.initializeController = initializeController;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should Select at least one column grouping ";
                }

                ctrl.addColumnGrouping = function () {
                    var onDataItemAdded = function (itemAdded) {
                        var alreadyExists = false;
                        angular.forEach(ctrl.datasource, function (item) {
                            if (item.RawFieldName == itemAdded.RawFieldName && item.SummaryFieldName == itemAdded.SummaryFieldName)
                                alreadyExists = true;
                        });
                        if (!alreadyExists)
                            ctrl.datasource.push(itemAdded);
                    }

                    VR_GenericData_KeyFieldMappingService.addItem(rawDataRecordTypeId, summaryDataRecordTypeId, onDataItemAdded, ctrl.datasource);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.setRecordTypeIds = function (selectedRawDataRecordTypeId, selectedSummaryDataRecordTypeId) {
                    rawDataRecordTypeId = selectedRawDataRecordTypeId;
                    summaryDataRecordTypeId = selectedSummaryDataRecordTypeId
                    ctrl.datasource.length = 0;
                    if (rawDataRecordTypeId == undefined || summaryDataRecordTypeId == undefined)
                        ctrl.enableAdd = false;
                    else
                        ctrl.enableAdd = true;
                }

                api.getData = function () {
                    var keyFieldMappings;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        keyFieldMappings = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            keyFieldMappings.push({
                                RawFieldName: ctrl.datasource[i].RawFieldName,
                                SummaryFieldName: ctrl.datasource[i].SummaryFieldName,
                            });
                        }

                    }

                    return keyFieldMappings;
                }

                api.load = function (payload) {
                    if (payload != undefined) {

                        if (payload.rawDataRecordTypeId)
                            rawDataRecordTypeId = payload.rawDataRecordTypeId;

                        if (payload.summaryDataRecordTypeId)
                            summaryDataRecordTypeId = payload.summaryDataRecordTypeId;

                        if (payload.KeyFieldMappings && payload.KeyFieldMappings.length > 0) {
                            for (var i = 0; i < payload.KeyFieldMappings.length; i++) {
                                var dataItem = payload.KeyFieldMappings[i];
                                ctrl.datasource.push(dataItem);
                            }
                        }

                        if (payload.rawDataRecordTypeId == undefined || payload.summaryDataRecordTypeId == undefined)
                            ctrl.enableAdd = false;
                        else
                            ctrl.enableAdd = true;
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editColumnGrouping,
                },
                {
                    name: "Delete",
                    clicked: deleteColumnGrouping,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editColumnGrouping(dataItem) {
                var onDataItemUpdated = function (updatedDataItem) {
                    var alreadyExists = false;
                    angular.forEach(ctrl.datasource, function (item) {
                        if (item.RawFieldName == updatedDataItem.RawFieldName && item.SummaryFieldName == updatedDataItem.SummaryFieldName)
                            alreadyExists = true;
                    });
                    if (!alreadyExists) {
                        var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.RawFieldName, 'RawFieldName');
                        ctrl.datasource[index] = updatedDataItem;
                    }
                }
                VR_GenericData_KeyFieldMappingService.editItem(rawDataRecordTypeId, summaryDataRecordTypeId, dataItem, onDataItemUpdated, ctrl.datasource);
            }

            function deleteColumnGrouping(dataItem) {
                var onDataItemDeleted = function (deletedDataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.RawFieldName, 'RawFieldName');
                    ctrl.datasource.splice(index, 1);
                };

                VR_GenericData_KeyFieldMappingService.deleteItem($scope, dataItem, onDataItemDeleted);
            }
        }

        return directiveDefinitionObject;

    }
]);