"use strict";

app.directive("vrIntegrationFiledatasourcesettings", ["UtilsService", "VRValidationService", "VRNotificationService", "VRUIUtilsService", "VR_Integration_DataSourceSettingService",
    function (UtilsService, VRValidationService, VRNotificationService, VRUIUtilsService, VR_Integration_DataSourceSettingService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FileDataSourceSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Integration/Directives/Settings/FileDataSourceSettings/Templates/FileDataSourceSettingsTemplate.html"
        };

        function FileDataSourceSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.peakTimeRanges = [];
                $scope.scopeModel.fileDataSourceDefinitions = [];

                $scope.scopeModel.addPeakTimeRange = function () {
                    var peakTimeRange = { Entity: { PeakTimeRangeId : UtilsService.guid(), From: undefined, To: undefined } };
                    $scope.scopeModel.peakTimeRanges.push(peakTimeRange);
                };

                $scope.scopeModel.addFileDataSourceDefinition = function () {
                    var onFileDataSourceDefinitionAdded = function (fileDataSourceDefinition) {
                        $scope.scopeModel.fileDataSourceDefinitions.push({ Entity: fileDataSourceDefinition });
                    };
                    VR_Integration_DataSourceSettingService.addFileDataSourceDefinition(onFileDataSourceDefinitionAdded, false);
                };

                $scope.scopeModel.removePeakTimeRange = function (item) {
                    //VRNotificationService.showConfirmation().then(function (confirmed) {
                    //    if (confirmed) {
                    var index = $scope.scopeModel.peakTimeRanges.indexOf(item);
                    $scope.scopeModel.peakTimeRanges.splice(index, 1);
                    //    }
                    //});
                };

                $scope.scopeModel.removeFileDataSourceDefinition = function (item) {
                    //VRNotificationService.showConfirmation().then(function (confirmed) {
                    //    if (confirmed) {
                    var index = $scope.scopeModel.fileDataSourceDefinitions.indexOf(item);
                    $scope.scopeModel.fileDataSourceDefinitions.splice(index, 1);
                    //    }
                    //});
                };

                $scope.scopeModel.validatePeakTimeRanges = function () {
                    return validateDateTime();

                };

                $scope.scopeModel.validateFileDataSourceDefinitions = function () {
                    if ($scope.scopeModel.fileDataSourceDefinitions.length == 0) {
                        return "You should add at least one File DataSource Definition.";
                    }
                    return null;
                };

                defineAPI();
                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var fileDataSourceSettings;

                    if (payload != undefined) {
                        fileDataSourceSettings = payload.fileDataSourceSettings;
                    }

                    if (fileDataSourceSettings != undefined && fileDataSourceSettings.PeakTimeRanges != undefined) {
                        for (var i = 0; i < fileDataSourceSettings.PeakTimeRanges.length; i++) {
                            var currentPeakTimeRange = fileDataSourceSettings.PeakTimeRanges[i];
                            $scope.scopeModel.peakTimeRanges.push({ Entity: currentPeakTimeRange });
                        }
                    }

                    if (fileDataSourceSettings != undefined && fileDataSourceSettings.FileDataSourceDefinitions != undefined) {
                        for (var j = 0; j < fileDataSourceSettings.FileDataSourceDefinitions.length; j++) {
                            var currentFileDataSourceDefinition = fileDataSourceSettings.FileDataSourceDefinitions[j];
                            $scope.scopeModel.fileDataSourceDefinitions.push({ Entity: currentFileDataSourceDefinition });
                        }
                    }
                };

                api.getData = function () {
                    var peakTimeRanges = $scope.scopeModel.peakTimeRanges;
                    var peakTimeRangesToReturn = [];

                    var fileDataSourceDefinitions = $scope.scopeModel.fileDataSourceDefinitions;
                    var fileDataSourceDefinitionsToReturn = [];

                    if (peakTimeRanges != undefined) {
                        for (var i = 0; i < peakTimeRanges.length; i++) {
                            var currentPeakTimeRange = peakTimeRanges[i].Entity;
                            peakTimeRangesToReturn.push(currentPeakTimeRange);
                        }
                    }

                    if (fileDataSourceDefinitions != undefined) {
                        for (var j = 0; j < fileDataSourceDefinitions.length; j++) {
                            var currentFileDataSourceDefinition = fileDataSourceDefinitions[j].Entity;
                            fileDataSourceDefinitionsToReturn.push(currentFileDataSourceDefinition);
                        }
                    }

                    return {
                        PeakTimeRanges: peakTimeRangesToReturn,
                        FileDataSourceDefinitions: fileDataSourceDefinitionsToReturn
                    };
                };

                if (ctrl.onReady != null && typeof(ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editFileDataSourceDefinition
                }];
            }

            function editFileDataSourceDefinition(fileDataSourceDefinition) {
                var onFileDataSourceDefinitionUpdated = function (fileDataSourceDefinitionUpdated) {
                    var index = $scope.scopeModel.fileDataSourceDefinitions.indexOf(fileDataSourceDefinition);
                    $scope.scopeModel.fileDataSourceDefinitions[index] = { Entity: fileDataSourceDefinitionUpdated };
                };

                VR_Integration_DataSourceSettingService.editFileDataSourceDefinition(onFileDataSourceDefinitionUpdated, fileDataSourceDefinition.Entity);
            }

            function validateDateTime() {
                var peakTimeRanges = $scope.scopeModel.peakTimeRanges;
                for (var i = 0; i < peakTimeRanges.length; i++) {
                    var currentPeakTimeRanges = peakTimeRanges[i];
                    var errorMessage = VRValidationService.validateTimeRange(currentPeakTimeRanges.Entity.From, currentPeakTimeRanges.Entity.To);
                    if (errorMessage != null)
                        return errorMessage;
                }
                return null;
            }
        }

        return directiveDefinitionObject;
    }
]);