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

                var ctor = new selectiveCustomersCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/CustomerGroup/Templates/SelectiveCustomersDirectiveTemplate.html';
            }

        };

        function selectiveCustomersCtor(ctrl, $scope) {
            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var customerIds;
                    var linkedCustomerId;

                    if (payload != undefined) {
                        if (payload.customerGroupSettings != undefined) {
                            customerIds = payload.customerGroupSettings.CustomerIds;
                        }
                        linkedCustomerId = payload.linkedCustomerId;
                    }

                    var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                    carrierAccountReadyPromiseDeferred.promise.then(function () {
                        var carrierAccountPayload = {
                            filter: {},
                            selectedIds: customerIds,
                            lockedIds: linkedCustomerId != undefined ? [linkedCustomerId] : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, carrierAccountPayload, loadCarrierAccountPromiseDeferred);
                    });

                    return loadCarrierAccountPromiseDeferred.promise;
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups.SelectiveCustomerGroup, TOne.WhS.BusinessEntity.MainExtensions",
                        CustomerIds: carrierAccountDirectiveAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);