'use strict';

app.directive('vrGenericdataLoadbebyidstep', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var loadBEByIDStep = new LoadBEByIDStep(ctrl, $scope);
            loadBEByIDStep.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/LoadBEByID/Templates/LoadBEByIDStepTemplate.html'
    };

    function LoadBEByIDStep(ctrl, $scope)
    {
        this.initializeController = initializeController;

        var beDefinitionSelectorAPI;
        var beDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var beIdExpressionBuilderAPI;
        var beIdExpressionBuilderReadyDeferred = UtilsService.createPromiseDeferred();

        var beExpressionBuilderAPI;
        var beExpressionBuilderReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController()
        {
            $scope.scopeModel = {};

            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                beDefinitionSelectorAPI = api;
                beDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onBusinessEntityIdExpressionBuilderReady = function (api) {
                beIdExpressionBuilderAPI = api;
                beIdExpressionBuilderReadyDeferred.resolve();
            };

            $scope.scopeModel.onBusinessEntityExpressionBuilderReady = function (api) {
                beExpressionBuilderAPI = api;
                beExpressionBuilderReadyDeferred.resolve();
            };

            UtilsService.waitMultiplePromises([beDefinitionSelectorReadyDeferred.promise, beIdExpressionBuilderReadyDeferred.promise, beExpressionBuilderReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (payload)
            {
                return UtilsService.waitMultipleAsyncOperations([loadBusinessEntityDefinitionSelector, loadBusinessEntityIdExpressionBuilder, loadBusinessEntityExpressionBuilder]);

                function loadBusinessEntityDefinitionSelector()
                {
                    var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    var selectorPayload = (payload != undefined && payload.stepDetails != undefined) ? { selectedIds: payload.stepDetails.BusinessEntityDefinitionId } : undefined;
                    VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, selectorPayload, beDefinitionSelectorLoadDeferred);
                    return beDefinitionSelectorLoadDeferred.promise;
                }
                function loadBusinessEntityIdExpressionBuilder()
                {
                    var beIdExpressionBuilderLoadDeferred = UtilsService.createPromiseDeferred();

                    var beIdExpressionBuilderPayload;

                    if (payload != undefined) {
                        beIdExpressionBuilderPayload = {};
                        beIdExpressionBuilderPayload.context = payload.context;
                        if (payload.stepDetails != undefined)
                            beIdExpressionBuilderPayload.selectedRecords = payload.stepDetails.BusinessEntityId;
                    }
                    
                    VRUIUtilsService.callDirectiveLoad(beIdExpressionBuilderAPI, beIdExpressionBuilderPayload, beIdExpressionBuilderLoadDeferred);
                    return beIdExpressionBuilderLoadDeferred.promise;
                }
                function loadBusinessEntityExpressionBuilder()
                {
                    var beExpressionBuilderLoadDeferred = UtilsService.createPromiseDeferred();

                    var beExpressionBuilderPayload;

                    if (payload != undefined) {
                        beExpressionBuilderPayload = {};
                        beExpressionBuilderPayload.context = payload.context;
                        if (payload.stepDetails != undefined)
                            beExpressionBuilderPayload.selectedRecords = payload.stepDetails.BusinessEntity;
                    }
                    
                    VRUIUtilsService.callDirectiveLoad(beExpressionBuilderAPI, beExpressionBuilderPayload, beExpressionBuilderLoadDeferred);
                    return beExpressionBuilderLoadDeferred.promise;
                }
            };

            api.getData = function ()
            {
                return {
                    $type: "Vanrise.GenericData.MainExtensions.MappingSteps.LoadBEByIDMappingStep, Vanrise.GenericData.MainExtensions",
                    BusinessEntityDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                    BusinessEntityId: beIdExpressionBuilderAPI.getData(),
                    BusinessEntity: beExpressionBuilderAPI.getData()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);