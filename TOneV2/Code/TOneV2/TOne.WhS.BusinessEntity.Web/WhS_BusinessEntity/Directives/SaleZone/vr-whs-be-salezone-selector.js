'use strict';
app.directive('vrWhsBeSalezoneSelector', ['WhS_BE_SaleZoneAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SaleZoneAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new saleZoneCtor(ctrl, $scope, $attrs);
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
                return getBeSaleZoneTemplate(attrs);
            }

        };


        function getBeSaleZoneTemplate(attrs) {

            var label = "Sale Zone";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Sale Zones";
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns width="normal" ng-if="ctrl.showselling">'
                   + '   <vr-whs-be-sellingnumberplan-selector on-ready="ctrl.onSellingNumberReady" onselectionchanged="ctrl.onSellingNumberPlanChange"></vr-whs-be-sellingnumberplan-selector>'
                   + ' </vr-columns>'
                   + ' <vr-columns width="normal" ng-if="ctrl.showselling">'
                   + '  <vr-select on-ready="ctrl.onSelectorReady"'
                   + '  datasource="ctrl.search"'
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  datavaluefield="SaleZoneId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
                   + '  vr-disabled="ctrl.isdisabled"'
                   + '  label="' + label + '"'
                   + '  entityName="' + label + '">'
                   + '</vr-select>'
                   + '</vr-columns>'
                   + ' <div ng-if="!ctrl.showselling">'
                   + '  <vr-select on-ready="ctrl.onSelectorReady"'
                   + '  datasource="ctrl.search"'
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  datavaluefield="SaleZoneId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
                   + '  vr-disabled="ctrl.isdisabled"'
                   + '  label="' + label + '"'
                   + '  entityName="' + label + '">'
                   + '</vr-select>'
                   + '</div>'

        }

        function saleZoneCtor(ctrl, $scope, attrs) {

            var filter;
            var selectorApi;
            var sellingDirectiveApi;
            var sellingReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var sellingNumberPlanId;
            var newsellingNumberPlanId;
            var oldsellingNumberPlanId;
            var isDirectiveLoaded = false;

            ctrl.onSellingNumberReady = function (api) {
                sellingDirectiveApi = api;
                sellingReadyPromiseDeferred.resolve();
            }
            ctrl.onSellingNumberPlanChange = function (item, datasource) {
                //UtilsService.safeApply($scope);

                oldsellingNumberPlanId = sellingNumberPlanId;
                newsellingNumberPlanId = sellingDirectiveApi.getSelectedIds();

                if (newsellingNumberPlanId == undefined && oldsellingNumberPlanId != undefined) {
                    sellingNumberPlanId = undefined;
                    selectorApi.clearDataSource();
                }
                else if (oldsellingNumberPlanId == undefined && newsellingNumberPlanId == undefined && oldsellingNumberPlanId == newsellingNumberPlanId) {
                    sellingNumberPlanId = newsellingNumberPlanId;
                }
                else if (oldsellingNumberPlanId != undefined && newsellingNumberPlanId != undefined && oldsellingNumberPlanId != newsellingNumberPlanId) {
                    sellingNumberPlanId = newsellingNumberPlanId;
                    selectorApi.clearDataSource();

                }
                else {
                    sellingNumberPlanId = newsellingNumberPlanId;
                }
                console.log(sellingNumberPlanId)
            }
            function initializeController() {

                ctrl.selectedvalues;
                if (attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;

                }

                ctrl.search = function (nameFilter) {
                    console.log(sellingNumberPlanId)
                    if (sellingNumberPlanId == undefined)
                        return function () { };

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo(nameFilter, sellingNumberPlanId, serializedFilter);
                }
                defineAPI();

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (selectorApi)
                        selectorApi.clearDataSource();
                    ctrl.showselling = true;
                    var selectedIds;
                    if (payload != undefined) {
                        sellingNumberPlanId = payload.sellingNumberPlanId;
                        ctrl.showselling = payload.sellingNumberPlanId != undefined ? false : true;
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }
                    var promises = [];
                    if (selectedIds != undefined) {
                        ctrl.datasource = [];
                        var defaultId = (attrs.ismultipleselection != undefined) ? selectedIds[0] : selectedIds;
                        WhS_BE_SaleZoneAPIService.GetSellingNumberPlanIdBySaleZoneId(defaultId).then(function (response) {
                            var input = {
                                SellingNumberPlanId: response,
                                SaleZoneIds: selectedIds,
                                SaleZoneFilterSettings: { RoutingProductId: (filter != undefined && filter.SaleZoneFilterSettings != undefined) ? filter.SaleZoneFilterSettings.RoutingProductId : undefined }
                            };

                            if (ctrl.showselling) {
                                var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
                                sellingReadyPromiseDeferred.promise.then(function () {
                                    var payloadDirective = {
                                        selectedIds: response
                                    };
                                    VRUIUtilsService.callDirectiveLoad(sellingDirectiveApi, payloadDirective, loadSellingNumberPlanPromiseDeferred);
                                });
                                loadSellingNumberPlanPromiseDeferred.promise.then(function () {
                                    WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                                        angular.forEach(response, function (item) {
                                            ctrl.datasource.push(item);
                                        });
                                        VRUIUtilsService.setSelectedValues(selectedIds, 'SaleZoneId', attrs, ctrl);
                                    });
                                });
                                promises.push(loadSellingNumberPlanPromiseDeferred.promise);
                            }
                            else {
                                WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                                    angular.forEach(response, function (item) {
                                        ctrl.datasource.push(item);
                                    });
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'SaleZoneId', attrs, ctrl);
                                });

                            }

                        });

                    }
                    if (!sellingNumberPlanId && selectedIds == undefined) {
                        var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
                        sellingReadyPromiseDeferred.promise.then(function () {
                            var payloadDirective;
                            VRUIUtilsService.callDirectiveLoad(sellingDirectiveApi, payloadDirective, loadSellingNumberPlanPromiseDeferred);

                        });
                        promises.push(loadSellingNumberPlanPromiseDeferred.promise);
                    }
                    return UtilsService.waitMultiplePromises(promises);


                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SaleZoneId', attrs, ctrl);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);