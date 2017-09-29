"use strict";

app.directive("vrInvoicetypeFilenameTime", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new TimeFileNamePart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceFileNameSettings/MainExtensions/Templates/TimeFileNamePartTemplate.html"

        };

        function TimeFileNamePart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    if (payload != undefined) {
                        context = payload.context;
                        if (context != undefined) {
                           
                        }
                        if (payload.concatenatedPartSettings != undefined) {
                            $scope.dateTimeFormatValue = payload.concatenatedPartSettings.DateTimeFormat;
                        }
                        var promises = [];
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.InvoiceFileNamePart.TimeFileNamePart ,Vanrise.Invoice.MainExtensions",
                        DateTimeFormat: $scope.dateTimeFormatValue
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);