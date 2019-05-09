(function (app) {

    'use strict';

    PermanentfilterRecordfilter.$inject = ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordTypeAPIService'];

    function PermanentfilterRecordfilter(VRUIUtilsService, UtilsService, VR_GenericData_DataRecordTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var permanentFilterRecordFilter = new PermanentFilterRecordFilter($scope, ctrl, $attrs);
                permanentFilterRecordFilter.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordStorage/MainExtensions/Templates/PermanentfilterRecordfilter.html'
        };

        function PermanentFilterRecordFilter($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var dataRecordTypeId;
            var permanentFilterRecordFilterGroup;
            var dataRecordTypeFields;
            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                    defineAPI();
                };
            }
            function loadRecordFilterDirective() {
                var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                recordFilterDirectiveReadyDeferred.promise.then(function () {

                    var recordFilterDirectivePayload = {
                        context: buildRecordFilterContext(dataRecordTypeFields),
                        FilterGroup: permanentFilterRecordFilterGroup
                    };
                    VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                });
                return recordFilterDirectiveLoadDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        dataRecordTypeId = payload.dataRecordTypeId;
                        permanentFilterRecordFilterGroup = payload.settings != undefined ? payload.settings.RecordFilterGroup : undefined;
                    }
                    var rootPromiseNode = { promises: [] };

                    if (dataRecordTypeId != undefined) {

                        var dataRecordTypePromise = VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                            if (response != undefined) {
                                dataRecordTypeFields = response.Fields;
                            }
                        });
                        rootPromiseNode.promises.push(dataRecordTypePromise);
                        rootPromiseNode.getChildNode = function () {
                            return { promises: [loadRecordFilterDirective()] };
                        };
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Entities.FilterGroupDataRecordStoragePermanentFilter,Vanrise.GenericData.Entities",
                        RecordFilterGroup: recordFilterDirectiveAPI.getData().filterObj
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function buildRecordFilterContext(outputFields) {
                var context = {
                    getFields: function () {
                        var fields = [];

                        if (outputFields) {
                            for (var i = 0; i < outputFields.length; i++) {
                                var field = outputFields[i];
                                fields.push({
                                    FieldName: field.Name,
                                    FieldTitle: field.Title,
                                    Type: field.Type
                                });
                            }
                        }
                        return fields;
                    }
                };
                return context;
            }
        }
    }

    app.directive('vrGenericdataDatarecordstoragePermanentfilterRecordfilter', PermanentfilterRecordfilter);

})(app);