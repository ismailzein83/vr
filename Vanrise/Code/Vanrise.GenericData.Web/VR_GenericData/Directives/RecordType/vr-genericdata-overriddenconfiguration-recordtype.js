(function (app) {

    'use strict';

    OverriddenSettings.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordTypeAPIService'];

    function OverriddenSettings(UtilsService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {

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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/RecordType/Templates/OverriddenConfigurationDataRecordType.html'
        };

        function OverriddenSettingsDirective(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var overriddenSettings;
            var overriddenFields;
            var overriddenExtraFieldsEvaluator;


            var filter;
            var selectedIds;
            var dataRecordTypeEntity;
            var dataRecordTypeSelectorApi;
            var dataRecordTypePromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordFieldAPI;
            var dataRecordFieldReadyPromiseDeferred;
            var selectedPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.isSettingsOverriddenValuechanged = function () {
                    if ($scope.scopeModel.isSettingsOverridden == true) {
                        loadDataRecordFieldDirective();
                    }
                    else {
                        hideOverriddenSettingsEditor();
                    }
                };
                $scope.scopeModel.dataRecordTypeSelectionChanged = function (value) {
                    if (value != undefined) {
                        if (selectedPromiseDeferred != undefined) {
                            selectedPromiseDeferred.resolve();
                        }
                        else {
                            $scope.scopeModel.name = "";
                            $scope.scopeModel.isSettingsOverridden = false;
                            overriddenSettings = undefined;
                            settingsAPI = undefined;
                            $scope.scopeModel.selectedSetingsTypeConfig = undefined;
                        }
                    }
                };
                $scope.scopeModel.isSettingsOverridden = false;

                $scope.scopeModel.onDataRecordFieldDirectiveReady = function (api) {
                    dataRecordFieldAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModel, dataRecordFieldAPI, undefined, setLoader, dataRecordFieldReadyPromiseDeferred);
                };
                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorApi = api;
                    dataRecordTypePromiseDeferred.resolve();
                };
                dataRecordTypePromiseDeferred.promise.then(function () {
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
                            selectedIds = extendedSettings.DataRecordTypeId;
                            overriddenSettings = extendedSettings.OverriddenSettings;
                            overriddenFields = extendedSettings.OverriddenFields;
                            overriddenExtraFieldsEvaluator = extendedSettings.OverriddenExtraFieldsEvaluator;
                            $scope.scopeModel.name = extendedSettings.OverriddenName;
                        }
                        
                        $scope.scopeModel.isSettingsOverridden = (overriddenSettings != undefined || overriddenFields != undefined || overriddenExtraFieldsEvaluator != undefined) ? true : false;
                        if ($scope.scopeModel.isSettingsOverridden) {
                            promises.push(loadDataRecordFieldDirective());
                        }
                    }


                    promises.push(loadRecordTypeSelector());

                    function loadRecordTypeSelector() {
                        var payloadSelector = {
                            selectedIds: selectedIds,
                            filter: filter
                        };
                        return dataRecordTypeSelectorApi.load(payloadSelector);
                    }

                    selectedPromiseDeferred.promise.then(function () {
                        selectedPromiseDeferred = undefined;
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                directiveAPI.getData = function () {
                    var dataRecordTypeFields;
                    if (dataRecordFieldAPI != undefined) {
                        dataRecordTypeFields = dataRecordFieldAPI.getData();
                    }
                    return {
                        $type: "Vanrise.GenericData.Business.DataRecordTypeOverriddenConfiguration ,Vanrise.GenericData.Business",
                        DataRecordTypeId : dataRecordTypeSelectorApi.getSelectedIds(),
                        OverriddenName : $scope.scopeModel.name,
                        OverriddenFields : dataRecordTypeFields != undefined ? dataRecordTypeFields.Fields : undefined,
                        OverriddenSettings : dataRecordTypeFields != undefined ? dataRecordTypeFields.Settings : undefined,
                        OverriddenExtraFieldsEvaluator : dataRecordTypeFields != undefined ? dataRecordTypeFields.ExtraFieldsEvaluator : undefined
                    };
                };

                return directiveAPI;
            }

            function loadDataRecordFieldDirective() {
                var loadSettingDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                dataRecordFieldReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                if (overriddenSettings == undefined) {
                    getDataRecordType().then(function () {
                        overriddenSettings = dataRecordTypeEntity.Settings;
                        overriddenFields = dataRecordTypeEntity.Fields;
                        overriddenExtraFieldsEvaluator = dataRecordTypeEntity.ExtraFieldsEvaluator;
                        loadSettings();
                    }).catch(function (error) {
                        loadSettingDirectivePromiseDeferred.reject();
                    });
                }
                else {
                    loadSettings();
                }

                function loadSettings() {
                    dataRecordFieldReadyPromiseDeferred.promise
                        .then(function () {
                            var directivePayload = {
                                Settings: overriddenSettings,
                                Fields: overriddenFields,
                                ExtraFieldsEvaluator: overriddenExtraFieldsEvaluator
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordFieldAPI, directivePayload, loadSettingDirectivePromiseDeferred);
                        });
                }

                return loadSettingDirectivePromiseDeferred.promise;
            }

            function hideOverriddenSettingsEditor() {
                dataRecordFieldAPI = undefined;
            }
          
            function getDataRecordType() {
                return VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeSelectorApi.getSelectedIds()).then(function (response) {
                    dataRecordTypeEntity = response;
                    if (dataRecordTypeEntity != undefined)
                      $scope.scopeModel.name = dataRecordTypeEntity.Name;
                });
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataOverriddenconfigurationRecordtype', OverriddenSettings);

})(app);
