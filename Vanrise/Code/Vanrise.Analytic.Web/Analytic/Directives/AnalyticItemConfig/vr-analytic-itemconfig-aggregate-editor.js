(function (app) {

    'use strict';

    ItemconfigAggregateEditorDirective.$inject = ['UtilsService', 'VRUIUtilsService','VR_Analytic_AnalyticAggregateTypeEnum'];

    function ItemconfigAggregateEditorDirective(UtilsService, VRUIUtilsService, VR_Analytic_AnalyticAggregateTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var itemconfigDimensionEditor = new ItemconfigAggregateEditor(ctrl, $scope, $attrs);
                itemconfigDimensionEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },

            templateUrl: function (element, attrs) {
                return '/Client/Modules/Analytic/Directives/AnalyticItemConfig/Templates/AggregateEditorTemplate.html';
            }
        };

        function ItemconfigAggregateEditor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var joinSelectorAPI;
            var joinReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.analyticAggregateTypes = UtilsService.getArrayEnum(VR_Analytic_AnalyticAggregateTypeEnum);
                $scope.scopeModel.onJoinSelectorDirectiveReady = function (api) {
                    joinSelectorAPI = api;
                    joinReadyDeferred.resolve();
                };

                defineAPI();

                
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var tableId;
                    var promises = [];
                    var configEntity;
                    if (payload != undefined) {
                        tableId = payload.tableId;
                        configEntity = payload.ConfigEntity;
                        if (configEntity != undefined) {
                            $scope.scopeModel.sqlColumn = configEntity.SQLColumn;
                            $scope.scopeModel.selectedAnalyticAggregateType = UtilsService.getItemByVal($scope.scopeModel.analyticAggregateTypes, configEntity.AggregateType, "value");
                            $scope.scopeModel.currencySQLColumnName = configEntity.CurrencySQLColumnName;

                        }
                        var loadJoinDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        joinReadyDeferred.promise.then(function () {
                            var payloadJoinDirective = {
                                filter: { TableIds: [tableId] },
                                selectedIds: configEntity != undefined ? configEntity.JoinConfigNames : undefined

                            };
                            VRUIUtilsService.callDirectiveLoad(joinSelectorAPI, payloadJoinDirective, loadJoinDirectivePromiseDeferred);
                        });
                        promises.push(loadJoinDirectivePromiseDeferred.promise);

                        return UtilsService.waitMultiplePromises(promises);
                    }


                };

                api.getData = function () {
                    var joinConfigNames = joinSelectorAPI != undefined ? joinSelectorAPI.getSelectedIds() : undefined;
                    var dimension = {
                        $type: "Vanrise.Analytic.Entities.AnalyticAggregateConfig ,Vanrise.Analytic.Entities",
                        SQLColumn: $scope.scopeModel.sqlColumn,
                        AggregateType: $scope.scopeModel.selectedAnalyticAggregateType.value,
                        JoinConfigNames: joinConfigNames,
                        CurrencySQLColumnName: $scope.scopeModel.currencySQLColumnName,
                    };
                    return dimension;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticItemconfigAggregateEditor', ItemconfigAggregateEditorDirective);

})(app);
