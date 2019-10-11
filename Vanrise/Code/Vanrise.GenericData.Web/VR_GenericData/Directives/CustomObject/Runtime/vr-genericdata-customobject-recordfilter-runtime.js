(function (app) {

    'use strict';

    CustomObjectRecordFilterRuntime.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService'];

    function CustomObjectRecordFilterRuntime(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CustomObjectRecordFilterRuntimeDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/CustomObject/Runtime/Templates/RecordFilterRuntimeTemplate.html'
        };

        function CustomObjectRecordFilterRuntimeDirectiveCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var fields = [];

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var fieldValue;
                    var fieldType;
                    var dataRecordTypeId;

                    if (payload != undefined) {
                        fieldValue = payload.fieldValue;

                        fieldType = payload.fieldType;
                        if (fieldType != undefined && fieldType.Settings != undefined) {
                            dataRecordTypeId = fieldType.Settings.DataRecordTypeID;
                        }

                        $scope.scopeModel.fieldTitle = payload.fieldTitle;
                    }

                    var rootPromiseNode = {
                        promises: [loadDataRecordTypeFields()],
                        getChildNode: function () {
                            return { promises: [loadRecordFilterDirective()] };
                        }
                    };

                    function loadDataRecordTypeFields() {
                        return VR_GenericData_GenericBEDefinitionAPIService.GetDataRecordTypeFields(dataRecordTypeId).then(function (response) {

                            if (response != undefined) {
                                for (var fieldName in response) {
                                    var currentItem = response[fieldName];

                                    fields.push({
                                        FieldName: currentItem.Name,
                                        FieldTitle: currentItem.Title,
                                        Type: currentItem.Type
                                    });
                                }
                            }
                        });
                    }

                    function loadRecordFilterDirective() {
                        var loadRecordFilterDirectiveDeferred = UtilsService.createPromiseDeferred();

                        recordFilterDirectiveReadyDeferred.promise.then(function () {

                            var recordFilterDirectivePayload = {
                                FilterGroup: fieldValue,
                                context: buildContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, loadRecordFilterDirectiveDeferred);
                        });

                        return loadRecordFilterDirectiveDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };


                api.getData = function () {
                    var recordFilter;
                    var data = recordFilterDirectiveAPI.getData();

                    if (data != undefined && data.filterObj != undefined) {
                        recordFilter = data.filterObj;
                    }

                    return recordFilter;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function buildContext() {
                var context = {
                    getFields: function () {
                        return fields;
                    }
                };
                return context;
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('vrGenericdataCustomobjectRecordfilterRuntime', CustomObjectRecordFilterRuntime);
})(app);