﻿//(function (app) {

//    'use strict';

//    AccountBERuntimeSelectorFilterDirective.$inject = ['Retail_BE_AccountConditionAPIService', 'UtilsService', 'VRUIUtilsService'];

//    function AccountBERuntimeSelectorFilterDirective(Retail_BE_AccountConditionAPIService, UtilsService, VRUIUtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//                normalColNum: '@',
//                label: '@',
//                customvalidate: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new AccountBERuntimeSelectorFilterCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Account/MainExtensions/Templates/AccountBERuntimeSelectorFilterTemplate.html"
//        };

//        function AccountBERuntimeSelectorFilterCtor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var accountBEDefinitionId;

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
//                        accountBEDefinitionId: accountBEDefinitionId
//                    };
//                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
//                };
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    selectorAPI.clearDataSource();

//                    var promises = [];
//                    var accountCondition;

//                    if (payload != undefined) {
//                        accountBEDefinitionId = payload.accountBEDefinitionId;
//                        accountCondition = payload.beFilter;
//                    }

//                    var getAccountConditionTemplateConfigsPromise = getAccountConditionTemplateConfigs();
//                    promises.push(getAccountConditionTemplateConfigsPromise);

//                    if (accountCondition != undefined) {
//                        var loadDirectivePromise = loadDirective();
//                        promises.push(loadDirectivePromise);
//                    }


//                    function getAccountConditionTemplateConfigs() {
//                        return Retail_BE_AccountConditionAPIService.GetAccountConditionConfigs().then(function (response) {
//                            if (response != undefined) {
//                                for (var i = 0; i < response.length; i++) {
//                                    $scope.scopeModel.templateConfigs.push(response[i]);
//                                }
//                                if (accountCondition != undefined) {
//                                    $scope.scopeModel.selectedTemplateConfig =
//                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, accountCondition.ConfigId, 'ExtensionConfigurationId');
//                                }
//                            }
//                        });
//                    }
//                    function loadDirective() {
//                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

//                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

//                        directiveReadyDeferred.promise.then(function () {
//                            directiveReadyDeferred = undefined;
//                            var directivePayload = {
//                                accountBEDefinitionId: accountBEDefinitionId,
//                                accountCondition: accountCondition
//                            };
//                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
//                        });

//                        return directiveLoadDeferred.promise;
//                    }

//                    return UtilsService.waitMultiplePromises(promises);
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
//        }
//    }

//    app.directive('retailBeAccountbeRuntimeselectorfilter', AccountBERuntimeSelectorFilterDirective);

//})(app);