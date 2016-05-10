(function (app) {

    'use strict';

    ReportsearchsettingsSelective.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ReportsearchsettingsSelective(VR_Analytic_AnalyticConfigurationAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                label: '@',
                customvalidate: '=',
                type: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var reportsearchsettings = new Reportsearchsettings($scope, ctrl, $attrs);
                reportsearchsettings.initializeController();
            },
            controllerAs: "searchSettingsCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Search Settings'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }


            var template =
                '<vr-row>'
              + '<vr-columns colnum="{{searchSettingsCtrl.normalColNum * 2}}">'
              + ' <vr-select on-ready="onSelectorReady"'
              + ' datasource="templateConfigs"'
              + ' selectedvalues="selectedTemplateConfig"'
               + 'datavaluefield="ExtensionConfigurationId"'
              + ' datatextfield="Title"'
              + label
               + ' isrequired="searchSettingsCtrl.isrequired"'
              + 'hideremoveicon>'
          + '</vr-select>'
           + '</vr-row>'
              + '<vr-directivewrapper directive="selectedTemplateConfig.Editor" on-ready="onDirectiveReady" normal-col-num="{{searchSettingsCtrl.normalColNum}}" isrequired="searchSettingsCtrl.isrequired" customvalidate="searchSettingsCtrl.customvalidate" type="searchSettingsCtrl.type"></vr-directivewrapper>';
            return template;

        }
        function Reportsearchsettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred;
            var directivePayload;
            var tableIds;
            function initializeController() {
                $scope.templateConfigs = [];
                $scope.selectedTemplateConfig;

                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload = {
                        tableIds: tableIds
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
                    var searchSettings;
                    if (payload != undefined) {
                        tableIds = payload.tableIds;
                        searchSettings = payload.searchSettings;
                        if ($scope.selectedTemplateConfig != undefined)
                            $scope.selectedTemplateConfig = undefined;

                        directiveReadyDeferred = UtilsService.createPromiseDeferred();
                        var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var payloadDirective = {
                                searchSettings: searchSettings,
                                tableIds: tableIds
                            };

                            VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                        });
                        promises.push(loadDirectivePromiseDeferred.promise);
                       
                    }

                    var getAnalyticReportSettingsTemplateConfigsPromise = getAnalyticReportSettingsTemplateConfigs();
                    promises.push(getAnalyticReportSettingsTemplateConfigsPromise);

                    return UtilsService.waitMultiplePromises(promises);

                    function getAnalyticReportSettingsTemplateConfigs() {
                        return VR_Analytic_AnalyticConfigurationAPIService.GetAnalyticReportSettingsTemplateConfigs().then(function (response) {
                            selectorAPI.clearDataSource();
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.templateConfigs.push(response[i]);
                                }
                                if (searchSettings != undefined)
                                    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, searchSettings.ConfigId, 'ExtensionConfigurationId');
                                else
                                    $scope.selectedTemplateConfig = $scope.templateConfigs[0];
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
                    if ($scope.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.selectedTemplateConfig.ExtensionConfigurationId;
                        }
                    }
                    return data;
                }
            }
        }
    }

    app.directive('vrAnalyticReportsearchsettingsSelective', ReportsearchsettingsSelective);

})(app);