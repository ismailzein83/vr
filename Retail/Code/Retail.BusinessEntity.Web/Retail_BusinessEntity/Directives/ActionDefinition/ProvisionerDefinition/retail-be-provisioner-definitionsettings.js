(function (app) {

    'use strict';

    ProvisionerDefinitionsettings.$inject = ['Retail_BE_ActionDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ProvisionerDefinitionsettings(Retail_BE_ActionDefinitionAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var provisionerDefinition = new ProvisionerDefinition($scope, ctrl, $attrs);
                provisionerDefinition.initializeController();
            },
            controllerAs: "provisionerDefinitionCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function ProvisionerDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var accountBEDefinitionId;
            function initializeController() {
                $scope.extensionConfigs = [];
                $scope.selectedExtensionConfig;
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = {
                        accountBEDefinitionId: accountBEDefinitionId
                    };
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var provisionerDefinitionSettings;

                    if (payload != undefined) {
                        provisionerDefinitionSettings = payload.provisionerDefinitionSettings;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                    }

                    var loadProvisionerDefinitionExtensionConfigsPromise = loadProvisionerDefinitionExtensionConfigs();
                    promises.push(loadProvisionerDefinitionExtensionConfigsPromise);

                    if (provisionerDefinitionSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function loadProvisionerDefinitionExtensionConfigs() {
                        return Retail_BE_ActionDefinitionAPIService.GetProvisionerDefinitionExtensionConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.extensionConfigs.push(response[i]);
                                }
                                if (provisionerDefinitionSettings != undefined)
                                    $scope.selectedExtensionConfig = UtilsService.getItemByVal($scope.extensionConfigs, provisionerDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
                                else if ($scope.extensionConfigs.length > 0)
                                    $scope.selectedExtensionConfig = $scope.extensionConfigs[0];
                            }
                        });
                    }

                    function loadDirective() {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                provisionerDefinitionSettings: provisionerDefinitionSettings,
                                accountBEDefinitionId: accountBEDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = directiveAPI.getData();
                    if (data != undefined)
                        data.ConfigId = $scope.selectedExtensionConfig.ExtensionConfigurationId;
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTamplate(attrs) {
            var label = "label='Type'";

            if (attrs.hidelabel != undefined) {
                label = "label='Types'";
            }

            return '<vr-row><vr-columns colnum="{{provisionerDefinitionCtrl.normalColNum * 2}}">'
                    + '<vr-select on-ready="onSelectorReady" datasource="extensionConfigs" selectedvalues="selectedExtensionConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" ' + label + ' isrequired="provisionerDefinitionCtrl.isrequired" hideremoveicon></vr-select>'
                + '</vr-columns></vr-row>'
                + '<vr-directivewrapper directive="selectedExtensionConfig.DefinitionEditor" on-ready="onDirectiveReady" normal-col-num="{{provisionerDefinitionCtrl.normalColNum}}" isrequired="provisionerDefinitionCtrl.isrequired" customvalidate="provisionerDefinitionCtrl.customvalidate"></vr-directivewrapper>';
        }
    }

    app.directive('retailBeProvisionerDefinitionsettings', ProvisionerDefinitionsettings);

})(app);