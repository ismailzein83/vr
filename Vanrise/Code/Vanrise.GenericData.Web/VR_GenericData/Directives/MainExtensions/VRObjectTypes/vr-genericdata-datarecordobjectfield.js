(function (app) {

    'use strict';

    VRDataRecordObjectField.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function VRDataRecordObjectField(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordObjectField = new DataRecordObjectField($scope, ctrl, $attrs);
                dataRecordObjectField.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/VRObjectTypes/Templates/VRDataRecordObjectFieldTemplate.html"

        };
        function DataRecordObjectField($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectorPayload = {};

                    if (payload != undefined && payload.objectType != undefined) {
                        selectorPayload.dataRecordTypeId = payload.objectType.RecordTypeId;
                    }

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined) {
                        selectorPayload.selectedIds = payload.objectPropertyEvaluator.FieldName;
                        $scope.scopeModel.useDescription = payload.objectPropertyEvaluator != undefined ? payload.objectPropertyEvaluator.UseDescription : undefined;
                    }

                    var dataRecordObjectTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, dataRecordObjectTypeSelectorLoadDeferred);

                    return dataRecordObjectTypeSelectorLoadDeferred.promise.catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordFieldPropertyEvaluator, Vanrise.GenericData.MainExtensions",
                        FieldName: selectorAPI.getSelectedIds(),
                        UseDescription: $scope.scopeModel.useDescription
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordobjectfield', VRDataRecordObjectField);

})(app);