"use strict";
app.directive("vrAnalyticReportgenerationcustomcodeFilegenerator", ["UtilsService","VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var customCode = new CustomCodeFileGenerator($scope, ctrl, $attrs);
                customCode.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/CustomCode/Templates/ReportGenerationCustomCodeFileGeneratorTemplate.html"
        };


        function CustomCodeFileGenerator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var customCodeSettingsAPI;
            var customCodeSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onCustomCodeSelectorReadyDeferred = function (api) {
                    customCodeSettingsAPI = api;
                    customCodeSettingsReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;
                    var promises = [];
                    var selectedCustomCodeSettingsId;
                    if (payload != undefined) {
                        if (payload.fileGenerator != undefined) {
                            selectedCustomCodeSettingsId = payload.fileGenerator.ReportGenerationCustomCodeFileGeneratorId;
                        }
                    }
                    promises.push(loadCustomCodeSelector());
                    function loadCustomCodeSelector() {
                        var customCodeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        customCodeSettingsReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                selectedIds: selectedCustomCodeSettingsId
                            };
                            VRUIUtilsService.callDirectiveLoad(customCodeSettingsAPI, selectorPayload, customCodeSelectorLoadDeferred);
                        });
                        return customCodeSelectorLoadDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.isLoading = false;
                    });

                };


                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.ReportGenerationCustomCodeFileGenerator,Vanrise.Analytic.MainExtensions",
                        ReportGenerationCustomCodeFileGeneratorId: customCodeSettingsAPI.getSelectedIds()
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }
        return directiveDefinitionObject;
    }
]);