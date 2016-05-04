(function (app) {

    'use strict';

    ItemconfigDimensionEditorDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function ItemconfigDimensionEditorDirective(UtilsService, VRUIUtilsService) {
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
            var fieldTypeAPI;
            var fieldTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
            
                $scope.onParentDimensionSelectorDirectiveReady = function (api) {
                    parentDimensionSelectorAPI = api;
                    parentDimensionReadyDeferred.resolve();
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
                           
                            $scope.idColumn = configEntity.IdColumn;
                            $scope.nameColumn = configEntity.NameColumn;
                            $scope.isRequiredFromParent = configEntity.IsRequiredFromParent;
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

                        var loadParentDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        parentDimensionReadyDeferred.promise.then(function () {
                            var payloadParentDirective = {
                                filter: { TableIds: [tableId] },
                                selectedIds: configEntity!=undefined?configEntity.ParentDimension:undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(parentDimensionSelectorAPI, payloadParentDirective, loadParentDirectivePromiseDeferred);
                        });
                        promises.push(loadParentDirectivePromiseDeferred.promise);


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
                    var parentDimension = parentDimensionSelectorAPI !=undefined?parentDimensionSelectorAPI.getSelectedIds():undefined;
                    
                    var dimension = {
                        $type: "Vanrise.Analytic.Entities.AnalyticDimensionConfig ,Vanrise.Analytic.Entities",
                        IdColumn : $scope.idColumn,
                        NameColumn:  $scope.nameColumn,
                        JoinConfigNames: joinConfigNames,
                        ParentDimension:parentDimension,
                        IsRequiredFromParent: $scope.isRequiredFromParent,
                        FieldType:fieldType,
                        CurrencySQLColumnName: $scope.currencySQLColumnName,
                     //   GroupByColumns: ,
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
