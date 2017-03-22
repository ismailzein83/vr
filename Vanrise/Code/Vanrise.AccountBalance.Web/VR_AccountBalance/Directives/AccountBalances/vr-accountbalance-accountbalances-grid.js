﻿'use strict';

app.directive('vrAccountbalanceAccountbalancesGrid', ['VR_AccountBalance_LiveBalanceAPIService', 'VRNotificationService','VR_AccountBalance_AccountTypeAPIService','UtilsService',
    function (VR_AccountBalance_LiveBalanceAPIService, VRNotificationService,VR_AccountBalance_AccountTypeAPIService,UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.showAccount = true;
                var grid = new AccountBalancesTransactionGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/AccountBalances/Templates/AccountBalancesGridTemplate.html'
        };

        function AccountBalancesTransactionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var accountTypeId;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.accountBalances = [];
            //    $scope.scopeModel.sortField = "";
                $scope.scopeModel.menuActions = [];
                $scope.gridFields = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_AccountBalance_LiveBalanceAPIService.GetFilteredAccountBalances(dataRetrievalInput).then(function (response) {
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
                    ctrl.orderBy = payload.query.OrderBy;
                    accountTypeId = payload.accountTypeId;
                    var promiseDeferred = UtilsService.createPromiseDeferred();
                    VR_AccountBalance_AccountTypeAPIService.ConvertToGridColumnAttribute(accountTypeId).then(function (response) {
                        buildGridFields(response);
                        return gridAPI.retrieveData(payload.query).then(function () {
                            promiseDeferred.resolve();
                        }).catch(function (error) {
                            promiseDeferred.reject(error);
                        });
                    }).catch(function (error) {
                        promiseDeferred.reject(error);
                    });

                    return promiseDeferred.promise;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function buildGridFields(gridAttributes) {

                $scope.gridFields.length = 0;
                if (gridAttributes != undefined) {

                    for (var i = 0; i < gridAttributes.length ; i++) {
                        var gridAttribute = gridAttributes[i];
                        if (gridAttribute != undefined) {
                            if ($scope.scopeModel.sortField === "" || $scope.scopeModel.sortField === undefined)
                                $scope.scopeModel.sortField = 'Items.'+gridAttribute.Field;
                            gridAttribute.Field = "Entity.Items." + gridAttribute.Field + ".Description";
                            $scope.gridFields.push(gridAttribute);
                           
                        }
                    }
                }
            }

            function defineMenuActions() {
            }

        }
    }]);
