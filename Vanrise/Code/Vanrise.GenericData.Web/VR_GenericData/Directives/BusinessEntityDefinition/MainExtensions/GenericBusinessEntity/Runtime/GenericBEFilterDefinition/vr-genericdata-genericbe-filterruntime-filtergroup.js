(function (app) {

    'use strict';

    FilterGroupFilterRuntimeSettingsDirective.$inject = ['UtilsService','VRUIUtilsService','VR_GenericData_DataRecordTypeAPIService'];

    function FilterGroupFilterRuntimeSettingsDirective(UtilsService,VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FilterGroupFilterRuntimeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/FilterGroupFilterRuntimeTemplate.html"
        };


        function FilterGroupFilterRuntimeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var dataRecordTypeId;

            var dataRecordType;

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var definitionSettings;

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
                    var promises = [];
                    if (payload != undefined) {
                        dataRecordTypeId = payload.dataRecordTypeId;
                        definitionSettings = payload.settings;
                    }
                    var loadDirectivePromise = UtilsService.createPromiseDeferred();
                    promises.push(loadDirectivePromise.promise);

                    loadDataRecordTypeFields().then(function () {
                        loadRecordFilterDirective().then(function () {
                            loadDirectivePromise.resolve();
                        }).catch(function (error) {
                            loadDirectivePromise.reject(error);
                        });
                    }).catch(function (error) {
                        loadDirectivePromise.reject(error);
                    });
      
                    return UtilsService.waitMultiplePromises(promises);
                };

                function loadDataRecordTypeFields() {
                  return  VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                        dataRecordType = response;
                    });
                }
                function loadRecordFilterDirective() {
                    var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    recordFilterDirectiveReadyDeferred.promise.then(function () {
                        var recordFilterDirectivePayload = {
                            context: buildContext()
                        };
                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    });

                    return recordFilterDirectiveLoadDeferred.promise;
                }
                api.getData = function () {
                    var data = recordFilterDirectiveAPI.getData();
                    return {
                        RecordFilter: data != undefined? data.filterObj:undefined
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function buildContext() {
                var context = {
                    getFields: function () {

                        var fields = [];
                        if (dataRecordType != undefined && dataRecordType.Fields && definitionSettings.AvailableFieldNames != undefined) {
                            for (var i = 0; i < definitionSettings.AvailableFieldNames.length; i++)
                            {
                                var field = definitionSettings.AvailableFieldNames[i];
                                var fieldType = UtilsService.getItemByVal(dataRecordType.Fields, field, "Name");
                                if(fieldType != undefined)
                                {
                                    fields.push({
                                        FieldName: fieldType.Name,
                                        FieldTitle: fieldType.Title,
                                        Type: fieldType.Type
                                    });
                                }
                            }
                        }
                        return fields;
                    }
                };
                return context;
            };
        }
    }

    app.directive('vrGenericdataGenericbeFilterruntimeFiltergroup', FilterGroupFilterRuntimeSettingsDirective);

})(app);