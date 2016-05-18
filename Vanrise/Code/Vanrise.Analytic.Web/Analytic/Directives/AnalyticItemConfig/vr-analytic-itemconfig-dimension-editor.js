(function (app) {

    'use strict';

    ItemconfigDimensionEditorDirective.$inject = ['UtilsService', 'VRUIUtilsService','VR_Analytic_ExpressionTypeEnum'];

    function ItemconfigDimensionEditorDirective(UtilsService, VRUIUtilsService, VR_Analytic_ExpressionTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var itemconfigDimensionEditor = new ItemconfigDimensionEditor(ctrl, $scope, $attrs);
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
                return '/Client/Modules/Analytic/Directives/AnalyticItemConfig/Templates/DimensionEditorTemplate.html';
            }
        };

        function ItemconfigDimensionEditor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;
            var joinSelectorAPI;
            var joinReadyDeferred = UtilsService.createPromiseDeferred();
            var parentDimensionSelectorAPI;
            var parentDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var dependentDimensionSelectorAPI;
            var dependentDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var requiredParentDimensionSelectorAPI;
            var requiredParentDimensionReadyDeferred = UtilsService.createPromiseDeferred();

            var fieldTypeAPI;
            var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.expressionType = UtilsService.getArrayEnum(VR_Analytic_ExpressionTypeEnum);

                $scope.showSQLExpression = true;
                $scope.showSQLExpressionMethod = false;
                $scope.selectedExpressionType = VR_Analytic_ExpressionTypeEnum.SQLExpression;
                $scope.onExpressionTypeSelectionChanged = function () {
                    if ($scope.selectedExpressionType != undefined) {

                        switch ($scope.selectedExpressionType.value) {
                            case VR_Analytic_ExpressionTypeEnum.SQLExpression.value: $scope.showSQLExpression = true; $scope.showSQLExpressionMethod = false; break;
                            case VR_Analytic_ExpressionTypeEnum.SQLExpressionMethod.value: $scope.showSQLExpressionMethod = true; $scope.showSQLExpression = false; break;
                        }

                    }
                }

                $scope.onParentDimensionSelectorDirectiveReady = function (api) {
                    parentDimensionSelectorAPI = api;
                    parentDimensionReadyDeferred.resolve();
                }
                $scope.onDependentDimensionSelectorDirectiveReady = function (api) {
                    dependentDimensionSelectorAPI = api;
                    dependentDimensionReadyDeferred.resolve();
                }
                $scope.onRequiredParentDimensionSelectorDirectiveReady = function (api) {
                    requiredParentDimensionSelectorAPI = api;
                    requiredParentDimensionReadyDeferred.resolve();
                }
                $scope.onJoinSelectorDirectiveReady = function (api) {
                    joinSelectorAPI = api;
                    joinReadyDeferred.resolve();
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
                        if (configEntity != undefined)
                        {
                           
                            $scope.sqlExpression = configEntity.SQLExpression;
                            $scope.sqlExpressionMethod = configEntity.GetValueMethod;

                            if ($scope.sqlExpression != undefined) {
                                $scope.selectedExpressionType = VR_Analytic_ExpressionTypeEnum.SQLExpression;
                            } else if ($scope.sqlExpressionMethod != undefined) {
                                $scope.selectedExpressionType = VR_Analytic_ExpressionTypeEnum.SQLExpressionMethod;
                            }
                          
                            $scope.currencySQLColumnName = configEntity.CurrencySQLColumnName;
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

                        var loadDependentDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        dependentDimensionReadyDeferred.promise.then(function () {
                            var payloadParentDirective = {
                                filter: { TableIds: [tableId] },
                                selectedIds: configEntity != undefined ? configEntity.DependentDimensions : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(dependentDimensionSelectorAPI, payloadParentDirective, loadDependentDirectivePromiseDeferred);
                        });
                        promises.push(loadDependentDirectivePromiseDeferred.promise);

                        var loadParentDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        parentDimensionReadyDeferred.promise.then(function () {
                            var payloadParentDirective = {
                                filter: { TableIds: [tableId] },
                                selectedIds: configEntity != undefined ? configEntity.Parents : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(parentDimensionSelectorAPI, payloadParentDirective, loadParentDirectivePromiseDeferred);
                        });
                        promises.push(loadParentDirectivePromiseDeferred.promise);


                        var loadRequiredParentDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        requiredParentDimensionReadyDeferred.promise.then(function () {
                            var payloadRequiredParentDirective = {
                                filter: { TableIds: [tableId] },
                                selectedIds: configEntity != undefined ? configEntity.RequiredParentDimension : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(requiredParentDimensionSelectorAPI, payloadRequiredParentDirective, loadRequiredParentDirectivePromiseDeferred);
                        });
                        promises.push(loadRequiredParentDirectivePromiseDeferred.promise);


                        var loadFieldTypePromiseDeferred = UtilsService.createPromiseDeferred();
                        fieldTypeReadyDeferred.promise.then(function () {
                            var payloadFieldTypeDirective = configEntity!=undefined?configEntity.FieldType:undefined;

                            VRUIUtilsService.callDirectiveLoad(fieldTypeAPI, payloadFieldTypeDirective, loadFieldTypePromiseDeferred);
                        });
                        promises.push(loadFieldTypePromiseDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }

                   
                }

                api.getData = function () {
                    var fieldType = fieldTypeAPI!=undefined?fieldTypeAPI.getData():undefined; 
                    var joinConfigNames  = joinSelectorAPI !=undefined?joinSelectorAPI.getSelectedIds():undefined;
                    var parents = parentDimensionSelectorAPI != undefined ? parentDimensionSelectorAPI.getSelectedIds() : undefined;
                    var requiredParentDimension = requiredParentDimensionSelectorAPI != undefined ? requiredParentDimensionSelectorAPI.getSelectedIds() : undefined;
                    var dependentDimensions = dependentDimensionSelectorAPI != undefined ? dependentDimensionSelectorAPI.getSelectedIds() : undefined;

                    var dimension = {
                        $type: "Vanrise.Analytic.Entities.AnalyticDimensionConfig ,Vanrise.Analytic.Entities",
                        SQLExpression: $scope.showSQLExpression ? $scope.sqlExpression : undefined,
                        GetValueMethod: $scope.showSQLExpressionMethod ? $scope.sqlExpressionMethod : undefined,
                        DependentDimensions:dependentDimensions,
                        JoinConfigNames: joinConfigNames,
                        Parents: parents,
                        RequiredParentDimension: requiredParentDimension,
                        FieldType:fieldType,
                        CurrencySQLColumnName: $scope.currencySQLColumnName,
                    };
                    return dimension;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticItemconfigDimensionEditor', ItemconfigDimensionEditorDirective);

})(app);
