'use strict';

app.directive('vrCommonCompanysettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VRCommon_CompanySettingService',
    function (UtilsService, VRUIUtilsService, VRCommon_CompanySettingService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CompanySettingsEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/Settings/CompanySettings/Templates/CompanySettingsTemplate.html"
        };

        function CompanySettingsEditor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                    {
                        for(var i=0;i<ctrl.datasource.length;i++)
                        {
                            var item = ctrl.datasource[i];
                            if(item.Entity.IsDefault)
                            {
                                return null;
                            }
                        }
                        return "You Should add at least one defualt setting.";

                    }
                    return "You Should add at least one setting.";
                }
                ctrl.addCompanySetting = function () {
                    var onCompanySettingAdded = function (companySetting) {
                        ctrl.datasource.push({ Entity: companySetting });
                    }

                    VRCommon_CompanySettingService.addCompanySetting(onCompanySettingAdded, ctrl.datasource);
                };
                ctrl.removeCompanySetting = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                }
                defineMenuActions();
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var companySettingsPayload;
                    if (payload != undefined && payload.data != undefined) {
                        companySettingsPayload = payload.data;
                    }
                    if (companySettingsPayload != undefined && companySettingsPayload.Settings != undefined) {
                        for (var i = 0; i < companySettingsPayload.Settings.length; i++) {
                            var companySetting = companySettingsPayload.Settings[i];
                            ctrl.datasource.push({ Entity: companySetting });
                        }
                    }
                }

                api.getData = function () {
                    var companySettings;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        companySettings = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            companySettings.push(currentItem.Entity);
                        }
                    }
                    return {
                        $type: "Vanrise.Entities.CompanySettings, Vanrise.Entities",
                        Settings: companySettings
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }
            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editCompanySetting,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }
            function editCompanySetting(companySettingObj) {
                var onCompanySettingUpdated = function (companySetting) {
                    var index = ctrl.datasource.indexOf(companySettingObj);
                    ctrl.datasource[index] = { Entity: companySetting };
                }
                VRCommon_CompanySettingService.editCompanySetting(companySettingObj.Entity, onCompanySettingUpdated, ctrl.datasource);
            }
        }
    }]);