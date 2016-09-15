'use strict';

app.directive('vrAnalyticDaprofcalcAlertruleaction', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var mailMessageTemplateSettings = new MailMessageTemplateSettings(ctrl, $scope, $attrs);
                mailMessageTemplateSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/VRActions/Templates/DAProfCalcAlertRuleActionTemplate.html"
        };

        function MailMessageTemplateSettings(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var mailMessageTemplateSelectorReadyAPI;
            var mailMessageTemplateSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onMailMessageTemplateSelectorReady = function (api) {
                    mailMessageTemplateSelectorReadyAPI = api;
                    mailMessageTemplateSelectorReadyPromiseDeferred.resolve();
                }


                var promises = [mailMessageTemplateSelectorReadyPromiseDeferred.promise];

                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var mailMessageTemplate;

                    if (payload != undefined && payload.vrActionEntity != undefined) {
                        mailMessageTemplate = payload.vrActionEntity.MailMessageTemplateId;
                    }

                    //Loading Mail Message Template Selector
                    var mailMessageTemplateSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var mailMessageTemplatePayload = {
                        selectedIds: mailMessageTemplate
                    }
                    VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSelectorReadyAPI, mailMessageTemplatePayload, mailMessageTemplateSelectorLoadPromiseDeferred);

                    return mailMessageTemplateSelectorLoadPromiseDeferred.promise;
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Analytic.MainExtensions.VRActions.DAProfCalcAlertRuleAction, Vanrise.Analytic.MainExtensions",
                        ActionName: "Send Email",
                        MailMessageTemplateId: mailMessageTemplateSelectorReadyAPI.getSelectedIds()
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

}]);