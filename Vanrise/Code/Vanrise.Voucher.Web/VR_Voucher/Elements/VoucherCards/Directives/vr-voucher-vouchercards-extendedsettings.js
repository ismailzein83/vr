"use strict";

app.directive("vrVoucherVouchercardsExtendedsettings", ["UtilsService", "VRNotificationService", "VR_Voucher_VouchercardsExtendedsettingsService",
    function (UtilsService, VRNotificationService, VR_Voucher_VouchercardsExtendedsettingsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new VoucherCardsExtendedsettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Voucher/Elements/VoucherCards/Directives/Templates/VoucherCardsExtendedsettingsTemplate.html"

        };

        function VoucherCardsExtendedsettings($scope, ctrl, $attrs) {
            var gridAPI;
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = [];
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource != undefined && $scope.scopeModel.datasource.length > 0)
                        return null;
                    return "You Should add at least one part.";
                };

                $scope.scopeModel.addSerialNumberPart = function () {
                    var onSerialNumberPartAdded = function (serialNumberPart) {
                        $scope.scopeModel.datasource.push({ Entity: serialNumberPart });
                    };

                    VR_Voucher_VouchercardsExtendedsettingsService.addSerialNumberPart(onSerialNumberPartAdded, getContext());
                };

                $scope.scopeModel.removeSerialNumberPart = function (dataItem) {
                    var index = $scope.scopeModel.datasource.indexOf(dataItem);
                    $scope.scopeModel.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var serialNumberParts;
                    if ($scope.scopeModel.datasource != undefined) {
                        serialNumberParts = [];
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var currentItem = $scope.scopeModel.datasource[i];
                            serialNumberParts.push(currentItem.Entity);
                        }
                    }
                    return serialNumberParts;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.serialNumberParts != undefined) {
                            for (var i = 0; i < payload.serialNumberParts.length; i++) {
                                var serialNumberPart = payload.serialNumberParts[i];
                                $scope.scopeModel.datasource.push({ Entity: serialNumberPart });
                            }
                        }
                    }
                };

                if ($scope.scopeModel.onReady != null)
                    $scope.scopeModel.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editSerialNumberPart,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editSerialNumberPart(serialNumberPartObj) {
                var onSerialNumberPartUpdated = function (serialNumberPart) {
                    var index = $scope.scopeModel.datasource.indexOf(serialNumberPartObj);
                    $scope.scopeModel.datasource[index] = { Entity: serialNumberPart };
                };

                VR_Voucher_VouchercardsExtendedsettingsService.editSerialNumberPart(serialNumberPartObj.Entity, onSerialNumberPartUpdated, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);