'use strict';

app.directive('retailBeAccounttypePartRuntimeGenericeditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountTypeGenericEditorPart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/AccountType/Part/Runtime/Templates/AccountTypeGenericEditorPartRuntimeTemplate.html'
        };

        function AccountTypeGenericEditorPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var definitionSettings;
            var dataRecordTypeId;
            var dataRecord;
            var items;
            var context;
            var dataRecordTypeFields;
            var getContextPromise;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tabItems = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        var partDefinition = payload.partDefinition;
                        if (partDefinition != undefined) {
                            definitionSettings = partDefinition.Settings;
                            if (definitionSettings != undefined) {
                                dataRecordTypeId = definitionSettings.DataRecordTypeId;
                                items = definitionSettings.Items;
                            }
                        }
                        dataRecord = payload.partSettings != undefined ? payload.partSettings.DataRecord : undefined;
                    }

                    getContextPromise = getContext();
                    
                    $scope.scopeModel.tabItems = [];

                    if (items != undefined && items.length > 0) {
                        for (var i = 0; i < items.length; i++) {
                            var item = items[i];
                            var tabItem = {
                                payload: item,
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(tabItem.loadPromiseDeferred.promise);
                            buildTabItems(tabItem);
                        }
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var object = {};
                    var length = $scope.scopeModel.tabItems.length;
                    for (var i = 0; i < length; i++) {
                        var tabItem = $scope.scopeModel.tabItems[i];
                        tabItem.rootEditorRuntimeDirectiveAPI.setData(object);
                    }
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.AccountParts.AccountPartGenericEditor, Retail.BusinessEntity.MainExtensions',
                        DataRecord: object
                    };

                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function buildTabItems(item) {
                var tabItem = {
                    header: item.payload.Title,
                };

                tabItem.onRootEditorRuntimeDirectiveReady = function (api) {
                    tabItem.rootEditorRuntimeDirectiveAPI = api;
                    item.readyPromiseDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([item.readyPromiseDeferred.promise, getContextPromise]).then(function () {
                    var runtimeEditorPayload = {
                        selectedValues: dataRecord,
                        dataRecordTypeId: dataRecordTypeId,
                        definitionSettings: item.payload.Settings,
                        runtimeEditor: item.payload.Settings != undefined ? item.payload.Settings.RuntimeEditor : undefined,
                        isEditMode: dataRecord != undefined,
                        context: context
                    };
                    VRUIUtilsService.callDirectiveLoad(tabItem.rootEditorRuntimeDirectiveAPI, runtimeEditorPayload, item.loadPromiseDeferred);
                });
                $scope.scopeModel.tabItems.push(tabItem);

            }

            function getContext() {
                return getDataRecordFieldsInfo().then(function () {
                    buildContext();
                });
            }

            function getDataRecordFieldsInfo() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(dataRecordTypeId).then(function (response) {
                    dataRecordTypeFields = [];
                    if (response != undefined) {
                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            dataRecordTypeFields.push(currentField.Entity);
                        }
                    }
                });
            }

            function buildContext() {
                context = {};
                context.getFields = function () {
                    var dataFields = [];
                    for (var i = 0; i < dataRecordTypeFields.length; i++) {
                        dataFields.push({
                            FieldName: dataRecordTypeFields[i].Name,
                            FieldTitle: dataRecordTypeFields[i].Title,
                            Type: dataRecordTypeFields[i].Type
                        });
                    }
                    return dataFields;
                };
            }

        }
    }]);