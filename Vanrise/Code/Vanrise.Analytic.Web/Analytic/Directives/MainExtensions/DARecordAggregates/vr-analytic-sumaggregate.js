(function (app) {

    'use strict';

    SumAggregateDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function SumAggregateDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var sumAggregate = new SumAggregate($scope, ctrl, $attrs);
                sumAggregate.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/DARecordAggregates/Templates/SumAggregateTemplate.html"

        };
        function SumAggregate($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var currencyDataRecordTypeFieldsSelectorAPI;
            var currencyDataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onCurrencyDataRecordTypeFieldsSelectorReady = function (api) {
                    currencyDataRecordTypeFieldsSelectorAPI = api;
                    currencyDataRecordTypeFieldsSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var dataRecordTypeId;
                    var recordAggregate;

                    if (payload != undefined) {
                        recordAggregate = payload.recordAggregate;

                        if (payload.context != undefined) {
                            dataRecordTypeId = payload.context.getDataRecordTypeId();
                        }
                    }

                    //Loading DataRecordTypeFields Selector
                    var dataRecordTypeFieldsSelectorLoadPromise = getDataRecordTypeFieldsSelectorLoadPromise();
                    promises.push(dataRecordTypeFieldsSelectorLoadPromise);

                    //Loading Currency DataRecordTypeFields Selector
                    var currencyDataRecordTypeFieldsSelectorLoadPromise = getCurrencyDataRecordTypeFieldsSelectorLoadPromise();
                    promises.push(currencyDataRecordTypeFieldsSelectorLoadPromise);


                    function getDataRecordTypeFieldsSelectorLoadPromise() {
                        var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {

                            var dataRecordTypeFieldsSelectorPayload = { dataRecordTypeId: dataRecordTypeId };
                            if (recordAggregate != undefined) {
                                dataRecordTypeFieldsSelectorPayload.selectedIds = recordAggregate.SumFieldName;
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                        });

                        return dataRecordTypeFieldsSelectorLoadDeferred.promise;
                    }
                    function getCurrencyDataRecordTypeFieldsSelectorLoadPromise() {
                        var currencyDataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        currencyDataRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {

                            var currencyDataRecordTypeFieldsSelectorPayload = { dataRecordTypeId: dataRecordTypeId };
                            if (recordAggregate != undefined) {
                                currencyDataRecordTypeFieldsSelectorPayload.selectedIds = recordAggregate.CurrencySQLColumnName;
                            }
                            VRUIUtilsService.callDirectiveLoad(currencyDataRecordTypeFieldsSelectorAPI, currencyDataRecordTypeFieldsSelectorPayload, currencyDataRecordTypeFieldsSelectorLoadDeferred);
                        });

                        return currencyDataRecordTypeFieldsSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.DARecordAggregates.SumAggregate, Vanrise.Analytic.MainExtensions",
                        SumFieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds(),
                        CurrencySQLColumnName: currencyDataRecordTypeFieldsSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticSumaggregate', SumAggregateDirective);

})(app);