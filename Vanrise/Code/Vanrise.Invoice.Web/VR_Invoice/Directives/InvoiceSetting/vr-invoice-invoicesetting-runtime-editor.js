'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeEditor', ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeConfigsAPIService',
    function (UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeConfigsAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new EditorCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/Templates/InvoiceSettingEditorTemplate.html';
            }

        };

        function EditorCtor(ctrl, $scope) {
            var selectedValues;
            var invoiceSettingsPartsConfigs;
            var invoiceTypeId;
            function initializeController() {
                ctrl.sections = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                  
                    if (payload != undefined && payload.sections != undefined) {
                        invoiceTypeId = payload.invoiceTypeId;
                        selectedValues = payload.selectedValues;
                        ctrl.sections.length = 0;
                        var promises = [];

                        var editorpromise = UtilsService.createPromiseDeferred();
                        var editorPromises = [];
                        for (var i = 0; i < payload.sections.length; i++) {
                            var section = payload.sections[i];
                            section.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                            section.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                            editorPromises.push(section.loadPromiseDeferred.promise);
                        }

                        getInvoiceSettingPartsConfigs().then(function () {
                            for (var i = 0; i < payload.sections.length; i++) {
                                var section = payload.sections[i];
                                prepareExtendedRowsObject(section);
                            }
                            UtilsService.waitMultiplePromises(editorPromises).then(function () {
                                editorpromise.resolve();
                            }).catch(function (error) {
                                editorpromise.reject(error);
                            });
                        });
                        promises.push(editorpromise.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var sections = {};
                    for (var i = 0; i < ctrl.sections.length; i++) {
                        var section = ctrl.sections[i];
                        sections = UtilsService.mergeObject(sections, section.rowsAPI.getData(), false);
                    }
                    return sections;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareExtendedRowsObject(section) {

                section.onSectionDirectiveReady = function (api) {
                    section.rowsAPI = api;
                    section.readyPromiseDeferred.resolve();
                };
                var payload = { context: getContext(), rows: section.Rows, invoiceTypeId: invoiceTypeId };
                if (section.readyPromiseDeferred != undefined) {
                    section.readyPromiseDeferred.promise.then(function () {
                        section.readyPromiseDeferred = undefined;

                        VRUIUtilsService.callDirectiveLoad(section.rowsAPI, payload, section.loadPromiseDeferred);
                    });
                }
                ctrl.sections.push(section);
            }

            function getContext() {
                var context = {
                    getRuntimeEditor: getRuntimeEditor,
                    getPartsPathValue: getPartsPathValue
                };
                return context;
            }

            function getPartsPathValue(fieldPath)
            {
                if (selectedValues != undefined && fieldPath != undefined)
                return selectedValues[fieldPath];
            }

            function getRuntimeEditor(configId)
            {
                if (invoiceSettingsPartsConfigs != undefined)
                {
                    var invoiceSettingsPartsConfig = UtilsService.getItemByVal(invoiceSettingsPartsConfigs, configId, 'ExtensionConfigurationId');
                    if (invoiceSettingsPartsConfig != undefined)
                        return invoiceSettingsPartsConfig.RuntimeEditor;
                }
            }
            function getInvoiceSettingPartsConfigs() {
              return  VR_Invoice_InvoiceTypeConfigsAPIService.GetInvoiceSettingPartsConfigs().then(function (response) {
                    invoiceSettingsPartsConfigs = response;
                });
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);