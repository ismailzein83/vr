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
                }
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
            
                $scope.analyticAggregateTypes = UtilsService.getArrayEnum(VR_Analytic_AnalyticAggregateTypeEnum);
                $scope.onJoinSelectorDirectiveReady = function (api) {
                    joinSelectorAPI = api;
                    joinReadyDeferred.resolve();
                }

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
                        if (configEntity != undefined)
                        {
                            $scope.sqlColumn = configEntity.SQLColumn;
                            $scope.selectedAnalyticAggregateType = UtilsService.getItemByVal($scope.analyticAggregateTypes, configEntity.AggregateType, "value");
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

                   
                }

                api.getData = function () {
                    var joinConfigNames  = joinSelectorAPI !=undefined?joinSelectorAPI.getSelectedIds():undefined;

                    var dimension = {
                        $type: "Vanrise.Analytic.Entities.AnalyticAggregateConfig ,Vanrise.Analytic.Entities",
                        SQLColumn: $scope.sqlColumn,
                        AggregateType: $scope.selectedAnalyticAggregateType.value,
                        JoinConfigNames: joinConfigNames,
                    };
                    return dimension;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticItemconfigAggregateEditor', ItemconfigAggregateEditorDirective);

})(app);
