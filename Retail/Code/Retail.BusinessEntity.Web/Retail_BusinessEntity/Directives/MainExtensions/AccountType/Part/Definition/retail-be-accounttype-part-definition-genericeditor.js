'use strict';

app.directive('retailBeAccounttypePartDefinitionGenericeditor', ["VRUIUtilsService", "UtilsService", "Retail_BE_AccountPartDefinitionService",
    function (VRUIUtilsService, UtilsService, Retail_BE_AccountPartDefinitionService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountTypeGenericEditorPartDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Definition/Templates/AccountTypeGenericEditorPartDefinitionTemplate.html'
        };

        function AccountTypeGenericEditorPartDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dataRecordTypeSelectedDeferred = UtilsService.createPromiseDeferred();

            var gridAPI;
            var gridReadyDeferred = UtilsService.createPromiseDeferred();
            var genericEditorItems;

            var dataRecordTypeId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataSource = [];

                $scope.scopeModel.onDataRecordTypeSelectorDirectiveReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridReadyDeferred.resolve();
                };

                $scope.scopeModel.onDataRecordTypeSelectionChanged = function (selectedValue) {
                    if (selectedValue != undefined) {
                        dataRecordTypeId = selectedValue.DataRecordTypeId;
                        if (dataRecordTypeSelectedDeferred != undefined) {
                            dataRecordTypeSelectedDeferred = undefined;
                        } else {
                            gridAPI.clearDataSource();
                        }

                    }
                };
                $scope.scopeModel.addGenericEditorItem = function () {
                    var onGenericEditorItemAdded = function (genericEditorItemAdded) {
                        $scope.scopeModel.dataSource.push({ Entity: genericEditorItemAdded });
                        genericEditorItems.push(genericEditorItemAdded);
                    };

                    var parameters = {
                        dataRecordTypeId: $scope.scopeModel.selectedDataRecordType.DataRecordTypeId
                    };
                    Retail_BE_AccountPartDefinitionService.addAccountPartGenericEditorItemDefinition(parameters, onGenericEditorItemAdded);
                };

                $scope.scopeModel.isGridEmpty = function () {
                    if ($scope.scopeModel.dataSource.length > 0)
                        return null;
                    return "Empty Grid is not Allowed";
                };


                $scope.scopeModel.removeRow = function (deletedItem) {
                    var index = $scope.scopeModel.dataSource.indexOf(deletedItem);
                    if (index >= 0) {
                        $scope.scopeModel.dataSource.splice(index, 1);
                        genericEditorItems.splice(index, 1);
                    }
                };

                UtilsService.waitMultiplePromises([dataRecordTypeSelectorReadyPromiseDeferred.promise, gridReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    genericEditorItems = [];
                    if (payload != undefined) {
                        var definitionSettingsPayload = payload.partDefinitionSettings;
                        if (definitionSettingsPayload != undefined) {
                            dataRecordTypeId = definitionSettingsPayload.DataRecordTypeId;
                            genericEditorItems = definitionSettingsPayload.Items;
                            if (genericEditorItems != undefined) {
                                var length = genericEditorItems.length;
                                for (var i = 0; i < length; i++) {
                                    var item = { Entity: genericEditorItems[i] };
                                    $scope.scopeModel.dataSource.push(item);
                                }
                            }
                        }
                    }

                    var loadDataRecordTypeSelectorPromise = loadDataRecordTypeSelector();
                    promises.push(loadDataRecordTypeSelectorPromise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartGenericEditorDefinition, Retail.BusinessEntity.MainExtensions',
                        DataRecordTypeId: $scope.scopeModel.selectedDataRecordType.DataRecordTypeId,
                        Items: genericEditorItems
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function loadDataRecordTypeSelector() {
                var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                var directivePayload = {
                    selectedIds: dataRecordTypeId
                };
                VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                return loadDataRecordTypeSelectorPromiseDeferred.promise;
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [
                    {
                        name: "Edit",
                        clicked: editGenericEditorItemDefinition
                    }
                ];
            }

            function editGenericEditorItemDefinition(genericEditorItemObj) {
                var onGenericEditorItemDefinitionUpdated = function (genericEditorItemUpdated) {
                    var index = $scope.scopeModel.dataSource.indexOf(genericEditorItemUpdated);
                    $scope.scopeModel.dataSource[index] = genericEditorItemUpdated;
                    genericEditorItems[index] = genericEditorItemUpdated.Entity;
                };
                var parameters = {
                    dataRecordTypeId: $scope.scopeModel.selectedDataRecordType.DataRecordTypeId,
                    item: genericEditorItemObj
                };
                Retail_BE_AccountPartDefinitionService.editAccountPartGenericEditorItemDefinition(parameters, onGenericEditorItemDefinitionUpdated);
            }
        }
    }]);