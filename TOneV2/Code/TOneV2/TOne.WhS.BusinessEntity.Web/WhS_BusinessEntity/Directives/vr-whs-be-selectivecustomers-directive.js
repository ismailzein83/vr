'use strict';
app.directive('vrWhsBeSelectivecustomers', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var beCustomersCtor = new beCustomers(ctrl, $scope);
                beCustomersCtor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return getBeSelectiveCustomersTemplate(attrs);
            }

        };

        function getBeSelectiveCustomersTemplate(attrs) {
            return '/Client/Modules/WhS_BusinessEntity/Directives/Templates/SelectiveCustomersDirectiveTemplate.html';
        }

        function beCustomers(ctrl, $scope) {
            var carrierAccountDirectiveAPI;

            function initializeController() {
                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    declareDirectiveAsReady();
                }

                declareDirectiveAsReady();
            }

            function declareDirectiveAsReady() {
                if (carrierAccountDirectiveAPI == undefined)
                    return;

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function () {
                    return carrierAccountDirectiveAPI.load();
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.SelectiveCustomersSettings, TOne.WhS.BusinessEntity.Entities",
                        CustomerIds: UtilsService.getPropValuesFromArray(carrierAccountDirectiveAPI.getData(), "CarrierAccountId")
                    };
                }

                api.setData = function (customerGroupSettings) {
                    carrierAccountDirectiveAPI.setData(customerGroupSettings.CustomerIds);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);