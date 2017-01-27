﻿'use strict';

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
        var dataAnalysisDefinitionSelectorDirectiveApi;

        var dataAnalysisDefinitionItemSelectorReadyPromiseDeferred;
        var dataAnalysisDefinitionItemSelectorDirectiveApi;

        function initializeController() {

            $scope.scopeModel.onDataAnalysisDefinitionSelectorReady = function (api) {
                dataAnalysisDefinitionSelectorDirectiveApi = api;
                dataAnalysisDefinitionSelectorReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onDataAnalysisItemDefinitionSelectorReady = function (api) {
                dataAnalysisDefinitionItemSelectorDirectiveApi = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isDirectiveLoading = value;
                };
                var daPayload = { dataAnalysisDefinitionId: dataAnalysisDefinitionSelectorDirectiveApi.getSelectedIds() };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataAnalysisDefinitionItemSelectorDirectiveApi, daPayload, setLoader, dataAnalysisDefinitionItemSelectorReadyPromiseDeferred);
            };

            $scope.scopeModel.onDataAnalysisDefinitionSelectionChanged = function () {
                if (dataAnalysisDefinitionItemSelectorDirectiveApi != undefined) {
                    var setLoader = function (value) {
                        $scope.scopeModel.isDirectiveLoading = value;
                    };
                    var daPayload = { dataAnalysisDefinitionId: dataAnalysisDefinitionSelectorDirectiveApi.getSelectedIds() };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataAnalysisDefinitionItemSelectorDirectiveApi, daPayload, setLoader, dataAnalysisDefinitionItemSelectorReadyPromiseDeferred);
                }
            }
            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                var loadDataAnalysisDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

                dataAnalysisDefinitionSelectorReadyPromiseDeferred.promise.then(function () {
                    var dataAnalysisDefinitionPayload;
                    if (payload != undefined) {

                        dataAnalysisDefinitionPayload = { selectedIds: payload.DataAnalysisDefinitionId };
                    }
                    VRUIUtilsService.callDirectiveLoad(dataAnalysisDefinitionSelectorDirectiveApi, dataAnalysisDefinitionPayload, loadDataAnalysisDefinitionPromiseDeferred);
                });
                promises.push(loadDataAnalysisDefinitionPromiseDeferred.promise);

                if (payload != undefined) {
                    dataAnalysisDefinitionItemSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                    var dataAnalysisDefinitionItemSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    dataAnalysisDefinitionItemSelectorReadyPromiseDeferred.promise.then(function () {
                        dataAnalysisDefinitionItemSelectorReadyPromiseDeferred = undefined;
                        var dataAnalysisDefinitionItemPayload = { selectedIds: payload.DataAnalysisItemDefinitionId, dataAnalysisDefinitionId: payload.DataAnalysisDefinitionId };
                        VRUIUtilsService.callDirectiveLoad(dataAnalysisDefinitionItemSelectorDirectiveApi, dataAnalysisDefinitionItemPayload, dataAnalysisDefinitionItemSelectorLoadDeferred);
                    });

                    promises.push(dataAnalysisDefinitionItemSelectorLoadDeferred.promise);
                }

                return UtilsService.waitMultiplePromises(promises);
            };


            api.getData = function () {
                return {
                    $type: 'Vanrise.Analytic.MainExtensions.DataAnalysis.RecordTypeExtraFields.DataAnalysisItemExtraFields,Vanrise.Analytic.MainExtensions',
                    DataAnalysisItemDefinitionId: dataAnalysisDefinitionItemSelectorDirectiveApi.getSelectedIds(),
                    DataAnalysisDefinitionId: dataAnalysisDefinitionSelectorDirectiveApi.getSelectedIds(),
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        };
    }

    return directiveDefinitionObject;
}]);