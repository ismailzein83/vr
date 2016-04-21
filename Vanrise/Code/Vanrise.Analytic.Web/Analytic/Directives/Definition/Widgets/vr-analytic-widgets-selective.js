(function (app) {

    'use strict';

    AnalyticWidgetsSelective.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AnalyticWidgetsSelective(VR_Analytic_AnalyticConfigurationAPIService, UtilsService, VRUIUtilsService) {
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
                var widgets = new Widgets($scope, ctrl, $attrs);
                widgets.initializeController();
            },
            controllerAs: "searchSettingsCtrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTamplate(attrs);
            }
        };
        function getTamplate(attrs) {
            var withemptyline = 'withemptyline';
            var label = "label='Widgets'";
            if (attrs.hidelabel != undefined) {
                label = "";
                withemptyline = '';
            }


            var template =
                '<vr-row>'

               + ' <vr-columns colnum="{{searchSettingsCtrl.normalColNum}}">'
                + ' <vr-analytic-table-selector on-ready="scopeModel.onTableSelectorDirectiveReady" selectedvalues="scopeModel.selectedTable"></vr-analytic-table-selector>'
              + ' </vr-columns>'
              + ' <vr-columns colnum="{{searchSettingsCtrl.normalColNum}}">'
                + ' <vr-textbox label="Title" value="scopeModel.widgetTitle"></vr-textbox>'
              + ' </vr-columns>'
              + '<vr-columns colnum="{{searchSettingsCtrl.normalColNum}}" ng-if="scopeModel.selectedTable !=undefined">'
              + ' <vr-select on-ready="scopeModel.onSelectorReady"'
              + ' datasource="scopeModel.templateConfigs"'
              + ' selectedvalues="scopeModel.selectedTemplateConfig"'
               + 'datavaluefield="TemplateConfigID"'
              + ' datatextfield="Name"'
              + label
               + ' isrequired="searchSettingsCtrl.isrequired"'
              + 'hideremoveicon>'
          + '</vr-select>'
           + ' </vr-columns>'
           + '</vr-row>'
              + '<vr-directivewrapper directive="scopeModel.selectedTemplateConfig.Editor" on-ready="scopeModel.onDirectiveReady" normal-col-num="{{searchSettingsCtrl.normalColNum}}" isrequired="searchSettingsCtrl.isrequired" customvalidate="searchSettingsCtrl.customvalidate" type="searchSettingsCtrl.type"></vr-directivewrapper>';
            return template;

        }
        function Widgets($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectorAPI;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();
            var directivePayload;

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var tableIds;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                  
                };
                $scope.scopeModel.onTableSelectorDirectiveReady = function (api) {
                    tableSelectorAPI = api;
                    tableSelectorReadyDeferred.resolve();
                }
                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directivePayload={
                        tableIds: $scope.scopeModel.selectedTable != undefined ? [$scope.scopeModel.selectedTable.AnalyticTableId] : undefined
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader);
                };
                defineAPI();

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
               
                    var promises = [];
                    if (payload != undefined) {
                        tableIds = payload.tableIds;

                            var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                            tableSelectorReadyDeferred.promise.then(function () {
                                var payLoadTableSelector = {
                                    filter: { OnlySelectedIds: tableIds }
                                };

                                VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payLoadTableSelector, loadTableSelectorPromiseDeferred);
                            });
                            promises.push(loadTableSelectorPromiseDeferred.promise);
                        //var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        //directiveReadyDeferred.promise.then(function () {
                        //    directiveReadyDeferred = undefined;
                        //    var payloadDirective;

                        //    VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                        //});
                        //promises.push(loadDirectivePromiseDeferred.promise);
                            var getWidgetsTemplateConfigsPromise = getWidgetsTemplateConfigs();
                            promises.push(getWidgetsTemplateConfigsPromise);

                            return UtilsService.waitMultiplePromises(promises);

                            function getWidgetsTemplateConfigs() {
                                return VR_Analytic_AnalyticConfigurationAPIService.GetWidgetsTemplateConfigs().then(function (response) {
                                    if (selectorAPI !=undefined)
                                    selectorAPI.clearDataSource();
                                    if (response != null) {
                                        for (var i = 0; i < response.length; i++) {
                                            $scope.scopeModel.templateConfigs.push(response[i]);
                                        }
                                        //if (fieldMapping != undefined)
                                        //    $scope.selectedTemplateConfig = UtilsService.getItemByVal($scope.templateConfigs, fieldMapping.ConfigId, 'TemplateConfigID');
                                        //else
                                        //$scope.selectedTemplateConfig = $scope.templateConfigs[0];
                                    }
                                });
                            }
                    }

                    


                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data;
                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                        data = directiveAPI.getData();
                        if (data != undefined) {
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.TemplateConfigID;
                            data.AnalyticTableId = $scope.scopeModel.selectedTable != undefined ? $scope.scopeModel.selectedTable.AnalyticTableId : undefined,
                            data.WidgetTitle = $scope.scopeModel.widgetTitle;
                        }
                    }
                    return data;
                }
            }
        }
    }

    app.directive('vrAnalyticWidgetsSelective', AnalyticWidgetsSelective);

})(app);