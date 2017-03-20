'use strict';

app.directive('vrCommonMailmessagetemplateGrid', ['VRCommon_VRMailMessageTemplateAPIService', 'VRCommon_VRMailMessageTemplateService', 'VRNotificationService', 'VRUIUtilsService',
    function (VRCommon_VRMailMessageTemplateAPIService, VRCommon_VRMailMessageTemplateService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var mailMessageTemplateGrid = new MailMessageTemplateGrid($scope, ctrl, $attrs);
                mailMessageTemplateGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRMail/Templates/VRMailMessageTemplateGridTemplate.html'
        };

        function MailMessageTemplateGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.mailMessageTemplates = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VRCommon_VRMailMessageTemplateService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRMailMessageTemplateAPIService.GetFilteredMailMessageTemplates(dataRetrievalInput).then(function (response) {
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

                api.onMailMessageTemplateAdded = function (addedMailMessageTemplate) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedMailMessageTemplate);
                    gridAPI.itemAdded(addedMailMessageTemplate);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editMailMessageTemplate,
                    haspermission: hasEditMailMessageTemplatePermission
                });
            }
            function editMailMessageTemplate(mailMessageTemplateItem) {
                var onMailMessageTemplateUpdated = function (updatedMailMessageTemplate) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedMailMessageTemplate);
                    gridAPI.itemUpdated(updatedMailMessageTemplate);
                };

                VRCommon_VRMailMessageTemplateService.editMailMessageTemplate(mailMessageTemplateItem.Entity.VRMailMessageTemplateId, onMailMessageTemplateUpdated);
            }
            function hasEditMailMessageTemplatePermission() {
                return VRCommon_VRMailMessageTemplateAPIService.HasEditMailMessageTemplatePermission();
            }

        }
    }]);
