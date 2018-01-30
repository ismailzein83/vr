"use strict";

app.directive("vrGenericdataGenericbusinessentityEditor", ["UtilsService", "VRNotificationService", "VR_GenericData_DataRecordTypeAPIService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VR_GenericData_DataRecordTypeAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericBusinessEntityDefinitionEditor = new GenericBusinessEntityDefinitionEditor($scope, ctrl, $attrs);
                genericBusinessEntityDefinitionEditor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/Editor/Templates/GenericBusinessEntityDefinitionEditor.html"

        };

        function GenericBusinessEntityDefinitionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var dataRecordStorageSelectorAPI;
            var dataRecordStorageSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var dataRecordTypeTitleFieldsSelectorAPI;
            var dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var columnDefinitionGridAPI;
            var columnDefinitionGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var recordTypeSelectedPromiseDeferred;
            var recordTypeEntity;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeSelectorDirectiveReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordStorageSelectorDirectiveReady = function (api) {
                    dataRecordStorageSelectorAPI = api;
                    dataRecordStorageSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeTitleFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeTitleFieldsSelectorAPI = api;
                    dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGenericBEColumnDefinitionGridReady = function (api) {
                    columnDefinitionGridAPI = api;
                    columnDefinitionGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRecordTypeSelectionChanged = function () {
                    var selectedRecordTypeId = dataRecordTypeSelectorAPI.getSelectedIds();
                    if (selectedRecordTypeId != undefined) {
                        var setLoader = function (value) { $scope.isLoadingRegions = value };
                        var payload = {
                            dataRecordTypeId: selectedRecordTypeId
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataRecordTypeTitleFieldsSelectorAPI, payload, setLoader, recordTypeSelectedPromiseDeferred);
                    }
                    else if (dataRecordTypeTitleFieldsSelectorAPI != undefined) {
                        dataRecordTypeTitleFieldsSelectorAPI.clearDataSource();
                    }

                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.GenericBEDefinitionSettings, Vanrise.GenericData.Entities",
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        DataRecordStorageId: dataRecordStorageSelectorAPI.getSelectedIds(),
                        TitleFieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds()
                    };
                };

                api.load = function (payload) {
                    var businessEntityDefinitionSettings;
                    var promises = [];
                    if (payload != undefined) {
                        businessEntityDefinitionSettings = payload.businessEntityDefinitionSettings;

                        if (businessEntityDefinitionSettings != undefined)
                            recordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();

                    }
                    promises.push(loadDataRecordTypeSelector());
                    promises.push(loadDataRecordStorageSelector());
                    promises.push(loadColumnDefinitionGrid());

                    if (businessEntityDefinitionSettings != undefined) {
                        promises.push(loadDataRecordTitleFieldsSelector());
                    }



                    function loadDataRecordTypeSelector() {

                        var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {
                            var directivePayload;

                            if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.DataRecordTypeId != undefined) {
                                directivePayload = {};
                                directivePayload.selectedIds = businessEntityDefinitionSettings.DataRecordTypeId;
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                        });

                        return loadDataRecordTypeSelectorPromiseDeferred.promise;
                    }

                    function loadDataRecordStorageSelector() {

                        var loadDataRecordStorageSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordStorageSelectorReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = (businessEntityDefinitionSettings != undefined) ? { selectedIds: businessEntityDefinitionSettings.DataRecordStorageId } : undefined;
                                VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, directivePayload, loadDataRecordStorageSelectorPromiseDeferred);
                            });

                        return loadDataRecordStorageSelectorPromiseDeferred.promise;
                    }

                    function loadDataRecordTitleFieldsSelector() {
                        var loadDataRecordTypeTitleFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([dataRecordTypeTitleFieldsSelectorReadyPromiseDeferred.promise, recordTypeSelectedPromiseDeferred.promise]).then(function () {
                            var typeFieldsPayload = {
                                dataRecordTypeId: businessEntityDefinitionSettings.DataRecordTypeId,
                                selectedIds: businessEntityDefinitionSettings.TitleFieldName
                            };

                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeTitleFieldsSelectorAPI, typeFieldsPayload, loadDataRecordTypeTitleFieldsSelectorPromiseDeferred);
                        });
                        return loadDataRecordTypeTitleFieldsSelectorPromiseDeferred.promise;
                    }


                    function loadColumnDefinitionGrid() {
                        var loadColumnDefinitionGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        columnDefinitionGridReadyPromiseDeferred.promise.then(function () {
                            var payload = {
                                context: getContext()
                            };
                            if (businessEntityDefinitionSettings != undefined && businessEntityDefinitionSettings.GridDefinition != undefined && businessEntityDefinitionSettings.GridDefinition.ColumnDefinitions != undefined)
                                payload.columnDefinitions = businessEntityDefinitionSettings.GridDefinition.ColumnDefinitions;

                            VRUIUtilsService.callDirectiveLoad(columnDefinitionGridAPI, payload, loadColumnDefinitionGridPromiseDeferred);
                        });
                        return loadColumnDefinitionGridPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        recordTypeSelectedPromiseDeferred = undefined;
                    });
                };



                if (ctrl.onReady != null)
                    ctrl.onReady(api);


                function getContext() {
                    return {
                        getDataRecordTypeId: function () {
                            return dataRecordTypeSelectorAPI.getSelectedIds();
                        }
                    };
                }
            }

        }

        return directiveDefinitionObject;

    }
]);