'use strict';

app.directive('vrGenericdataDatarecordVractiondefinitionExtendedsettingsSendemail', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new sendEmailActionDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecord/VRActionDefinition/Templates/VRActionDefinitionSendEmail.html'
        };

        function sendEmailActionDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeAPI;
            var dataRecordTypeReadyDeferred = UtilsService.createPromiseDeferred();

            var mailMessageTypeAPI;
            var mailMessageTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeAPI = api;
                    dataRecordTypeReadyDeferred.resolve();
                };

                $scope.scopeModel.onMailMessageTypeSelectorReady = function (api) {
                    mailMessageTypeAPI = api;
                    mailMessageTypeReadyDeferred.resolve();
                };

                defineAPI();
            };

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    if (payload != undefined && payload.Settings != undefined && payload.Settings.ExtendedSettings != undefined) {
                        extendedSettings = payload.Settings.ExtendedSettings;
                    }
                    var promises = [];

                    var dataRecordTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    dataRecordTypeReadyDeferred.promise.then(function () {
                        var dataRecordTypePayload;
                        if (extendedSettings != undefined) {
                            dataRecordTypePayload = { selectedIds: extendedSettings.DataRecordTypeId };
                        }
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeAPI, dataRecordTypePayload, dataRecordTypeLoadDeferred);
                    });
                    promises.push(dataRecordTypeLoadDeferred.promise);

                    var mailMessageTypeLoadDeferred = UtilsService.createPromiseDeferred();
                    mailMessageTypeReadyDeferred.promise.then(function () {
                        var mailMessageTypePayload;
                        if (extendedSettings != undefined) {
                            mailMessageTypePayload = { selectedIds: extendedSettings.MailMessageTypeId };
                        }
                        VRUIUtilsService.callDirectiveLoad(mailMessageTypeAPI, mailMessageTypePayload, mailMessageTypeLoadDeferred);
                    });
                    promises.push(mailMessageTypeLoadDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Vanrise.GenericData.Notification.DataRecordSendEmailDefinitionSettings, Vanrise.GenericData.Notification',
                        DataRecordTypeId: dataRecordTypeAPI.getSelectedIds(),
                        MailMessageTypeId: mailMessageTypeAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);