"use strict";

app.directive("vrInvoicetypeItemsetnamepartsCurrentitemsetname", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CurrentItemSetNameConcatenatedPart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/SubSectionSettings/InvoiceUISubSectionSettings/MainExtensions/VRConcatenatedPart/Templates/CurrentItemSetNameConcatenatedPartTemplate.html"

        };

        function CurrentItemSetNameConcatenatedPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.concatenatedPartSettings) {
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.VRConcatenatedPart.CurrentItemSetNameConcatenatedPart ,Vanrise.Invoice.MainExtensions",
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);