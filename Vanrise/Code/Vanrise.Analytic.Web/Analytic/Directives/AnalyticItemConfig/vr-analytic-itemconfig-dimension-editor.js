(function (app) {

    'use strict';

    ItemconfigDimensionEditorDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Analytic_ExpressionTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function ItemconfigDimensionEditorDirective(UtilsService, VRUIUtilsService, VR_Analytic_ExpressionTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {
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
                $scope.scopeModel = {};
                $scope.scopeModel.expressionType = UtilsService.getArrayEnum(VR_Analytic_ExpressionTypeEnum);
                $scope.scopeModel.dimensionFieldMappings = [];
                $scope.scopeModel.showSQLExpression = true;
                $scope.scopeModel.showSQLExpressionMethod = false;
                $scope.scopeModel.selectedExpressionType = VR_Analytic_ExpressionTypeEnum.SQLExpression;
                $scope.scopeModel.onExpressionTypeSelectionChanged = function () {
                    if ($scope.scopeModel.selectedExpressionType != undefined) {

                        switch ($scope.scopeModel.selectedExpressionType.value) {
                            case VR_Analytic_ExpressionTypeEnum.SQLExpression.value: $scope.scopeModel.showSQLExpression = true; $scope.scopeModel.showSQLExpressionMethod = false; break;
                            case VR_Analytic_ExpressionTypeEnum.SQLExpressionMethod.value: $scope.scopeModel.showSQLExpressionMethod = true; $scope.scopeModel.showSQLExpression = false; break;
                        }

                    }
                };

                $scope.scopeModel.onParentDimensionSelectorDirectiveReady = function (api) {
                    parentDimensionSelectorAPI = api;
                    parentDimensionReadyDeferred.resolve();
                };
                $scope.scopeModel.onDependentDimensionSelectorDirectiveReady = function (api) {
                    dependentDimensionSelectorAPI = api;
                    dependentDimensionReadyDeferred.resolve();
                };
                $scope.scopeModel.onRequiredParentDimensionSelectorDirectiveReady = function (api) {
                    requiredParentDimensionSelectorAPI = api;
                    requiredParentDimensionReadyDeferred.resolve();
                };
                $scope.scopeModel.onJoinSelectorDirectiveReady = function (api) {
                    joinSelectorAPI = api;
                    joinReadyDeferred.resolve();
                };
                $scope.scopeModel.onFieldTypeReady = function (api) {
                    fieldTypeAPI = api;
                    fieldTypeReadyDeferred.resolve();
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

                            $scope.scopeModel.sqlExpression = configEntity.SQLExpression;
                            $scope.scopeModel.sqlExpressionMethod = configEntity.GetValueMethod;

                            if ($scope.scopeModel.sqlExpression != undefined) {
                                $scope.scopeModel.selectedExpressionType = VR_Analytic_ExpressionTypeEnum.SQLExpression;
                            } else if ($scope.scopeModel.sqlExpressionMethod != undefined) {
                                $scope.scopeModel.selectedExpressionType = VR_Analytic_ExpressionTypeEnum.SQLExpressionMethod;
                            }

                        }


                        getAnalyticDimensionEditorRuntime(tableId).then(function (response) {
                            $scope.scopeModel.dimensionFieldMappings.length = 0;
                            if (response && response.DataRecordTypeInfo != undefined) {
                                for (var i = 0; i < response.DataRecordTypeInfo.length; i++) {
                                    var filterItem = {
                                        payload: response.DataRecordTypeInfo[i],
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    promises.push(filterItem.loadPromiseDeferred.promise);
                                    addFilterItemToGrid(filterItem);
                                }
                            }

                        });

                        function addFilterItemToGrid(filterItem) {

                            var dataItem = {
                                Name: filterItem.payload.Name,
                                DataRecordTypeId: filterItem.payload.DataRecordTypeId
                            };
                            var dataItemPayload = { dataRecordTypeId: filterItem.payload.DataRecordTypeId };

                            if (configEntity != undefined && configEntity.DimensionFieldMappings != undefined) {
                                var selectedRecordField = UtilsService.getItemByVal(configEntity.DimensionFieldMappings, filterItem.payload.DataRecordTypeId, "DataRecordTypeId");
                                if (selectedRecordField != undefined)
                                    dataItemPayload.selectedIds = selectedRecordField.FieldName;
                            }
                            dataItem.onDataRecordTypeFieldsSelectorReady = function (api) {
                                dataItem.directiveAPI = api;
                                filterItem.readyPromiseDeferred.resolve();
                            };

                            filterItem.readyPromiseDeferred.promise
                                .then(function () {
                                    VRUIUtilsService.callDirectiveLoad(dataItem.directiveAPI, dataItemPayload, filterItem.loadPromiseDeferred);
                                });

                            $scope.scopeModel.dimensionFieldMappings.push(dataItem);
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
                            var payloadFieldTypeDirective = configEntity != undefined ? configEntity.FieldType : undefined;

                            VRUIUtilsService.callDirectiveLoad(fieldTypeAPI, payloadFieldTypeDirective, loadFieldTypePromiseDeferred);
                        });
                        promises.push(loadFieldTypePromiseDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }


                };

                api.getData = function () {
                    var fieldType = fieldTypeAPI != undefined ? fieldTypeAPI.getData() : undefined;
                    var joinConfigNames = joinSelectorAPI != undefined ? joinSelectorAPI.getSelectedIds() : undefined;
                    var parents = parentDimensionSelectorAPI != undefined ? parentDimensionSelectorAPI.getSelectedIds() : undefined;
                    var requiredParentDimension = requiredParentDimensionSelectorAPI != undefined ? requiredParentDimensionSelectorAPI.getSelectedIds() : undefined;
                    var dependentDimensions = dependentDimensionSelectorAPI != undefined ? dependentDimensionSelectorAPI.getSelectedIds() : undefined;

                    var dimensionFieldMappings = [];
                    if ($scope.scopeModel.dimensionFieldMappings.length > 0) {
                        for (var i = 0; i < $scope.scopeModel.dimensionFieldMappings.length; i++) {
                            var dimensionFieldMapping = $scope.scopeModel.dimensionFieldMappings[i];
                            dimensionFieldMappings.push({
                                DataRecordTypeId: dimensionFieldMapping.DataRecordTypeId,
                                FieldName: dimensionFieldMapping.directiveAPI.getSelectedIds()
                            });
                        }
                    }

                    var dimension = {
                        $type: "Vanrise.Analytic.Entities.AnalyticDimensionConfig ,Vanrise.Analytic.Entities",
                        SQLExpression: $scope.scopeModel.showSQLExpression ? $scope.scopeModel.sqlExpression : undefined,
                        GetValueMethod: $scope.scopeModel.showSQLExpressionMethod ? $scope.scopeModel.sqlExpressionMethod : undefined,
                        DependentDimensions: dependentDimensions,
                        JoinConfigNames: joinConfigNames,
                        Parents: parents,
                        RequiredParentDimension: requiredParentDimension,
                        FieldType: fieldType,
                        DimensionFieldMappings: dimensionFieldMappings

                    };
                    return dimension;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function getAnalyticDimensionEditorRuntime(tableId)
            {
                return   VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticDimensionEditorRuntime({ TableId: tableId })
            }
        }
    }

    app.directive('vrAnalyticItemconfigDimensionEditor', ItemconfigDimensionEditorDirective);

})(app);
