'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelist', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new dataRecordTypeListTypeCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/Templates/DataRecordTypeListFieldTypeTemplate.html';
            }
        };

        function dataRecordTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectedPromiseDeferred;

            var listRecordRuntimeViewTypeSelectorAPI;
            var listRecordRuntimeViewTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
               

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyDeferred.resolve();

                };

                $scope.scopeModel.onListRecordRuntimeViewTypeSelectorReady = function (api) {
                    listRecordRuntimeViewTypeSelectorAPI = api;
                    listRecordRuntimeViewTypeSelectorReadyDeferred.resolve();

                };

                $scope.scopeModel.onDataRecordTypeSelectionChanged = function (value) {

                    if (dataRecordTypeSelectorAPI) {
                        if (value) {
                            if (dataRecordTypeSelectedPromiseDeferred != undefined) {
                                dataRecordTypeSelectedPromiseDeferred = undefined;
                            }
                            else {
                                listRecordRuntimeViewTypeSelectorAPI.load();
                            }
                        }
                    }



                };
                defineAPI();

            }

            function loadDataRecordTypeSelector(payload) {
                var dataRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                dataRecordTypeSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, payload, dataRecordTypeSelectorLoadDeferred);
                });

                return dataRecordTypeSelectorLoadDeferred.promise;
            }

            function loadListRecordRuntimeViewTypeSelector(payload) {
                var listRecordRuntimeViewTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                listRecordRuntimeViewTypeSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(listRecordRuntimeViewTypeSelectorAPI, payload, listRecordRuntimeViewTypeSelectorLoadDeferred);
                });

                return listRecordRuntimeViewTypeSelectorLoadDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var dataRecordTypeId;
                    var runtimeViewType;
                    dataRecordTypeSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                    if (payload != undefined) {
                        dataRecordTypeId = payload.DataRecordTypeId;
                        runtimeViewType = payload.RuntimeViewType; 
                    }

                    loadDataRecordTypeSelector({ selectedIds: dataRecordTypeId });
                    loadListRecordRuntimeViewTypeSelector({ configId: runtimeViewType != undefined ? runtimeViewType.ConfigId : undefined });

                    
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldListDataRecordType, Vanrise.GenericData.MainExtensions",
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                        RuntimeViewType: listRecordRuntimeViewTypeSelectorAPI.getData()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);