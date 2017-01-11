(function (app) {

    'use strict';

    VRDataRecordObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function VRDataRecordObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordObjectType = new DataRecordObjectType($scope, ctrl, $attrs);
                dataRecordObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/VRObjectTypes/Templates/VRDataRecordObjectTypeTemplate.html"

        };
        function DataRecordObjectType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var context;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordObjectTypeSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDataRecordObjectTypeSelectionChanged = function (selectedItem) {
                    if (selectedItem != undefined) {
                        context.canDefineProperties(true);
                    }
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectorPayload;

                    if (payload != undefined) {
                        context = payload.context;

                        context.canDefineProperties(false);
                    }

                    if (payload != undefined && payload.objectType != undefined) {
                        selectorPayload = { selectedIds: payload.objectType.RecordTypeId };
                    }

                    var dataRecordObjectTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, dataRecordObjectTypeSelectorLoadDeferred);

                    return dataRecordObjectTypeSelectorLoadDeferred.promise
                };

                api.getData = function () {
                    var data = {
                        $type: "Vanrise.GenericData.MainExtensions.VRObjectTypes.VRDataRecordObjectType, Vanrise.GenericData.MainExtensions",
                        RecordTypeId: selectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataDatarecordobjecttype', VRDataRecordObjectType);

})(app);