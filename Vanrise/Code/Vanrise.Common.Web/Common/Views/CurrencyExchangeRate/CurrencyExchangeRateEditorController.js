﻿( function ( appControllers )
{

    "use strict";

    currencyExchangeRateEditorController.$inject = ['$scope', 'VRCommon_CurrencyExchangeRateAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRDateTimeService', 'VRCommon_CurrencyAPIService', '$filter', 'UISettingsService'];

    function currencyExchangeRateEditorController( $scope, VRCommon_CurrencyExchangeRateAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VRDateTimeService, VRCommon_CurrencyAPIService, $filter, UISettingsService )
    {

        var currencySelectorAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var editMode;
        var currencyExchangeRateId;
        var currencyExchangeRateEntity;
        var systemCurrency;
        var isMainCurrency;

        loadParameters();
        defineScope();
        load();

        function loadParameters()
        {
            var parameters = VRNavigationService.getParameters( $scope );
            if ( parameters != undefined && parameters != null )
            {
                currencyExchangeRateId = parameters.currencyExchangeRateId;
                isMainCurrency = parameters.isMainCurrency;
            }
            editMode = $scope.disableControls = currencyExchangeRateId != undefined;
        }

        function defineScope()
        {
            $scope.saveExchangeRate = function ()
            {
                if ( editMode )
                    return updateCurrencyExchangeRate();
                else
                    return insertCurrencyExchangeRate();
            };
            $scope.close = function ()
            {
                $scope.modalContext.closeModal();
            };
            $scope.getRateDescription = function ()
            {
                if ( isMainCurrency ) return;
                var rateConverted = $filter( 'vrtextOrNumber' )( $scope.rate, UISettingsService.getLongPrecision() );
                if ( currencySelectorAPI.getSelectedIds() && $scope.rate && systemCurrency )
                {
                    return "1 " + systemCurrency.Symbol + " equals " + rateConverted + " " + $scope.selectedCurrency.Symbol + ".";
                }
            };
            $scope.validateTodayDateTime = function ()
            {
                if ( $scope.exchangeDate > VRDateTimeService.getNowDateTime() )
                    return "Date cannot be greater than today";
                return null;
            };
            $scope.onCurrencySelectReady = function ( api )
            {
                currencySelectorAPI = api;
                currencyReadyPromiseDeferred.resolve();
            };
        }

        function load()
        {
            $scope.isLoading = true;
            if ( editMode )
            {
                getCurrencyExchangeRate().then( function ()
                {
                    loadAllControls().finally( function ()
                    {
                        currencyExchangeRateEntity = undefined;
                    } );
                } ).catch( function ( error )
                {
                    VRNotificationService.notifyExceptionWithClose( error, $scope );
                    $scope.isLoading = false;
                } );
            }
            else
            {
                loadAllControls();
            }

        }

        function getCurrencyExchangeRate()
        {
            return VRCommon_CurrencyExchangeRateAPIService.GetCurrencyExchangeRate( currencyExchangeRateId ).then( function ( response )
            {
                currencyExchangeRateEntity = response;
            } );
        }
        function loadAllControls()
        {
            return UtilsService.waitMultipleAsyncOperations( [setTitle, loadStaticData, loadCurrencySelector, getSystemCurrency] )
                .catch( function ( error )
                {
                    VRNotificationService.notifyExceptionWithClose( error, $scope );
                } )
                .finally( function ()
                {
                    $scope.isLoading = false;
                } );
        }
        function setTitle()
        {
            if ( editMode && currencyExchangeRateEntity != undefined )
                $scope.title = "Edit Currency Exchange Rate";
            else
                $scope.title = "New Currency Exchange Rate";
        }

        function loadStaticData()
        {
            if ( currencyExchangeRateEntity == undefined )
            {
                $scope.exchangeDate = VRDateTimeService.getNowDateTime();
                return;
            }
            $scope.rate = currencyExchangeRateEntity.Rate;
            $scope.exchangeDate = currencyExchangeRateEntity.ExchangeDate;
        }

        function loadCurrencySelector()
        {
            var currencyLoadPromiseDeferred = UtilsService.createPromiseDeferred();

            currencyReadyPromiseDeferred.promise
                .then( function ()
                {
                    var directivePayload = {
                        selectedIds: currencyExchangeRateEntity != undefined && currencyExchangeRateEntity.CurrencyId || undefined,
                        excludeSystemCurrency : editMode == false
                    };

                    VRUIUtilsService.callDirectiveLoad( currencySelectorAPI, directivePayload, currencyLoadPromiseDeferred );
                } );
            return currencyLoadPromiseDeferred.promise;
        }


        function buildCurrencyExchangeRateObjFromScope()
        {
            var obj = {
                CurrencyExchangeRateId: currencyExchangeRateId,
                CurrencyId: currencySelectorAPI.getSelectedIds(),
                Rate: $scope.rate,
                ExchangeDate: $scope.exchangeDate
            };
            return obj;
        }


        function insertCurrencyExchangeRate()
        {
            $scope.isLoading = true;

            var object = buildCurrencyExchangeRateObjFromScope();

            return VRCommon_CurrencyExchangeRateAPIService.AddCurrencyExchangeRate( object )
                .then( function ( response )
                {
                    if ( VRNotificationService.notifyOnItemAdded( "Currency Exchange Rate", response ) )
                    {
                        if ( $scope.onCurrencyExchangeRateAdded != undefined )
                            $scope.onCurrencyExchangeRateAdded( response.InsertedObject );
                        $scope.modalContext.closeModal();
                    }
                } ).catch( function ( error )
                {
                    VRNotificationService.notifyException( error, $scope );
                } ).finally( function ()
                {
                    $scope.isLoading = false;
                } );
        }

        function updateCurrencyExchangeRate()
        {
            $scope.isLoading = true;
            var object = buildCurrencyExchangeRateObjFromScope();
            VRCommon_CurrencyExchangeRateAPIService.UpdateCurrencyExchangeRate( object )
                .then( function ( response )
                {
                    if ( VRNotificationService.notifyOnItemUpdated( "Currency Exchange Rate", response ) )
                    {
                        if ( $scope.onCurrencyExchangeRateUpdated != undefined )
                            $scope.onCurrencyExchangeRateUpdated( response.UpdatedObject );
                        $scope.modalContext.closeModal();
                    }
                } ).catch( function ( error )
                {
                    VRNotificationService.notifyException( error, $scope );
                } ).finally( function ()
                {
                    $scope.isLoading = false;
                } );
        }

        function getSystemCurrency()
        {
            return VRCommon_CurrencyAPIService.GetSystemCurrency().then( function ( response )
            {
                systemCurrency = response;
            } );
        }
    }

    appControllers.controller( 'VRCommon_CurrencyExchangeRateEditorController', currencyExchangeRateEditorController );
} )( appControllers );
