"use strict";

app.directive("vrGenericdataSummarytransformationSummarybatchstartidentificationSelector", ['UtilsService', '$compile', 'VRUIUtilsService', 'VR_GenericData_SummaryTransformationDefinitionAPIService',
    function (UtilsService, $compile, VRUIUtilsService, VR_GenericData_SummaryTransformationDefinitionAPIService) {

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
            templateUrl: function (element, attrs) {
                return getDirectiveTemplateUrl();
            }
        };

        function getDirectiveTemplateUrl() {
            return "/Client/Modules/VR_GenericData/Directives/MainExtensions/SummaryBatchTimeIntervalRange/Templates/SummaryBatchStartIdentificationSelector.html";
        }

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            var summaryBatchIntervalSourceDirectiveAPI;
            var summaryBatchIntervalSourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            $scope.onSummaryBatchIntervalSourceTypeDirectiveReady = function (api) {
                summaryBatchIntervalSourceDirectiveAPI = api;
                summaryBatchIntervalSourceDirectiveReadyPromiseDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.getData = function () {
                    var sourceSummaryBatchInterval;
                    if ($scope.selectedSummaryBatchIntervalSourceTypeTemplate != undefined) {
                        if (summaryBatchIntervalSourceDirectiveAPI != undefined) {
                            sourceSummaryBatchInterval = summaryBatchIntervalSourceDirectiveAPI.getData();
                            sourceSummaryBatchInterval.ConfigId = $scope.selectedSummaryBatchIntervalSourceTypeTemplate.ExtensionConfigurationId

                        }
                    }
                    return sourceSummaryBatchInterval;
                };


                api.load = function (payload) {
                    var promises = [];
                    $scope.summaryBatchIntervalSourceTypeTemplates = [];

                    var sourceConfigId;
                    var settings;

                    if (payload != undefined) {
                        sourceConfigId = payload.Settings.ConfigId;
                        settings = payload.Settings;
                    }
                    var loadSummaryBatchIntervalTypeTemplatesPromise = VR_GenericData_SummaryTransformationDefinitionAPIService.GetSummaryBatchIntervalSourceTemplates().then(function (response) {
                        angular.forEach(response, function (item) {
                            $scope.summaryBatchIntervalSourceTypeTemplates.push(item);
                        });
                        if (sourceConfigId != undefined)
                            $scope.selectedSummaryBatchIntervalSourceTypeTemplate = UtilsService.getItemByVal($scope.summaryBatchIntervalSourceTypeTemplates, sourceConfigId, "ExtensionConfigurationId");
                    });
                    promises.push(loadSummaryBatchIntervalTypeTemplatesPromise);

                    if (payload != undefined && sourceConfigId != undefined) {
                        var loadSummaryBatchIntervalSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                        summaryBatchIntervalSourceDirectiveReadyPromiseDeferred.promise.then(function () {
                            var paylod = {
                                Settings: settings,
                            };
                            VRUIUtilsService.callDirectiveLoad(summaryBatchIntervalSourceDirectiveAPI, paylod, loadSummaryBatchIntervalSourceTemplatePromiseDeferred);
                        });

                        promises.push(loadSummaryBatchIntervalSourceTemplatePromiseDeferred.promise);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);




