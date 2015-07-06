﻿'use strict';


app.directive('vrDatagridBi', ['UtilsService','BIDataAPIService', 'BIUtilitiesService', 'BIVisualElementService1', 'VRModalService', 'BIConfigurationAPIService', function (UtilsService ,BIDataAPIService, BIUtilitiesService, BIVisualElementService1, VRModalService, BIConfigurationAPIService) {

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

            var biDataGrid = new BIDataGrid(ctrl, ctrl.settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1, BIConfigurationAPIService, UtilsService);
            biDataGrid.initializeController();

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
            return getDataGridTemplate(attrs.previewmode);
        }

    };

    function getDataGridTemplate(previewmode) {
        if (previewmode != 'true') {
            return '<div ng-if="!ctrl.isAllowed"  class="alert alert-danger" role="alert"> <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span> You Don\'t Have Permission To See This Widget..!!</div><div ng-if="ctrl.isAllowed"><vr-datagrid datasource="ctrl.data" on-ready="ctrl.onGridReady" maxheight="300px">'
                                        + '<vr-datagridcolumn ng-show="ctrl.isTopEntities" headertext="ctrl.entityType.description" field="\'EntityName\'" isclickable="\'true\'" \ onclicked="openReportEntityModal"></vr-datagridcolumn>'
                                        + '<vr-datagridcolumn ng-show="ctrl.isDateTimeGroupedData" headertext="\'Time\'" field="\'dateTimeValue\'"></vr-datagridcolumn>'
                                        + '<vr-datagridcolumn ng-repeat="measureType in ctrl.measureTypes" headertext="measureType" field="\'Values[\' + $index + \']\'" type="\'Number\'"></vr-datagridcolumn>'
                                    + '</vr-datagrid></div>';
        }
        else
            return '</br><vr-textbox value="ctrl.settings.OperationType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.entityType.description" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.measureTypes" vr-disabled="true"></vr-textbox>';



    }
    function BIDataGrid(ctrl, settings, retrieveDataOnLoad, BIDataAPIService, BIVisualElementService1, BIConfigurationAPIService, UtilsService) {

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
               ctrl.onGridReady = function (api) {
                   gridAPI = api;
                   if (retrieveDataOnLoad)
                       retrieveData();
               }
               ctrl.entityType = entity;
               ctrl.measureTypes = measures;

               ctrl.data = [];
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
            return BIVisualElementService1.retrieveWidgetData(ctrl, settings)
                        .then(function (response) {
                            if (ctrl.isDateTimeGroupedData)
                                BIUtilitiesService.fillDateTimeProperties(response, ctrl.filter.timeDimensionType.value, ctrl.filter.fromDate, ctrl.filter.toDate, true);
                            refreshDataGrid(response);
                          
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

