'use strict';

app.directive('partnerportalCustomeraccessRetailsubaccountsgrid', ['VRNotificationService', 'UtilsService', 'PartnerPortal_CustomerAccess_RetailAccountInfoAPIService','VRUIUtilsService',
    function (VRNotificationService, UtilsService, PartnerPortal_CustomerAccess_RetailAccountInfoAPIService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountGrid = new AccountGrid($scope, ctrl, $attrs);
                accountGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/PartnerPortal_CustomerAccess/Elements/RetailAccountInfo/Directives/Templates/SubAccountsGridTemplate.html'
        };

        function AccountGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var vrConnectionId;
            var accountBEDefinitionId;
            var parentAccountId;
            var gridColumnFieldNames = [];
            var gridAPI;
            var gridDrillDownTabsObj;
            var accountGridFields;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showExpend = function (dataItem) {
                    return dataItem.HasSubAccounts;
                };
                $scope.scopeModel.columns = [];
                $scope.scopeModel.accounts = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(getDrillDownDefinition(), gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return PartnerPortal_CustomerAccess_RetailAccountInfoAPIService.GetFilteredSubAccounts(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];
                                gridDrillDownTabsObj.setDrillDownExtensionObject(item);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
             
            }
            function getDrillDownDefinition()
            {
                var drillDownDefinitions = [];

                var drillDownDefinition = {};

                drillDownDefinition.title = "Sub Accounts";
                drillDownDefinition.directive = "partnerportal-customeraccess-retailsubaccountsgrid";

                drillDownDefinition.loadDirective = function (directiveAPI, currentItem) {
                    currentItem.subAccountsGridAPI = directiveAPI;
                    var query = {
                        vrConnectionId: vrConnectionId,
                        parentAccountId: currentItem.AccountId,
                        accountGridFields: accountGridFields,
                    };
                    return currentItem.subAccountsGridAPI.load(query);
                };
                drillDownDefinition.hideDrillDownFunction = function (dataItem) {
                    return !dataItem.HasSubAccounts;
                };
                drillDownDefinitions.push(drillDownDefinition);
                return drillDownDefinitions;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var gridQuery;

                    if (payload != undefined) {
                        parentAccountId = payload.parentAccountId;
                        gridQuery = payload.query;
                        accountGridFields = payload.accountGridFields;
                        vrConnectionId = payload.vrConnectionId;
                    }

                    if ($scope.scopeModel.columns.length == 0) {
                        //Loading AccountGridColumns
                        var accountGridColumnsLoadPromise = getAccountGridColumnsLoadPromise();
                        promises.push(accountGridColumnsLoadPromise);
                    }

                    var gridLoadDeferred = UtilsService.createPromiseDeferred();
                    //Retrieving Data
                    UtilsService.waitMultiplePromises(promises).then(function () {

                        gridQuery = buildGridQuery(gridQuery);
                        gridAPI.retrieveData(gridQuery).then(function () {
                            gridLoadDeferred.resolve();
                        }).catch(function (error) {
                            gridLoadDeferred.reject(error);
                        });

                    }).catch(function (error) {
                        gridLoadDeferred.reject(error);
                    });

                    function getAccountGridColumnsLoadPromise() {
                        var accountGridColumnAttributesLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        PartnerPortal_CustomerAccess_RetailAccountInfoAPIService.GetSubAccountsGridColumnAttributes({
                            ParentAccountId: parentAccountId,
                            AccountGridFields: accountGridFields,
                            VRConnectionId: vrConnectionId
                        }).then(function (response) {
                            var accountGridColumnAttributes = response;
                            if (accountGridColumnAttributes != undefined) {
                                for (var index = 0; index < accountGridColumnAttributes.length; index++) {
                                    var accountGridColumnAttribute = accountGridColumnAttributes[index];
                                    //var column = {
                                    //    HeaderText: accountGridColumnAttribute.Attribute.HeaderText,
                                    //    Field: accountGridColumnAttribute.Attribute.Field,
                                    //    Type: accountGridColumnAttribute.Attribute.Type
                                    //};
                                    gridColumnFieldNames.push(accountGridColumnAttribute.Name);
                                    $scope.scopeModel.columns.push(accountGridColumnAttribute.Attribute);
                                }
                            }
                            accountGridColumnAttributesLoadPromiseDeferred.resolve();

                        }).catch(function (error) {
                            accountGridColumnAttributesLoadPromiseDeferred.reject(error);
                        });

                        return accountGridColumnAttributesLoadPromiseDeferred.promise;
                    }
                    function buildGridQuery(gridQuery) {
                        return {
                            VRConnectionId:vrConnectionId,
                            ParentAccountId: parentAccountId,
                            Columns: gridColumnFieldNames,
                        };
                    }

                    return gridLoadDeferred.promise;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
