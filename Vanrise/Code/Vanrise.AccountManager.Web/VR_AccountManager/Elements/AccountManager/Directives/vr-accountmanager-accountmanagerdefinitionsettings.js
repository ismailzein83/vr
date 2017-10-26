(function (app) {

    'use strict';

    accountmanagerdefinitionsSelectiveDirective.$inject = ['VR_AccountManager_AccountManagerAPIService', 'UtilsService', 'VRUIUtilsService'];

    function accountmanagerdefinitionsSelectiveDirective(VR_AccountManager_AccountManagerAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountmanagerdefinitionsSelective = new AccountmanagerdefinitionsSelective($scope, ctrl, $attrs);
                accountmanagerdefinitionsSelective.initializeController();
            },

            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_AccountManager/Elements/AccountManager/Directives/Template/AccountManagerDefinitionSettings.html",


        };


        function AccountmanagerdefinitionsSelective($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();
            var directivePayload;



            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.accountManagerDefinitionConfigs = [];
                $scope.scopeModel.selectedAccountManagerDefinitionConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                   
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
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    var promises = [];
                    if (payload != undefined) {
                        extendedSettings = payload.extendedSettings;
                        var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var payloadDirective = {
                            };
                            if (extendedSettings != undefined) {
                                payloadDirective.extendedSettings = extendedSettings;
                            }
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                        });
                        promises.push(loadDirectivePromiseDeferred.promise);
                    }

                    var getAccountManagerDefinitionConfigsPromise = getAccountManagerDefinitionConfigs();
                    promises.push(getAccountManagerDefinitionConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getAccountManagerDefinitionConfigs() {
                        return VR_AccountManager_AccountManagerAPIService.GetAccountManagerDefinitionConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.accountManagerDefinitionConfigs.push(response[i]);
                                }

                                if (extendedSettings != undefined) {
                                    $scope.scopeModel.selectedAccountManagerDefinitionConfig = UtilsService.getItemByVal($scope.scopeModel.accountManagerDefinitionConfigs, extendedSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                                
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
                    if ($scope.scopeModel.selectedAccountManagerDefinitionConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedAccountManagerDefinitionConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                }

            }

        }
    }

    app.directive('vrAccountmanagerAccountmanagerdefinitionsettings', accountmanagerdefinitionsSelectiveDirective);

})(app);