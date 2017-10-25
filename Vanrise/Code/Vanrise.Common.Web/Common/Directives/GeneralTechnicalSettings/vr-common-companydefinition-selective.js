(function (app) {

    'use strict';

    companydefinitionsSelectiveDirective.$inject = ['VRCommon_CompanyDefinitionsAPIService', 'UtilsService', 'VRUIUtilsService'];

    function companydefinitionsSelectiveDirective(VRCommon_CompanyDefinitionsAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var companydefinitionsSelective = new CompanydefinitionsSelective($scope, ctrl, $attrs);
                companydefinitionsSelective.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/GeneralTechnicalSettings/Templates/CompanyDefinitionSelective.html",


        };


        function CompanydefinitionsSelective($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();
            var directivePayload;

            

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.companyDefinitionConfigs = [];
                $scope.scopeModel.selectedCompanyDefinitionConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = {
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var companyDefinition;
                    var promises = [];
                    if (payload != undefined) {
                        companyDefinition = payload.companyDefinition;
                        var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var payloadDirective = {
                            };
                            if (companyDefinition != undefined) {
                                payloadDirective.companyDefinition = companyDefinition;
                            }
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                        });
                        promises.push(loadDirectivePromiseDeferred.promise);
                    }

                    var getCompanyDefinitionConfigsPromise = getCompanyDefinitionConfigs();
                    promises.push(getCompanyDefinitionConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getCompanyDefinitionConfigs() {
                        return VRCommon_CompanyDefinitionsAPIService.GetCompanyDefinitionConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.companyDefinitionConfigs.push(response[i]);
                                }

                                if (companyDefinition != undefined)
                                    $scope.scopeModel.selectedCompanyDefinitionConfig = UtilsService.getItemByVal($scope.scopeModel.companyDefinitionConfigs, companyDefinition.ConfigId, 'ExtensionConfigurationId');
                                else
                                    $scope.scopeModel.selectedCompanyDefinitionConfig = $scope.scopeModel.companyDefinitionConfigs[0];
                            }
                        });
                    }


                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.scopeModel.selectedCompanyDefinitionConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedCompanyDefinitionConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                }

            }

        }
    }

    app.directive('vrCommonCompanydefinitionsSelective', companydefinitionsSelectiveDirective);

})(app);