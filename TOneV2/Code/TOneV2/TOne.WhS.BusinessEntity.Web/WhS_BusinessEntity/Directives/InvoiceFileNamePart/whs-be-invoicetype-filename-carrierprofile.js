"use strict";

app.directive("whsBeInvoicetypeFilenameCarrierprofile", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "WhS_BE_ProfileFieldEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_BE_ProfileFieldEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ProfileFieldFileNamePart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/InvoiceFileNamePart/Templates/ProfileFieldFileNameTemplate.html"

        };

        function ProfileFieldFileNamePart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.profileFields = UtilsService.getArrayEnum(WhS_BE_ProfileFieldEnum);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.concatenatedPartSettings != undefined) {
                            $scope.scopeModel.selectedProfileField = UtilsService.getItemByVal($scope.scopeModel.profileFields, payload.concatenatedPartSettings.PartName, "value");
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.InvoiceFileNamePart.CarrierFileNamePart ,TOne.WhS.BusinessEntity.MainExtensions",
                        PartName: $scope.scopeModel.selectedProfileField.value,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);