(function (app) {

    'use strict';

    ItemconfigMeasureEditorDirective.$inject = ['UtilsService', 'VRUIUtilsService','VR_Analytic_ExpressionTypeEnum'];

    function ItemconfigMeasureEditorDirective(UtilsService, VRUIUtilsService,VR_Analytic_ExpressionTypeEnum) {
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

            var fieldTypeAPI;
            var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.expressionType = UtilsService.getArrayEnum(VR_Analytic_ExpressionTypeEnum);
                $scope.showSQLExpression = false;
                $scope.showSQLExpressionMethod = false;
                $scope.onExpressionTypeSelectionChanged = function()
                {
                    if($scope.selectedExpressionType !=undefined)
                    {
                      
                        switch ($scope.selectedExpressionType.value)
                        {
                            case VR_Analytic_ExpressionTypeEnum.SQLExpression.value: $scope.showSQLExpression = true; $scope.showSQLExpressionMethod = false; break;
                            case VR_Analytic_ExpressionTypeEnum.SQLExpressionMethod.value: $scope.showSQLExpressionMethod = true; $scope.showSQLExpression = false; break;
                        }

                    }
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
                        if (configEntity != undefined) {
                            
                            $scope.sqlExpression = configEntity.SQLExpression;
                            $scope.sqlExpressionMethod = configEntity.GetSQLExpressionMethod;

                            if( $scope.sqlExpression !=undefined)
                            {
                                $scope.selectedExpressionType =  VR_Analytic_ExpressionTypeEnum.SQLExpression;
                            }else if ($scope.sqlExpressionMethod !=undefined)
                            {
                                $scope.selectedExpressionType = VR_Analytic_ExpressionTypeEnum.SQLExpressionMethod;
                            }
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
                    var dimension = {
                        $type: "Vanrise.Analytic.Entities.AnalyticMeasureConfig ,Vanrise.Analytic.Entities",
                        SQLExpression:$scope.showSQLExpression? $scope.sqlExpression:undefined,
                        JoinConfigNames: joinConfigNames,
                        FieldType: fieldType,
                        GetSQLExpressionMethod:$scope.showSQLExpressionMethod? $scope.sqlExpressionMethod:undefined,
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
