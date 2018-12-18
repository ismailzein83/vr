'use strict';

app.directive('vrBiDatagrid', ['UtilsService', 'VR_BI_BIAPIService', 'BIUtilitiesService', 'BIVisualElementService', 'VRModalService', 'VR_BI_BIConfigurationAPIService', 'MainService', function (UtilsService, VR_BI_BIAPIService, BIUtilitiesService, BIVisualElementService, VRModalService, VR_BI_BIConfigurationAPIService, MainService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            previewmode: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var biDataGrid = new BIDataGrid(ctrl, $scope);
            biDataGrid.initializeController();

            //$scope.openReportEntityModal = function (item) {

            //    BIUtilitiesService.openEntityReport(item.EntityType, item.EntityId, item.EntityName);

            //}

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) { }
            };
        },
        template: function (element, attrs) {
            return getDataGridTemplate(attrs.previewmode);
        }

    };

    function getDataGridTemplate(previewmode) {
        if (previewmode == undefined) {
            return '<vr-section title="{{ctrl.title}}"><div ng-if="ctrl.isAllowed==false" ng-class="\'gridpermission\'"><div  style="padding-top:115px;"><div   class="alert alert-danger" role="alert"> <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span class="sr-only">Error:</span> You Don\'t Have Permission, Please Contact Your Administrator..!!</div></div></div><div ng-if="ctrl.isAllowed" vr-loader="ctrl.isGettingData"><vr-datagrid  onexport="ctrl.onexport" datasource="ctrl.data" on-ready="ctrl.onGridReady" maxheight="300px">' + '<vr-datagridcolumn  ng-show="ctrl.isTopEntities" ng-repeat="entityType in ctrl.entityType" headertext="entityType.DisplayName" field="\'EntityName[\' + $index + \']\'"' // isclickable="\'true\'" \onclicked="openReportEntityModal"
                + '  columnindex="$index"></vr-datagridcolumn>' + '<vr-datagridcolumn ng-show="ctrl.isDateTimeGroupedData" headertext="\'Time\'" field="\'dateTimeValue\'"></vr-datagridcolumn>' + '<vr-datagridcolumn ng-repeat="measureType in ctrl.measureTypes" headertext="measureType.DisplayName" field="\'Values[\' + $index + \']\'" type="\'Number\'"></vr-datagridcolumn>' + '</vr-datagrid></div></vr-section>';
        } else
            return '<vr-section title="{{ctrl.title}}"></br><vr-textbox value="ctrl.settings.OperationType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.settings.EntityType" vr-disabled="true"></vr-textbox></br><vr-textbox value="ctrl.settings.MeasureTypes" vr-disabled="true"></vr-textbox></vr-section>';

    }

    function BIDataGrid(ctrl, $scope) {
        var gridAPI;
        var gridAPIReadyDeferred = UtilsService.createPromiseDeferred();
        var measures = [];
        var entity = [];

        function initializeController() {
            ctrl.onGridReady = function (api) {
                gridAPI = api;
                gridAPIReadyDeferred.resolve();
            };

            ctrl.onexport = function () {
                var promises = [];

                var exportWidgetDataPromise = BIVisualElementService.exportWidgetData(ctrl, ctrl.settings, ctrl.filter);
                promises.push(exportWidgetDataPromise);

                var downloadFileDeferred = UtilsService.createPromiseDeferred();
                promises.push(downloadFileDeferred.promise);

                exportWidgetDataPromise.then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers).then(function () {
                        downloadFileDeferred.resolve();
                    }).catch(function (error) {
                        downloadFileDeferred.reject(error);
                    });
                });

                return UtilsService.waitMultiplePromises(promises);
            };

            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.retrieveData = retrieveData;

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                    ctrl.title = payload.title;
                    ctrl.settings = payload.settings;
                    ctrl.filter = payload.filter;
                }

                var loadMeasuresAndEntitiesPromise = UtilsService.waitMultipleAsyncOperations([loadMeasures, loadEntities]);
                promises.push(loadMeasuresAndEntitiesPromise);

                var checkPermissionsDeferred = UtilsService.createPromiseDeferred();
                promises.push(checkPermissionsDeferred.promise);

                loadMeasuresAndEntitiesPromise.then(function () {
                    ctrl.data = [];
                    ctrl.entityType = entity;
                    ctrl.measureTypes = measures;

                    if (payload != undefined && !payload.previewMode) {
                        var retrieveDataDeferred = UtilsService.createPromiseDeferred();
                        promises.push(retrieveDataDeferred.promise);

                        BIUtilitiesService.checkPermissions(measures).then(function (isAuthorized) {
                            checkPermissionsDeferred.resolve();

                            if (!isAuthorized) {
                                ctrl.isAllowed = false;
                                retrieveDataDeferred.resolve();
                            }
                            else {
                                ctrl.isAllowed = true;
                                retrieveData(ctrl.filter).then(function () { retrieveDataDeferred.resolve(); }).catch(function (error) { retrieveDataDeferred.reject(error); });
                            }
                        }).catch(function (error) {
                            checkPermissionsDeferred.reject(error);
                        });
                    }
                    else { checkPermissionsDeferred.resolve(); }
                });

                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function retrieveData(filter) {

            if (!ctrl.isAllowed)
                return;
            ctrl.isGettingData = true;
            return BIVisualElementService.retrieveWidgetData(ctrl, ctrl.settings, filter)
                .then(function (response) {
                    if (ctrl.isDateTimeGroupedData)
                        BIUtilitiesService.fillDateTimeProperties(response, filter.timeDimensionType.value, filter.fromDate, filter.toDate, true);
                    return refreshDataGrid(response);

                })
                .finally(function () {
                    ctrl.isGettingData = false;
                });
        }

        function refreshDataGrid(response) {
            ctrl.data.length = 0;
            gridAPI.addItemsToSource(response);
        }

        function loadMeasures() {
            measures.length = 0;
            return VR_BI_BIConfigurationAPIService.GetMeasuresInfo()
                .then(function (response) {
                    for (var i = 0; i < ctrl.settings.MeasureTypes.length; i++) {
                        var value = UtilsService.getItemByVal(response, ctrl.settings.MeasureTypes[i], 'Name');
                        if (value != null)
                            measures.push(value);
                    }
                });
        }

        function loadEntities() {
            entity.length = 0;
            return VR_BI_BIConfigurationAPIService.GetEntitiesInfo()
                .then(function (response) {
                    if (ctrl.settings.EntityType != undefined) {
                        for (var i = 0; i < ctrl.settings.EntityType.length; i++)
                            entity.push(UtilsService.getItemByVal(response, ctrl.settings.EntityType[i], 'Name'));
                    }

                });

        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }

    return directiveDefinitionObject;
}]);