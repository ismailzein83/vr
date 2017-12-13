'use strict';

app.directive('vrWhsBeSupplierzoneSelector', ['WhS_BE_SupplierZoneAPIService', 'VRCommon_EntityFilterEffectiveModeEnum', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SupplierZoneAPIService, VRCommon_EntityFilterEffectiveModeEnum, UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                onblurdropdown: '=',
                isrequired: "=",
                supplierid: "=",
                selectedvalues: '=',
                hidetitle: '@',
                normalColNum: '@'
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
                };
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
                   + ' onselectionchanged="ctrl.onSupplierSelectionchanged"></vr-whs-be-carrieraccount-selector>'
                   + ' </span>'
                   + ' <span vr-disabled="ctrl.isSupplierZoneDisabled">'
                   + ' <vr-columns colnum="{{ctrl.normalColNum}}" >'
                   + ' <vr-select ' + multipleselection + ' on-ready="ctrl.SelectorReady"  datatextfield="Name" datavaluefield="SupplierZoneId"  limitcharactercount="ctrl.limitcharactercount"'
                   + ' isrequired="ctrl.isrequired" datasource="ctrl.searchSupplierZones" selectedvalues="ctrl.selectedvalues"' + label + 'onselectionchanged="ctrl.onselectionchanged" onblurdropdown="ctrl.onblurdropdown" entityName="Supplier Zone"></vr-select>'
                   + ' </vr-columns>'
                   + ' </span>';
        }

        function supplierZoneCtor(ctrl, $scope, $attrs) {

            var filter;
            var supplierId;
            var availableZoneIds;
            var excludedZoneIds;

            var supplierDirectiveApi;
            var suppliersReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var selectorApi;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var genericUIContext;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.SelectorReady = function (api) {
                    selectorApi = api;
                    selectorReadyPromiseDeferred.resolve();
                };

                ctrl.onSupplierReady = function (api) {
                    supplierDirectiveApi = api;
                    suppliersReadyPromiseDeferred.resolve();
                };

                ctrl.onSupplierSelectionchanged = function () {
                    selectorApi.clearDataSource();
                    if (supplierDirectiveApi.getSelectedIds() != undefined)
                        supplierId = supplierDirectiveApi.getSelectedIds();
                };

                ctrl.searchSupplierZones = function (searchValue) {
                    if (supplierId == undefined) {
                        var deferredPromise = UtilsService.createPromiseDeferred();
                        deferredPromise.resolve();
                        return deferredPromise.promise;
                    }

                    if (filter == undefined)
                        filter = {};

                    if (filter.EffectiveMode == undefined)
                        filter.EffectiveMode = VRCommon_EntityFilterEffectiveModeEnum.Current.value;

                    if (availableZoneIds != undefined)
                        filter.AvailableZoneIds = availableZoneIds;

                    if (excludedZoneIds != undefined)
                        filter.ExcludedZoneIds = excludedZoneIds;

                    var serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_BE_SupplierZoneAPIService.GetSupplierZoneInfo(searchValue, supplierId, serializedFilter);
                };

                UtilsService.waitMultiplePromises([suppliersReadyPromiseDeferred.promise, selectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.isSupplierZoneVisible = true;
                    selectorApi.clearDataSource();

                    var selectedIds;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        supplierId = payload.supplierId;
                        availableZoneIds = payload.availableZoneIds;
                        excludedZoneIds = payload.excludedZoneIds;
                        genericUIContext = payload.genericUIContext;
                    }

                    if (supplierId != undefined) {
                        var promises = [];
                        ctrl.isSupplierVisible = false;

                        if (selectedIds != undefined) {
                            promises.push(GetSupplierZonesInfo($attrs, ctrl, selectedIds, supplierId));
                        }

                        if (filter != undefined && filter.CountryIds != undefined && filter.CountryIds.length == 1) {
                            ctrl.limitcharactercount = 0;
                            promises.push(selectorApi.loadDataSource("").then(function (res) { }));
                        }
                        return UtilsService.waitMultiplePromises(promises);
                    }

                    else {
                        ctrl.isSupplierVisible = true;

                        if (genericUIContext != undefined && genericUIContext.getFields != undefined && typeof (genericUIContext.getFields) == "function") {
                            var fields = genericUIContext.getFields();
                            if (fields != undefined) {
                                for (var x = 0; x < fields.length; x++) {
                                    var currentField = fields[x];
                                    if (currentField != undefined && currentField.FieldType != undefined && currentField.FieldType.BusinessEntityDefinitionId != undefined
                                        && currentField.FieldType.BusinessEntityDefinitionId.toUpperCase() == "8C286BCD-5766-487A-8B32-5D167EC342C0") {
                                        ctrl.isSupplierVisible = false;
                                        ctrl.isSupplierZoneDisabled = true;
                                        genericUIContext.onvaluechanged = function (field, selectedValue) {
                                            if (field.FieldType.BusinessEntityDefinitionId.toUpperCase() == "8C286BCD-5766-487A-8B32-5D167EC342C0") {

                                                ctrl.isSupplierZoneDisabled = true;
                                                
                                                if (selectedValue == undefined) {
                                                    selectorApi.clearDataSource();
                                                    return;
                                                }

                                                if (selectedValue instanceof Array) {
                                                    switch (selectedValue.length) {
                                                        case 1: ctrl.isSupplierZoneDisabled = false; supplierId = selectedValue[0].CarrierAccountId; break;
                                                        default: selectorApi.clearDataSource(); supplierId = undefined; break;
                                                    }
                                                }
                                                else {
                                                    supplierId = selectedValue.CarrierAccountId;
                                                }
                                            }
                                        };

                                        break;
                                    }
                                }
                            }
                        }

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
                                        });
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

                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SupplierZoneId', $attrs, ctrl);
                };

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