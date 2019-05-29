'use strict';
app.directive('vrGenericdataFieldtypeDatarecord', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new dataRecordTypeCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/Templates/DataRecordFieldTypeTemplate.html';
            }
        };

        function dataRecordTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyDeferred.resolve();

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

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var dataRecordTypeId;
                    var promises = [];
                    var rootPromiseNode = {
                        promises: promises
                    };
                   
                    if (payload != undefined) {
                        dataRecordTypeId = payload.DataRecordTypeId;
                    }
                    promises.push(loadDataRecordTypeSelector({ selectedIds: dataRecordTypeId }));
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldDataRecordType, Vanrise.GenericData.MainExtensions",
                        DataRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds(),
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);