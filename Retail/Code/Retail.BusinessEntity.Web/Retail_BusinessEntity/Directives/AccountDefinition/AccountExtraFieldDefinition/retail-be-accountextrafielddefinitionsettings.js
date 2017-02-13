(function (app) {

    'use strict';

    AccountExtraFieldDefinitionSettingsDirective.$inject = ['Retail_BE_AccountBEDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AccountExtraFieldDefinitionSettingsDirective(Retail_BE_AccountBEDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountExtraFieldDefinitionSettings = new AccountExtraFieldDefinitionSettings($scope, ctrl, $attrs);
                accountExtraFieldDefinitionSettings.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function AccountExtraFieldDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

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
                    var directivePayload = {
                        accountBEDefinitionId: accountBEDefinitionId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};


                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];

                    var accountExtraFieldDefinitionSettings;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountExtraFieldDefinitionSettings = payload.accountExtraFieldDefinitionSettings;
                    }

                    if (accountExtraFieldDefinitionSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    var getAccountExtraFieldDefinitionSettingsTemplateConfigsPromise = getAccountExtraFieldDefinitionSettingsTemplateConfigs();
                    promises.push(getAccountExtraFieldDefinitionSettingsTemplateConfigsPromise);


                    function getAccountExtraFieldDefinitionSettingsTemplateConfigs() {
                        return Retail_BE_AccountBEDefinitionAPIService.GetAccountExtraFieldDefinitionSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (accountExtraFieldDefinitionSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, accountExtraFieldDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
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
                                accountBEDefinitionId: accountBEDefinitionId,
                                accountExtraFieldDefinitionSettings: accountExtraFieldDefinitionSettings
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
            var withemptyline = 'withemptyline';

            var label = "label='Extra Field Type'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }

            var template =
                '<vr-row>'
                    + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                        + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                            + ' datasource="scopeModel.templateConfigs"'
                            + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                            + ' datavaluefield="ExtensionConfigurationId"'
                            + ' datatextfield="Title"'
                            + label
                            + ' isrequired="true"'
                            + 'hideremoveicon>'
                        + '</vr-select>'
                    + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"></vr-directivewrapper>';

            return template;
        }
    }

    app.directive('retailBeAccountextrafielddefinitionsettings', AccountExtraFieldDefinitionSettingsDirective);

})(app);