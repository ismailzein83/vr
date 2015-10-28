'use strict';
app.directive('vrWhsBeCustomergroupSelective', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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
            return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/CustomerGroup/Templates/SelectiveCustomersDirectiveTemplate.html';
        }

        function beCustomers(ctrl, $scope) {
            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (customerGroupSettings) {
                    var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                    carrierAccountReadyPromiseDeferred.promise.then(function () {
                        var directivePayload;
                        if (customerGroupSettings != undefined && customerGroupSettings != null)
                            directivePayload = customerGroupSettings.CustomerIds;
                        VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, directivePayload, loadCarrierAccountPromiseDeferred);
                    });

                    return loadCarrierAccountPromiseDeferred.promise;
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups.SelectiveCustomerGroup, TOne.WhS.BusinessEntity.MainExtensions",
                        CustomerIds: UtilsService.getPropValuesFromArray(carrierAccountDirectiveAPI.getData(), "CarrierAccountId")
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);