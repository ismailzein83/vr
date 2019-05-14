(function (app) {

    'use strict';

    IsEmptyFilterDefintionSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRLocalizationService'];

    function IsEmptyFilterDefintionSettingsDirective(UtilsService, VRUIUtilsService, VRLocalizationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new IsEmptyFilterDefintionSettingsController($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEFilterDefinition/Templates/IsEmptyFilterDefinitonTemplate.html"
        };


        function IsEmptyFilterDefintionSettingsController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            var settings;

            var fieldGridAPI;
            var fieldGridReadyDeferred = UtilsService.createPromiseDeferred();

            var fieldSelectorAPI;
            var fieldSelectoReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];
                $scope.scopeModel.showResourceKeyColumn = VRLocalizationService.isLocalizationEnabled();

                $scope.scopeModel.onFieldGridReady = function (api) {
                    fieldGridAPI = api;
                    fieldGridReadyDeferred.resolve();
                    defineAPI();
                };

                $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                    fieldSelectorAPI = api;
                    fieldSelectoReadyDeferred.resolve();
                };

                $scope.scopeModel.validateGrid = function () {
                    if ($scope.scopeModel.fields.length > 0 && checkDuplicateDefaultField())
                        return "One row only can be selected as default";

                    return null;
                }
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    fieldGridAPI.clearDataSource();
                    var initialPromises = [];
               
                    if (payload != undefined) {
                        context = payload.context;
                        settings = payload.settings;

                        if (settings != undefined) {
                            $scope.scopeModel.isRequired = settings.IsRequired;
                        }
                    }

                    initialPromises.push(loadFieldsSelector());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            $scope.scopeModel.isGridLoading = true;

                            var dataItemAllField = {
                                field: 'All',
                                payload: (settings != undefined) ? settings.AllField : undefined,
                                readyResourceKeyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadResourceKeyPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            addItemToGrid(dataItemAllField);

                            var dataItemNullField = {
                                field: 'Null',
                                payload: (settings != undefined) ? settings.NullField : undefined,
                                readyResourceKeyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadResourceKeyPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            addItemToGrid(dataItemNullField);

                            var dataItemNotNullField = {
                                field: 'Not Null',
                                payload: (settings != undefined) ? settings.NotNullField : undefined,
                                readyResourceKeyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadResourceKeyPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            addItemToGrid(dataItemNotNullField);

                            if ($scope.scopeModel.showResourceKeyColumn) {
                                directivePromises.push(dataItemAllField.loadResourceKeyPromiseDeferred.promise);
                                directivePromises.push(dataItemNullField.loadResourceKeyPromiseDeferred.promise);
                                directivePromises.push(dataItemNotNullField.loadResourceKeyPromiseDeferred.promise);
                            }

                            $scope.scopeModel.isGridLoading = false;

                            return {
                                promises: directivePromises,

                            };
                        }
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.IsEmptyFilterDefinitonSettings, Vanrise.GenericData.MainExtensions",
                        IsRequired: $scope.scopeModel.isRequired,
                        FieldName: fieldSelectorAPI.getSelectedIds(),
                        AllField: getFieldObj($scope.scopeModel.fields[0]),
                        NullField: getFieldObj($scope.scopeModel.fields[1]),
                        NotNullField: getFieldObj($scope.scopeModel.fields[2])
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function loadFieldsSelector() {
                var fieldSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                fieldSelectoReadyDeferred.promise.then(function () {
                    var dataRecordTypeFieldSelectorPayload = {
                        dataRecordTypeId: context.getDataRecordTypeId(),
                        selectedIds: settings != undefined ? settings.FieldName : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(fieldSelectorAPI, dataRecordTypeFieldSelectorPayload, fieldSelectorLoadDeferred);
                });
                return fieldSelectorLoadDeferred.promise;
            }

            function addItemToGrid(dataItem) {
                var payload = dataItem.payload;

                if (payload != undefined) {
                    dataItem.title = payload.Title;
                    dataItem.isDefault = payload.IsDefault;
                }

                dataItem.onResourceKeySelectorReady = function (api) {
                    dataItem.resourceKeyAPI = api;
                    dataItem.readyResourceKeyPromiseDeferred.resolve();
                };

                dataItem.readyResourceKeyPromiseDeferred.promise.then(function () {
                    var resourceKeySelectorPayload = {
                        selectedResourceKey: payload != undefined ? payload.Resourcekey : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.resourceKeyAPI, resourceKeySelectorPayload, dataItem.loadResourceKeyPromiseDeferred);
                });

                $scope.scopeModel.fields.push(dataItem);
            }

            function getFieldObj(column) {
                var obj = {};
                if (column != undefined) {
                    obj.Title = column.title;
                    obj.Resourcekey = (column.resourceKeyAPI) ? column.resourceKeyAPI.getResourceKey() : undefined;
                    obj.IsDefault = column.isDefault;
                }
                return obj;
            }

            function checkDuplicateDefaultField() {
                var fieldLength = $scope.scopeModel.fields.length;
                for (var i = 0; i < fieldLength; i++) {
                    var currentItem = $scope.scopeModel.fields[i];
                    for (var j = i + 1; j < fieldLength; j++) {
                        if (i != j && $scope.scopeModel.fields[j].isDefault && currentItem.isDefault)
                            return true;
                    }
                }
                return false;
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterdefinitionIsemptyfilter', IsEmptyFilterDefintionSettingsDirective);

})(app);