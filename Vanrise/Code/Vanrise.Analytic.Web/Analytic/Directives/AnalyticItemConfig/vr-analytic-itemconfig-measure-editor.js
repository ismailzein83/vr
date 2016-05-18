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

            var dependentAggregateDimensionSelectorAPI;
            var dependentAggregateDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var fieldTypeAPI;
            var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.onJoinSelectorDirectiveReady = function (api) {
                    joinSelectorAPI = api;
                    joinReadyDeferred.resolve();
                }

                $scope.onDependentAggregateSelectorDirectiveReady = function (api) {
                    dependentAggregateDimensionSelectorAPI = api;
                    dependentAggregateDimensionReadyDeferred.resolve();
                }

                $scope.onFieldTypeReady = function (api) {
                    fieldTypeAPI = api;
                    fieldTypeReadyDeferred.resolve();
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
                            $scope.sqlExpressionMethod = configEntity.GetValueMethod;
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

                        var loadAggregateDependentDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        dependentAggregateDimensionReadyDeferred.promise.then(function () {
                            var payloadParentDirective = {
                                filter: { TableIds: [tableId] },
                                selectedIds: configEntity != undefined ? configEntity.DependentAggregateNames : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(dependentAggregateDimensionSelectorAPI, payloadParentDirective, loadAggregateDependentDirectivePromiseDeferred);
                        });
                        promises.push(loadAggregateDependentDirectivePromiseDeferred.promise);

                        var loadFieldTypePromiseDeferred = UtilsService.createPromiseDeferred();
                        fieldTypeReadyDeferred.promise.then(function () {
                            var payloadFieldTypeDirective = configEntity != undefined ? configEntity.FieldType : undefined;

                            VRUIUtilsService.callDirectiveLoad(fieldTypeAPI, payloadFieldTypeDirective, loadFieldTypePromiseDeferred);
                        });
                        promises.push(loadFieldTypePromiseDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);


                        return UtilsService.waitMultiplePromises(promises);
                    }


                }

                api.getData = function () {
                    var fieldType = fieldTypeAPI != undefined ? fieldTypeAPI.getData() : undefined;
                    var joinConfigNames = joinSelectorAPI != undefined ? joinSelectorAPI.getSelectedIds() : undefined;
                    var dependentAggregateNames = dependentAggregateDimensionSelectorAPI != undefined ? dependentAggregateDimensionSelectorAPI.getSelectedIds() : undefined;
                    var dimension = {
                        $type: "Vanrise.Analytic.Entities.AnalyticMeasureConfig ,Vanrise.Analytic.Entities",
                        GetValueMethod: $scope.sqlExpressionMethod,
                        JoinConfigNames: joinConfigNames,
                        DependentAggregateNames: dependentAggregateNames,
                        FieldType: fieldType,
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
