'use strict';

app.directive('vrCommonSmsmessagetemplateGrid', ['VRCommon_SMSMessageTemplateAPIService', 'VRCommon_SMSMessageTemplateService', 'VRNotificationService', 'VRUIUtilsService',
    function (VRCommon_SMSMessageTemplateAPIService, VRCommon_SMSMessageTemplateService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var smsMessageTemplateGrid = new SMSMessageTemplateGrid($scope, ctrl, $attrs);
                smsMessageTemplateGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/SMS/Templates/SMSMessageTemplateGridTemplate.html'
        };

        function SMSMessageTemplateGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.smsMessageTemplates = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VRCommon_SMSMessageTemplateService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_SMSMessageTemplateAPIService.GetFilteredSMSMessageTemplates(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (query) {

                    return gridAPI.retrieveData(query);
                };

                api.onSMSMessageTemplateAdded = function (addedSMSMessageTemplate) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedSMSMessageTemplate);
                    gridAPI.itemAdded(addedSMSMessageTemplate);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editSMSMessageTemplate,
                    haspermission: hasEditSMSMessageTemplatePermission
                });
            }
            function editSMSMessageTemplate(smsMessageTemplateItem) {
                var onSMSMessageTemplateUpdated = function (updatedSMSMessageTemplate) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedSMSMessageTemplate);
                    gridAPI.itemUpdated(updatedSMSMessageTemplate);
                };
                

                VRCommon_SMSMessageTemplateService.editSMSMessageTemplate(smsMessageTemplateItem.Entity.SMSMessageTemplateId, onSMSMessageTemplateUpdated);
            }
            function hasEditSMSMessageTemplatePermission() {
                return VRCommon_SMSMessageTemplateAPIService.HasEditSMSMessageTemplatePermission();
            }

        }
    }]);
