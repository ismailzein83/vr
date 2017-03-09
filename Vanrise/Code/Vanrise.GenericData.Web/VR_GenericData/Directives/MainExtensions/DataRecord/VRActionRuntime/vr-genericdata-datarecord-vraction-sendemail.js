'use strict';

app.directive('vrGenericdataDatarecordVractionSendemail', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new sendEmailActionDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecord/VRActionRuntime/Templates/SendEmail.html'
        };

        function sendEmailActionDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var mailMessageTemplateSelectorReadyAPI;
            var mailMessageTemplateSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onMailMessageTemplateSelectorReady = function (api) {
                    mailMessageTemplateSelectorReadyAPI = api;
                    mailMessageTemplateSelectorReadyPromiseDeferred.resolve();
                };

                var promises = [mailMessageTemplateSelectorReadyPromiseDeferred.promise];

                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};
                var actionDefinitionId;
                api.load = function (payload) {
                    var promises = [];
                    var mailMessageTemplate;

                    if (payload != undefined && payload.vrActionEntity != undefined) {
                        mailMessageTemplate = payload.vrActionEntity.MailMessageTemplateId;
                    }

                    //Loading Mail Message Template Selector
                    var mailMessageTemplateSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    var mailMessageTemplatePayload = {
                        selectedIds: mailMessageTemplate,
                        filter: { VRMailMessageTypeId: payload.selectedVRActionDefinition.Settings.ExtendedSettings.MailMessageTypeId }
                    };
                    VRUIUtilsService.callDirectiveLoad(mailMessageTemplateSelectorReadyAPI, mailMessageTemplatePayload, mailMessageTemplateSelectorLoadPromiseDeferred);
                    promises.push(mailMessageTemplateSelectorLoadPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises); 
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.VRActions.DataRecordAlertRuleSendEmailAction,Vanrise.GenericData.MainExtensions",
                        ActionName: "Send Email",
                        MailMessageTemplateId: mailMessageTemplateSelectorReadyAPI.getSelectedIds(),
                        Settings: {
                            ExtendedSettings: {
                                $type: "Vanrise.GenericData.Notification.DataRecordSendEmailExtendedSettings, Vanrise.GenericData.Notification",
                            }
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);