'use strict';

app.directive('retailBeAccountbulkactionsettingsSendemail', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SendEmailBulkActionSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountBulkActions/MainExtensions/Templates/SendEmailBulkActionSettingsTemplate.html'
        };

        function SendEmailBulkActionSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var mailMessageTypeSelectorAPI;
            var mailMessageTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                    mailMessageTypeSelectorAPI = api;
                    mailMessageTypeReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([mailMessageTypeReadyDeferred.promise]).then(function () {
                    defineAPI();
                });


            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var bulkActionSettings;
                    if (payload != undefined) {
                        bulkActionSettings = payload.bulkActionSettings;
                    }

                    function loadMailMessageTypeSelector() {
                        var mailMessageTypeSelectorPayload = {
                            selectedIds: bulkActionSettings != undefined ? bulkActionSettings.MailMessageTypeId : undefined,
                        };
                        return mailMessageTypeSelectorAPI.load(mailMessageTypeSelectorPayload);
                    }

                    var promises = [];

                    promises.push(loadMailMessageTypeSelector());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountBulkAction.SendEmailAccountBulkActionDefinition, Retail.BusinessEntity.MainExtensions',
                        MailMessageTypeId: mailMessageTypeSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);