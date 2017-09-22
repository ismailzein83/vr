"use strict";
app.directive("partnerportalCustomeraccessRetailsubaccountsinfotiledefinitionsettings", ["UtilsService", "VRUIUtilsService","VRCommon_GridWidthFactorEnum",
    function (UtilsService, VRUIUtilsService, VRCommon_GridWidthFactorEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailSubAccountsInfoTileDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/PartnerPortal_CustomerAccess/Elements/RetailAccountInfo/Directives/Templates/RetailSubAccountsInfoTileDefinitionSettings.html"
        };
        function RetailSubAccountsInfoTileDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionSelectorApi;
            var connectionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var selectedConnectionPromiseDeferred;

            var genericFieldDefinitionSelectorApi;
            var genericFieldDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.genericFieldDefinitions = [];
                $scope.scopeModel.selectedGenericFields = [];
                $scope.scopeModel.onConnectionSelectorReady = function (api) {
                    connectionSelectorApi = api;
                    connectionSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onConnectionSelectionChanged = function (value) {
                    if(value != undefined)
                    {
                        if (selectedConnectionPromiseDeferred != undefined)
                            selectedConnectionPromiseDeferred.resolve();
                        else
                        {
                            $scope.scopeModel.genericFieldDefinitions.length = 0;
                            var selectorPayload = {
                                connectionId: connectionSelectorApi.getSelectedIds(),
                                accountBEDefinitionId:$scope.scopeModel.accountBEDefinitionId
                            };
                            var setLoader = function (value) { $scope.scopeModel.isLoadingGenericFieldDefinitionDirective = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, genericFieldDefinitionSelectorApi, selectorPayload, setLoader);
                        }
                    }
                };

                $scope.scopeModel.onSelectGenericFieldItem = function (genericField) {
                    var dataItem = {
                        Title: genericField.Title,
                        FieldName: genericField.Name,
                    };
                    dataItem.onGridWidthFactorEditorReady = function (api) {
                        dataItem.gridWidthFactorAPI = api;
                        var dataItemPayload = {
                            data: {
                                Width: VRCommon_GridWidthFactorEnum.Normal.value
                            }
                        };
                        var setLoader = function (value) { $scope.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.gridWidthFactorAPI, dataItemPayload, setLoader);
                    };
                    $scope.scopeModel.genericFieldDefinitions.push(dataItem);
                };
                $scope.scopeModel.onDeselectGenericFieldItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.genericFieldDefinitions, dataItem.Name, 'Name');
                    $scope.scopeModel.genericFieldDefinitions.splice(datasourceIndex, 1);
                };
                $scope.scopeModel.removeGenericField = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedGenericFields, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedGenericFields.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.genericFieldDefinitions, dataItem.Name, 'Name');
                    $scope.scopeModel.genericFieldDefinitions.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.onGenericFieldDefinitionSelectorReady = function (api) {
                    genericFieldDefinitionSelectorApi = api;
                    genericFieldDefinitionSelectorPromiseDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([connectionSelectorPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var tileExtendedSettings;
                    var selectedGenericFieldsIds;

                    if (payload != undefined) {
                        tileExtendedSettings = payload.tileExtendedSettings;
                        $scope.scopeModel.accountBEDefinitionId = tileExtendedSettings.AccountBEDefinitionId;

                        if (tileExtendedSettings != undefined) {
                            selectedGenericFieldsIds = [];
                            if (tileExtendedSettings.AccountGridFields != undefined) {
                                for (var i = 0; i < tileExtendedSettings.AccountGridFields.length; i++) {
                                    var accountGridField = tileExtendedSettings.AccountGridFields[i];
                                    selectedGenericFieldsIds.push(accountGridField.FieldName);
                                    var genericFieldGrid = {
                                        payload: accountGridField,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    promises.push(genericFieldGrid.loadPromiseDeferred.promise);
                                    addGenericFieldGridWidthAPI(genericFieldGrid);
                                }
                            }
                        }
                    }
                    if (tileExtendedSettings != undefined) {
                        selectedConnectionPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadGenericFieldDefinitionSelector());
                    }

                    function loadConnectionSelector()
                    {
                        var payloadConnectionSelector = {
                            filter: { Filters: [] }
                        };
                        payloadConnectionSelector.filter.Filters.push({
                            $type: "Vanrise.Common.Business.VRInterAppRestConnectionFilter, Vanrise.Common.Business"
                        });
                        if (tileExtendedSettings != undefined) {
                            payloadConnectionSelector.selectedIds = tileExtendedSettings.VRConnectionId;
                        }
                        return connectionSelectorApi.load(payloadConnectionSelector);

                    }
                 
                    promises.push(loadConnectionSelector());

                  
                    function loadGenericFieldDefinitionSelector()
                    {
                        if (tileExtendedSettings != undefined)
                        {
                            var genericFieldDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                            genericFieldDefinitionSelectorPromiseDeferred.promise.then(function () {
                                var genericFieldDefinitionPayload = {
                                    connectionId: tileExtendedSettings.VRConnectionId,
                                    selectedIds: selectedGenericFieldsIds,
                                    accountBEDefinitionId: tileExtendedSettings.AccountBEDefinitionId

                                };
                                VRUIUtilsService.callDirectiveLoad(genericFieldDefinitionSelectorApi, genericFieldDefinitionPayload, genericFieldDefinitionSelectorLoadDeferred);
                            });
                            return genericFieldDefinitionSelectorLoadDeferred.promise;
                        }
                        
                    }
                  
                    function addGenericFieldGridWidthAPI(genericFieldGrid) {
                        var dataItemPayload = {
                            data: {
                                Width: VRCommon_GridWidthFactorEnum.Normal.value
                            }
                        };
                        var dataItem = {};
                        if (genericFieldGrid.payload != undefined) {
                            dataItem.FieldName = genericFieldGrid.payload.FieldName;
                            dataItem.Title = genericFieldGrid.payload.FieldTitle;
                            dataItemPayload.data = genericFieldGrid.payload.ColumnSettings;
                        }
                        dataItem.onGridWidthFactorEditorReady = function (api) {
                            dataItem.gridWidthFactorAPI = api;
                            genericFieldGrid.readyPromiseDeferred.resolve();
                        };
                        genericFieldGrid.readyPromiseDeferred.promise
                            .then(function () {
                                VRUIUtilsService.callDirectiveLoad(dataItem.gridWidthFactorAPI, dataItemPayload, genericFieldGrid.loadPromiseDeferred);
                            });
                        $scope.scopeModel.genericFieldDefinitions.push(dataItem);
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var accountGridFields;
                    if ($scope.scopeModel.genericFieldDefinitions != undefined) {
                        accountGridFields = [];
                        for (var i = 0; i < $scope.scopeModel.genericFieldDefinitions.length; i++) {
                            var genericFieldDefinition = $scope.scopeModel.genericFieldDefinitions[i];
                            accountGridFields.push({
                                FieldName: genericFieldDefinition.FieldName,
                                FieldTitle: genericFieldDefinition.Title,
                                ColumnSettings: genericFieldDefinition.gridWidthFactorAPI.getData()
                            });
                        }
                    }

                    return {
                        $type: "PartnerPortal.CustomerAccess.Business.RetailSubAccountsInfoTileDefinitionSettings, PartnerPortal.CustomerAccess.Business",
                        VRConnectionId: connectionSelectorApi.getSelectedIds(),
                        AccountGridFields: accountGridFields,
                        AccountBEDefinitionId: $scope.scopeModel.accountBEDefinitionId
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }

        return directiveDefinitionObject;
    }
]);