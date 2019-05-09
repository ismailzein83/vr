"use strict";

app.directive("vrGenericdataGenericbeAdditionalsettingsGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new AdditionalSettingsGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/AdditionalSettingsGridTemplate.html"

        };

        function AdditionalSettingsGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];
                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource.length > 0 && checkDuplicateName())
                        return "Name in each setting should be unique.";
                    return null;
                };

                $scope.scopeModel.addAdditionalSetting = function () {
                    var onAdditionalSettingAdded = function (addedItem) {
                        $scope.scopeModel.datasource.push({Entity:{ Name: addedItem.objectName, Settings: addedItem.objectSettings }});
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBEAdditionalSetting(onAdditionalSettingAdded, getContext());
                };
              
                $scope.scopeModel.removeSetting = function (dataItem) {
                    var index = $scope.scopeModel.datasource.indexOf(dataItem);
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var additionalSettings;
                    if ($scope.scopeModel.datasource != undefined && $scope.scopeModel.datasource.length > 0) {
                        additionalSettings = {};
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var currentItem = $scope.scopeModel.datasource[i];
                            if (currentItem.Entity != undefined) {
                                additionalSettings[currentItem.Entity.Name] = currentItem.Entity.Settings;
                            }
                        }
                    }
                    return additionalSettings;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        $scope.scopeModel.datasource.length = 0;
                        if (payload.additionalSettings != undefined) {
                            for (var key in payload.additionalSettings) {
                                if (key != "$type") {
                                    $scope.scopeModel.datasource.push({
                                        Entity: {
                                            Name: key,
                                            Settings: payload.additionalSettings[key]
                                        }
                                    });
                                }
                            }
                        }
                    }
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function defineMenuActions() {
                var defaultMenuActions = [
                    {
                        name: "Edit",
                        clicked: editSetting
                    }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editSetting(settingObj) {
                var onSettingUpdated = function (updatedSetting) {
                    var index = $scope.scopeModel.datasource.indexOf(settingObj);
                    $scope.scopeModel.datasource[index] = {
                        Entity: {
                            Name: updatedSetting.objectName,
                            Settings: updatedSetting.objectSettings
                        }
                    };
                };
                VR_GenericData_GenericBEDefinitionService.editGenericBEAdditionalSetting(onSettingUpdated, settingObj.Entity, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

            function checkDuplicateName() {
                for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                    var currentItem = $scope.scopeModel.datasource[i];
                    for (var j = i+1; j < $scope.scopeModel.datasource.length; j++) {
                        if ($scope.scopeModel.datasource[j].Name == currentItem.Name)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);