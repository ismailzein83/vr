"use strict";

app.directive("vrVoucherVouchercardsExtendedsettingsSequenceserialnumberpart", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Voucher_DateCounterType",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Voucher_DateCounterType) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceDateSerialNumberPart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Voucher/Elements/VoucherCards/Directives/MainExtensions/Templates/SequenceSerialNumberPartTemplate.html"

        };

        function InvoiceDateSerialNumberPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dateCounterType = UtilsService.getArrayEnum(VR_Voucher_DateCounterType);
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.concatenatedPartSettings != undefined) {
                            $scope.scopeModel.paddingLeft = payload.concatenatedPartSettings.PaddingLeft;
                            $scope.scopeModel.selectedDateCounterType = UtilsService.getItemByVal($scope.scopeModel.dateCounterType, payload.concatenatedPartSettings.DateCounterType, "value");

                        }
                        var promises = [];
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Voucher.Business.VoucharCardSequenceSerialNumberPart ,Vanrise.Voucher.Business",
                        PaddingLeft: $scope.scopeModel.paddingLeft,
                        DateCounterType: $scope.scopeModel.selectedDateCounterType != undefined ? $scope.scopeModel.selectedDateCounterType.value : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);