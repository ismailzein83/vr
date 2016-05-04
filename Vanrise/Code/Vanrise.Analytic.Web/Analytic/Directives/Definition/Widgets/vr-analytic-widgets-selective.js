(function (app) {

    'use strict';

    AnalyticWidgetsSelective.$inject = ['VR_Analytic_AnalyticConfigurationAPIService', 'UtilsService', 'VRUIUtilsService'];

    function AnalyticWidgetsSelective(VR_Analytic_AnalyticConfigurationAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
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
                + ' <vr-textbox label="Title" value="scopeModel.widgetTitle" isrequired="true"></vr-textbox>'
              + ' </vr-columns>'
               + ' <vr-columns colnum="{{searchSettingsCtrl.normalColNum}}">'
                + ' <vr-analytic-table-selector on-ready="scopeModel.onTableSelectorDirectiveReady" isrequired="true" selectedvalues="scopeModel.selectedTable" hideremoveicon onselectionchanged="scopeModel.onSelectionTableChanged"></vr-analytic-table-selector>'
              + ' </vr-columns>'

              + '<vr-columns colnum="{{searchSettingsCtrl.normalColNum}}" ng-if="scopeModel.selectedTable !=undefined">'
              + ' <vr-select on-ready="scopeModel.onSelectorReady"'
              + ' datasource="scopeModel.templateConfigs"'
              + ' selectedvalues="scopeModel.selectedTemplateConfig"'
               + 'datavaluefield="ExtensionConfigurationId"'
              + ' datatextfield="Title"'
              + label
               + ' isrequired="true"'
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
            var directiveReadyDeferred;
            var directivePayload;

            var tableSelectorAPI;
            var tableSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var tableIds;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onSelectionTableChanged = function()
                {
                    $scope.scopeModel.selectedTemplateConfig = undefined;
                }

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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
                defineAPI();

            }

            function defineAPI() {
                var api = {};
                var widgetEntity;
                api.load = function (payload) {
               
                    var promises = [];
                    if (payload != undefined) {
                        tableIds = payload.tableIds;

                        if (payload.widgetEntity != undefined)
                        {
                            widgetEntity = payload.widgetEntity;
                            $scope.scopeModel.widgetTitle = payload.widgetEntity.WidgetTitle;
                            directiveReadyDeferred = UtilsService.createPromiseDeferred();
                            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            directiveReadyDeferred.promise.then(function () {
                                directiveReadyDeferred = undefined;
                                var payloadDirective = {
                                    tableIds: $scope.scopeModel.selectedTable != undefined ? [$scope.scopeModel.selectedTable.AnalyticTableId] : undefined,
                                    widgetEntity: payload.widgetEntity
                                };

                                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
                            });
                            promises.push(loadDirectivePromiseDeferred.promise);

                        }
                        var loadTableSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        tableSelectorReadyDeferred.promise.then(function () {
                            var payLoadTableSelector = {
                                filter: { OnlySelectedIds: tableIds },
                                selectedIds: payload.widgetEntity != undefined ? payload.widgetEntity.AnalyticTableId : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(tableSelectorAPI, payLoadTableSelector, loadTableSelectorPromiseDeferred);
                        });
                        promises.push(loadTableSelectorPromiseDeferred.promise);

                           
                            var getWidgetsTemplateConfigsPromise = getWidgetsTemplateConfigs();
                            promises.push(getWidgetsTemplateConfigsPromise);

                            return UtilsService.waitMultiplePromises(promises);
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
                            data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                            data.AnalyticTableId = $scope.scopeModel.selectedTable != undefined ? $scope.scopeModel.selectedTable.AnalyticTableId : undefined,
                            data.WidgetTitle = $scope.scopeModel.widgetTitle;
                        }
                    }
                    return data;
                }
                function getWidgetsTemplateConfigs() {
                    return VR_Analytic_AnalyticConfigurationAPIService.GetWidgetsTemplateConfigs().then(function (response) {
                        if (selectorAPI != undefined)
                            selectorAPI.clearDataSource();
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.templateConfigs.push(response[i]);
                            }
                            if (widgetEntity != undefined)
                                $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, widgetEntity.ConfigId, 'ExtensionConfigurationId');
                            //else
                            //$scope.selectedTemplateConfig = $scope.templateConfigs[0];
                        }
                    });
                }
            }
        }
    }

    app.directive('vrAnalyticWidgetsSelective', AnalyticWidgetsSelective);

})(app);