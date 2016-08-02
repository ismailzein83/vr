(function (app) {

    'use strict';

    ObjectPropertySelectiveSelective.$inject = ['VRCommon_VEObjectPropertyAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ObjectPropertySelectiveSelective(VRCommon_VEObjectPropertyAPIService, UtilsService, VRUIUtilsService) {
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
                var objectPropertySelective = new ObjectPropertySelective($scope, ctrl, $attrs);
                objectPropertySelective.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRObjectProperty/Templates/VRObjectPropertySelectiveTemplate.html'
        };

        function ObjectPropertySelective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var objectSelectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig = {};

                $scope.scopeModel.onObjectSelectorReady = function (api) {
                    objectSelectorAPI = api;
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
                    var objectVariables;

                    if (payload != undefined && payload.objectVariables != undefined) {
                        ctrl.objectVariables = payload.objectVariables;
                    }

                    //var getObjectPropertyTemplateConfigsPromise = getObjectPropertySelectiveTemplateConfigs();
                    //promises.push(getObjectPropertyTemplateConfigsPromise);

                    //if (objectVariables != undefined) {
                    //    var loadDirectivePromise = loadDirective();
                    //    promises.push(loadDirectivePromise);
                    //}


                    function getObjectPropertySelectiveTemplateConfigs() {
                        return VRCommon_VEObjectPropertyAPIService.GetObjectPropertyExtensionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (objectProperty != undefined && objectProperty.ConfigId != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, objectProperty.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload;
                            if (objectProperty != undefined && objectProperty.RecordTypeId != undefined) {
                                directivePayload = { recordTypeId: objectProperty.RecordTypeId }
                            }
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    //return UtilsService.waitMultiplePromises(promises);
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
    }

    app.directive('vrCommonObjecttypeSelector', ObjectPropertySelectiveSelective);

})(app);