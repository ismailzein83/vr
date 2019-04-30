//'use strict';

//app.directive('vrGenericadataRdbdatarecordstoragesettingsJoinotherrecordstorageStoragefieldeditor', ['UtilsService', 'VRUIUtilsService',
//    function (UtilsService, VRUIUtilsService) {

//        var directiveDefinitionObject = {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new JoinOtherRecordStorageFieldEditorCtol(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/RDBJoinSettings/Templates/JoinOtherRecordStorageFieldEditorTemplate.html"
//        };

//        function JoinOtherRecordStorageFieldEditorCtol(ctrl, $scope, attrs) {
//            this.initializeController = initializeController;

//            var sourceStorageFieldName;
//            var context;

//            var sourceStorageFieldNameSelectorAPI;
//            var sourceStorageFieldNameSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onSourceStorageFieldNameSelectorReady = function (api) {
//                    sourceStorageFieldNameSelectorAPI = api;
//                    sourceStorageFieldNameSelectorReadyDeferred.resolve();
//                };

//                UtilsService.waitMultiplePromises([sourceStorageFieldNameSelectorReadyDeferred.promise]).then(function () {
//                    defineAPI();
//                });
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        sourceStorageFieldName = payload.sourceStorageFieldName;
//                        context = payload.context;
//                    }

//                    initialPromises.push(loadSourceStorageFieldNameSelector());

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            return {
//                                promises: []
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    return $scope.scopeModel.sourceStorageFieldName;
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function loadSourceStorageFieldNameSelector() {
//                var loadSourceStorageFieldNameSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

//                sourceStorageFieldNameSelectorReadyDeferred.promise.then(function () {
//                    var sourceStorageFieldPayload = {
//                        dataRecordTypeId: context.getDataRecordTypeId()
//                    };
//                    if (sourceStorageFieldName != undefined) {
//                        directivePayload.selectedIds = sourceStorageFieldName;
//                    }
//                    VRUIUtilsService.callDirectiveLoad(sourceStorageFieldNameSelectorAPI, sourceStorageFieldPayload, loadSourceStorageFieldNameSelectorPromiseDeferred);
//                });
//                return loadSourceStorageFieldNameSelectorPromiseDeferred.promise;
//            }
//        }

//        return directiveDefinitionObject;
//    }]);