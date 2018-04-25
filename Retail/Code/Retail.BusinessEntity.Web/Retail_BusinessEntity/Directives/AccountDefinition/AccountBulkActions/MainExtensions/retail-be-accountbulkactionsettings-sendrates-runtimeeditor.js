'use strict';

app.directive('retailBeAccountbulkactionsettingsSendratesRuntimeeditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SendRatesRuntimeEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountBulkActions/MainExtensions/Templates/SendRatesBulkActionSettingsRuntimeEditorTemplate.html'
        };

        function SendRatesRuntimeEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var mailMessageTemplateSelectorAPI;
            var mailMessageTemplateReadyDeferred = UtilsService.createPromiseDeferred();

            
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onMailMessageTemplateSelectorReady = function (api) {
                    mailMessageTemplateSelectorAPI = api;
                    mailMessageTemplateReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([mailMessageTemplateReadyDeferred.promise]).then(function () {
                    defineAPI();
                });


            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var mailMessageTypeId;
                    if (payload != undefined) {
                        mailMessageTypeId = payload.mailMessageTypeId;
                    }

                    function loadMailMessageTemplateSelector() {
                        var mailMessageTemplateSelectorPayload = {
                            filter: { VRMailMessageTypeId: mailMessageTypeId }
                        };
                        return mailMessageTemplateSelectorAPI.load(mailMessageTemplateSelectorPayload);
                    }

                    var promises = [];

                    promises.push(loadMailMessageTemplateSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return mailMessageTemplateSelectorAPI.getSelectedIds();
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);