'use strict';
app.directive('vrInvoiceInvoicesettingRuntimeFilenamepatterninvoicesettingpart', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new RowCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_Invoice/Directives/InvoiceSetting/MainExtensions/Templates/FileNamePatternInvoiceSettingPartTemplate.html';
            }

        };

        function RowCtor(ctrl, $scope) {
            var currentContext;
            var fileNamePatternAPI;
            var fileNamePatternReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onFileNamePatternReady = function (api) {
                    fileNamePatternAPI = api;
                    fileNamePatternReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined && payload.fieldValue != undefined)
                    {
                        $scope.scopeModel.enableAutomaticInvoice = payload.fieldValue.IsEnabled;
                    }

                    function loadFileNamePattern() {
                        var fileNamePatternDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        fileNamePatternReadyPromiseDeferred.promise.then(function () {
                            var fileNamePatternDirectivePayload = { invoiceTypeId:payload.invoiceTypeId  };
                            if (payload != undefined && payload.fieldValue!=undefined)
                                fileNamePatternDirectivePayload.fileNamePattern = payload.fieldValue.FileNamePattern;
                            VRUIUtilsService.callDirectiveLoad(fileNamePatternAPI, fileNamePatternDirectivePayload, fileNamePatternDeferredLoadPromiseDeferred);
                        });
                        return fileNamePatternDeferredLoadPromiseDeferred.promise;
                    }
                    return loadFileNamePattern();
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.Entities.FileNamePatternInvoiceSettingPart,Vanrise.Invoice.Entities",
                        FileNamePattern: fileNamePatternAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);