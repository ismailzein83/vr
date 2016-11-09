"use strict";

app.directive("vrInvoicetypeInvoicefilterconditionFiltergroup", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new FilterGroupCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/GridSettings/InvoiceGridActionSettings/MainExtensions/InvoiceFilterCondition/Templates/FilterGroupConditionTemplate.html"

        };

        function FilterGroupCondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var recordFilterAPI;
            var recordFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onRecordFilterReady = function (api) {
                    recordFilterAPI = api;
                    recordFilterReadyPromiseDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceFilterConditionEntity;
                    if (payload != undefined) {
                        invoiceFilterConditionEntity = payload.invoiceFilterConditionEntity;
                        context = payload.context;
                    }
                    var promises = [];
                    var recordFilterLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    recordFilterReadyPromiseDeferred.promise.then(function () {
                        var recordFilterPayload = { context: getContext() }
                        if (invoiceFilterConditionEntity != undefined) {
                            recordFilterPayload.FilterGroup = invoiceFilterConditionEntity.FilterGroup;
                        }
                        VRUIUtilsService.callDirectiveLoad(recordFilterAPI, recordFilterPayload, recordFilterLoadPromiseDeferred);
                    });
                    promises.push(recordFilterLoadPromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    var filterGroup = recordFilterAPI.getData();

                    return {
                        $type: "Vanrise.Invoice.MainExtensions.FilterGroupCondition ,Vanrise.Invoice.MainExtensions",
                        FilterGroup: filterGroup != undefined ? filterGroup.filterObj : undefined,
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);