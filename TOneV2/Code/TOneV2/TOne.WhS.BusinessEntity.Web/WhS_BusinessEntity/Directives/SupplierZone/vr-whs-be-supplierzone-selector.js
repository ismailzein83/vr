'use strict';
app.directive('vrWhsBeSupplierzoneSelector', ['WhS_BE_SupplierZoneAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SupplierZoneAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                onblurdropdown:'=',
                isrequired: "=",
                supplierid: "=",
                selectedvalues: '=',
                hidetitle: '@',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var ctor = new supplierZoneCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getBeSupplierZoneTemplate(attrs);
            }

        };


        function getBeSupplierZoneTemplate(attrs) {

            var multipleselection = "";
            var label = 'label="Supplier Zone"';
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
                label = 'label="Supplier Zones"';
            }
            if (attrs.hidetitle != undefined) {
                label = "";
            }

            return '<span  ng-show="ctrl.isSupplierVisible">'
                   + '  <vr-whs-be-carrieraccount-selector  normal-col-num="{{ctrl.normalColNum}}"   getsuppliers on-ready="ctrl.onSupplierReady"'
                   + ' onselectionchanged="ctrl.onSupplierSelectionchanged"></vr-whs-be-sellingnumberplan-selector>'
                   + ' </span>'
                   + '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                   + '<vr-select ' + multipleselection + ' on-ready="ctrl.SelectorReady"  datatextfield="Name" datavaluefield="SupplierZoneId"'
                   + 'isrequired="ctrl.isrequired" datasource="ctrl.searchSupplierZones" selectedvalues="ctrl.selectedvalues"' + label + 'onselectionchanged="ctrl.onselectionchanged" onblurdropdown="ctrl.onblurdropdown" entityName="Supplier Zone"></vr-select>'
                   + '</vr-columns>'
        }

        function supplierZoneCtor(ctrl, $scope, $attrs) {
            var filter;
            var selectorApi;

            var selectorApi;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierDirectiveApi;
            var suppliersReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var supplierId;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.SelectorReady = function (api) {
                    selectorApi = api;
                    selectorReadyPromiseDeferred.resolve();
                }

                ctrl.onSupplierReady = function (api) {
                    supplierDirectiveApi = api;
                    suppliersReadyPromiseDeferred.resolve();
                }

                ctrl.onSupplierSelectionchanged = function () {
                    selectorApi.clearDataSource();
                    if(supplierDirectiveApi.getSelectedIds() != undefined)
                        supplierId = supplierDirectiveApi.getSelectedIds();
                }

                ctrl.searchSupplierZones = function (searchValue) {
                    if (supplierId == undefined)
                    {
                        var deferredPromise = UtilsService.createPromiseDeferred();
                        deferredPromise.resolve();
                        return deferredPromise.promise;
                    }

                    var getEffectiveOnly = true;

                    if (filter != undefined) {
                        if (filter.GetEffectiveOnly == undefined) {
                            filter.GetEffectiveOnly = getEffectiveOnly;
                        }
                    }
                    else {
                        filter = { GetEffectiveOnly: getEffectiveOnly };
                    }

                    var serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_BE_SupplierZoneAPIService.GetSupplierZoneInfo(searchValue, supplierId, serializedFilter);
                }

                UtilsService.waitMultiplePromises([suppliersReadyPromiseDeferred.promise, selectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });

            }


            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    selectorApi.clearDataSource();

                    var selectedIds;


                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        supplierId = payload.supplierId;
                    }

                    if (supplierId != undefined) {
                        ctrl.isSupplierVisible = false;

                        if (selectedIds != undefined) {
                            return GetSupplierZonesInfo($attrs, ctrl, selectedIds, supplierId);
                        }

                    }

                    else {
                        ctrl.isSupplierVisible = true;
                        if (selectedIds != undefined) {
                            var selectedSupplierZoneIds = [];

                            if ($attrs.ismultipleselection != undefined)
                                selectedSupplierZoneIds = selectedIds;
                            else
                                selectedSupplierZoneIds.push(selectedIds);

                            var loadSuppliersSectionPromiseDeferred = UtilsService.createPromiseDeferred();

                            suppliersReadyPromiseDeferred.promise.then(function () {

                                var promises = [];

                                var loadSupplierPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(loadSupplierPromiseDeferred.promise);

                                var setSelectedSupplierZonesPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(setSelectedSupplierZonesPromiseDeferred.promise);


                                var loadSupplierZonesPromise = WhS_BE_SupplierZoneAPIService.GetDistinctSupplierIdsBySupplierZoneIds(selectedSupplierZoneIds).then(function (response) {

                                    var selectedSupplierIds = [];
                                    for (var i = 0 ; i < response.length; i++) {
                                        selectedSupplierIds.push(response[i]);
                                    }

                                    var supplierSelectorPayload = {
                                        selectedIds: selectedSupplierIds
                                    };

                                    VRUIUtilsService.callDirectiveLoad(supplierDirectiveApi, supplierSelectorPayload, loadSupplierPromiseDeferred);

                                    loadSupplierPromiseDeferred.promise.then(function () {
                                        WhS_BE_SupplierZoneAPIService.GetSupplierZoneInfoByIds(UtilsService.serializetoJson(selectedIds)).then(function (response) {
                                            angular.forEach(response, function (item) {
                                                ctrl.datasource.push(item);
                                            });

                                            VRUIUtilsService.setSelectedValues(selectedIds, 'SupplierZoneId', $attrs, ctrl);
                                            setSelectedSupplierZonesPromiseDeferred.resolve();
                                        })


                                    });

                                });

                                promises.push(loadSupplierZonesPromise);

                                UtilsService.waitMultiplePromises(promises).then(function () {
                                    loadSuppliersSectionPromiseDeferred.resolve();
                                }).catch(function (error) {
                                    loadSuppliersSectionPromiseDeferred.reject(error);
                                });

                            });

                            return loadSuppliersSectionPromiseDeferred.promise;
                        }
                        else {
                            var loadSuppliersPromiseDeferred = UtilsService.createPromiseDeferred();
                            suppliersReadyPromiseDeferred.promise.then(function () {
                                VRUIUtilsService.callDirectiveLoad(supplierDirectiveApi, undefined, loadSuppliersPromiseDeferred);
                            });
                            return loadSuppliersPromiseDeferred.promise;
                        }
                    }

                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SupplierZoneId', $attrs, ctrl);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;
            }

            this.initializeController = initializeController;
        }

        function GetSupplierZonesInfo(attrs, ctrl, selectedIds, supplierId) {
            return WhS_BE_SupplierZoneAPIService.GetSupplierZonesInfo(supplierId).then(function (response) {
                angular.forEach(response, function (item) {
                    ctrl.datasource.push(item);
                });
                if (selectedIds != undefined)
                    VRUIUtilsService.setSelectedValues(selectedIds, 'SupplierZoneId', attrs, ctrl);
            });
        }



        return directiveDefinitionObject;
    }]);