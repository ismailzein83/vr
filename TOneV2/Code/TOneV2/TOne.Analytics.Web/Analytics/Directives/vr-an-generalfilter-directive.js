(function (angular, app) {

    "use strict";

    function vrDirectiveObj(analyticsService, BusinessEntityAPIService, ZonesService, CarrierAccountConnectionAPIService, CarrierTypeEnum, GenericAnalyticDimensionEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                dimensionsvalues: "=",
                filtervalues: "=",
                filters: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                function onLoad() {
                    ctrl.GenericAnalyticDimensionEnum = GenericAnalyticDimensionEnum;
                    ctrl.switches = [];
                    ctrl.selectedSwitches = [];
                    ctrl.codeGroups = [];
                    ctrl.selectedCodeGroups = [];

                    ctrl.customers = [];
                    ctrl.selectedCustomers = [];
                    ctrl.suppliers = [];
                    ctrl.selectedSuppliers = [];

                    ctrl.filters = [];

                    ctrl.filterCustomer = {
                        Dimension: GenericAnalyticDimensionEnum.Customer.value,
                        FilterValues: []
                    };
                    ctrl.filterSupplier = {
                        Dimension: GenericAnalyticDimensionEnum.Supplier.value,
                        FilterValues: []
                    };


                    ///////////////////////////////////////////////
                    var now = new Date();
                    ctrl.fromDate = new Date(2013, 1, 1);
                    ctrl.toDate = now;
                    //////////////////////////////////////////////


                    ctrl.zones = [];
                    ctrl.selectedZones = [];

                    ctrl.selectedGroupKeys = [];


                    ctrl.connections = [];
                    ctrl.selectedConnections = [];
                    ctrl.selectedConnectionIndex;

                    loadSwitches();
                    loadCodeGroups();
                }

                function loadSwitches() {
                    return BusinessEntityAPIService.GetSwitches().then(function (response) {
                        ctrl.switches = response;
                    });
                }

                function loadCodeGroups() {
                    return BusinessEntityAPIService.GetCodeGroups().then(function (response) {
                        ctrl.codeGroups = response;
                    });
                }
                
                function searchZones(text) {
                    return ZonesService.getSalesZones(text);
                }

                function onSelectionChanged() {
                    var value;
                    switch (ctrl.selectedConnectionIndex) {
                        case 0: ctrl.selectedConnections.length = 0; ctrl.connections.length = 0; return;
                        case 1: value = CarrierTypeEnum.Customer.value;
                            break;
                        case 2: value = CarrierTypeEnum.Supplier.value;
                            break;
                    }
                    return CarrierAccountConnectionAPIService.GetConnectionByCarrierType(value).then(function (response) {
                        ctrl.selectedConnections.length = 0;
                        ctrl.connections.length = 0;
                        ctrl.connections = response;
                    });
                }

                function onselectionvalueschanged() {


                    ctrl.selectedCustomers.forEach(function (item) {
                        ctrl.filterCustomer.FilterValues.push(item.CarrierAccountID);
                    });

                    console.log(ctrl.filterCustomer);
                    //if (ctrl.filterCustomer.FilterValues.length > 0)
                    //    ctrl.filters.push(ctrl.filterCustomer);


                    //ctrl.filters = ctrl.selectedSuppliers;
                }


                function onselectionvalueschanged2() {



                    //ctrl.selectedSuppliers.forEach(function (item) {
                    //    ctrl.filterSupplier.FilterValues.push(item.CarrierAccountID);
                    //});

                    //if (ctrl.filterSupplier.FilterValues.length > 0)
                    //    ctrl.filters.push(ctrl.filterSupplier);

                    //ctrl.filters = ctrl.selectedSuppliers;
                }

                angular.extend(this, {
                    searchZones: searchZones,
                    onSelectionChanged: onSelectionChanged,
                    onselectionvalueschanged: onselectionvalueschanged,
                    onselectionvalueschanged2: onselectionvalueschanged2
                });
               
                
                onLoad();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    post: function ($scope, iElem, iAttrs, ctrl) {
                        $scope.$watch('ctrl.selectedvalues.length', function () {
                            if (iAttrs.onselectionchanged != undefined) {
                                var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onselectionchanged);
                                if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                                    onvaluechangedMethod();
                                }
                            }
                        });
                    }
                }
            },
            templateUrl: function () {
                return "/Client/Modules/Analytics/Directives/vr-generalfilter.html";
            }
        };

        return directiveDefinitionObject;
    }
    
    vrDirectiveObj.$inject = ['AnalyticsService', 'BusinessEntityAPIService_temp', 'ZonesService', 'CarrierAccountConnectionAPIService','CarrierTypeEnum', 'GenericAnalyticDimensionEnum'];
    
    app.directive('vrAnGeneralfilter', vrDirectiveObj);

})(angular, app);