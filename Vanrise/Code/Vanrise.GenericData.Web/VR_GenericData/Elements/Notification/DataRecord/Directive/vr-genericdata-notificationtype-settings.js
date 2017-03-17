'use strict';

app.directive('vrGenericdataNotificationtypeSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DataRecordNotificationTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Directive/Templates/NotificationTypeSettingsTemplate.html'
        };

        function DataRecordNotificationTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeId;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectorSelectionChangedDeferred;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.gridColumnDefinitions = [];
                $scope.scopeModel.showDataRecordFieldsGrid = false;

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onDataRecordFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeSelectionChanged = function (selectedItem) {

                    if (selectedItem != undefined) {
                        dataRecordTypeId = selectedItem.DataRecordTypeId;
                        $scope.scopeModel.showDataRecordFieldsGrid = true;

                        if (dataRecordTypeSelectorSelectionChangedDeferred != undefined) {
                            dataRecordTypeSelectorSelectionChangedDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.gridColumnDefinitions = [];
                            loadDataRecordTypeFieldsSelector();

                            function loadDataRecordTypeFieldsSelector() {
                                var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                                var dataRecordTypeFieldsSelectorPayload = {
                                    dataRecordTypeId: dataRecordTypeId
                                };
                                VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);

                                return dataRecordTypeFieldsSelectorLoadDeferred.promise;
                            }
                        }
                    }
                };

                $scope.scopeModel.onSelectDataRecordTypeField = function (selectedItem) {
                    var dataRecordTypeField = selectedItem;

                    $scope.scopeModel.gridColumnDefinitions.push({
                        FieldName: dataRecordTypeField.Name,
                        Header: dataRecordTypeField.Title
                    });
                };
                $scope.scopeModel.onDeselectDataRecordTypeField = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.gridColumnDefinitions, deselectedItem.Name, 'FieldName');
                    $scope.scopeModel.gridColumnDefinitions.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index;

                    index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedDataRecordFields, deletedItem.FieldName, 'Name');
                    $scope.scopeModel.selectedDataRecordFields.splice(index, 1);

                    index = UtilsService.getItemIndexByVal($scope.scopeModel.gridColumnDefinitions, deletedItem.FieldName, 'FieldName');
                    $scope.scopeModel.gridColumnDefinitions.splice(index, 1);
                };

                $scope.scopeModel.validateGrid = function () {

                    if ($scope.scopeModel.gridColumnDefinitions == undefined || $scope.scopeModel.gridColumnDefinitions.length == 0)
                        return "Please Add a Column";

                    return null;
                }

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var gridColumnDefinitions;

                    if (payload != undefined) {
                        dataRecordTypeId = payload.DataRecordTypeId;
                        gridColumnDefinitions = payload.GridColumnDefinitions;
                    }

                    //Loading DataRecordType Selector
                    var dataRecordTypeSelectorLoadPromise = getDataRecordTypeSelectorLoadPromise();
                    promises.push(dataRecordTypeSelectorLoadPromise);

                    //Loading DataRecordTypeFields Selector
                    if (dataRecordTypeId != undefined) {
                        var dataRecordTypeFieldsSelectorLoadPromise = getDataRecordTypeFieldsSelectorLoadPromise();
                        promises.push(dataRecordTypeFieldsSelectorLoadPromise);
                    }

                    //Loading Grid
                    if (gridColumnDefinitions != undefined) {
                        $scope.scopeModel.gridColumnDefinitions = gridColumnDefinitions;
                    }


                    function getDataRecordTypeSelectorLoadPromise() {
                        if (dataRecordTypeId != undefined)
                            dataRecordTypeSelectorSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var dataRecordTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorPromiseDeferred.promise.then(function () {
                            var dataRecordTypeSelectorPayload;
                            if (dataRecordTypeId != undefined) {
                                dataRecordTypeSelectorPayload = { selectedIds: dataRecordTypeId };
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, dataRecordTypeSelectorPayload, dataRecordTypeSelectorLoadPromiseDeferred);
                        });

                        return dataRecordTypeSelectorLoadPromiseDeferred.promise;
                    }
                    function getDataRecordTypeFieldsSelectorLoadPromise() {
                        var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([dataRecordTypeFieldsSelectorReadyDeferred.promise, dataRecordTypeSelectorSelectionChangedDeferred.promise]).then(function () {
                            dataRecordTypeSelectorSelectionChangedDeferred = undefined;

                            var dataRecordTypeFieldsSelectorPayload = {
                                dataRecordTypeId: dataRecordTypeId
                            };
                            if (gridColumnDefinitions != undefined) {
                                dataRecordTypeFieldsSelectorPayload.selectedIds = UtilsService.getPropValuesFromArray(gridColumnDefinitions, 'FieldName');
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                        });

                        return dataRecordTypeFieldsSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.Notification.DataRecordNotificationTypeSettings, Vanrise.GenericData.Notification',
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        GridColumnDefinitions: $scope.scopeModel.gridColumnDefinitions
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);