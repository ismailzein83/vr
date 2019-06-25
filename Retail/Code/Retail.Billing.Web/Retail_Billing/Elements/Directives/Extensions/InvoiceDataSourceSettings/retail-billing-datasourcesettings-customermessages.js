"use strict";

app.directive("retailBillingDatasourcesettingsCustomermessages", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CustomerMessagesDataSourceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/Extensions/InvoiceDataSourceSettings/Templates/CustomerMessagesTemplate.html"

        };

        function CustomerMessagesDataSourceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectedIds;

            var beDefinitionSelectorApi;
            var beDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var onBusinessEntityDefinitionSelectionChangeDeffered;



            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorApi = api;
                    beDefinitionSelectorPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined && payload.dataSourceEntity != undefined) {
                        $scope.scopeModel.messageFieldName = payload.dataSourceEntity.MessageFieldName;
                        $scope.scopeModel.categoryFieldName = payload.dataSourceEntity.CategoryFieldName;
                        selectedIds = payload.dataSourceEntity.CustomerMessagesBeDefinitionId;
                    };

                    promises.push(loadBusinessEntityDefinitionSelector());
                    return UtilsService.waitMultiplePromises(promises);

                    function loadBusinessEntityDefinitionSelector() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        beDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload;
                            if (selectedIds != undefined)
                                selectorPayload = {
                                    selectedIds: selectedIds
                                }
                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });
                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.Billing.Business.CustomerMessagesDataSourceSettings, Retail.Billing.Business",
                        CustomerMessagesBeDefinitionId: beDefinitionSelectorApi.getSelectedIds(),
                        MessageFieldName: $scope.scopeModel.messageFieldName,
                        CategoryFieldName: $scope.scopeModel.categoryFieldName
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);