'use strict';

app.directive( 'vrCommonCurrencySettingsEditor', ['UtilsService', 'VRUIUtilsService', 'VRCommon_CurrencyExchangeRateAPIService',
    function ( UtilsService, VRUIUtilsService, VRCommon_CurrencyExchangeRateAPIService )
    {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ( $scope, $element, $attrs )
            {
                var ctrl = this;
                var ctor = new settingEditorCtor( ctrl, $scope, $attrs );
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function ( element, attrs )
            {

            },
            templateUrl: "/Client/Modules/Common/Directives/Currency/Templates/CurrencySettings.html"
        };

        function settingEditorCtor( ctrl, $scope, $attrs )
        {
            var currencySelectorAPI;
            var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var currencyPayload;
            $scope.showChangeCurrencyWarning = false;
            $scope.onCurrencySelectReady = function ( api )
            {
                currencySelectorAPI = api;
                currencyReadyPromiseDeferred.resolve();
            };
            $scope.onSystemCurrencySelectionChange = function ()
            {
                if ( currencySelectorAPI.getSelectedIds() != undefined && currencyPayload != undefined && currencyPayload.selectedIds != undefined && currencySelectorAPI.getSelectedIds() != currencyPayload.selectedIds )
                {
                    VRCommon_CurrencyExchangeRateAPIService.GetAllCurrenciesExchangeRate().then( function ( response )
                    {
                        if ( response.length > 0 ) $scope.showChangeCurrencyWarning = true;
                    } );
                }
                else
                    $scope.showChangeCurrencyWarning = false;
            };
            function initializeController()
            {
                currencyReadyPromiseDeferred.promise.then( function ()
                {
                    defineAPI();
                } );
            }

            function defineAPI()
            {
                var api = {};

                api.load = function ( payload )
                {
                    currencyPayload = undefined;
                    var currencyLoadPromiseDeferred = UtilsService.createPromiseDeferred();


                    if ( payload != undefined && payload.data != undefined )
                    {
                        currencyPayload = { selectedIds: payload.data.CurrencyId };
                    }
                    VRUIUtilsService.callDirectiveLoad( currencySelectorAPI, currencyPayload, currencyLoadPromiseDeferred );

                    return currencyLoadPromiseDeferred.promise;
                };

                api.getData = function ()
                {
                    return {
                        $type: "Vanrise.Entities.CurrencySettingData, Vanrise.Entities",
                        CurrencyId: currencySelectorAPI.getSelectedIds()
                    };
                };

                if ( ctrl.onReady != null )
                    ctrl.onReady( api );
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }] );