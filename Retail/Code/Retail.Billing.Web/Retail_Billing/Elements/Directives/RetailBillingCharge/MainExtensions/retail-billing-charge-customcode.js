"use strict";

app.directive("retailBillingChargeCustomcode", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new BillingChargeTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingCharge/MainExtensions/Templates/CustomCodeChargeTemplate.html"
        };

        function BillingChargeTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var editorRuntimeDirectiveAPI;
            var editorRuntimeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.hasRuntimeEditor;

                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                    editorRuntimeDirectiveAPI = api;
                    editorRuntimeDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    var fieldValues = {};
                    var promises = [];

                    if (payload != undefined) {
                        extendedSettings = payload.extendedSettings;
                        if (payload.charge != undefined)
                            fieldValues = payload.charge.FieldValues;

                        if (extendedSettings != undefined)
                            $scope.scopeModel.hasRuntimeEditor = extendedSettings.ChargeSettingsRecordTypeId != undefined;
                    }
                    if ($scope.scopeModel.hasRuntimeEditor)
                        promises.push(loadRuntimeEditor());

                    function loadRuntimeEditor() {
                        var editorRuntimeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        editorRuntimeDirectiveReadyDeferred.promise.then(function () {

                            var runtimeEditorPayload = {
                                selectedValues: fieldValues,
                                dataRecordTypeId: extendedSettings != undefined ? extendedSettings.ChargeSettingsRecordTypeId : undefined,
                                definitionSettings: extendedSettings != undefined ? extendedSettings.ChargeSettingsEditorDefinition : undefined,
                                runtimeEditor: extendedSettings != undefined && extendedSettings.ChargeSettingsEditorDefinition != undefined ? extendedSettings.ChargeSettingsEditorDefinition.RuntimeEditor : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(editorRuntimeDirectiveAPI, runtimeEditorPayload, editorRuntimeDirectiveLoadPromiseDeferred);
                        });
                        return editorRuntimeDirectiveLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.Billing.MainExtensions.RetailBillingCharge.RetailBillingCustomCodeCharge,Retail.Billing.MainExtensions",
                        FieldValues: {}
                    };

                    if (editorRuntimeDirectiveAPI != undefined)
                        editorRuntimeDirectiveAPI.setData(obj.FieldValues);

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);