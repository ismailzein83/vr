(function (app) {

    'use strict';

    ListEditorDefinition.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function ListEditorDefinition(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var listEditorDefinition = new ListEditorDefinition($scope, ctrl, $attrs);
                listEditorDefinition.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl:"/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/ListEditorDefinitionSettingTemplate.html"

        };
        function ListEditorDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    defineAPI();
                };


            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var context;
                    var recordTypeId;
                    var settings;
                    var selectedDataRecordType;

                    if (payload != undefined) {
                        context = payload.context;
                        settings = payload.settings;
                        if (settings != undefined) {
                            selectedDataRecordType = settings.FieldName;
                            $scope.rootFQTN = settings.RootFQTN;
                            $scope.rootFieldName = settings.RootFieldName;
                            $scope.childFQTN = settings.ChildFQTN;
                            $scope.childFieldName = settings.ChildFieldName;
                        }
                        if (context == undefined) return;
                        recordTypeId = context.getDataRecordTypeId();
                    }

                    var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    
                    if (recordTypeId) {
                        var dataRecordTypeFieldsSelectorPayload = {
                            dataRecordTypeId: recordTypeId
                        };
                        if (selectedDataRecordType != undefined) {
                            dataRecordTypeFieldsSelectorPayload.selectedIds = selectedDataRecordType;
                        }
                        dataRecordTypeFieldsSelectorPayload.dataRecordTypeId = recordTypeId;
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                    }
                    else {
                        dataRecordTypeFieldsSelectorLoadDeferred.resolve();
                    }


                    return dataRecordTypeFieldsSelectorLoadDeferred.promise;

                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.GenericData.MainExtensions.ListEditorDefinitionSetting,Vanrise.GenericData.MainExtensions",
                        FieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                        RootFQTN: $scope.rootFQTN,
                        RootFieldName: $scope.rootFieldName,
                        ChildFQTN: $scope.childFQTN,
                        ChildFieldName: $scope.childFieldName
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataListeditorDefinition', ListEditorDefinition);

})(app);