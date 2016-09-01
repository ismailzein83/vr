
'use strict';

app.directive('vrAnalyticDataanalysisitemdefinitionSettings', ['VR_Analytic_DataAnalysisDefinitionAPIService', 'UtilsService', 'VRUIUtilsService', function (VR_Analytic_DataAnalysisDefinitionAPIService, UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var dataAnalysisItemDefinitionSettings = new DataAnalysisItemDefinitionSettings($scope, ctrl, $attrs);
            dataAnalysisItemDefinitionSettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/DataAnalysisItemDefinition/Templates/DataAnalysisItemDefinitionSettingsTemplate.html'
    };

    function DataAnalysisItemDefinitionSettings($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var directiveAPI;
        var directiveReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                directiveReadyDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var promises = [];
                var dataAnalysisItemDefinition;
                var dataAnalysisDefinitionId;
                var itemDefinitionTypeId;
                var context;

                if (payload != undefined) {
                    dataAnalysisItemDefinition = payload.dataAnalysisItemDefinition;
                    dataAnalysisDefinitionId = payload.dataAnalysisDefinitionId;
                    itemDefinitionTypeId = payload.itemDefinitionTypeId;
                    context = payload.context;
                }

                var getDataAnalysisDefinitionSettingsPromise = getDataAnalysisDefinitionSettings();
                promises.push(getDataAnalysisDefinitionSettingsPromise);

                var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                promises.push(directiveLoadDeferred.promise);

                UtilsService.waitMultiplePromises([getDataAnalysisDefinitionSettingsPromise, directiveReadyDeferred.promise]).then(function () {

                    var directivePayload = {};
                    directivePayload.context = buildContext();
                    if (dataAnalysisItemDefinition != undefined)
                        directivePayload.dataAnalysisItemDefinitionSettings = dataAnalysisItemDefinition.Settings;

                    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                });

                function getDataAnalysisDefinitionSettings() {

                    return VR_Analytic_DataAnalysisDefinitionAPIService.GetDataAnalysisDefinition(dataAnalysisDefinitionId).then(function (response) {
 
                        var dataAnalysisDefinitionEntity = response;
                        var itemsConfig = dataAnalysisDefinitionEntity.Settings.ItemsConfig;

                        for (var i = 0; i < itemsConfig.length; i++) 
                            if (itemsConfig[i].TypeId == itemDefinitionTypeId) {
                                $scope.scopeModel.directiveEditor = itemsConfig[i].Editor;
                            }
                    });
                }
                function buildContext() {
                    return context;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return directiveAPI.getData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
