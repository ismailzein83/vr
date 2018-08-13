'use strict';

app.directive('retailBeFaultticketSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Settings/Templates/FaultTicketSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            var faultTicketSetting;
           
            var faultTicketSerialNumberEditorAPI;
            var faultTicketSerialNumberEditorReadyDeferred = UtilsService.createPromiseDeferred();
                        
            var faultTicketOpenMailTemplateSelectorAPI;
            var faultTicketOpenMailTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var faultTicketPendingMailTemplateSelectorAPI;
            var faultTicketPendingMailTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var faultTicketClosedMailTemplateSelectorAPI;
            var faultTicketClosedMailTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();        



            $scope.scopeModel = {};
            $scope.scopeModel.faultTicketInitialSequence = 0;
            $scope.scopeModel.onFaultTicketSerialNumberEditorReady = function (api) {
                faultTicketSerialNumberEditorAPI = api;
                faultTicketSerialNumberEditorReadyDeferred.resolve();
            };
            $scope.scopeModel.onFaultTicketOpenMailTemplateSelectorReady = function (api) {
                faultTicketOpenMailTemplateSelectorAPI = api;
                faultTicketOpenMailTemplateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onFaultTicketPendingMailTemplateSelectorReady = function (api) {
                faultTicketPendingMailTemplateSelectorAPI = api;
                faultTicketPendingMailTemplateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onFaultTicketClosedMailTemplateSelectorReady = function (api) {
                faultTicketClosedMailTemplateSelectorAPI = api;
                faultTicketClosedMailTemplateSelectorReadyDeferred.resolve();
            };
            

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var data;

                    if (payload != undefined) {
                        data = payload.data;
                    }
                    if (data != undefined) {
                        faultTicketSetting = data.FaultTicketSetting;
                        if (faultTicketSetting != undefined)
                            $scope.scopeModel.faultTicketInitialSequence = faultTicketSetting.InitialSequence;
                    }

                    var loadFaultTicketSerialNumberEditorPromise = loadFaultTicketSerialNumberEditor(faultTicketSetting);
                    
                    var loadFaultTicketOpenMailTemplateSelectorPromise = loadFaultTicketOpenMailTemplateSelector();
                    var loadFaultTicketPendingMailTemplateSelectorPromise = loadFaultTicketPendingMailTemplateSelector();
                    var loadFaultTicketClosedMailTemplateSelectorPromise = loadFaultTicketClosedMailTemplateSelector();
                    

                    promises.push(loadFaultTicketSerialNumberEditorPromise);
                    promises.push(loadFaultTicketOpenMailTemplateSelectorPromise);
                    promises.push(loadFaultTicketPendingMailTemplateSelectorPromise);
                    promises.push(loadFaultTicketClosedMailTemplateSelectorPromise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.FaultTicketsSettingsData, Retail.BusinessEntity.Business",
                        FaultTicketSetting: {
                            SerialNumberPattern: faultTicketSerialNumberEditorAPI.getData().serialNumberPattern,
                            InitialSequence: $scope.scopeModel.faultTicketInitialSequence,
                            OpenMailTemplateId: faultTicketOpenMailTemplateSelectorAPI.getSelectedIds(),
                            PendingMailTemplateId: faultTicketPendingMailTemplateSelectorAPI.getSelectedIds(),
                            ClosedMailTemplateId: faultTicketClosedMailTemplateSelectorAPI.getSelectedIds()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function loadFaultTicketSerialNumberEditor(faultTicketSetting) {
                var serialNumberEditorEditorLoadDeferred = UtilsService.createPromiseDeferred();
                faultTicketSerialNumberEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: faultTicketSetting,
                        businessEntityDefinitionId: "0d7dd0d6-ab3c-4e58-bd5f-926a260f1891"
                    };
                    VRUIUtilsService.callDirectiveLoad(faultTicketSerialNumberEditorAPI, payload, serialNumberEditorEditorLoadDeferred);
                });
                return serialNumberEditorEditorLoadDeferred.promise;
            }
            function loadFaultTicketOpenMailTemplateSelector() {

                var faultTicketOpenMailTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                faultTicketOpenMailTemplateSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            VRMailMessageTypeId: "805743a7-fc4c-46dc-b99e-b4fa4ae051b1",
                        },
                        selectedIds: faultTicketSetting != undefined ? faultTicketSetting.OpenMailTemplateId : undefined

                    };
                    VRUIUtilsService.callDirectiveLoad(faultTicketOpenMailTemplateSelectorAPI, payload, faultTicketOpenMailTemplateSelectorLoadDeferred);
                });
                return faultTicketOpenMailTemplateSelectorLoadDeferred.promise;
            }
            function loadFaultTicketPendingMailTemplateSelector() {

                var faultTicketPendingMailTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                faultTicketPendingMailTemplateSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            VRMailMessageTypeId: "805743a7-fc4c-46dc-b99e-b4fa4ae051b1"
                        },
                        selectedIds: faultTicketSetting != undefined ? faultTicketSetting.PendingMailTemplateId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(faultTicketPendingMailTemplateSelectorAPI, payload, faultTicketPendingMailTemplateSelectorLoadDeferred);
                });
                return faultTicketPendingMailTemplateSelectorLoadDeferred.promise;
            }
            function loadFaultTicketClosedMailTemplateSelector() {

                var faultTicketClosedMailTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                faultTicketClosedMailTemplateSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            VRMailMessageTypeId: "805743a7-fc4c-46dc-b99e-b4fa4ae051b1"
                        },
                        selectedIds: faultTicketSetting != undefined ? faultTicketSetting.ClosedMailTemplateId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(faultTicketClosedMailTemplateSelectorAPI, payload, faultTicketClosedMailTemplateSelectorLoadDeferred);
                });
                return faultTicketClosedMailTemplateSelectorLoadDeferred.promise;
            }    


        }

        return directiveDefinitionObject;
    }]);