(function (app) {

    'use strict';

    assignmentdefinitionsSelectiveDirective.$inject = ['VR_AccountManager_AccountManagerDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService'];

    function assignmentdefinitionsSelectiveDirective(VR_AccountManager_AccountManagerDefinitionAPIService, UtilsService, VRUIUtilsService, VRNavigationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var assignmentdefinitionsSelective = new AssignmentdefinitionsSelective($scope, ctrl, $attrs);
                assignmentdefinitionsSelective.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AssignmentDefinitionSelectiveTemplate.html",


        };


        function AssignmentdefinitionsSelective($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var selectorAPI;

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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
                };
              
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var assignmentDefinitionEntity;
                    selectorAPI.clearDataSource();

                    if (payload != undefined)
                    {
                        assignmentDefinitionEntity = payload.assignmentDefinitionEntity;
                    }
                    if (assignmentDefinitionEntity != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }


                    var getAssignmentDefinitionConfigsPromise = getAssignmentDefinitionConfigs();
                    promises.push(getAssignmentDefinitionConfigsPromise);
                    function getAssignmentDefinitionConfigs() {
                        return VR_AccountManager_AccountManagerDefinitionAPIService.GetAssignmentDefinitionConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (assignmentDefinitionEntity != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, assignmentDefinitionEntity.ConfigId, 'ExtensionConfigurationId');
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
                                assignmentDefinitionEntity: assignmentDefinitionEntity
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
                    
                    }
                    return data;
                }

            }

        }
    }

    app.directive('vrAccountmanagerAssignmentdefinitionSelective', assignmentdefinitionsSelectiveDirective);

})(app);