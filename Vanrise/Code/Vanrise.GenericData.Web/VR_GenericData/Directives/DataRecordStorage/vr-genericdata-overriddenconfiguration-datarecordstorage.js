(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordStorageAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordStorageAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var overriddenSettingsDirective = new OverriddenSettingsDirective(ctrl, $scope, $attrs);
                overriddenSettingsDirective.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordStorage/Templates/OverriddenConfigurationDataRecordStorage.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var settings;
            var devProjectId;
            var dataRecordTypeId;
            var dataStoreId;
            var overridenSettings;
            var filter;
            var selectedIds;
            var dataRecordStorageEntity;

            var dataRecordStorageSelectorApi;
            var dataRecordStoragePromiseDeferred = UtilsService.createPromiseDeferred();
            var editorDirectiveAPI;
            var editorDirectiveReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.isSettingsOverriddenValuechanged = function () {
                    if ($scope.scopeModel.isSettingsOverridden == true) {
                        loadDataRecordStorageEditorDirective();
                    }
                    else {
                        hideOverriddenSettingsEditor();
                    }
                };

                $scope.scopeModel.dataRecordStorageSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.name = "";
                            $scope.scopeModel.isSettingsOverridden = false;
                            overridenSettings = undefined;
                            editorDirectiveAPI = undefined;
                        }
                    }
                };

                $scope.scopeModel.isSettingsOverridden = false;

                $scope.scopeModel.onEditorDirectiveReady = function (api) {
                    editorDirectiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, editorDirectiveAPI, undefined, setLoader, editorDirectiveReadyPromiseDeferred);
                };

                $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                    dataRecordStorageSelectorApi = api;
                    dataRecordStoragePromiseDeferred.resolve();
                };

                dataRecordStoragePromiseDeferred.promise.then(function () {
                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                });
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {
                    var promises = [];
                    if (payload) {
                        var extendedSettings = payload.extendedSettings;
                        if (extendedSettings != undefined) {
                            selectedIds = extendedSettings.DataRecordStorageId;
                            overridenSettings = extendedSettings.Settings;
                            if (overridenSettings != undefined) {
                                dataRecordTypeId = overridenSettings.OverriddenDataRecordTypeId;
                                dataStoreId = overridenSettings.OverriddenDataStoreId;
                                settings = overridenSettings.OverriddenSettings;
                            }
                            $scope.scopeModel.name = extendedSettings.OverriddenName;
                        }
                        
                        $scope.scopeModel.isSettingsOverridden = overridenSettings != undefined? true : false;
                        if ($scope.scopeModel.isSettingsOverridden) {
                            promises.push(loadDataRecordStorageEditorDirective());
                        }
                    }


                    promises.push(loadRecordStorageSelector());

                    function loadRecordStorageSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return dataRecordStorageSelectorApi.load(payloadSelector);
                    }

                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined;
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                directiveAPI.getData = function () {
                    var editorData;
                    var settingData;
                    if (editorDirectiveAPI != undefined) {
                        editorData = editorDirectiveAPI.getData();
                        if (editorData != undefined) {
                            settingData = {
                                OverriddenDataRecordTypeId: editorData.DataRecordTypeId,
                                OverriddenDataStoreId: editorData.DataStoreId,
                                OverriddenSettings: editorData.Settings
                            };
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.Business.DataRecordStorageOverriddenConfiguration ,Vanrise.GenericData.Business",
                        DataRecordStorageId : dataRecordStorageSelectorApi.getSelectedIds(),
                        OverriddenName : $scope.scopeModel.name,
                        Settings: settingData
                    };
                };

                return directiveAPI;
            }

            function loadDataRecordStorageEditorDirective() {
                var loadEditorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                editorDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                if (overridenSettings == undefined) {
                    getDataRecordStorage().then(function () {
                        settings = dataRecordStorageEntity.Settings;
                        dataRecordTypeId = dataRecordStorageEntity.DataRecordTypeId;
                        dataStoreId = dataRecordStorageEntity.DataStoreId;
                        devProjectId = dataRecordStorageEntity.DevProjectId;
                        loadSettings();
                    }).catch(function (error) {
                        loadEditorDirectivePromiseDeferred.reject();
                    });
                }
                else {
                    loadSettings();
                }

                function loadSettings() {
                    editorDirectiveReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = {
                                hideName:true,
                                dataRecordStorageEntity :{
                                    Settings: settings,
                                    DataRecordTypeId: dataRecordTypeId,
                                    DataStoreId: dataStoreId,
                                    DevProjectId: devProjectId
                                }
                            };
                            VRUIUtilsService.callDirectiveLoad(editorDirectiveAPI, directivePayload, loadEditorDirectivePromiseDeferred);
                        });
                }

                return loadEditorDirectivePromiseDeferred.promise;
            }

            function hideOverriddenSettingsEditor() {
                editorDirectiveAPI = undefined;
            }
          
            function getDataRecordStorage() {
                return VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorage(dataRecordStorageSelectorApi.getSelectedIds()).then(function (response) {
                    dataRecordStorageEntity = response;
                });
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataOverriddenconfigurationDatarecordstorage', OverriddenSettings);

})(app);
