(function (app) {

    'use strict';

    PermanentfilterRecordfilter.$inject = ['VRUIUtilsService', 'UtilsService', 'VR_Analytic_MeasureStyleRuleAPIService', 'VR_Analytic_AnalyticTableAPIService', 'VR_Analytic_AnalyticTypeEnum', 'VR_Analytic_AnalyticItemConfigAPIService'];

    function PermanentfilterRecordfilter(VRUIUtilsService, UtilsService, VR_Analytic_MeasureStyleRuleAPIService, VR_Analytic_AnalyticTableAPIService, VR_Analytic_AnalyticTypeEnum, VR_Analytic_AnalyticItemConfigAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var permanentFilterRecordFilter = new PermanentFilterRecordFilter($scope, ctrl, $attrs);
                permanentFilterRecordFilter.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/PermanentFilter/MainExtensions/Templates/PermanentfilterRecordfilter.html"
        };

        function PermanentFilterRecordFilter($scope, ctrl, $attrs) {

            this.initializeController = initializeController;
            var analyticTableId;
            var dimensions = [];
            var permanentFilterRecordFilterGroup;
            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                    defineAPI();
                };
            }
            function loadRecordFilterDirective() {
                var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                recordFilterDirectiveReadyDeferred.promise.then(function () {

                    var recordFilterDirectivePayload = {
                        context: { getFields: function () { return dimensions; } },
                        FilterGroup: permanentFilterRecordFilterGroup
                    };
                    VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);

                });
                return recordFilterDirectiveLoadDeferred.promise;
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        analyticTableId = payload.analyticTableId;
                        permanentFilterRecordFilterGroup = payload.settings != undefined ? payload.settings.RecordFilterGroup : undefined;
                    } 
                    var rootPromiseNode = {promises:[]};

                    if (analyticTableId != undefined) {
                        var input = {
                            TableIds: [analyticTableId],
                            ItemType: VR_Analytic_AnalyticTypeEnum.Dimension.value,
                        };
                      
                        var analyticItemConfigPromise = VR_Analytic_AnalyticItemConfigAPIService.GetAnalyticItemConfigs(input).then(function (response) {
                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++) {
                                    var dimensionData = response[i];
                                    var dimension = {
                                        Type: dimensionData.Config.FieldType,
                                        FieldName: dimensionData.Name,
                                        FieldTitle: dimensionData.Title,
                                    };
                                    dimensions.push(dimension);
                                };
                            }
                        });
                        rootPromiseNode.promises.push(analyticItemConfigPromise);
                        rootPromiseNode.getChildNode = function () {
                            return { promises: [loadRecordFilterDirective()] };
                        };
                    }
                    
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type:"Vanrise.Analytic.Entities.FilterGroupAnalyticTablePermanentFilter,Vanrise.Analytic.Entities",
                        RecordFilterGroup:recordFilterDirectiveAPI.getData().filterObj
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
      
            
        }
    }

    app.directive('vrAnalyticPermanentfilterRecordfilter', PermanentfilterRecordfilter);

})(app);