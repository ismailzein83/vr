'use strict';

app.directive('vrGenericdataDatarecordnotificationtypeSettings', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: '/Client/Modules/VR_GenericData/Elements/Notification/DataRecord/Notification/Directive/Templates/NotificationTypeSettingsTemplate.html'
        };

        function DataRecordNotificationTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeId;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectorSelectionChangedDeferred;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var onNotificationCreatedHandlerDirectiveAPI;
            var onNotificationCreatedHandlerDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.gridColumnDefinitions = [];
                $scope.scopeModel.dataRecordTypeSelected = false;

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onNotificationCreatedHandlerDirectiveReady = function (api) {
                    onNotificationCreatedHandlerDirectiveAPI = api;
                    onNotificationCreatedHandlerDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeSelectionChanged = function (selectedItem) {

                    if (selectedItem != undefined) {
                        dataRecordTypeId = selectedItem.DataRecordTypeId;
                        $scope.scopeModel.dataRecordTypeSelected = true;

                        if (dataRecordTypeSelectorSelectionChangedDeferred != undefined) {
                            dataRecordTypeSelectorSelectionChangedDeferred.resolve();
                        }
                        else {
                            var promises = [];
                            $scope.scopeModel.isLoading = true;
                            $scope.scopeModel.gridColumnDefinitions = [];

                            promises.push(loadDataRecordTypeFieldsSelector());
                            promises.push(reloadOnNotificationCreatedHandlerDirective());

                            UtilsService.waitMultiplePromises(promises).then(function () {
                                $scope.scopeModel.isLoading = false;
                            }); 
                        }
                    }

                    function loadDataRecordTypeFieldsSelector() {
                        var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        var dataRecordTypeFieldsSelectorPayload = {
                            dataRecordTypeId: dataRecordTypeId
                        };
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);

                        return dataRecordTypeFieldsSelectorLoadDeferred.promise;
                    }

                    function reloadOnNotificationCreatedHandlerDirective() {
                        var loadOnNotificationCreatedHandlerDirectivePromiseDeferred = UtilsService.createPromiseDeferred("loadOnNotificationCreatedHandlerDirectivePromiseDeferred");

                        var onNotificationCreatedHandlerPayload = {
                            dataRecordTypeId: dataRecordTypeId
                        };

                        VRUIUtilsService.callDirectiveLoad(onNotificationCreatedHandlerDirectiveAPI, onNotificationCreatedHandlerPayload, loadOnNotificationCreatedHandlerDirectivePromiseDeferred);

                        return loadOnNotificationCreatedHandlerDirectivePromiseDeferred.promise;
                    }
                };

                $scope.scopeModel.onSelectDataRecordTypeField = function (selectedItem) {
                    var dataRecordTypeField = selectedItem;

                    $scope.scopeModel.gridColumnDefinitions.push({
                        FieldName: dataRecordTypeField.Name,
                        Header: dataRecordTypeField.Title,
                        HeaderDescription: undefined
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
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var gridColumnDefinitions;
                    var onNotificationCreatedHandler;

                    if (payload != undefined) {
                        dataRecordTypeId = payload.DataRecordTypeId;
                        gridColumnDefinitions = payload.GridColumnDefinitions;
                        onNotificationCreatedHandler = payload.OnNotificationCreatedHandler;
                    }

                    //Loading DataRecordType Selector
                    var dataRecordTypeSelectorLoadPromise = getDataRecordTypeSelectorLoadPromise();
                    promises.push(dataRecordTypeSelectorLoadPromise);


                    if (dataRecordTypeId != undefined) {
                        //Loading DataRecordTypeFields Selector
                        var dataRecordTypeFieldsSelectorLoadPromise = getDataRecordTypeFieldsSelectorLoadPromise();
                        promises.push(dataRecordTypeFieldsSelectorLoadPromise);

                        //Loading OnNotificationCreatedHandler
                        var loadOnNotificationCreatedHandlerDirectivePromise = loadOnNotificationCreatedHandlerDirective();
                        promises.push(loadOnNotificationCreatedHandlerDirectivePromise);
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

                    function loadOnNotificationCreatedHandlerDirective() {
                        var loadOnNotificationCreatedHandlerDirectivePromiseDeferred = UtilsService.createPromiseDeferred("loadOnNotificationCreatedHandlerDirectivePromiseDeferred");

                        onNotificationCreatedHandlerDirectiveReadyDeferred.promise.then(function () {
                            var onNotificationCreatedHandlerPayload = {
                                dataRecordTypeId: dataRecordTypeId
                            };

                            if (onNotificationCreatedHandler != undefined) {
                                onNotificationCreatedHandlerPayload.onNotificationCreatedHandler = onNotificationCreatedHandler;
                            }

                            VRUIUtilsService.callDirectiveLoad(onNotificationCreatedHandlerDirectiveAPI, onNotificationCreatedHandlerPayload, loadOnNotificationCreatedHandlerDirectivePromiseDeferred);
                        });

                        return loadOnNotificationCreatedHandlerDirectivePromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.Notification.DataRecordNotificationTypeSettings, Vanrise.GenericData.Notification',
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        GridColumnDefinitions: $scope.scopeModel.gridColumnDefinitions,
                        OnNotificationCreatedHandler: onNotificationCreatedHandlerDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);