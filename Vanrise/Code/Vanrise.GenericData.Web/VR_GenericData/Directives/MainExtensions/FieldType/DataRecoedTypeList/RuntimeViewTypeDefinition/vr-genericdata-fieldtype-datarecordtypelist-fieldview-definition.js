'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistFieldviewDefinition', ['VRUIUtilsService', 'UtilsService','VR_GenericData_DataRecordFieldAPIService',
    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new fieldViewTypeListTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeDefinition/Templates/FieldViewTypeDefinitionTemplate.html';
            }
        };

        function fieldViewTypeListTypeCtor(ctrl, $scope) {
            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var settings;
            var dataRecordTypeFields = [];

            $scope.scopeModel = {};

            function initializeController() {

                $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
          
            function getDataRecordFieldsInfo(dataRecordTypeId) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId, null).then(function (response) {
                    dataRecordTypeFields.length = 0;
                    if (response != undefined)
                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            dataRecordTypeFields.push(currentField.Entity);
                        }
                });
            }

            function loadFieldsSelector(dataRecordTypeId, selectedDataRecordType) {
                var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                var dataRecordTypeFieldsSelectorPayload = {
                    dataRecordTypeId: dataRecordTypeId
                };
                if (selectedDataRecordType != undefined) {
                    dataRecordTypeFieldsSelectorPayload.selectedIds = selectedDataRecordType;
                }
                VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                return dataRecordTypeFieldsSelectorLoadDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    settings = payload.settings;
                    if (settings != undefined )
                        settings = settings.RecordField;
                    var dataRecordTypeId = payload.dataRecordTypeId;
                    var selectedDataRecordType = settings != undefined ? settings.Name : undefined;
                    var rootPromiseNode = {
                        promises: [getDataRecordFieldsInfo(dataRecordTypeId)]
                    };

                    rootPromiseNode.getChildNode = function () {
                        return { promises: [loadFieldsSelector(dataRecordTypeId, selectedDataRecordType)] };
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldViewListRecordRuntimeViewType, Vanrise.GenericData.MainExtensions",
                        RecordField: {
                            Name: dataRecordTypeFieldsSelectorAPI.getSelectedIds()
                        }
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);