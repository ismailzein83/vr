"use strict";

app.directive("vrInvoicetypeItemsetnamestoragerulesettingsDefault", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new DefaultItemSetNameStorageRule($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/ItemSetNameStorageRule/MainExtensions/Templates/DefaultItemSetNameStorageRuleTemplate.html"

        };

        function DefaultItemSetNameStorageRule($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var textFilterAPI;
            var textFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onTextFilterDirectiveReady = function (api) {
                    textFilterAPI = api;
                    textFilterReadyPromiseDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([textFilterReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var itemSetNameStorageRuleSettings;
                    if (payload != undefined) {
                        itemSetNameStorageRuleSettings = payload.itemSetNameStorageRuleSettings;
                        if (itemSetNameStorageRuleSettings != undefined)
                            $scope.scopeModel.storageConnectionStringKey = itemSetNameStorageRuleSettings.StorageConnectionStringKey;
                        context = payload.context;
                    }
                    var promises = [];

                    function loadTextFilterDirective()
                    {
                        var textFilterPayload = { context: getContext() };
                        if (itemSetNameStorageRuleSettings != undefined) {
                            textFilterPayload.text = itemSetNameStorageRuleSettings.ItemSetName;
                            textFilterPayload.textFilterTypeValue = itemSetNameStorageRuleSettings.Condition;
                        }
                      return  textFilterAPI.load(textFilterPayload);
                    }
                    promises.push(loadTextFilterDirective());
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var textFilter = textFilterAPI.getData();

                    return {
                        $type: "Vanrise.Invoice.MainExtensions.ItemSetNameStorageRule.DefaultItemSetNameStorageRule ,Vanrise.Invoice.MainExtensions",
                        ItemSetName: textFilter != undefined ? textFilter.Text : undefined,
                        StorageConnectionStringKey: $scope.scopeModel.storageConnectionStringKey,
                        Condition:textFilter != undefined ? textFilter.TextFilterType : undefined,
                    };
                };

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