'use strict';
app.directive('vrSummaryBi', ['BIDataAPIService', 'BIUtilitiesService', 'BIVisualElementService1', 'VRModalService', function (BIDataAPIService, BIUtilitiesService, BIVisualElementService1, VRModalService) {

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
            
            var biSummary = new BISummary(ctrl, ctrl.settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1);
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
     //   console.log(previewmode);
        if (previewmode != 'true') {
            return '<div width="normal"><table class="table  table-striped" ><tr ng-repeat="value in ctrl.dataSource" ><td><vr-label isValue="{{value.description}}">{{value.description}}</vr-label></td><td><vr-label isValue="{{value.value}}">{{value.value}}</vr-label></td></tr></table></div>';
        }
        else
            return '<div  ng-repeat="value in ctrl.dataSource"><vr-label isValue="{{value.description}}">{{value.description}}: {{value.value}}</vr-label></div>';



    }
    function BISummary(ctrl, settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1) {

        var summaryAPI;

        function initializeController() {
            
            ctrl.onSummaryReady = function (api) {
                summaryAPI = api;
                
            }
            if (retrieveDataOnLoad)
                    retrieveData();
            ctrl.measureTypes = settings.MeasureTypes;
            ctrl.dataSource = [];
            
        }

        function defineAPI() {
            var api = {};
            api.retrieveData = retrieveData;
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function retrieveData() {
            return BIDataAPIService.GetMeasureValues1(ctrl.filter.fromDate, ctrl.filter.toDate, settings.MeasureTypes)
                        .then(function (response) {
                          
                            for (var i = 0; i < response.length; i++) {
                                ctrl.dataSource[i] = {
                                    value:response[i],
                                    description: ctrl.measureTypes[i]
                                }
                            }
                           
                           
                        });
        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }




    return directiveDefinitionObject;
}]);

