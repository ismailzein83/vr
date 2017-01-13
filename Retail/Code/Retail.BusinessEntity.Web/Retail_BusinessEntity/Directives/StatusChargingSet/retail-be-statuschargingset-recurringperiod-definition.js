(function (app) {

    'use strict';

    function ChargingSetPeriodtDefinitionDirective(retailBeStatusChargingSetApiService, utilsService, vruiUtilsService) {

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
                var chargingSetPeriodDefinition = new ChargingSetPeriodDefinition($scope, ctrl, $attrs);
                chargingSetPeriodDefinition.initializeController();
            },
            controllerAs: "periodDefinitionCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function ChargingSetPeriodDefinition($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var selectorAPI;
            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;

            function initializeController() {
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
                    vruiUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    var recurringPeriodSettings;

                    if (payload != undefined) {
                        recurringPeriodSettings = payload.data;
                    }
                    var loadChargingSetPeriodDefinitionExtensionConfigsPromise = loadChargingSetPeriodDefinitionExtensionConfigs();
                    promises.push(loadChargingSetPeriodDefinitionExtensionConfigsPromise);

                    if (recurringPeriodSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }
                    function loadChargingSetPeriodDefinitionExtensionConfigs() {
                        return retailBeStatusChargingSetApiService.GetRecurringPeriodExtensionConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.extensionConfigs.push(response[i]);
                                }
                                if (recurringPeriodSettings != undefined)
                                    $scope.selectedExtensionConfig = utilsService.getItemByVal($scope.extensionConfigs, recurringPeriodSettings.ConfigId, 'ExtensionConfigurationId');
                             
                            }
                        });
                    }
                    function loadDirective() {
                        directiveReadyDeferred = utilsService.createPromiseDeferred();
                        var directiveLoadDeferred = utilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = recurringPeriodSettings ;
                            vruiUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }
                    return utilsService.waitMultiplePromises(promises);
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
        function getTemplate(attrs) {
            var label = "label='Recurring Period'";

            if (attrs.hidelabel != undefined) {
                label = "label='Recurring Periods'";
            }

            return '<vr-columns colnum="{{periodDefinitionCtrl.normalColNum}}">'
                    + '<vr-select on-ready="onSelectorReady" datasource="extensionConfigs" selectedvalues="selectedExtensionConfig" datavaluefield="ExtensionConfigurationId" datatextfield="Title" ' + label + ' isrequired="periodDefinitionCtrl.isrequired" hideremoveicon></vr-select>'
                + '</vr-columns>'
                + '<vr-directivewrapper normal-col-num="{{periodDefinitionCtrl.normalColNum}}" ng-if="selectedExtensionConfig.Editor" directive="selectedExtensionConfig.Editor" on-ready="onDirectiveReady"  isrequired="periodDefinitionCtrl.isrequired" customvalidate="periodDefinitionCtrl.customvalidate"></vr-directivewrapper>';
        }
    }

    ChargingSetPeriodtDefinitionDirective.$inject = ['Retail_BE_StatusChargingSetAPIService', 'UtilsService', 'VRUIUtilsService'];

    app.directive('retailBeStatuschargingsetRecurringperiodDefinition', ChargingSetPeriodtDefinitionDirective);
})(app);