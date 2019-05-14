//(function (app) {

//    'use strict';

//    DataRecordStorageRDBFilter.$inject = ['VRNotificationService', 'VRUIUtilsService', 'UtilsService', 'VR_GenericData_RDBDataRecordStorageAPIService'];

//    function DataRecordStorageRDBFilter(VRNotificationService, VRUIUtilsService, UtilsService, VR_GenericData_RDBDataRecordStorageAPIService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                normalColNum: '@'
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctrol = new DataRecordStorageRDBFilterController($scope, ctrl, $attrs);
//                ctrol.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {
//            },
//            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordStorage/Templates/RDBDataRecordStorageFilterTemplate.html'
//        };

//        function DataRecordStorageRDBFilterController($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var filter;
//            var context = [];
//            var selectorAPI;

//            var directiveAPI;
//            var directiveReadyDeferred;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.templateConfigs = [];
//                $scope.scopeModel.selectedTemplateConfig;

//                $scope.scopeModel.onSelectorReady = function (api) {
//                    selectorAPI = api;
//                    defineAPI();
//                };

//                $scope.scopeModel.onDirectiveReady = function (api) {
//                    directiveAPI = api;
//                    var setLoader = function (value) {
//                        $scope.scopeModel.isLoadingDirective = value;
//                    };
//                    var directivePayload = {
//                        context: getContext()
//                    };
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
//                };
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    selectorAPI.clearDataSource();

//                    var initialPromises = [];

//                    if (payload != undefined) {
//                        filter = payload.filter;
//                        context = payload.context;
//                    }

//                    initialPromises.push(getRDBDataRecordStorageSettingsFilterConfigs());

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            if (filter != undefined) {
//                                directivePromises.push(loadDirective());
//                            }

//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    var data;
//                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
//                        data = directiveAPI.getData();
//                        if (data != undefined) {
//                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
//                        }
//                    }
//                    return data;
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }

//            function getRDBDataRecordStorageSettingsFilterConfigs() {
//                return VR_GenericData_RDBDataRecordStorageAPIService.GetRDBDataRecordStorageSettingsFilterConfigs().then(function (response) {
//                    if (response != undefined) {
//                        for (var i = 0; i < response.length; i++) {
//                            $scope.scopeModel.templateConfigs.push(response[i]);
//                        }

//                        if (filter != undefined) {
//                            $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, filter.ConfigId, 'ExtensionConfigurationId');
//                        }
//                    }
//                });
//            }

//            function loadDirective() {
//                directiveReadyDeferred = UtilsService.createPromiseDeferred();
//                var directiveLoadDeferred = UtilsService.createPromiseDeferred();

//                directiveReadyDeferred.promise.then(function () {
//                    directiveReadyDeferred = undefined;
//                    var directivePayload = {
//                        filter: filter,
//                        context: getContext()
//                    };
//                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
//                });

//                return directiveLoadDeferred.promise;
//            }

//            function getContext() {
//                var currentContext = context;
//                if (currentContext == undefined)
//                    currentContext = {};
//                return currentContext;
//            }
//        }
//    }

//    app.directive('vrGenericdataDatarecordstorageRdbFilter', DataRecordStorageRDBFilter);

//})(app);