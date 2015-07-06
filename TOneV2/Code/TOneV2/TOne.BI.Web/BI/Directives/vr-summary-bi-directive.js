﻿'use strict';
app.directive('vrSummaryBi', ['UtilsService','BIConfigurationAPIService', 'BIDataAPIService', 'BIUtilitiesService', 'BIVisualElementService1', 'VRModalService', function (UtilsService ,BIConfigurationAPIService,BIDataAPIService, BIUtilitiesService, BIVisualElementService1, VRModalService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            filter: '=',
            previewmode: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);
            
            var biSummary = new BISummary(ctrl, ctrl.settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1, BIConfigurationAPIService, UtilsService);
            biSummary.initializeController();

            biSummary.defineAPI();
            $scope.openReportEntityModal = function (item) {

                BIUtilitiesService.openEntityReport(item.EntityType, item.EntityId, item.EntityName);

            }

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                }
            }
        },
        template: function (element, attrs) {
            return getSummaryTemplate(attrs.previewmode);
        }

    };

    function getSummaryTemplate(previewmode) {
        if (previewmode != 'true') {
            return '<div ng-if="!ctrl.isAllowed"  class="alert alert-danger" role="alert"> <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span> You Don\'t Have Permission To See This Widget..!!</div><div ng-if="ctrl.isAllowed"><div width="normal"><table class="table  table-striped" ><tr ng-repeat="value in ctrl.dataSource" ><td><vr-label isValue="{{value.description.DisplayName}}">{{value.description.DisplayName}}</vr-label></td><td><vr-label isValue="{{value.value}}">{{value.value}}</vr-label></td></tr></table></div></div>';
        }
        else
            return '<div><table class="table  table-striped" ><tr ng-repeat="value in ctrl.measureTypes" ><td><vr-label isValue="{{value}}">{{value.DisplayName}}</vr-label></td></tr></table>';



    }
    function BISummary(ctrl, settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1, BIConfigurationAPIService, UtilsService) {

        var summaryAPI;
        var measures = [];
        function initializeController() {
            UtilsService.waitMultipleAsyncOperations([loadMeasures])
           .then(function () {
               
               if (!BIUtilitiesService.checkPermissions(measures)) {
                   ctrl.isAllowed = false;
                   return;
               }
               ctrl.isAllowed = true;
               ctrl.onSummaryReady = function (api) {
                   summaryAPI = api;

               }
               ctrl.measureTypes = measures;


               if (retrieveDataOnLoad)
                   retrieveData();
               ctrl.measureTypes = measures;
               ctrl.dataSource = [];

               defineAPI();

           })
           .finally(function () {
           }).catch(function (error) {
           });



        
            
        }

        function defineAPI() {
            var api = {};
            api.retrieveData = retrieveData;
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function retrieveData() {
            if (!ctrl.isAllowed)
                return;
            return BIDataAPIService.GetMeasureValues1(ctrl.filter.fromDate, ctrl.filter.toDate, settings.MeasureTypes)
                        .then(function (response) {
                          
                            for (var i = 0; i < response.length; i++) {
                                ctrl.dataSource[i] = {
                                    value: response[i].toFixed(2),
                                    description: ctrl.measureTypes[i]
                                }
                            }
                           
                           
                        });
        }
        function loadMeasures() {
           
            return BIConfigurationAPIService.GetMeasures().then(function (response) {
                for (var i = 0; i < settings.MeasureTypes.length; i++) {
                    var value = UtilsService.getItemByVal(response, settings.MeasureTypes[i], 'Name');
                    if (value != null)
                        measures.push(value);
                }
            });
        }
        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }




    return directiveDefinitionObject;
}]);

