'use strict';

app.directive('vrGenericdataRdbrecordstoragesettingsFilterFiltergroup', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FilterGroupCtol(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/Filters/Templates/RecordFilterGroupTemplate.html"
        };

        function FilterGroupCtol(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var context;
            var filter;

            var recordFilterGroupDirectiveAPI;
            var recordFilterGroupDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterGroupDirectiveAPI = api;
                    recordFilterGroupDirectiveReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([recordFilterGroupDirectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        filter = payload.filter;
                    }

                    initialPromises.push(loadRecordFilterGroup());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.RDBDataStorage.MainExtensions.Filters.RDBDataRecordStorageSettingsFilterGroup, Vanrise.GenericData.RDBDataStorage",
                        RecordFilterGroup: recordFilterGroupDirectiveAPI.getData().filterObj
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadRecordFilterGroup() {
                var recordFilterGroupLoadDeferred = UtilsService.createPromiseDeferred();

                recordFilterGroupDirectiveReadyDeferred.promise.then(function () {
                    var recordFilterGroupDirectivePayload = {
                        context: context,
                        FilterGroup: filter != undefined ? filter.RecordFilterGroup : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(recordFilterGroupDirectiveAPI, recordFilterGroupDirectivePayload, recordFilterGroupLoadDeferred);
                });
                return recordFilterGroupLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);