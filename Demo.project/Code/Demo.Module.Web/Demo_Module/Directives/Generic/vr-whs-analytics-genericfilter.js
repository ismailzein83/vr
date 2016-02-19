"use strict";

app.directive("vrWhsAnalyticsGenericfilter", [ 'UtilsService', 'VRNotificationService', 'WhS_Analytics_GenericAnalyticDimensionEnum', 'VRUIUtilsService','WhS_Analytics_GenericAnalyticMeasureEnum','VRValidationService',
function (UtilsService, VRNotificationService, WhS_Analytics_GenericAnalyticDimensionEnum, VRUIUtilsService, WhS_Analytics_GenericAnalyticMeasureEnum, VRValidationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady:'='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var genericFilter = new GenericFilter($scope, ctrl, $attrs);
            genericFilter.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: function () {
            return "/Client/Modules/Demo_Module/Directives/Generic/Templates/GenericFilterTemplate.html";
        }
    };
    function GenericFilter($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var operatorAccountDirectiveAPI;
        var operatorAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        
        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencyDirectiveAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {


            ctrl.GenericAnalyticDimensionEnum = WhS_Analytics_GenericAnalyticDimensionEnum;   
            ctrl.dimensions = [];

            ctrl.periods = [];
            ctrl.selectedCountries = [];
            ctrl.selectedCustomers = [];
            ctrl.selectedSuppliers = [];
            ctrl.selecteddimensions = [];
            ctrl.measureThresholds = [];

            ctrl.fromdate = new Date();

            ctrl.validateDateTime = function () {
                return VRValidationService.validateTimeRange(ctrl.fromdate, ctrl.todate);
            }

            ctrl.onOperatorAccountDirectiveReady = function (api)
            {
                operatorAccountDirectiveAPI = api;
                operatorAccountReadyPromiseDeferred.resolve();
            }


            ctrl.onCurrencyDirectiveReady = function (api)
            {
                currencyDirectiveAPI = api;
                currencyReadyPromiseDeferred.resolve();
            }

            ctrl.onCountryDirectiveReady = function (api)
            {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            }

           

            ctrl.onSaleZoneDirectiveReady = function (api)
            {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }

            

            ctrl.isrequired = function ()
            {
                return ctrl.periods.length==0;
            }

            defineAPI();
      
        }

        function defineAPI() {
            var api = {};

            api.getData = function ()
            {
                var filterOperator = {
                    Dimension: WhS_Analytics_GenericAnalyticDimensionEnum.Operator.value,
                    FilterValues: operatorAccountDirectiveAPI.getSelectedIds()
                };
                               
                var filterZones = {
                    Dimension: WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value,
                    FilterValues: saleZoneDirectiveAPI.getSelectedIds()
                };

                var selectedfilters = [];

                if (filterOperator.FilterValues != undefined && filterOperator.FilterValues.length > 0)
                    selectedfilters.push(filterOperator);

                if (filterZones.FilterValues != undefined && filterZones.FilterValues.length > 0)
                    selectedfilters.push(filterZones);

                var selectedobject = {
                    selecteddimensions: UtilsService.getPropValuesFromArray(ctrl.selecteddimensions, "value"),
                    selectedfilters: selectedfilters,
                    selectedperiod: ctrl.selectedperiod != undefined ? ctrl.selectedperiod.value : undefined,
                    fromdate: ctrl.fromdate,
                    todate: ctrl.todate,
                    currency: currencyDirectiveAPI != undefined ? currencyDirectiveAPI.getSelectedIds() : undefined
                };
                return selectedobject;
            }

            api.load = function (payload)
            {
                ctrl.isLoadOperators;
                ctrl.isLoadCountries;
                ctrl.isLoadCurrencies;

                if (payload != undefined)
                {
                    if (payload.filters != undefined)
                    {
                        for (var i = 0; i < payload.filters.length; i++)
                        {
                            switch (payload.filters[i])
                            {
                                case WhS_Analytics_GenericAnalyticDimensionEnum.Operator.value: ctrl.isLoadOperators = true; break;
                                case WhS_Analytics_GenericAnalyticDimensionEnum.Zone.value: ctrl.isLoadSellingNumberPlan = true; break;
                                case WhS_Analytics_GenericAnalyticDimensionEnum.Country: ctrl.isLoadCountries = true; break;
                                case WhS_Analytics_GenericAnalyticDimensionEnum.Currency.value: ctrl.isLoadCurrencies = true; break;
                            }
                        }
                    }

                    for (var p in WhS_Analytics_GenericAnalyticDimensionEnum)
                    {
                        if (payload.dimensions != undefined || payload.dimensions.length > 0)
                        {
                            for (var i = 0; i < payload.dimensions.length; i++)
                            {
                                if (WhS_Analytics_GenericAnalyticDimensionEnum[p].value == payload.dimensions[i])
                                    ctrl.dimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);

                            }

                           
                        }

                        if (payload.periods != undefined && payload.periods.length > 0)
                        {
                            for (var i = 0; i < payload.periods.length; i++)
                            {
                                if (WhS_Analytics_GenericAnalyticDimensionEnum[p].value == payload.periods[i])
                                    ctrl.periods.push(WhS_Analytics_GenericAnalyticDimensionEnum[p]);
                            }
                        }
                    }
                    if(ctrl.dimensions.length>0)
                    {
                        ctrl.selecteddimensions.push(WhS_Analytics_GenericAnalyticDimensionEnum.Zone);
                    }
                }

                var promises = [];

                

                function loadOperators()
                {
                    var loadOperatorAccountPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadOperatorAccountPromiseDeferred.promise);
                    operatorAccountReadyPromiseDeferred.promise.then(function ()
                    {
                        VRUIUtilsService.callDirectiveLoad(operatorAccountDirectiveAPI, undefined, loadOperatorAccountPromiseDeferred);
                    });
                }
                               
                function loadCountries()
                {
                    var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadCountryPromiseDeferred.promise);
                    countryReadyPromiseDeferred.promise.then(function ()
                    {
                        VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, undefined, loadCountryPromiseDeferred);
                    });
                }

                function loadCurrencies() {
                    var loadCurrencyPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadCurrencyPromiseDeferred.promise);
                    currencyReadyPromiseDeferred.promise.then(function ()
                    {
                        VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, undefined, loadCurrencyPromiseDeferred);
                    });
                }

                if (ctrl.isLoadOperators)
                    loadOperators();
                if (ctrl.isLoadSuppliers)
                    loadSuppliers();
                if (ctrl.isLoadCountries)
                    loadCountries();
                if (ctrl.isLoadCurrencies)
                    loadCurrencies();
                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
    return directiveDefinitionObject;

}]);