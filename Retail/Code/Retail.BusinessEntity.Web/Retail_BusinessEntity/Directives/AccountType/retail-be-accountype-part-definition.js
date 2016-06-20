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
            controllerAs: "ctrl",
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
                ctrl.extensionConfig = [];
                ctrl.selectedExtensionConfig;

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                ctrl.onDirectiveReady = function (api) {
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
                    selectorAPI.clearDataSource();

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
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.extensionConfigs.push(response[i]);
                                }
                                if (partDefinitionSettings != undefined)
                                    UtilsService.getItemByVal(ctrl.extensionConfigs, partDefinitionSettings.ConfigId, 'ExtensionConfigurationId') :
                                else if (ctrl.extensionConfigs.length > 0)
                                    ctrl.selectedExtensionConfig = ctrl.extensionConfigs[0];
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
                    data.ConfigId = ctrl.selectedExtensionConfig.ExtensionConfigurationId;
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTamplate(attrs)
        {
            var label = "label='Part Definition'";

            if (attrs.hidelabel != undefined) {
                label = "label='Part Definitions'";
            }

            return '<vr-row><vr-columns colnum="{{ctrl.normalColNum * 2}}">'
                    + '<vr-select on-ready="ctrl.onSelectorReady" datasource="ctrl.extensionConfigs" selectedvalues="ctrl.selectedExtensionConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" ' + label + ' isrequired="ctrl.isrequired" hideremoveicon></vr-select>'
                + '</vr-columns></vr-row>'
                + '<vr-row><vr-directivewrapper directive="ctrl.selectedExtensionConfig.Editor" on-ready="ctrl.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"></vr-directivewrapper></vr-row>';
        }
    }

    app.directive('retailBeAccounttypePartDefinition', AccountTypePartDefinitionDirective);

})(app);