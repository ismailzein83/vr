(function (app) {

    'use strict';

    MaxAggregateDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function MaxAggregateDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var maxAggregate = new MaxAggregate($scope, ctrl, $attrs);
                maxAggregate.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/DARecordAggregates/Templates/MaxAggregateTemplate.html"

        };
        function MaxAggregate($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    defineAPI();
                };


            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var context;
                    var recordAggregate;

                    if (payload != undefined) {
                        context = payload.context;
                        recordAggregate = payload.recordAggregate;
                    }

                    if(context == undefined) return;

                    var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    var dataRecordTypeFieldsSelectorPayload = {};
                    if (recordAggregate != undefined) {
                        dataRecordTypeFieldsSelectorPayload.selectedIds = recordAggregate.MaxFieldName;
                    }

                    var _dataRecordTypeId = context.getDataRecordTypeId();
                    if (_dataRecordTypeId) {
                        dataRecordTypeFieldsSelectorPayload.dataRecordTypeId = _dataRecordTypeId;
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                    }
                    else {
                        dataRecordTypeFieldsSelectorLoadDeferred.resolve();
                    }


                    return dataRecordTypeFieldsSelectorLoadDeferred.promise;

                };

                api.getData = function () {

                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.DARecordAggregates.MaxAggregate, Vanrise.Analytic.MainExtensions",
                        MaxFieldName: dataRecordTypeFieldsSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrAnalyticMaxaggregate', MaxAggregateDirective);

})(app);