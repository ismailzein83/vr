"use strict";

app.directive("vrWhsBeVolumecommitmentItems", ["UtilsService", "VRNotificationService", "WhS_BE_VolumeCommitmentService",
    function (UtilsService, VRNotificationService, WhS_BE_VolumeCommitmentService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new Items($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Deal/Templates/VolumeCommitmentItemsManagement.html"

        };

        function Items($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one item.";
                }

                ctrl.addItem = function () {
                    var onVolumeCommitmentItemAdded = function (volumeCommitmentItem) {
                        ctrl.datasource.push({ Entity: volumeCommitmentItem });
                    }
                    WhS_BE_VolumeCommitmentService.addVolumeCommitmentItem(onVolumeCommitmentItemAdded, getContext());
                };

                ctrl.removeItem = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                }
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var volumeCommitmentItems;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        volumeCommitmentItems = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            volumeCommitmentItems.push({
                                Name: currentItem.Entity.Name,
                                ZoneIds: currentItem.Entity.ZoneIds,
                                Rates: currentItem.Entity.Rates,
                            });
                        }
                    }
                    return volumeCommitmentItems;
                }

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.volumeCommitmentItems != undefined) {
                            for (var i = 0; i < payload.volumeCommitmentItems.length; i++) {
                                var volumeCommitmentItem = payload.volumeCommitmentItems[i];
                                ctrl.datasource.push({ Entity: volumeCommitmentItem });
                            }
                        }
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editVolumeCommitmentItem,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                }
            }

            function editVolumeCommitmentItem(volumeCommitmentItemObj) {
                var onVolumeCommitmentItemUpdated = function (volumeCommitmentItem) {
                    var index = ctrl.datasource.indexOf(volumeCommitmentItemObj);
                    ctrl.datasource[index] = { Entity: volumeCommitmentItem };
                }
                WhS_BE_VolumeCommitmentService.editVolumeCommitmentItem(volumeCommitmentItemObj.Entity, onVolumeCommitmentItemUpdated, getContext());
            }
            function getContext()
            {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);