'use strict';

app.directive('vrCommonCompanysettingsEditor', ['UtilsService', 'VRCommon_CompanySettingService', 'VRCommon_CompanySettingsAPIService','VRNotificationService',
    function (UtilsService, VRCommon_CompanySettingService, VRCommon_CompanySettingsAPIService, VRNotificationService) {

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
            var companyDefinitionSettings = {};
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        var defaultCount = 0;
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var item = ctrl.datasource[i];
                            if (item.Entity.IsDefault) {
                                defaultCount++;
                            }
                        }
                        if (defaultCount == 0)
                            return "You Should add at least one default settings.";
                        if (defaultCount == 1)
                            return null;
                        if (defaultCount > 1)
                            return "Only one default settings is permitted.";
                    }
                    return "You Should add at least one settings.";
                };
                ctrl.addCompanySetting = function () {
                    var onCompanySettingAdded = function (companySetting) {
                        if (ctrl.datasource.length == 0) companySetting.IsDefault = true;
                        ctrl.datasource.push({ Entity: companySetting });
                    };

                    VRCommon_CompanySettingService.addCompanySetting(onCompanySettingAdded, ctrl.datasource.length == 0, getContext());
                };
                ctrl.removeCompanySetting = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                    if (ctrl.datasource.length == 1) {
                        var companySetting = ctrl.datasource[0].Entity;
                        companySetting.IsDefault = true;
                        ctrl.datasource[0] = { Entity: companySetting };
                    }

                };
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
                    return VRCommon_CompanySettingsAPIService.GetCompanyDefinitionSettings().then(function (response) {
                        if (response != undefined && response != null) {
                            companyDefinitionSettings = response;
                        }
                    });
                };

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
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);


            }
            function defineMenuActions() {

                $scope.gridMenuActions = function (dataItem) {
                    var defaultMenuActions = [
                        {
                            name: "Edit",
                            clicked: editCompanySetting
                        }];
                    if (UtilsService.isContextReadOnly($scope)) {
                        defaultMenuActions.length = 0;
                        defaultMenuActions = [{
                            name: "View",
                            clicked: viewCompanySetting
                        }];
                    }
                    if (!UtilsService.isContextReadOnly($scope) && dataItem.Entity.IsDefault == false) {
                        defaultMenuActions[defaultMenuActions.length] = {
                            name: "Set As Default",
                            clicked: setAsDefaultCompanySetting
                        };
                    }
                    return defaultMenuActions;
                };
            }
            function editCompanySetting(companySettingObj) {
                var onCompanySettingUpdated = function (companySetting) {
                    var index = ctrl.datasource.indexOf(companySettingObj);
                    ctrl.datasource[index] = { Entity: companySetting };
                };
                VRCommon_CompanySettingService.editCompanySetting(companySettingObj.Entity, onCompanySettingUpdated, getContext(), ctrl.datasource);
            }
            function setAsDefaultCompanySetting(companySettingObj) {

                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            if (currentItem.Entity.IsDefault == true) {
                                currentItem.Entity.IsDefault = false;
                                var index = ctrl.datasource.indexOf(currentItem);
                                ctrl.datasource[index] = { Entity: currentItem.Entity };
                                break;
                            }
                        }
                        companySettingObj.Entity.IsDefault = true;
                        var index = ctrl.datasource.indexOf(companySettingObj);
                        ctrl.datasource[index] = { Entity: companySettingObj.Entity };
                        defineMenuActions();
                    }
                });
            }
            function viewCompanySetting(companySettingObj) {
                VRCommon_CompanySettingService.viewCompanySetting(companySettingObj.Entity, getContext());
            }

            function getContext() {

                var context = {
                };
                context.getCompanyDefinitionSettings = function () {
                    return companyDefinitionSettings;
                };
                return context;
            }
        }
    }]);