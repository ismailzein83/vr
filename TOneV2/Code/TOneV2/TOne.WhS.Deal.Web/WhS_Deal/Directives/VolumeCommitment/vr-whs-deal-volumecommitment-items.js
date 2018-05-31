"use strict";

app.directive("vrWhsDealVolumecommitmentItems", ["UtilsService", "VRNotificationService", "WhS_Deal_VolumeCommitmentService",
    function (UtilsService, VRNotificationService, WhS_Deal_VolumeCommitmentService) {

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
            templateUrl: "/Client/Modules/WhS_Deal/Directives/VolumeCommitment/Templates/VolumeCommitmentItemsManagement.html"

        };

        function Items($scope, ctrl, $attrs) {

            var gridAPI;
            var context;
            var lastGroupNumber;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one item.";
                };

                ctrl.addItem = function () {
                    var onVolumeCommitmentItemAdded = function (volumeCommitmentItem) {
                        lastGroupNumber = lastGroupNumber + 1;
                        volumeCommitmentItem.ZoneGroupNumber = lastGroupNumber;
                        ctrl.datasource.push({ Entity: volumeCommitmentItem });
                    };
                    WhS_Deal_VolumeCommitmentService.addVolumeCommitmentItem(onVolumeCommitmentItemAdded, getContext());
                };

                ctrl.removeItem = function (dataItem) {
                    VRNotificationService.showConfirmation().then(function (response) {
                        if (response) {
                            var index = ctrl.datasource.indexOf(dataItem);
                            ctrl.datasource.splice(index, 1);
                        }
                    });

                };
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
                                SaleZones: currentItem.Entity.SaleZones,
                                CountryId: currentItem.Entity.CountryId,
                                Tiers: currentItem.Entity.Tiers,
                                ZoneGroupNumber: currentItem.Entity.ZoneGroupNumber
                            });
                        }
                    }
                    return { lastGroupNumber: lastGroupNumber, volumeCommitmentItems: volumeCommitmentItems };
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        lastGroupNumber = context.lastGroupNumber;
                        if (payload.volumeCommitmentItems != undefined) {
                            for (var i = 0; i < payload.volumeCommitmentItems.length; i++) {
                                var volumeCommitmentItem = payload.volumeCommitmentItems[i];
                                ctrl.datasource.push({ Entity: volumeCommitmentItem });
                            }
                        }
                        else
                            ctrl.datasource.length = 0;
                    }
                };

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
                };
            }

            function editVolumeCommitmentItem(volumeCommitmentItemObj) {
                var onVolumeCommitmentItemUpdated = function (volumeCommitmentItem) {
                    volumeCommitmentItem.ZoneGroupNumber = volumeCommitmentItemObj.Entity.ZoneGroupNumber;
                    var index = ctrl.datasource.indexOf(volumeCommitmentItemObj);
                    ctrl.datasource[index] = { Entity: volumeCommitmentItem };
                };
                WhS_Deal_VolumeCommitmentService.editVolumeCommitmentItem(volumeCommitmentItemObj.Entity, onVolumeCommitmentItemUpdated, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);