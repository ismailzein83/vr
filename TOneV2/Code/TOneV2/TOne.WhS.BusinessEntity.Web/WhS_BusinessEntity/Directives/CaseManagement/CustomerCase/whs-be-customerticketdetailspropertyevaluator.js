(function (app) {

    'use strict';

    CustomerTicketDetailsPropertyEvaluator.$inject = ['WhS_BE_CustomerPropertyEnum', 'UtilsService', 'VRUIUtilsService'];

    function CustomerTicketDetailsPropertyEvaluator(WhS_BE_CustomerPropertyEnum, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var selector = new customerTicketDetailsPropertyEvaluator($scope, ctrl, $attrs);
                selector.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/CaseManagement/CustomerCase/Templates/CustomerTicketDetailsPropertyEvaluatorTemplate.html'
        };

        function customerTicketDetailsPropertyEvaluator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.customerTicketDetailsFields = UtilsService.getArrayEnum(WhS_BE_CustomerTicketDetailsFieldPropertyEnum);

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.objectPropertyEvaluator != undefined)
                        $scope.scopeModel.selectedField = UtilsService.getItemByVal($scope.scopeModel.customerTicketDetailsFields, payload.objectPropertyEvaluator.TicketDetailsField, "value");
                };

                api.getData = function () {

                    var data = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CustomerTicketDetailsPropertyEvaluator, TOne.WhS.BusinessEntity.MainExtensions",
                        TicketDetailsField: $scope.scopeModel.selectedField.value
                    };
                    return data;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('whsBeCustomerticketdetailspropertyevaluator', CustomerTicketDetailsPropertyEvaluator);

})(app);
