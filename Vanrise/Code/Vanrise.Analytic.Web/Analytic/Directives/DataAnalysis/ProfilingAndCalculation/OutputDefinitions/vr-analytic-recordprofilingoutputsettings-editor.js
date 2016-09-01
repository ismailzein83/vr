
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

        function initializeController() {
            var promises = [recordFilterDirectiveReadyDeferred.promise];

            $scope.scopeModel = {};

            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
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
                var filterGroup;

                if(payload != undefined){
                    context = payload.context
                    filterGroup = payload.dataAnalysisItemDefinitionSettings.RecordFilter;
                }

                //Loading Record Filter Directive
                var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                var recordFilterDirectivePayload = {
                    context: buildContext(),
                    FilterGroup: filterGroup
                };
                VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                promises.push(recordFilterDirectiveLoadDeferred.promise);


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
                function buildContext() {
                    return context;
                }

                return UtilsService.waitMultiplePromises(promises);

            };

            api.getData = function () {
                var data = {
                    $type: "Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions.RecordProfilingOutputSettings, Vanrise.Analytic.Entities",
                    RecordFilter: recordFilterDirectiveAPI.getData().filterObj
                }
                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);
