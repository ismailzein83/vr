'use strict';

app.directive('retailBePackageusagevolumeRecurringperiod', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_BEConfigurationAPIService',
    function (UtilsService, VRUIUtilsService, Retail_BE_BEConfigurationAPIService) {
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
                var packageUsageVolumeRecurringPeriodSelector = new PackageUsageVolumeRecurringPeriodSelector($scope, ctrl, $attrs);
                packageUsageVolumeRecurringPeriodSelector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };

        function PackageUsageVolumeRecurringPeriodSelector($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;

            var packageUsageVolumeRecurringPeriodId;

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
                    var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value; };

                    var directivePayload = {
                        packageUsageVolumeRecurringPeriodId: packageUsageVolumeRecurringPeriodId
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var packageUsageVolumeRecurringPeriod;

                    if (payload != undefined) {
                        packageUsageVolumeRecurringPeriod = payload.packageUsageVolumeRecurringPeriod;
                        packageUsageVolumeRecurringPeriodId = payload.packageUsageVolumeRecurringPeriodId;
                    }

                    var promises = [];

                    var getPackageUsageVolumeRecurringPeriodConfigsPromise = getPackageUsageVolumeRecurringPeriodConfigs();
                    promises.push(getPackageUsageVolumeRecurringPeriodConfigsPromise);

                    if (packageUsageVolumeRecurringPeriod != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    function getPackageUsageVolumeRecurringPeriodConfigs() {
                        return Retail_BE_BEConfigurationAPIService.GetPackageUsageVolumeRecurringPeriodConfigs().then(function (response) {

                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }

                                if (packageUsageVolumeRecurringPeriod != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, packageUsageVolumeRecurringPeriod.ConfigId, 'ExtensionConfigurationId');
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
                                packageUsageVolumeRecurringPeriod: packageUsageVolumeRecurringPeriod,
                                packageUsageVolumeRecurringPeriodId: packageUsageVolumeRecurringPeriodId
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

            var label = "label='Recurring Period'";
            if (attrs.hidelabel != undefined) {
                label = "";
            }
            var template =
                '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + label
                + ' isrequired="ctrl.isrequired"'
                + '>'
                + '</vr-select>'
                + ' </vr-columns>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"></vr-directivewrapper>';
            return template;
        }
    }]);