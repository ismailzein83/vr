(function (app) {

    'use strict';

    whsJazzAccountCodeCustomObjectStaticEditor.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService','WhS_Jazz_AccountCodeAPIService'];

    function whsJazzAccountCodeCustomObjectStaticEditor(UtilsService, VRUIUtilsService, VRNotificationService, WhS_Jazz_AccountCodeAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                onselectionchanged: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Jazz/Elements/Directives/Templates/AccountCodeCustomObjectRuntime.html"

        };
        function SettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var transactionTypeSelectorAPI;
            var transactionTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var transactionTypeSelectedPromise;

            var carriersSelectorAPI;
            var carriersSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            var genericBusinessEntity;

            function initializeController() {

                $scope.scopeModel = {};

                $scope.scopeModel.onTransactionTypeSelectorReady = function (api) {
                    transactionTypeSelectorAPI = api;
                    transactionTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCarriersSelectorReady = function (api) {
                    carriersSelectorAPI = api;
                    carriersSelectorReadyPromiseDeferred.resolve();
                };
                
                $scope.scopeModel.onTransactionTypeSelectionChanged = function (value) {
                    if (transactionTypeSelectorAPI != undefined) {
                        if (value) {
                            if(transactionTypeSelectedPromise==undefined)
                            WhS_Jazz_AccountCodeAPIService.GetTransctionType(value.GenericBusinessEntityId, "476cc49c-7fab-482a-b5f3-f91772af9edf").then(function (response) {
                                genericBusinessEntity = response;
                                var setLoader = function (value) { $scope.scopeModel.isCarriersSelectorloading = value; };
                                var carriersPayload = {
                                    filter: genericBusinessEntity.FieldValues.CarrierType == 1 ? { GetCustomers: true, GetSuppliers: false } : { GetCustomers: false, GetSuppliers: true }
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, carriersSelectorAPI, carriersPayload, setLoader);
                            });
                        }
                    }
                };

                defineAPI();
            }
            function loadTransactionTypeSelector(payload) {
                var transactionTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                transactionTypeSelectorReadyPromiseDeferred.promise.then(function () {
                    payload.businessEntityDefinitionId = "476cc49c-7fab-482a-b5f3-f91772af9edf";
                    VRUIUtilsService.callDirectiveLoad(transactionTypeSelectorAPI, payload, transactionTypeSelectorLoadPromiseDeferred);
                });
                return transactionTypeSelectorLoadPromiseDeferred.promise;
            }
            function loadCarriersSelector(payload) {
                var carriersSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                carriersSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(carriersSelectorAPI, payload, carriersSelectorLoadPromiseDeferred);
                    transactionTypeSelectedPromise = undefined;
                });
                return carriersSelectorLoadPromiseDeferred.promise;
            } 

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.selectedValues != undefined && payload.selectedValues.TransactionTypeId != undefined) {
                        transactionTypeSelectedPromise = UtilsService.createPromiseDeferred();
                        var transactionTypePayload = {
                            selectedIds: payload.selectedValues.TransactionTypeId
                        };
                        return loadTransactionTypeSelector(transactionTypePayload).then(function () {
                            WhS_Jazz_AccountCodeAPIService.GetTransctionType(payload.selectedValues.TransactionTypeId, "476cc49c-7fab-482a-b5f3-f91772af9edf").then(function (response) {
                                genericBusinessEntity = response;
                                var selectedIds = [];
                                for (var j = 0; j < payload.selectedValues.Carriers.Carriers.length; j++) {

                                    var carrier = payload.selectedValues.Carriers.Carriers[j];
                                    selectedIds.push(carrier.CarrierAccountId);
                                }
                                loadCarriersSelector({
                                    filter: genericBusinessEntity.FieldValues.CarrierType == 1 ? { GetCustomers: true, GetSuppliers: false } : { GetCustomers: false, GetSuppliers: true },
                                    selectedIds: selectedIds
                                })
                            });
                        });

                    }
                    else loadTransactionTypeSelector({});
                };

                api.setData = function (payload) {
                    var Carriers = [];
                    if (carriersSelectorAPI != undefined && carriersSelectorAPI.getSelectedIds() != undefined) {
                        var selectedIds = carriersSelectorAPI.getSelectedIds();
                        for (var i = 0; i < selectedIds.length; i++) {
                            Carriers.push({
                                CarrierAccountId: selectedIds[i]
                            });
                        }
                    }
                    
                    payload.Carriers = {
                        $type:"TOne.WhS.Jazz.Entities.WhsJazzAccountCodeCarriers,TOne.WhS.Jazz.Entities",
                        Carriers: Carriers,
                    };
                    
                    payload.TransactionTypeId = transactionTypeSelectorAPI.getSelectedIds();

                    return payload;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsJazzAccountcodeCustomobjectStaticeditor', whsJazzAccountCodeCustomObjectStaticEditor);

})(app);
