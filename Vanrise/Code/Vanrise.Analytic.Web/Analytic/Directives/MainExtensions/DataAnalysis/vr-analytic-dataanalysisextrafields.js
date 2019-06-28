'use strict';

app.directive('vrAnalyticDataanalysisextrafields', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new extraFieldCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Analytic/Directives/MainExtensions/DataAnalysis/Templates/DataAnalysisExtraFields.html'
        };


        function extraFieldCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};

            var dataAnalysisDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataAnalysisDefinitionSelectionChangedPromiseDeferred;
            var dataAnalysisDefinitionSelectorDirectiveApi;

            var dataAnalysisDefinitionItemSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataAnalysisDefinitionItemSelectorDirectiveApi;

            function initializeController() {

                $scope.scopeModel.onDataAnalysisDefinitionSelectorReady = function (api) {
                    dataAnalysisDefinitionSelectorDirectiveApi = api;
                    dataAnalysisDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataAnalysisItemDefinitionSelectorReady = function (api) {
                    dataAnalysisDefinitionItemSelectorDirectiveApi = api;
                    dataAnalysisDefinitionItemSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onDataAnalysisDefinitionSelectionChanged = function () {                  
                    if (dataAnalysisDefinitionSelectorDirectiveApi.getSelectedIds() !=undefined) {
                        var setLoader = function (value) {
                            $scope.scopeModel.isDirectiveLoading = value;
                        };
                        var daPayload = { filter: { Filters: [] }, dataAnalysisDefinitionId: dataAnalysisDefinitionSelectorDirectiveApi.getSelectedIds() };
                        daPayload.filter.Filters.push({ $type: "Vanrise.Analytic.Entities.DAProfCalcDataAnalysisItemDefinitionFilter, Vanrise.Analytic.Entities" });

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataAnalysisDefinitionItemSelectorDirectiveApi, daPayload, setLoader, dataAnalysisDefinitionSelectionChangedPromiseDeferred);
                    }
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var loadDataAnalysisDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

                    dataAnalysisDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
                        var dataAnalysisDefinitionPayload = { filter: { Filters: [] } };
                        dataAnalysisDefinitionPayload.filter.Filters.push({ $type: "Vanrise.Analytic.Entities.DAProfCalcDataAnalysisDefinitionFilter, Vanrise.Analytic.Entities" });

                        if (payload != undefined) {

                            dataAnalysisDefinitionPayload.selectedIds = payload.DataAnalysisDefinitionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(dataAnalysisDefinitionSelectorDirectiveApi, dataAnalysisDefinitionPayload, loadDataAnalysisDefinitionPromiseDeferred);
                    });
                    promises.push(loadDataAnalysisDefinitionPromiseDeferred.promise);

                    if (payload != undefined) {
                        dataAnalysisDefinitionSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

                        var dataAnalysisDefinitionItemSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        UtilsService.waitMultiplePromises([dataAnalysisDefinitionItemSelectorReadyPromiseDeferred.promise, dataAnalysisDefinitionSelectionChangedPromiseDeferred.promise]).then(function () {
                            dataAnalysisDefinitionSelectionChangedPromiseDeferred = undefined;

                            var dataAnalysisDefinitionItemPayload = { filter: { Filters: [] }, selectedIds: payload.DataAnalysisItemDefinitionId, dataAnalysisDefinitionId: payload.DataAnalysisDefinitionId };
                            dataAnalysisDefinitionItemPayload.filter.Filters.push({ $type: "Vanrise.Analytic.Entities.DAProfCalcDataAnalysisItemDefinitionFilter, Vanrise.Analytic.Entities" });

                            VRUIUtilsService.callDirectiveLoad(dataAnalysisDefinitionItemSelectorDirectiveApi, dataAnalysisDefinitionItemPayload, dataAnalysisDefinitionItemSelectorLoadDeferred);
                        });

                        promises.push(dataAnalysisDefinitionItemSelectorLoadDeferred.promise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    return {
                        $type: 'Vanrise.Analytic.Business.DAProfCalcRecordTypeExtraFields,Vanrise.Analytic.Business',
                        DataAnalysisItemDefinitionId: dataAnalysisDefinitionItemSelectorDirectiveApi.getSelectedIds(),
                        DataAnalysisDefinitionId: dataAnalysisDefinitionSelectorDirectiveApi.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            };
        }

        return directiveDefinitionObject;
    }]);