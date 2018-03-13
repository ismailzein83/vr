"use strict";
app.directive("vrAnalyticDimensionSame", ["UtilsService","VRUIUtilsService","VR_Analytic_SameDimensionTypeEnum",
function (UtilsService, VRUIUtilsService, VR_Analytic_SameDimensionTypeEnum) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var sameDimension = new SameDimension($scope, ctrl, $attrs);
                sameDimension.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/MeasureExternalSource/Dimension/Templates/AnalyticSameDimensionTemplate.html" 
        };
        function SameDimension($scope, ctrl, $attrs) {
  
            var dimensionsSelectorAPI;
            var dimensionsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onDimensionsSelectorReady = function (api) {
                    dimensionsSelectorAPI = api;
                    dimensionsSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.sameDimensionTypes = UtilsService.getArrayEnum(VR_Analytic_SameDimensionTypeEnum);
                defineAPI();
            };
            function defineAPI() {
                var api = {};
                
                api.load = function (payload) {

                    var context;
                    var ruleEntity;
                    var tableId;
                    var promises = [];

                    if (payload != undefined) {

                        context = payload.context;
                        ruleEntity = payload.ruleEntity;
                        tableId = payload.tableId;

                        if (ruleEntity != undefined) {
                            var settings = ruleEntity.Entity != undefined ? ruleEntity.Entity.Settings : undefined;
                            var selectedIds = settings != undefined ? settings.DimensionNames : undefined;
                            var dimensionType = (ruleEntity.Entity != undefined && ruleEntity.Entity.Settings != undefined) ? ruleEntity.Entity.Settings.Type : undefined;
                            $scope.scopeModel.selectedSameDimensionType = UtilsService.getItemByVal($scope.scopeModel.sameDimensionTypes, dimensionType, "value");
                        }
                    }
                    promises.push(loadDimensionsSelector());
                    
                    function loadDimensionsSelector() {
                        
                        var dimensionsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        dimensionsSelectorReadyDeferred.promise.then(function () {
                            
                            var payload = {
                                filter: { TableIds: [tableId] },
                                selectedIds: selectedIds
                            };
                            VRUIUtilsService.callDirectiveLoad(dimensionsSelectorAPI, payload, dimensionsSelectorLoadDeferred);
                        });
                        return dimensionsSelectorLoadDeferred.promise;
                    }
                    
                    return UtilsService.waitMultiplePromises(promises);
                };
                api.getData = function () {
                    return {
                        $type: "Vanrise.Analytic.MainExtensions.AnalyticMeasureExternalSources.AnalyticTable.DimensionMappingRules.SameDimensionName, Vanrise.Analytic.MainExtensions",
                        Type: $scope.scopeModel.selectedSameDimensionType.value,
                        DimensionNames: dimensionsSelectorAPI.getSelectedIds()
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined) 
                    currentContext = {};
                
                return currentContext;
            }
        }
        return directiveDefinitionObject;
    }
]);