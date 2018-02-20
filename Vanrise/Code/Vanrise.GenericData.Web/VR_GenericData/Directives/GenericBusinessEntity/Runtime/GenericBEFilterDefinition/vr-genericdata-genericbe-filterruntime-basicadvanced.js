(function (app) {

    'use strict';

    BasicAdvancedFilterRuntimeSettingsDirective.$inject = ['UtilsService', 'VRUIUtilsService','VR_GenericData_RecordQueryLogicalOperatorEnum'];

    function BasicAdvancedFilterRuntimeSettingsDirective(UtilsService, VRUIUtilsService, VR_GenericData_RecordQueryLogicalOperatorEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BasicAdvancedFilterRuntimeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/BasicAdvancedFilterRuntimeTemplate.html"
        };


        function BasicAdvancedFilterRuntimeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeId;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    $scope.scopeModel.filters = [];
                    if (payload != undefined) {
                         dataRecordTypeId = payload.dataRecordTypeId;
                        var settings = payload.settings;
                        if(settings != undefined)
                        {
                            if(settings.Filters != undefined)
                            {
                                for (var i = 0; i < settings.Filters.length; i++) {
                                    var filter = settings.Filters[i];
                                    var filterItem = {
                                        payload:filter,
                                        readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                        loadPromisedeferred: UtilsService.createPromiseDeferred(),
                                    };
                                    promises.push(filterItem.loadPromisedeferred.promise);
                                    addFilterAPI(filterItem)
                                }
                            }

                        }
                        function addFilterAPI(filterItem) {
                            var filter = {
                                editor: filterItem.payload.FilterSettings.RuntimeEditor,
                                showInBasic: filterItem.payload.ShowInBasic,
                            };
                            filter.onFilterReady = function (api) {
                                filter.filterAPI = api;
                                filterItem.readyPromiseDeferred.resolve();
                            };

                            filterItem.readyPromiseDeferred.promise.then(function () {
                                var filterDirectivePayload = {
                                    settings: filterItem.payload.FilterSettings,
                                    dataRecordTypeId: dataRecordTypeId
                                };
                                VRUIUtilsService.callDirectiveLoad(filter.filterAPI, filterDirectivePayload, filterItem.loadPromisedeferred);
                            });
                            $scope.scopeModel.filters.push(filter);
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function (filterObject) {
                    var filterData = {};
                    var recordFilters = [];
                    var filters = [];

                    for(var i=0;i<$scope.scopeModel.filters.length;i++)
                    {
                        var filter = $scope.scopeModel.filters[i];
                        var data = filter.filterAPI.getData();
                        if (data != undefined) {
                            if (data.RecordFilter != undefined)
                                recordFilters.push(data.RecordFilter);
                            if (data.FromTime != undefined)
                                filterData.FromTime = data.FromTime;
                            if (data.ToTime != undefined)
                                filterData.ToTime = data.ToTime;
                            if (data.Filters != undefined)
                            {
                                for (var j = 0; j < data.Filters.length; j++) {
                                    filters.push(data.Filters[j]);
                                }
                            }
                        }
                    }
                    filterData.RecordFilter = {
                        $type: "Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities",
                        LogicalOperator: VR_GenericData_RecordQueryLogicalOperatorEnum.And.value,
                        Filters: recordFilters
                    };
                    filterData.Filters = filters;

                    return filterData;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterruntimeBasicadvanced', BasicAdvancedFilterRuntimeSettingsDirective);

})(app);