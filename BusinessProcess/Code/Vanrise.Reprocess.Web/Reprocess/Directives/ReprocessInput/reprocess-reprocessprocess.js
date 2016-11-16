"use strict";

app.directive("reprocessReprocessprocess", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'ReprocessChunkTimeEnum',
    function (UtilsService, VRUIUtilsService, VRValidationService, ReprocessChunkTimeEnum) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Reprocess/Directives/ReprocessInput/Templates/ReprocessProcessTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {

            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }
            var reprocessDefinitionSelectorAPI;
            var reprocessDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.onReprocessDefinitionSelectorReady = function (api) {
                reprocessDefinitionSelectorAPI = api;
                reprocessDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.Reprocess.BP.Arguments.ReProcessingProcessInput, Vanrise.Reprocess.BP.Arguments",
                            ReprocessDefinitionId: reprocessDefinitionSelectorAPI.getSelectedIds(),
                            FromTime: $scope.fromDate,
                            ToTime: new Date($scope.toDate.setDate($scope.toDate.getDate() + 1)) ,
                            ChunkTime: $scope.selectedChunkTime.value
                        }
                    };
                };

                api.load = function (payload) {
                    $scope.chunkTimes = UtilsService.getArrayEnum(ReprocessChunkTimeEnum);

                    var promises = [];

                    var reprocessDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    reprocessDefinitionSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(reprocessDefinitionSelectorAPI, undefined, reprocessDefinitionSelectorLoadDeferred);
                    });

                    promises.push(reprocessDefinitionSelectorLoadDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
