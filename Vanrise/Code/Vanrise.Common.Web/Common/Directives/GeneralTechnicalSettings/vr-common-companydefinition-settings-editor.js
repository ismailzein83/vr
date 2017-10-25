'use strict';

app.directive('vrCommonCompanydefinitionSettingsEditor', ['VRCommon_CompanyDefinitionService','UtilsService', 'VRUIUtilsService',
    function (VRCommon_CompanyDefinitionService,UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/GeneralTechnicalSettings/Templates/CompanyDefinitionSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;
            var gridAPI;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.companyDefinitions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };


                $scope.scopeModel.addCompanyDefinition = function () {
                    var onCompanyDefinitionAdded = function (companyDefinition) {
                        $scope.scopeModel.companyDefinitions.push({Entity: companyDefinition});
                    };
                    VRCommon_CompanyDefinitionService.addCompanyDefinition(onCompanyDefinitionAdded);
                };




                $scope.scopeModel.validateCompanyDefinitions = function () {
                    if (!validateNameIdentity())
                        return "Setting Definition name should be identical.";
                    if (!validateSettingIdentity())
                        return "Setting Definition should be identical.";
                    return null;
                };

                $scope.scopeModel.removeCompanyDefinition = function (companyDefinitionObj) {
                    $scope.scopeModel.companyDefinitions.splice($scope.scopeModel.companyDefinitions.indexOf({ Entity: companyDefinitionObj }), 1);
                };

                function validateSettingIdentity() {
                    var definitions = $scope.scopeModel.companyDefinitions;
                    var definitionsLength = definitions.length;
                    for (var i = 0; i < definitionsLength ; i++) {
                        var currentDdefinition = definitions[i];
                        for (var j = 0 ; j < definitionsLength ; j++) {
                            if (j != i && currentDdefinition.Entity.Setting.ConfigId == definitions[j].Entity.Setting.ConfigId)
                                return false;
                        }
                    }
                    return true;
                }

                function validateNameIdentity() {
                    var definitions = $scope.scopeModel.companyDefinitions;
                    var definitionsLength = definitions.length;
                    for (var i = 0; i < definitionsLength ; i++) {
                        var currentDdefinition = definitions[i];
                        for (var j = 0 ; j < definitionsLength ; j++) {
                            if (j != i && currentDdefinition.Entity.Name == definitions[j].Entity.Name)
                                return false;
                        }
                    }
                    return true;
                }
                defineMenuActions();
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.companyDefinitions.length = 0;
                        if (payload.extendedSettings != undefined) {

                            for (var currentCompanyDefinitionId in payload.extendedSettings) {
                                if (payload.extendedSettings[currentCompanyDefinitionId] != undefined && currentCompanyDefinitionId != "$type") {
                                    console.log(currentCompanyDefinitionId, payload.extendedSettings[currentCompanyDefinitionId]);
                                    $scope.scopeModel.companyDefinitions.push({ Entity: payload.extendedSettings[currentCompanyDefinitionId]});
                                }
                            }
                        }
                    }
                };

                api.getData = function () {
                    var companyDefinitionSettings = {};
                    for (var i = 0; i < $scope.scopeModel.companyDefinitions.length; i++) {
                        var companyDefinitionSetting = $scope.scopeModel.companyDefinitions[i];
                        if (companyDefinitionSettings[companyDefinitionSetting.Entity.Setting.ConfigId] == undefined) {
                            companyDefinitionSettings[companyDefinitionSetting.Entity.Setting.ConfigId] = {
                                CompanyDefinitionSettingId: UtilsService.guid(),
                                Name: companyDefinitionSetting.Entity.Name,
                                Setting: companyDefinitionSetting.Entity.Setting,
                            };
                        }
                    }
                    return (companyDefinitionSettings);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [{
                    name: "Edit",
                    clicked: editCompanyDefinition
                }];

                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }
            function editCompanyDefinition(companyDefinitionObj) {
                var onCompanyDefinitionUpdated = function (companyDefinitionEntity) {
                    var index = $scope.scopeModel.companyDefinitions.indexOf(companyDefinitionObj);
                    $scope.scopeModel.companyDefinitions[index] = { Entity: companyDefinitionEntity };
                };
                VRCommon_CompanyDefinitionService.editCompanyDefinition(companyDefinitionObj.Entity, onCompanyDefinitionUpdated);
            }
        }
        return directiveDefinitionObject;
    }]);