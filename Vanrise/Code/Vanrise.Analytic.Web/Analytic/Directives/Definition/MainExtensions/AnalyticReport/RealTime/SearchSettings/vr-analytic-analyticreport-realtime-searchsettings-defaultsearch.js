(function (app) {

    'use strict';

    AnalyticreportRealtimeDefaultsearch.$inject = ["UtilsService", 'VRUIUtilsService','VRLocalizationService'];

    function AnalyticreportRealtimeDefaultsearch(UtilsService, VRUIUtilsService, VRLocalizationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '=',
                customvalidate: '=',
                type: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var defaultRealTimeReportSearch = new DefaultRealTimeReportSearch($scope, ctrl, $attrs);
                defaultRealTimeReportSearch.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/MainExtensions/AnalyticReport/RealTime/SearchSettings/Templates/DefaultRealTimeReportSearchTemplate.html"

        };
        function DefaultRealTimeReportSearch($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var filterDimensionSelectorAPI;
            var filterDimensionReadyDeferred = UtilsService.createPromiseDeferred();
            var localizationTextResourceSelectorAPI;
            var localizationTextResourceSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.timeInterval = 15;
                $scope.scopeModel.filterDimensions = [];
                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                $scope.scopeModel.onFilterDimensionSelectorDirectiveReady = function (api) {
                    filterDimensionSelectorAPI = api;
                    filterDimensionReadyDeferred.resolve();
                };

                $scope.scopeModel.onLocalizationTextResourceSelectorReady = function (api) {
                    localizationTextResourceSelectorAPI = api;
                    localizationTextResourceSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectFilterDimensionItem = function (dimension) {
                    var dataItem = {
                        AnalyticItemConfigId: dimension.AnalyticItemConfigId,
                        Title: dimension.Title,
                        Name: dimension.Name,
                        IsRequired: false,
                        TitleResourceKey: dimension.TitleResourceKey
                    };
                    dataItem.onTextResourceSelectorReady = function (api) {
                        dataItem.textResourceSeletorAPI = api;
                        var setLoader = function (value) { dataItem.isDimensionTextResourceSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                    };
                    $scope.scopeModel.filterDimensions.push(dataItem);
                };

                $scope.scopeModel.onDeselectFilterDimensionItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                };

                $scope.scopeModel.removeFilterDimension = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedFilterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.selectedFilterDimensions.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.filterDimensions, dataItem.Name, 'Name');
                    $scope.scopeModel.filterDimensions.splice(datasourceIndex, 1);
                };

                defineAPI();
            }

     
            function addSelectedFilterDimension(gridDimension) {

                var textResourcePayload;

                var dataItem = {};
                if (gridDimension.payload != undefined) {
                    dataItem.Name = gridDimension.payload.DimensionName;
                    dataItem.Title = gridDimension.payload.Title;
                    dataItem.IsRequired = gridDimension.IsRequired;
                    textResourcePayload = { selectedValue: gridDimension.payload.TitleResourceKey };

                }
              
                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    gridDimension.textResourceReadyPromiseDeferred.resolve();
                };

                gridDimension.textResourceReadyPromiseDeferred.promise
                    .then(function () {
                        VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, gridDimension.textResourceLoadPromiseDeferred);
                    });

                $scope.scopeModel.filterDimensions.push(dataItem);
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.tableIds != undefined) {
                        var promises = [];
                        var tableIds = payload.tableIds;
                        var selectedFilterIds;
                        if (payload.searchSettings != undefined)
                        {
                            $scope.scopeModel.timeInterval = payload.searchSettings.TimeIntervalInMin;
                            if (payload.searchSettings.Filters != undefined && payload.searchSettings.Filters.length > 0) {

                                selectedFilterIds = [];
                                for (var i = 0 ; i < payload.searchSettings.Filters.length; i++) {
                                    var filterDimension = payload.searchSettings.Filters[i];
                                    var dimensionGridField = {
                                        payload: filterDimension,
                                        textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                    };
                                    selectedFilterIds.push(filterDimension.DimensionName);
                                    if ($scope.scopeModel.isLocalizationEnabled)
                                        promises.push(dimensionGridField.textResourceLoadPromiseDeferred.promise);
                                    addSelectedFilterDimension(dimensionGridField);
                                }
                            }
                        }
                        var loadFilterDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                        filterDimensionReadyDeferred.promise.then(function () {
                            var payloadFilterDirective = {
                                filter: { TableIds: tableIds, HideIsRequiredFromParent: true },
                                selectedIds: selectedFilterIds
                            };

                            VRUIUtilsService.callDirectiveLoad(filterDimensionSelectorAPI, payloadFilterDirective, loadFilterDirectivePromiseDeferred);
                        });
                        promises.push(loadFilterDirectivePromiseDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var filterDimensions;
                    if($scope.scopeModel.filterDimensions  !=undefined && $scope.scopeModel.filterDimensions.length>0)
                    {
                        filterDimensions=[];
                        for(var i=0; i<$scope.scopeModel.filterDimensions.length; i++)
                        {
                            var filterDimension = $scope.scopeModel.filterDimensions[i];
                            filterDimensions.push({
                                DimensionName:filterDimension.Name,
                                Title: filterDimension.Title,
                                IsRequired: filterDimension.IsRequired,
                                TitleResourceKey: filterDimension.textResourceSeletorAPI != undefined ? filterDimension.textResourceSeletorAPI.getSelectedValues() : undefined
                            });
                        }
                    }
                    var data = {
                        $type: "Vanrise.Analytic.MainExtensions.RealTimeReport.SearchSettings.DefaultRealTimeReportSearch, Vanrise.Analytic.MainExtensions ",
                        Filters: filterDimensions,
                        TimeIntervalInMin: $scope.scopeModel.timeInterval
                    };
                    return data;
                }
            }
        }
    }

    app.directive('vrAnalyticAnalyticreportRealtimeSearchsettingsDefaultsearch', AnalyticreportRealtimeDefaultsearch);

})(app);