﻿//"use strict";

//app.directive("retailDemoInvoicetypeNetworkrentalinvoicesettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
//    function (UtilsService, VRNotificationService, VRUIUtilsService) {

//        var directiveDefinitionObject = {

//            restrict: "E",
//            scope:
//            {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;

//                var ctor = new NetworkRentalInvoiceSettings($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/Retail_Demo/Elements/NetworkRental/Directives/Templates/NetworkRentalInvoiceSettingsTemplate.html"

//        };

//        function NetworkRentalInvoiceSettings($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var beDefinitionSelectorAPI;
//            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
//                    beDefinitionSelectorAPI = api;
//                    beDefinitionSelectorPromiseDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    promises.push(getBusinessEntityDefinitionSelectorLoadPromise());

//                    function getBusinessEntityDefinitionSelectorLoadPromise() {
//                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

//                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
//                            var selectorPayload = {
//                                filter: {
//                                    Filters: [{
//                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
//                                    }]
//                                }
//                            };
//                            if (payload != undefined && payload.extendedSettingsEntity != undefined) {
//                                selectorPayload.selectedIds = payload.extendedSettingsEntity.AccountBEDefinitionId;
//                            }
//                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
//                        });
//                        return businessEntityDefinitionSelectorLoadDeferred.promise;
//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                };


//                api.getData = function () {
//                    return {
//                        $type: "Retail.Demo.Business.NetworkRentalnvoiceSettings, Retail.Demo.Business",
//                        AccountBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
//                    };
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }
//        }

//        return directiveDefinitionObject;
//    }
//]);