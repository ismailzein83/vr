'use strict';

app.directive('npIvswitchTranslationrulePoolbasedcligroupStaticeditor', ['UtilsService', 'VRUIUtilsService', 'VRDateTimeService',
    function (UtilsService, VRUIUtilsService, VRDateTimeService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PoolBasedCLIGroupStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: '/Client/Modules/NP_IVSwitch/Directives/TranslationRule/Templates/PoolBasedCLIGroupStaticEditorTemplate.html'
        };

        function PoolBasedCLIGroupStaticEditor(ctrl, $scope, $attrs) {
            var isEditMode;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.cliPatterns = [];
                $scope.scopeModel.disableAddCLIPattern = true;

                $scope.scopeModel.addCLIPattern = function () {
                    $scope.scopeModel.cliPatterns.push({ cliPattern: $scope.scopeModel.cliPattern, message: null });
                    $scope.scopeModel.cliPattern = undefined;
                    $scope.scopeModel.disableAddCLIPattern = true;
                };

                $scope.scopeModel.onCLIPatternValueChange = function (value) {
                    $scope.scopeModel.disableAddCLIPattern = (value == undefined && $scope.scopeModel.cliPattern.length - 1 < 1) || UtilsService.getItemIndexByVal($scope.scopeModel.cliPatterns, $scope.scopeModel.cliPattern, "cliPattern") != -1;
                };

                $scope.scopeModel.getImportedCLIPatterns = function (values) {
                    $scope.scopeModel.cliPatterns.length = 0;
                    for (var i = 0; i < values.length ; i++) {
                        if (UtilsService.getItemIndexByVal($scope.scopeModel.cliPatterns, values[i], "cliPattern") == -1)
                            $scope.scopeModel.cliPatterns.push({ cliPattern: values[i], message: null });
                    }
                };
                $scope.scopeModel.validateCLIPatterns = function () {
                    if ($scope.scopeModel.cliPatterns.length == 0)
                        return "Enter at least one CLI pattern.";
                    if (!validateCLIPatterns())
                        return "Invalid CLI Patterns.";
                    if (UtilsService.getItemIndexByVal($scope.scopeModel.cliPatterns, $scope.scopeModel.cliPattern, "cliPattern") != -1)
                        return "CLI Pattern already exists.";
                    return null;
                };

                defineApi();
            }

            function defineApi() {
                var api = {};
                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.selectedValues != undefined) {
                        $scope.scopeModel.name = payload.selectedValues.Name;
                        if (payload.selectedValues.CLIPatterns != undefined && payload.selectedValues.CLIPatterns.length > 0) {
                            for (var i = 0; i < payload.selectedValues.CLIPatterns.length; i++) {
                                $scope.scopeModel.cliPatterns.push({ cliPattern: payload.selectedValues.CLIPatterns[i].CLIPattern, message: null });
                            }
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises).finally(function () {
                    });

                };

                api.setData = function (poolBasedCLIGroupObject) {
                    if (!isEditMode) {
                        poolBasedCLIGroupObject.Name = $scope.scopeModel.name;
                        poolBasedCLIGroupObject.CLIPatterns = {
                            $type: "NP.IVSwitch.Entities.PoolBasedCLIDetailsCollection, NP.IVSwitch.Entities",
                            $values: getCLIPatterns()
                        };
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function validateCLIPatterns() {
                for (var i = 0; i < $scope.scopeModel.cliPatterns.length ; i++) {
                    if ($scope.scopeModel.cliPatterns[i].message != null)
                        return false;
                }
              
                return true;
            }
            function getCLIPatterns() {
                var cliPatterns;
                if ($scope.scopeModel.cliPatterns.length > 0) {
                    cliPatterns = [];
                    for (var i = 0; i < $scope.scopeModel.cliPatterns.length; i++) {
                        cliPatterns.push({
                            $type: "NP.IVSwitch.Entities.PoolBasedCLIDetails, NP.IVSwitch.Entities",
                            CLIPattern: $scope.scopeModel.cliPatterns[i].cliPattern
                        });
                    }
                }
                return cliPatterns;
            }
        }
        return directiveDefinitionObject;
    }]);