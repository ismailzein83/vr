﻿'use strict';

app.directive('vrCommonMailmessagetypeGrid', ['VRCommon_VRMailMessageTypeAPIService', 'VRCommon_VRMailMessageTypeService', 'VRNotificationService',
    function (VRCommon_VRMailMessageTypeAPIService, VRCommon_VRMailMessageTypeService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var mailMessageTypeGrid = new MailMessageTypeGrid($scope, ctrl, $attrs);
                mailMessageTypeGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Common/Directives/VRMail/Templates/VRMailMessageTypeGridTemplate.html'
        };

        function MailMessageTypeGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.mailMessageTypes = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VRCommon_VRMailMessageTypeAPIService.GetFilteredMailMessageTypes(dataRetrievalInput).then(function (response) {
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

                api.onMailMessageTypeAdded = function (addedMailMessageType) {
                    gridAPI.itemAdded(addedMailMessageType);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editMailMessageType,
                    haspermission: hasEditMailMessageTypePermission
                });
            }
            function editMailMessageType(mailMessageTypeItem) {
                var onMailMessageTypeUpdated = function (updatedMailMessageType) {
                    gridAPI.itemUpdated(updatedMailMessageType);
                };

                VRCommon_VRMailMessageTypeService.editMailMessageType(mailMessageTypeItem.Entity.VRMailMessageTypeId, onMailMessageTypeUpdated);
            }
            function hasEditMailMessageTypePermission() {
                return VRCommon_VRMailMessageTypeAPIService.HasEditMailMessageTypePermission();
            }
        }
    }]);
