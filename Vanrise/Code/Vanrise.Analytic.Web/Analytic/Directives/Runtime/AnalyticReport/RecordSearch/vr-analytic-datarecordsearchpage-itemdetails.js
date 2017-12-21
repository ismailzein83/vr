(function (app) {

    'use strict';

    DRSearchPageItemDetailsDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function DRSearchPageItemDetailsDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DRSearchPageItemDetailsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Runtime/AnalyticReport/RecordSearch/Templates/DataRecordSearchPageItemDetailsTemplate.html"
        };

        function DRSearchPageItemDetailsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridDrillDownTabsObj;

            var gridWidths;
            var detailWidths;
            var gridAPI;
            var itemDetails;

            var sortColumns;
            var dataRecordTypeAttributes;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var dataRecordStorageLog;

                    if (payload != undefined) {
                        dataRecordStorageLog = payload.dataRecordStorageLog;
                    }

                    $scope.details = dataRecordStorageLog ? dataRecordStorageLog.details : undefined;

                    if ($scope.details != undefined) {
                        for (var x = 0; x < $scope.details.length; x++) {
                            var currentDetail = $scope.details[x];
                            promises.push(extendDetailItemObject(currentDetail, dataRecordStorageLog));
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

            function extendDetailItemObject(detailItem, dataRecordStorageLog) {
                detailItem.detailViewerLoadDeferred = UtilsService.createPromiseDeferred();
                detailItem.onDetailViewerReady = function (api) {
                    detailItem.detailViewerAPI = api;
                    var payload = { detailItem: detailItem, fieldValue: dataRecordStorageLog.FieldValues[detailItem.FieldName] };
                    VRUIUtilsService.callDirectiveLoad(detailItem.detailViewerAPI, payload, detailItem.detailViewerLoadDeferred);
                };
                return detailItem.detailViewerLoadDeferred.promise;
            };
        }
    }

    app.directive('vrAnalyticDatarecordsearchpageItemdetails', DRSearchPageItemDetailsDirective);

})(app);