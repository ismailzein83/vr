"use strict";

app.directive("vrInvoicetypeInvoicesubsectionsettingsItemgrouping", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ItemGroupingSubSection($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/SubSectionSettings/InvoiceUISubSectionSettings/MainExtensions/Templates/ItemGroupingSubSectionTemplate.html"

        };

        function ItemGroupingSubSection($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            var itemGroupingSubSectionSettingsAPI;
            var itemGroupingSubSectionSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var itemGroupingSelectedReadyPromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.itemGroupings = [];
                $scope.scopeModel.onItemGroupingSubSectionSettingsReady = function (api) {
                    itemGroupingSubSectionSettingsAPI = api;
                    itemGroupingSubSectionSettingsReadyPromiseDeferred.resolve();

                };
                $scope.scopeModel.onItemGroupingSelectionChanged = function (selectedGroupItem) {
                    if (context != undefined && selectedGroupItem != undefined) {
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoading = value;
                        };
                        var payload = { context: getContext() };

                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, itemGroupingSubSectionSettingsAPI, payload, setLoader, itemGroupingSelectedReadyPromiseDeferred);
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        var invoiceSubSectionSettingsEntity = payload.invoiceSubSectionSettingsEntity;
                        if (context != undefined)
                        {
                            $scope.scopeModel.itemGroupings = context.getItemGroupingsInfo();
                        }
                        if(invoiceSubSectionSettingsEntity != undefined)
                        {
                            $scope.scopeModel.selectedItemGrouping = UtilsService.getItemByVal($scope.scopeModel.itemGroupings, invoiceSubSectionSettingsEntity.ItemGroupingId, "ItemGroupingId");
                            itemGroupingSelectedReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                            var itemGroupingSubSectionSettingsDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                            UtilsService.waitMultiplePromises([itemGroupingSubSectionSettingsReadyPromiseDeferred.promise, itemGroupingSelectedReadyPromiseDeferred.promise]).then(function () {
                                var itemGroupingDirectivePayload = { context: getContext() };
                                if (invoiceSubSectionSettingsEntity != undefined)
                                    itemGroupingDirectivePayload.subSectionSettingsEntity = invoiceSubSectionSettingsEntity.Settings;
                                VRUIUtilsService.callDirectiveLoad(itemGroupingSubSectionSettingsAPI, itemGroupingDirectivePayload, itemGroupingSubSectionSettingsDeferredLoadPromiseDeferred);
                            });
                            promises.push(itemGroupingSubSectionSettingsDeferredLoadPromiseDeferred.promise);
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.ItemGroupingSection ,Vanrise.Invoice.MainExtensions",
                        ItemGroupingId: $scope.scopeModel.selectedItemGrouping.ItemGroupingId,
                        Settings: itemGroupingSubSectionSettingsAPI.getData(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getContext()
            {
                var currentContext = context;
                if(currentContext == undefined)
                {
                    currentContext = {};
                }
                currentContext.getItemGroupingId = function () {
                    if ($scope.scopeModel.selectedItemGrouping != undefined) {
                        return $scope.scopeModel.selectedItemGrouping.ItemGroupingId;
                    }
                };
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);