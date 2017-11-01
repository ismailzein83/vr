(function (app) {

    'use strict';

    subViewsSelectiveDirective.$inject = ['VR_AccountManager_AccountManagerDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function subViewsSelectiveDirective(VR_AccountManager_AccountManagerDefinitionAPIService, UtilsService, VRUIUtilsService, VRNavigationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var subViewsSelective = new SubViewsSelective($scope, ctrl, $attrs);
                subViewsSelective.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/SubViews/Templates/subViewSelectiveTemplate.html",


        };


        function SubViewsSelective($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var selectorAPI;

            var context;

            var subViewDefinitionEntity;

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    var payload = {
                        context: getContext()
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
                };
               
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    selectorAPI.clearDataSource();
                    if (payload != undefined) {
                        context = payload.context;
                        subViewDefinitionEntity = payload.subViewEntity;

                    };
                    var getSubViewsDefinitionConfigsPromise = getSubViewsDefinitionConfigs();
                    promises.push(getSubViewsDefinitionConfigsPromise);

                    if (subViewDefinitionEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    };
                    function getSubViewsDefinitionConfigs() {
                        return VR_AccountManager_AccountManagerDefinitionAPIService.GetSubViewsDefinitionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                };
                                if (subViewDefinitionEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, subViewDefinitionEntity.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                assignmentDefinitionId:subViewDefinitionEntity.AccountManagerAssignementDefinitionId,
                                context: getContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    data = directiveAPI.getData();
                    if (data != undefined) {
                        data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                    };
                    return data;
                }
               
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }
    }

    app.directive('vrAccountmanagerSubviewSelective', subViewsSelectiveDirective);

})(app);