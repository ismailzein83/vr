(function (app) {

    'use strict';

    ItemconfigMeasureEditorDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function ItemconfigMeasureEditorDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var itemconfigMeasureEditor = new ItemconfigMeasureEditor(ctrl, $scope, $attrs);
                itemconfigMeasureEditor.initializeController();
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
                return '/Client/Modules/Analytic/Directives/AnalyticItemConfig/Templates/MeasureEditorTemplate.html';
            }
        };

        function ItemconfigMeasureEditor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var joinSelectorAPI;
            var joinReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

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
                        if (configEntity != undefined) {
                            $scope.sqlExpression = configEntity.SQLExpression;
                            $scope.sqlExpressionMethod = configEntity.GetSQLExpressionMethod;
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
                    var joinConfigNames = joinSelectorAPI != undefined ? joinSelectorAPI.getSelectedIds() : undefined;
                    var dimension = {
                        $type: "Vanrise.Analytic.Entities.AnalyticMeasureConfig ,Vanrise.Analytic.Entities",
                        SQLExpression: $scope.sqlExpression,
                        JoinConfigNames:joinConfigNames,
                        GetSQLExpressionMethod: $scope.sqlExpressionMethod,
                    };
                    return dimension;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticItemconfigMeasureEditor', ItemconfigMeasureEditorDirective);

})(app);
