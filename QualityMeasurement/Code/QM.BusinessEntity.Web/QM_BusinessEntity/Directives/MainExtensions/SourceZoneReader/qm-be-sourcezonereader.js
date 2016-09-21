"use strict";

app.directive("qmBeSourcezonereader", ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'QM_BE_ZoneAPIService', function (UtilsService, VRUIUtilsService, VRNotificationService, QM_BE_ZoneAPIService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: function (element, attrs) {

            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {

        return "/Client/Modules/QM_BusinessEntity/Directives/MainExtensions/SourceZoneReader/Templates/SourceZoneReaderTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        
        

        var sourceTypeDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countrySourceTypeDirectiveAPI;
        var countrySourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            $scope.sourceTypeTemplates = [];

            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTypeDirectiveAPI = api;
                sourceDirectiveReadyPromiseDeferred.resolve();
            }
            $scope.onSourceCountryDirectiveReady = function (api) {                
                countrySourceTypeDirectiveAPI = api;
                countrySourceDirectiveReadyPromiseDeferred.resolve();
            }

            var api = {};

            api.getData = function () {               
                var schedulerTaskAction;
                if ($scope.selectedSourceTypeTemplate != undefined) {
                    if (sourceTypeDirectiveAPI != undefined) {
                        schedulerTaskAction = {};
                        schedulerTaskAction.$type = "QM.BusinessEntity.Business.ZoneSyncTaskActionArgument, QM.BusinessEntity.Business",
                        schedulerTaskAction.SourceZoneReader = sourceTypeDirectiveAPI.getData();
                        schedulerTaskAction.SourceZoneReader.ConfigId = $scope.selectedSourceTypeTemplate.ExtensionConfigurationId;
                        if (countrySourceTypeDirectiveAPI != undefined) {
                            schedulerTaskAction.SourceZoneReader.CountryReader = countrySourceTypeDirectiveAPI.getData();
                        }
                    }
                    if (countrySourceTypeDirectiveAPI != undefined) {
                        schedulerTaskAction.SourceCountryReader = countrySourceTypeDirectiveAPI.getData();
                    }

                }
                return schedulerTaskAction;
            };


            api.load = function (payload) {
                var promises = [];
                var zoneSourceTemplatesLoad = QM_BE_ZoneAPIService.GetZoneSourceTemplates().then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.sourceTypeTemplates.push(item);
                    });
                    var sourceConfigId;
                    if (payload != undefined && payload.data != undefined && payload.data.SourceZoneReader != undefined)
                        sourceConfigId = payload.data.SourceZoneReader.ConfigId;
                    if (sourceConfigId != undefined)
                        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "ExtensionConfigurationId");

                });
                promises.push(zoneSourceTemplatesLoad);

                if (payload != undefined && payload.data != undefined && payload.data.SourceZoneReader != undefined) {
                    var loadZoneSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
                    sourceDirectiveReadyPromiseDeferred.promise.then(function () {
                        var obj;
                        if (payload != undefined && payload.data != undefined && payload.data.SourceZoneReader != undefined)
                            obj = {
                                connectionString: payload.data.SourceZoneReader.ConnectionString,
                                zoneNames: payload.data.SourceZoneReader.ZoneNames
                            };
                        VRUIUtilsService.callDirectiveLoad(sourceTypeDirectiveAPI, obj, loadZoneSourceTemplatePromiseDeferred);
                    });
                    promises.push(loadZoneSourceTemplatePromiseDeferred.promise);
                }
                
                var loadCountrySourcePromiseDeferred = UtilsService.createPromiseDeferred();
                countrySourceDirectiveReadyPromiseDeferred.promise.then(function () {
                    var obj ;
                    if (payload != undefined && payload.data != undefined &&  payload.data.SourceCountryReader != undefined)
                        obj = {
                            connectionString: payload.data.SourceCountryReader.ConnectionString,
                            sourceConfigId: payload.data.SourceCountryReader.ConfigId
                        };
                    VRUIUtilsService.callDirectiveLoad(countrySourceTypeDirectiveAPI, obj, loadCountrySourcePromiseDeferred);
                });
                promises.push(loadCountrySourcePromiseDeferred.promise);


                return UtilsService.waitMultiplePromises(promises);
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
