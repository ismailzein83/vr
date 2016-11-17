"use strict";

app.directive("vrBebridgeBedefinitionSettingsGrid", ["UtilsService", "VRNotificationService", "VR_BEBridge_BEReceiveDefinitionSettingsService",
    function (UtilsService, VRNotificationService, VR_BEBridge_BEReceiveDefinitionSettingsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var bedefinitionSettingsGrid = new BedefinitionSettingsGrid($scope, ctrl, $attrs);
                bedefinitionSettingsGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_BEBridge/Directives/BERecieveDefinitionSetting/Templates/BeRecieveDefinitionGridSettingTemplate.html"

        };

        function BedefinitionSettingsGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            var counter = 1;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You should add at least one setting";
                };

                ctrl.addBERecieveDefinitionSetting = function () {
                    var onBEReceiveDefinitionSettingsAdded = function (dataRecordField) {
                        dataRecordField.ID = counter++;
                        ctrl.datasource.push(dataRecordField);
                    };

                    VR_BEBridge_BEReceiveDefinitionSettingsService.addReceiveDefinitionSettings(onBEReceiveDefinitionSettingsAdded, ctrl.datasource);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var fields;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        fields = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            fields.push({
                                TargetBESynchronizer: currentItem.TargetBESynchronizer,
                                TargetBEConvertor: currentItem.TargetBEConvertor
                            });
                        }

                    }
                    var obj = {
                        Fields: fields
                    };
                    return obj;
                };

                api.load = function (payload) {

                    if (payload != undefined) {
                        if (payload.Fields && payload.Fields.length > 0) {
                            for (var i = 0; i < payload.Fields.length; i++) {
                                var dataItem = payload.Fields[i];
                                dataItem.ID = counter++;
                                ctrl.datasource.push(dataItem);
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
                    clicked: editReceiveDefinitionSettings
                },
                {
                    name: "Delete",
                    clicked: deleteReceiveDefinitionSettings
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editReceiveDefinitionSettings(dataRecordFieldObj) {
                var onBEReceiveDefinitionSettingsUpdated = function (dataRecordField) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordFieldObj.ID, 'ID');
                    dataRecordField.ID = counter++;
                    ctrl.datasource[index] = dataRecordField;
                };

                VR_BEBridge_BEReceiveDefinitionSettingsService.editReceiveDefinitionSettings(dataRecordFieldObj, onBEReceiveDefinitionSettingsUpdated, ctrl.datasource);
            }

            function deleteReceiveDefinitionSettings(dataRecordFieldObj) {
                var onBEReceiveDefinitionSettingsDeleted = function (dataRecordField) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataRecordFieldObj.ID, 'ID');
                     ctrl.datasource.splice(index, 1);
                };

                VR_BEBridge_BEReceiveDefinitionSettingsService.deleteReceiveDefinitionSettings($scope, dataRecordFieldObj, onBEReceiveDefinitionSettingsDeleted);
            }
        }
        return directiveDefinitionObject;
    }
]);