"use strict";
app.directive("vWhsBeSalepricelistemailsendertask", ['UtilsService',
    function (UtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/ScheduleTask/Templates/SalePricelistEmailSenderTaskTemplate.html"
        };

        function DirectiveConstructor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.getData = function () {
                    return {
                        $type: 'TOne.WhS.BusinessEntity.BP.Arguments.SalePricelistNotificationProcessInput, TOne.WhS.BusinessEntity.BP.Arguments'
                    };
                };

                api.getExpressionsData = function () {
                    return null;
                };

                api.load = function (payload) {
                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);
