'use strict';

app.directive('vrWhsBePurchasePricelistsettingsEditor', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_PurchaseSendEmailEnum', 'WhS_BE_PurchaseSettingsContextEnum',
    function (UtilsService, VRUIUtilsService, WhS_BE_PurchaseSendEmailEnum, WhS_BE_PurchaseSettingsContextEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new pricelistSettingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/PurchasePricelistSettingsTemplate.html"
        };

        function pricelistSettingEditorCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            var mailMsgTemplateSelectorAPI;
            var mailMsgTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                ctrl.sendEmailOption = UtilsService.getArrayEnum(WhS_BE_PurchaseSendEmailEnum);

                ctrl.onPriceListEmailTemplateSelectorReady = function (api) {
                    mailMsgTemplateSelectorAPI = api;
                    mailMsgTemplateSelectorReadyDeferred.resolve();
                };
                ctrl.onselectionchanged = function () {
                    if (ctrl.selecteSendEmailOption != undefined)
                        ctrl.isPricelistTemplateRequired = ctrl.selecteSendEmailOption.value == WhS_BE_PurchaseSendEmailEnum.No;
                }
                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var data;
                    var selectedMailMsgTemplateId;
                    var directiveContext;

                    if (payload != undefined) {
                        data = payload.data;
                        directiveContext = payload.directiveContext;
                    }

                    prepareDirectivesViewForContext(directiveContext);
                    prepareDirectivesRequiredForContext(directiveContext);
                    if (data != undefined) {
                        selectedMailMsgTemplateId = data.DefaultSupplierPLMailTemplateId;
                        ctrl.selecteSendEmailOption = UtilsService.getItemByVal(ctrl.sendEmailOption, data.SendEmail, 'value');
                    }
                    if (ctrl.showMailTemplateSelector) {
                        var loadMailMsgTemplateSelectorPromise = loadMailMsgTemplateSelector(selectedMailMsgTemplateId);
                        promises.push(loadMailMsgTemplateSelectorPromise);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var object = {
                        DefaultSupplierPLMailTemplateId: (mailMsgTemplateSelectorAPI != undefined) ? mailMsgTemplateSelectorAPI.getSelectedIds() : undefined,
                        SendEmail: ctrl.selecteSendEmailOption != undefined ? ctrl.selecteSendEmailOption.value : undefined
                    };
                    return object;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function prepareDirectivesViewForContext(directiveContext) {

                var systemEnumValue = WhS_BE_PurchaseSettingsContextEnum.System.value;
                var companyEnumValue = WhS_BE_PurchaseSettingsContextEnum.Company.value;
                var supplierEnumValue = WhS_BE_PurchaseSettingsContextEnum.Supplier.value;

                ctrl.showSendEmailSelector = (directiveContext == systemEnumValue || directiveContext == companyEnumValue || directiveContext == supplierEnumValue);
                ctrl.showMailTemplateSelector = (directiveContext == systemEnumValue || directiveContext == companyEnumValue);
            }
            function prepareDirectivesRequiredForContext(directiveContext) {
                var systemEnumValue = WhS_BE_PurchaseSettingsContextEnum.System.value;

                ctrl.isPricelistTemplateRequired = systemEnumValue == directiveContext;
                ctrl.isSendEmailRequired = systemEnumValue == directiveContext;
            }
            function loadMailMsgTemplateSelector(selectedId) {
                var mailMsgTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                mailMsgTemplateSelectorReadyDeferred.promise.then(function () {
                    var mailMsgTemplateSelectorPayload = {
                        selectedIds: selectedId,
                        filter: {
                            VRMailMessageTypeId: "243e6a26-d329-4eeb-95e2-6cd4ae86e451"
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(mailMsgTemplateSelectorAPI, mailMsgTemplateSelectorPayload, mailMsgTemplateSelectorLoadDeferred);
                });
                return mailMsgTemplateSelectorLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);