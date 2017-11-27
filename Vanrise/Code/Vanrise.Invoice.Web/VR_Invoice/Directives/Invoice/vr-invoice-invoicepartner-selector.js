(function (app) {

    'use strict';

    InvoicepartnerSelectorDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService'];

    function InvoicepartnerSelectorDirective(UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '=',
                ismultipleselection:'@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InvoicepartnerSelectorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrlIP",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_Invoice/Directives/Invoice/Templates/InvoicePartnerSelectorTemplate.html"
        };

        function InvoicepartnerSelectorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var invoiceTypeId;
            var filter;

            var directiveAPI;
            var directiveReadyDeferred = UtilsService.createPromiseDeferred();

            var invoiceTypeEntity;
            var partnerInvoiceFilters;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    directiveReadyDeferred.resolve();
                    //var setLoader = function (value) {
                    //    $scope.scopeModel.isLoadingDirective = value;
                    //};
                    //var directivePayload = {
                    //    filter: filter,
                    //    extendedSettings: invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined? invoiceTypeEntity.Settings.ExtendedSettings:undefined,
                    //    invoiceTypeId: invoiceTypeId
                    //};
                    //VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var selectedIds;

                    if (payload != undefined) {

                        invoiceTypeId = payload.invoiceTypeId;
                        partnerInvoiceFilters = payload.partnerInvoiceFilters;
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;

                        //if(partnerInvoiceFilters != undefined)
                        //{
                        //    if (filter == undefined)
                        //        filter = {};
                        //    if (filter.Filters == undefined)
                        //        filter.Filters = [];
                        //    for (var i = 0; i < partnerInvoiceFilters.length; i++) {
                        //        var partnerInvoiceFilter = partnerInvoiceFilters[i];
                        //        filter.Filters.push(partnerInvoiceFilter);
                        //    }
                        //}
                    }

                    var invoiceTypeSelectorPromise = getInvoiceTypeSelector(invoiceTypeId);
                    promises.push(invoiceTypeSelectorPromise);


                    var invoiceTypePromise = getInvoiceType(invoiceTypeId);
                    promises.push(invoiceTypePromise);
                    

                    var loadDirectivePromise = UtilsService.createPromiseDeferred();
                    UtilsService.waitMultiplePromises([invoiceTypeSelectorPromise, invoiceTypePromise]).then(function () {


                        loadDirective().then(function () {
                            loadDirectivePromise.resolve();
                        }).catch(function (error) {
                            loadDirectivePromise.reject(error);
                        });
                    });
                    promises.push(loadDirectivePromise.promise);
                 

                    function getInvoiceTypeSelector(invoiceTypeId) {
                        return VR_Invoice_InvoiceTypeAPIService.GetInvoicePartnerSelector(invoiceTypeId).then(function (response) {
                            $scope.scopeModel.Editor = response;
                        });
                    }
                    function getInvoiceType(invoiceTypeId) {
                        return VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeId).then(function (response) {
                            invoiceTypeEntity = response;
                        });
                    }
                    function loadDirective() {

                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            var directivePayload = {
                                selectedIds: selectedIds,
                                extendedSettings: invoiceTypeEntity != undefined && invoiceTypeEntity.Settings != undefined ? invoiceTypeEntity.Settings.ExtendedSettings : undefined,
                                filter: filter,
                                invoiceTypeId: invoiceTypeId,
                                partnerInvoiceFilters: partnerInvoiceFilters
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });

                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return directiveAPI.getData();
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrInvoiceInvoicepartnerSelector', InvoicepartnerSelectorDirective);

})(app);