(function (app) {

    'use strict';

    RouteSyncDefinitionSettingsDirective.$inject = ['WhS_RouteSync_RouteSyncDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function RouteSyncDefinitionSettingsDirective(WhS_RouteSync_RouteSyncDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var routeSyncDefinitionSettings = new RouteSyncDefinitionSettings($scope, ctrl, $attrs);
                routeSyncDefinitionSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function RouteSyncDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    var routeReader;

                    if (payload != undefined) {
                        routeReader = payload.routeReader;
                    }

                    var getRouteSyncDefinitionSettingsTemplateConfigsPromise = getRouteReaderTemplateConfigs();
                    promises.push(getRouteSyncDefinitionSettingsTemplateConfigsPromise);

                    if (routeReader != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getRouteReaderTemplateConfigs() {
                        return WhS_RouteSync_RouteSyncDefinitionAPIService.GetRouteReaderExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (routeReader != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, routeReader.ConfigId, 'ExtensionConfigurationId');
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
                                routeReader: routeReader
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }

        function getTamplate(attrs) {

            var template =
                      ' <vr-columns width="1/2row">'
                        + '<vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + ' isrequired="ctrl.isrequired"'
                            + ' label="Route Reader"'
                            + ' hideremoveicon>'
                        + '</vr-select>'
                    + ' </vr-columns>'
                    + ' <vr-columns width="1/2row">'
                        + ' <vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                                + ' on-ready="scopeModel.onDirectiveReady" isrequired="ctrl.isrequired" normal-col-num="{{ctrl.normalColNum}}" customvalidate="ctrl.customvalidate">'
                        + ' </vr-directivewrapper>'
                    + ' </vr-columns>'
            return template;
        }
    }

    app.directive('whsRoutesyncRoutereaderSelective', RouteSyncDefinitionSettingsDirective);

})(app);