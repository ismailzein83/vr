//(function (app) {

//    'use strict';

//    DataproviderTableSelector.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'UtilsService', 'VRUIUtilsService'];

//    function DataproviderTableSelector(VR_Analytic_AnalyticConfigurationAPIService, UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//                normalColNum: '@',
//                label: '@'
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var dataproviderTableSelector = new DataproviderTableSelector($scope, ctrl, $attrs);
//                dataproviderTableSelector.initializeController();
//            },
//            controllerAs: "providerCtrl",
//            bindToController: true,
//            templateUrl: '/Client/Modules/Analytic/Directives/AnalyticDataProvider/AnalyticDataProviderTable/Templates/AnalyticDataProviderTableTemplate.html'
//        };

//        function DataproviderTableSelector($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

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
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
//                };
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    selectorAPI.clearDataSource();

//                    var promises = [];
//                    var analyticDataProviderTable; 

//                    if (payload != undefined) {
//                        analyticDataProviderTable = payload.analyticDataProviderTable;
//                    }

//                    if (analyticDataProviderTable != undefined) {
//                        var loadDirectivePromise = loadDirective();
//                        promises.push(loadDirectivePromise);
//                    }

//                    var getAnalyticDataProviderTableConfigsPromise = getAnalyticDataProviderTableConfigs();
//                    promises.push(getAnalyticDataProviderTableConfigsPromise);

//                    function getAnalyticDataProviderTableConfigs() {
//                        return VR_Analytic_AnalyticConfigurationAPIService.GetAnalyticDataProviderTableConfigs().then(function (response) {
//                            if (response != null) {
//                                for (var i = 0; i < response.length; i++) {
//                                    $scope.scopeModel.templateConfigs.push(response[i]);
//                                }
//                                if (analyticDataProviderTable != undefined) {
//                                    $scope.scopeModel.selectedTemplateConfig =
//                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, analyticDataProviderTable.ConfigId, 'ExtensionConfigurationId');

//                                }
//                            }
//                        });
//                    }

//                    function loadDirective() {
//                        $scope.scopeModel.isLoadingDirective = true;
//                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

//                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

//                        directiveReadyDeferred.promise.then(function () {
//                            directiveReadyDeferred = undefined;
//                            var directivePayload = { analyticDataProviderTable: analyticDataProviderTable };
//                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
//                            $scope.scopeModel.isLoadingDirective = false;
//                        });
                        
//                        return directiveLoadDeferred.promise;
//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {
//                    var data;
//                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
//                        data = directiveAPI.getData();
//                    }
//                    return data;
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }
//        }
//    }
    
//    app.directive('vrDataproviderTableSelector', DataproviderTableSelector);

//})(app);