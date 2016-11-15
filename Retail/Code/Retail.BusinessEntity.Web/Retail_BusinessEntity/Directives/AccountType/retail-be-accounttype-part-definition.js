(function (app) {

    'use strict';

    AccountTypePartDefinitionDirective.$inject = ['Retail_BE_AccountTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AccountTypePartDefinitionDirective(Retail_BE_AccountTypeAPIService, UtilsService, VRUIUtilsService) {
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
                var accountTypePartDefinition = new AccountTypePartDefinition($scope, ctrl, $attrs);
                accountTypePartDefinition.initializeController();
            },
            controllerAs: "partDefinitionCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function AccountTypePartDefinition($scope, ctrl, $attrs)
        {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var chargingPolicy;

            function initializeController()
            {
                $scope.extensionConfigs = [];
                $scope.selectedExtensionConfig;
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = undefined;
                    var setLoader = function (value) {
                        $scope.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }
            function defineAPI()
            {
                var api = {};

                api.load = function (payload)
                {
    
                    var promises = [];
                    var partDefinitionSettings;

                    if (payload != undefined) {
                        partDefinitionSettings = payload.partDefinitionSettings;
                    }

                    var loadAccountTypePartDefinitionExtensionConfigsPromise = loadAccountTypePartDefinitionExtensionConfigs();
                    promises.push(loadAccountTypePartDefinitionExtensionConfigsPromise);

                    if (partDefinitionSettings != undefined)
                    {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function loadAccountTypePartDefinitionExtensionConfigs() {
                        return Retail_BE_AccountTypeAPIService.GetAccountTypePartDefinitionExtensionConfigs().then(function (response)
                        {
                            selectorAPI.clearDataSource();
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.extensionConfigs.push(response[i]);
                                }
                                if (partDefinitionSettings != undefined)
                                    $scope.selectedExtensionConfig =  UtilsService.getItemByVal($scope.extensionConfigs, partDefinitionSettings.ConfigId, 'ExtensionConfigurationId');
                                else if ($scope.extensionConfigs.length > 0)
                                    $scope.selectedExtensionConfig = $scope.extensionConfigs[0];
                            }
                        });
                    }
                    function loadDirective()
                    {
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = { partDefinitionSettings: partDefinitionSettings };
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

        function getTamplate(attrs)
        {
            var label = "label='Section Definition'";

            if (attrs.hidelabel != undefined) {
                label = "label='Section Definitions'";
            }

            return '<vr-row><vr-columns colnum="{{partDefinitionCtrl.normalColNum * 2}}">'
                    + '<vr-select on-ready="onSelectorReady" datasource="extensionConfigs" selectedvalues="selectedExtensionConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" ' + label + ' isrequired="partDefinitionCtrl.isrequired" hideremoveicon></vr-select>'
                + '</vr-columns></vr-row>'
                + '<vr-row><vr-directivewrapper directive="selectedExtensionConfig.DefinitionEditor" on-ready="onDirectiveReady" normal-col-num="{{partDefinitionCtrl.normalColNum}}" isrequired="partDefinitionCtrl.isrequired" customvalidate="partDefinitionCtrl.customvalidate"></vr-directivewrapper></vr-row>';
        }
    }

    app.directive('retailBeAccounttypePartDefinition', AccountTypePartDefinitionDirective);

})(app);