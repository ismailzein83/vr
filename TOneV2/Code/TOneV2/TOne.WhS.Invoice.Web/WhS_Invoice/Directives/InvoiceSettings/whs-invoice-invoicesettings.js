(function (app) {

    'use strict';

    whsInvoiceSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function whsInvoiceSettingsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InvoiceSettingsDirective($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Invoice/Directives/InvoiceSettings/Templates/InvoiceSettingsTemplate.html'
            
        };

        function InvoiceSettingsDirective($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            var gridAPI;

            var invoiceTypeSelectorAPI;
            var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                };

                ctrl.onAddInvoiceSetting = function () {
                    var dataItem;
                    dataItem = {
                        id:ctrl.datasource.length + 1,
                        needApproval: ""
                    };
                    dataItem.onInvoiceTypeSelectorReady = function (api) {
                        dataItem.invoiceTypeSelectorAPI = api;
                        var setLoader = function (value) { ctrl.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.invoiceTypeSelectorAPI, undefined, setLoader);
                    };
                    ctrl.datasource.push(dataItem);
                    
                    dataItem.saveDataItem = function () {
                        ctrl.datasource[dataItem.id-1].InvoiceTypeId = dataItem.invoiceTypeSelectorAPI.getSelectedIds();
                    };

                };

                ctrl.isInvoiceTypeValid = function () {

                    for (var x = 0; x < ctrl.datasource.length; x++) {
                        var currentItem = ctrl.datasource[x];
                        for (var y = 0; y < ctrl.datasource.length; y++) {
                            if (x == y)
                                continue;
                            var dataItem = ctrl.datasource[y];
                            if (dataItem.InvoiceTypeId == currentItem.InvoiceTypeId)
                                return 'This type already exists';
                        }
                    }
                    return null;
                };


                ctrl.removeRow = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, dataItem.id, 'id');
                    ctrl.datasource.splice(index, 1);
                };

                defineAPI();
            }
            
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    
                    var promises = [];

                    if (payload != undefined && payload.data != undefined && payload.data.InvoiceTypeSettings != undefined) {
                       
                        for (var i = 0; i < payload.data.InvoiceTypeSettings.length; i++) {
                            var item = {
                                payload: payload.data.InvoiceTypeSettings[i],
                                readyInvoiceTypeSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadInvoiceTypeSelectorPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(item.loadInvoiceTypeSelectorPromiseDeferred.promise);
                            addItemToGrid(item);
                        }
                    }

                    function addItemToGrid(Item) {
                        
                        var dataItem = {
                            id: ctrl.datasource.length + 1,
                            InvoiceTypeId : Item.payload.InvoiceTypeId,
                            NeedApproval: Item.payload.NeedApproval
                        };

                        var dataItemPayload = { selectedIds: Item.payload.InvoiceTypeId };

                        dataItem.onInvoiceTypeSelectorReady = function (api) {
                            dataItem.invoiceTypeSelectorAPI = api;
                            Item.readyInvoiceTypeSelectorPromiseDeferred.resolve();
                        };

                        Item.readyInvoiceTypeSelectorPromiseDeferred.promise
                            .then(function () {
                                VRUIUtilsService.callDirectiveLoad(dataItem.invoiceTypeSelectorAPI, dataItemPayload, Item.loadInvoiceTypeSelectorPromiseDeferred);
                            });

                        dataItem.saveDataItem = function () {
                            ctrl.datasource[dataItem.id-1].InvoiceTypeId = dataItem.invoiceTypeSelectorAPI.getSelectedIds();
                        };

                        ctrl.datasource.push(dataItem);
                    }

                  return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.InvoiceSettings,TOne.WhS.Invoice.Business",
                        InvoiceTypeSettings: getSettings()
                    };
                };

                function getSettings() {
                    var settingsList = [];

                    angular.forEach(ctrl.datasource, function (item) {
                        var dataItem = {
                            InvoiceTypeId: item.invoiceTypeSelectorAPI.getSelectedIds(),
                            NeedApproval: item.NeedApproval
                        };

                        settingsList.push(dataItem);
                    });
                   
                    return settingsList;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

    }

    app.directive('whsInvoiceInvoicesettings', whsInvoiceSettingsDirective);

})(app);