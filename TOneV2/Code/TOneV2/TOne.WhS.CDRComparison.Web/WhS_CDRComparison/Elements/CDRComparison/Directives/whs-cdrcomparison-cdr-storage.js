"use strict";

app.directive("whsCdrcomparisonCdrStorage", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_GenericData_DataRecordTypeAPIService",
function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_DataRecordTypeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new CDRComparisonCDRStorage($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_CDRComparison/Elements/CDRComparison/Directives/Templates/CDRComparisonCDRStorage.html"
    };

    function CDRComparisonCDRStorage($scope, ctrl) {

        var context;
        var dataRecordTypeEntity;
        var dataRecordTypeId = "6cf5f7ad-5123-45d2-b47f-eca613d454f7";

        var dataRecordStorageSelectorAPI;
        var dataRecordStorageSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var recordFilterAPI;
        var recordFilterReadyPromiseDeffered = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;

        function initializeController() {

            $scope.scopeModel = {};
            $scope.scopeModel.onDataRecordStorageSelectorReady = function (api) {
                dataRecordStorageSelectorAPI = api;
                dataRecordStorageSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterAPI = api;
                recordFilterReadyPromiseDeffered.resolve();
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                var promises = [];
                if (payload != undefined) {
                    if (payload.ValueParser != undefined) {
                        $scope.scopeModel.fieldName = payload.ValueParser.FieldName;
                    }
                    context = payload.context;
                }

                promises.push(loadDataRecordStorage());
                function loadDataRecordStorage() {
                    return dataRecordStorageSelectorReadyPromiseDeferred.promise.then(function () {
                        var payload = { DataRecordTypeId: "6cf5f7ad-5123-45d2-b47f-eca613d454f7" };
                        VRUIUtilsService.callDirectiveLoad(dataRecordStorageSelectorAPI, payload, undefined);
                    });
                }

                promises.push(getDataRecordType());
                function getDataRecordType() {
                    var defferd = UtilsService.createPromiseDeferred();
                    defferd.resolve();
                    return defferd.promise.then(function () {
                        VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                            dataRecordTypeEntity = response;
                            var recordFilterPayload = { context: getContext() };
                            VRUIUtilsService.callDirectiveLoad(recordFilterAPI, recordFilterPayload, recordFilterReadyPromiseDeffered);

                        });
                    });
                }

                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.CDRComparison.Business.CDRRecordStorageSource,TOne.WhS.CDRComparison.Business",
                    DataRecordStorageIds: dataRecordStorageSelectorAPI.getSelectedIds(),
                    FromDate: $scope.scopeModel.fromDateTime,
                    ToDate: $scope.scopeModel.toDateTime,
                    FilterGroup: recordFilterAPI.getData()
                };

            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined) {
                currentContext = {};
            }

            currentContext.getFields = function () {
                var fields = [];
                for (var i = 0 ; i < dataRecordTypeEntity.Fields.length; i++) {
                    var field = dataRecordTypeEntity.Fields[i];
                    fields.push({
                        FieldName: field.Name,
                        FieldTitle: field.Title,
                        Type: field.Type
                    });
                }
                return fields;
            };

            return currentContext;
        }

    }

    return directiveDefinitionObject;

}]);