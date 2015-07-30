'use strict';


app.directive('vrBiDatagrid', ['UtilsService', 'BIAPIService', 'BIUtilitiesService', 'BIVisualElementService', 'VRModalService', 'BIConfigurationAPIService', 'MainService', function (UtilsService, BIAPIService, BIUtilitiesService, BIVisualElementService, VRModalService, BIConfigurationAPIService, MainService) {

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

            var biDataGrid = new BIDataGrid(ctrl, ctrl.settings, retrieveDataOnLoad, BIAPIService, BIVisualElementService, BIConfigurationAPIService, UtilsService);
            biDataGrid.initializeController();

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
            return getDataGridTemplate(attrs.previewmode);
        }

    };

    function getDataGridTemplate(previewmode) {
        if (previewmode != 'true') {
            return '<vr-section title="{{ctrl.title}}"><div ng-if="ctrl.isAllowed==false" ng-class="\'gridpermission\'"><div  style="padding-top:115px;"><div   class="alert alert-danger" role="alert"> <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span> You Don\'t Have Permission, Please Contact Your Administrator..!!</div></div></div><div ng-if="ctrl.isAllowed" vr-loader="ctrl.isGettingData"><vr-datagrid  onexport="ctrl.onexport" datasource="ctrl.data" on-ready="ctrl.onGridReady" maxheight="300px">'
                                        + '<vr-datagridcolumn  ng-show="ctrl.isTopEntities" headertext="ctrl.entityType.description" field="\'EntityName\'"'// isclickable="\'true\'" \onclicked="openReportEntityModal"
                                    +'  ></vr-datagridcolumn>'
                                        + '<vr-datagridcolumn ng-show="ctrl.isDateTimeGroupedData" headertext="\'Time\'" field="\'dateTimeValue\'"></vr-datagridcolumn>'
                                        + '<vr-datagridcolumn ng-repeat="measureType in ctrl.measureTypes" headertext="measureType.DisplayName" field="\'Values[\' + $index + \']\'" type="\'Number\'"></vr-datagridcolumn>'
                                    + '</vr-datagrid></div></vr-section>';
        }
        else
            return '<vr-section title="{{ctrl.title}}"></br><vr-textbox value="ctrl.settings.OperationType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.entityType.description" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.measureTypes" vr-disabled="true"></vr-textbox></vr-section>';



    }
    function BIDataGrid(ctrl, settings, retrieveDataOnLoad, BIAPIService, BIVisualElementService, BIConfigurationAPIService, UtilsService) {

        var gridAPI;
        var measures = [];
        var entity;
        function initializeController() {
            UtilsService.waitMultipleAsyncOperations([loadMeasures, loadEntities])
           .then(function () {
               if (!BIUtilitiesService.checkPermissions(measures)) {
                   ctrl.isAllowed = false;
                   return;
               }
               ctrl.isAllowed = true;
               ctrl.onexport = function () {
                  
                   return BIVisualElementService.exportWidgetData(ctrl, ctrl.settings, ctrl.filter).then(function (response) {

                       return UtilsService.downloadFile(response.data, response.headers);
                   });
               }
               ctrl.entityType = entity;
               ctrl.measureTypes = measures;

               ctrl.data = [];
               ctrl.onGridReady = function (api) {
                   gridAPI = api;
                   defineAPI();
               }
              
            

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
            return BIVisualElementService.retrieveWidgetData(ctrl, settings, filter)
                        .then(function (response) {
                            if (ctrl.isDateTimeGroupedData)
                                BIUtilitiesService.fillDateTimeProperties(response, filter.timeDimensionType.value, filter.fromDate, filter.toDate, true);
                            refreshDataGrid(response);
                          
                        }).finally(function () {
                            ctrl.isGettingData = false;
                        });
        }

        function refreshDataGrid(response) {
            ctrl.data.length = 0;
            gridAPI.addItemsToSource(response);
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
        function loadEntities() {
            return BIConfigurationAPIService.GetEntities().then(function (response) {
                entity = UtilsService.getItemByVal(response, settings.EntityType, 'Name');
            });
        }
        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }




    return directiveDefinitionObject;
}]);

