﻿'use strict';

app.directive('whsAccountbalanceFinancialaccountGrid', ['WhS_AccountBalance_FinancialAccountAPIService', 'VRNotificationService','VR_AccountBalance_FinancialAccountService',
    function (WhS_AccountBalance_FinancialAccountAPIService, VRNotificationService, VR_AccountBalance_FinancialAccountService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.showAccount = true;
                var financialAccountGrid = new FinancialAccountGrid($scope, ctrl, $attrs);
                financialAccountGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_AccountBalance/Elements/FinancialAccount/Directives/Templates/FinancialAccountGridTemplate.html'
        };

        function FinancialAccountGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.financialAccounts = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return WhS_AccountBalance_FinancialAccountAPIService.GetFilteredFinancialAccounts(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }   

            function defineAPI() {
                var api = {};

                api.loadGrid = function (payload) {
                    if (payload != undefined)
                    {
                        context = payload.context;
                        return gridAPI.retrieveData(payload.query);
                    }
                };
                api.onFinancialAccountAdded = function (financialAccount) {
                   
                    return gridAPI.itemAdded(financialAccount);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editFinancialAccount,
                }];
            }

            function editFinancialAccount(dataItem) {
                var onFinancialAccountUpdated = function (financialAccount) {
                    if (context != undefined && context.checkAllowAddFinancialAccount != undefined)
                        context.checkAllowAddFinancialAccount();
                    gridAPI.itemUpdated(financialAccount);
                };
                VR_AccountBalance_FinancialAccountService.editFinancialAccount(onFinancialAccountUpdated, dataItem.Entity.FinancialAccountId);
            }

        }
    }]);
