'use strict';

app.directive('vrGenericadataRdbdatarecordstoragesettingsJoinotherrecordstorageStoragefieldeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordStorageAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordStorageAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new JoinOtherRecordStorageFieldEditorCtol(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/RDBJoinSettings/Templates/JoinOtherRecordStorageFieldEditorTemplate.html"
        };

        function JoinOtherRecordStorageFieldEditorCtol(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var fieldName;
            var settings;
            var dataRecordTypeId;

            var sourceStorageFieldNameSelectorAPI;
            var sourceStorageFieldNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSourceStorageFieldNameSelectorReady = function (api) {
                    sourceStorageFieldNameSelectorAPI = api;
                    sourceStorageFieldNameSelectorReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([sourceStorageFieldNameSelectorReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        fieldName = payload.fieldName;
                        settings = payload.settings;
                    }

                    initialPromises.push(getDataRecordTypeId());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            if (dataRecordTypeId != undefined) {
                                directivePromises.push(loadSourceStorageFieldNameSelector());
                            }

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return sourceStorageFieldNameSelectorAPI.getSelectedIds();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function getDataRecordTypeId() {
                if (settings != undefined && settings.RecordStorageId != undefined) {
                    return VR_GenericData_DataRecordStorageAPIService.GetDataRecordStorage(settings.RecordStorageId).then(function (response) {
                        if (response != undefined)
                            dataRecordTypeId = response.DataRecordTypeId;
                    });
                }
            }

            function loadSourceStorageFieldNameSelector() {
                var loadSourceStorageFieldNameSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                sourceStorageFieldNameSelectorReadyDeferred.promise.then(function () {
                    var sourceStorageFieldPayload = {
                        dataRecordTypeId: dataRecordTypeId
                    };
                    if (fieldName != undefined) {
                        sourceStorageFieldPayload.selectedIds = fieldName;
                    }
                    VRUIUtilsService.callDirectiveLoad(sourceStorageFieldNameSelectorAPI, sourceStorageFieldPayload, loadSourceStorageFieldNameSelectorPromiseDeferred);
                });
                return loadSourceStorageFieldNameSelectorPromiseDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);