
'use strict';
                
app.directive('vrAnalyticRecordprofilingoutputsettingsEditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) { 
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var recordProfilingOutputSettingsEditor = new RecordProfilingOutputSettingsEditor($scope, ctrl, $attrs);
            recordProfilingOutputSettingsEditor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/OutputDefinitions/Templates/RecordProfilingOutputSettingsEditorTemplate.html'
    };

    function RecordProfilingOutputSettingsEditor($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var dataRecordTypeFieldsSelectorAPI;
        var dataRecordTypeFieldsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var aggregationFieldsDirectiveAPI;
        var aggregationFieldsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        function initializeController() {
            var promises = [recordFilterDirectiveReadyDeferred.promise];

            $scope.scopeModel = {};

            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onDataRecordTypeFieldsSelectorReady = function (api) {
                dataRecordTypeFieldsSelectorAPI = api;
                dataRecordTypeFieldsSelectorReadyDeferred.resolve();
            }

            $scope.scopeModel.onAggregationFieldsDirectiveReady = function (api) {
                aggregationFieldsDirectiveAPI = api;
                aggregationFieldsDirectiveReadyDeferred.resolve();
            };

            UtilsService.waitMultiplePromises(promises).then(function () {
                defineAPI();
            })
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                console.log(payload);

                var promises = [];

                var context;
                var recordFilter;
                var groupingFields;
                var aggregationFields;

                if(payload != undefined){
                    context = payload.context

                    if (payload.dataAnalysisItemDefinitionSettings != undefined) {
                        recordFilter = payload.dataAnalysisItemDefinitionSettings.RecordFilter;
                        groupingFields = payload.dataAnalysisItemDefinitionSettings.GroupingFields;
                        aggregationFields = payload.dataAnalysisItemDefinitionSettings.AggregationFields;
                    }
                }

                //Loading Record Filter Directive
                var recordFilterDirectiveLoadPromise = getRecordFilterDirectiveLoadPromise();
                promises.push(recordFilterDirectiveLoadPromise);

                //Loading Data Record Type Fields Selector
                var dataRecordTypeFieldsSelectorLoadPromise = getDataRecordTypeFieldsSelectorLoadPromise();
                promises.push(dataRecordTypeFieldsSelectorLoadPromise);

                //Loading Aggregation Fields Directive
                var AggregationFieldsDirectiveLoadPromise = getAggregationFieldsDirectiveLoadPromise();
                promises.push(AggregationFieldsDirectiveLoadPromise);


                function getRecordFilterDirectiveLoadPromise() {
                    var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    recordFilterDirectiveReadyDeferred.promise.then(function () {
                        var recordFilterDirectivePayload = {};
                        recordFilterDirectivePayload.context = buildContext();
                        if(recordFilter != undefined){
                            recordFilterDirectivePayload.FilterGroup = recordFilter;
                        }

                        VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                    })

                    return recordFilterDirectiveLoadDeferred.promise;
                }
                function getDataRecordTypeFieldsSelectorLoadPromise() {
                    var dataRecordTypeFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    dataRecordTypeFieldsSelectorReadyDeferred.promise.then(function () {
                        var dataRecordTypeFieldsSelectorPayload = {};
                        dataRecordTypeFieldsSelectorPayload.dataRecordTypeId = context.getDataRecordTypeId();
                        if (groupingFields != undefined) {
                            dataRecordTypeFieldsSelectorPayload.selectedIds = buildDataRecordTypeFieldsSelectorPayload();
                        }

                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, dataRecordTypeFieldsSelectorPayload, dataRecordTypeFieldsSelectorLoadDeferred);
                    });
                    
                    return dataRecordTypeFieldsSelectorLoadDeferred.promise;
                }
                function getAggregationFieldsDirectiveLoadPromise() {
                    var aggregationFieldsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    
                    aggregationFieldsDirectiveReadyDeferred.promise.then(function () {
                        var aggregationFieldsDirectivePayload = {}
                        aggregationFieldsDirectivePayload.context = buildContext();
                        aggregationFieldsDirectivePayload.aggregationFields = aggregationFields;

                        VRUIUtilsService.callDirectiveLoad(aggregationFieldsDirectiveAPI, aggregationFieldsDirectivePayload, aggregationFieldsDirectiveLoadDeferred);
                    })

                    return aggregationFieldsDirectiveLoadDeferred.promise;
                }

                function buildContext() {
                    return context;
                }
                function buildDataRecordTypeFieldsSelectorPayload() {
                    var array = [];

                    if (groupingFields != undefined) {
                        for (var i = 0; i < groupingFields.length; i++)
                            array.push(groupingFields[i].FieldName);
                    }
                    return array;
                }
                function virtual() {

                    //var promises = [];

                    //var serviceTypeId;
                    //var chargingPolicy;
                    //var chargingPolicyDefinitionSettings;
                    //if (payload != undefined) {

                    //    serviceTypeId = payload.serviceTypeId;
                    //    chargingPolicy = payload.chargingPolicy;
                    //}

                    //var getChargingPolicyDefinitionSettingsPromise = getChargingPolicyDefinitionSettings();
                    //promises.push(getChargingPolicyDefinitionSettingsPromise);

                    //var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                    //promises.push(directiveLoadDeferred.promise);

                    //UtilsService.waitMultiplePromises([getChargingPolicyDefinitionSettingsPromise, directiveReadyDeferred.promise]).then(function () {
                    //    var directivePayload = {
                    //        definitionSettings: chargingPolicyDefinitionSettings,
                    //        settings: (chargingPolicy != undefined) ? chargingPolicy.Settings : undefined
                    //    };
                    //    VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                    //});

                    //function getChargingPolicyDefinitionSettings() {
                    //    return Retail_BE_ServiceTypeAPIService.GetServiceTypeChargingPolicyDefinitionSettings(serviceTypeId).then(function (response) {
                    //        chargingPolicyDefinitionSettings = response;
                    //        if (response != null) {
                    //            $scope.scopeModel.directiveEditor = chargingPolicyDefinitionSettings.ChargingPolicyEditor;
                    //        }
                    //    });
                    //}

                    //return UtilsService.waitMultiplePromises(promises);
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {

                console.log(aggregationFieldsDirectiveAPI.getData());

                var data = {
                    $type: "Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions.RecordProfilingOutputSettings, Vanrise.Analytic.Entities",
                    RecordFilter: recordFilterDirectiveAPI.getData().filterObj,
                    GroupingFields: buildListOfDAProfCalcGroupingField(),
                    AggregationFields: aggregationFieldsDirectiveAPI.getData()
                }

                function buildListOfDAProfCalcGroupingField() {
                    var array = [];

                    var dataRecordTypeFieldsArray = dataRecordTypeFieldsSelectorAPI.getSelectedIds();
                    for (var i = 0 ; i < dataRecordTypeFieldsArray.length; i++) {
                        var daProfCalcGroupingField = {
                            FieldName: dataRecordTypeFieldsArray[i]
                        }
                        array.push(daProfCalcGroupingField);
                    }
                    return array;
                }

                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
