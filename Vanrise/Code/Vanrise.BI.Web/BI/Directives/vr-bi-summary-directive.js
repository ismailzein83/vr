'use strict';
app.directive('vrBiSummary', ['UtilsService', 'BIConfigurationAPIService', 'BIAPIService', 'BIUtilitiesService', 'BIVisualElementService', 'VRModalService', function (UtilsService, BIConfigurationAPIService, BIAPIService, BIUtilitiesService, BIVisualElementService, VRModalService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            settings: '=',
            filter: '=',
            title: '=',
            previewmode: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var biSummary = new BISummary(ctrl, ctrl.settings, retrieveDataOnLoad, BIAPIService, BIVisualElementService, BIConfigurationAPIService, UtilsService);
            biSummary.initializeController();


            //$scope.openReportEntityModal = function (item) {

            //    BIUtilitiesService.openEntityReport(item.EntityType, item.EntityId, item.EntityName);

            //}

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
            return '<vr-section title="{{ctrl.title}}">'
                    + '<div ng-if="ctrl.isAllowed==false"  class="alert alert-danger" role="alert">'
                        + '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>'
                        + '<span class="sr-only">Error:</span> You Don\'t Have Permission, Please Contact Your Administrator..!!'
                    + '</div>'
                    + '<div ng-if="ctrl.isAllowed" vr-loader="ctrl.isGettingData">'
                        + '<div width="normal">'
                            + '<div class="circle-div" ng-repeat="value in ctrl.dataSource" ng-class="\'circle-bg-\'+ $index % 4" >'
                                + '<div class="circle-value" >'
                                  + '<vr-label isValue="{{value.value}}">{{value.value}}</vr-label>'
                                + '</div>'
                                +'<div class="circle-unit" >'
                                  + '<vr-label isValue="{{value.value}}">{{value.description.Unit}}</vr-label>'
                                + '</div>'
                                + '<div  class="circle-label" >'
                                 + '<vr-label isValue="{{value.description.DisplayName}}">{{value.description.DisplayName}}</vr-label>'
                                + '</div>'
                            + '</div>'
                        + '</div>'
                    + '</div>'
                + '</vr-section>'
        }
        else
            //  return '<vr-section title="{{ctrl.title}}"><div><table class="table  table-striped" ><tr ng-repeat="value in ctrl.measureTypes" ><td><vr-label isValue="{{value}}">{{value.DisplayName}}</vr-label></td></tr></table></vr-section>';
            return '<vr-section title="{{ctrl.title}}"><div><div class="circle-div" ng-repeat="value in ctrl.measureTypes" ng-class="\'circle-bg-\'+ $index % 4"  ><div class="circle-value" ></div><div  class="circle-label" ><vr-label isValue="{{value}}">{{value.DisplayName}}</vr-label</div></vr-section>';



    }
    function BISummary(ctrl, settings, retrieveDataOnLoad, BIAPIService, BIVisualElementService, BIConfigurationAPIService, UtilsService) {

        var summaryAPI;
        var measures = [];
        var units = [];
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

        function retrieveData(filter) {
            if (!ctrl.isAllowed)
                return;
            ctrl.isGettingData = true;
            return BIAPIService.GetMeasureValues1(filter.fromDate, filter.toDate, settings.MeasureTypes)
                        .then(function (response) {
                           
                            for (var i = 0; i < response.length; i++) {
                                ctrl.dataSource[i] = {
                                    value: response[i].toFixed(2),
                                    description: ctrl.measureTypes[i],
                                   
                                }
                            }
                        }).finally(function () {
                            ctrl.isGettingData = false;
                        });
        }
        function loadMeasures() {

            return BIConfigurationAPIService.GetMeasures().then(function (response) {
                
                for (var i = 0; i < settings.MeasureTypes.length; i++) {
                    var value = UtilsService.getItemByVal(response, settings.MeasureTypes[i], 'Name');
                    
                    if (value != null)
                    {

                        if (value.Unit === "Currency")
                            BIConfigurationAPIService.GetSystemCurrency().then(function (response) {
                                value.Unit = response;
                                measures.push(value);
                            });
                        else
                            measures.push(value);
                    }
                       
                }
            });
        }
        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }




    return directiveDefinitionObject;
}]);

