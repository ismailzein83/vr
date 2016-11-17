'use strict';
app.directive('vrBiSummary', ['UtilsService', 'VR_BI_BIConfigurationAPIService', 'VR_BI_BIAPIService', 'BIUtilitiesService', 'BIVisualElementService', 'VRModalService', function (UtilsService, VR_BI_BIConfigurationAPIService, VR_BI_BIAPIService, BIUtilitiesService, BIVisualElementService, VRModalService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            previewmode: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var biSummary = new BISummary(ctrl, $scope);
            biSummary.initializeController();

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
            return getSummaryTemplate(attrs.previewmode);
        }

    };

    function getSummaryTemplate(previewmode) {
        if (previewmode == undefined) {
            return '<vr-section title="{{ctrl.title}}"><div ng-if="ctrl.isAllowed==false"  class="alert alert-danger" role="alert"><span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>'
                    + '<span class="sr-only">Error:</span> You Don\'t Have Permission, Please Contact Your Administrator..!!</div><div ng-if="ctrl.isAllowed" vr-loader="ctrl.isGettingData">'
                    + '<div width="normal"><div class="circle-div" ng-repeat="value in ctrl.dataSource" ng-class="\'circle-bg-\'+ $index % 4" ><div  class="circle-label" >'
                    + '<vr-label isValue="{{value.description.DisplayName}}">{{value.description.DisplayName}}</vr-label></div><div class="circle-value" >'
                    + '<vr-label isValue="{{value.value}}">{{value.value | number:2}}</vr-label></div><div class="circle-unit" ><vr-label isValue="{{value.value}}" ng-show="value.description.Unit.length!=0">({{value.description.Unit}})</vr-label>'
                    + '<vr-label  ng-show="value.description.Unit.length==0"></vr-label></div></div></div></div></vr-section>';
        } else
            return '<vr-section title="{{ctrl.title}}"><div><div class="circle-div" ng-repeat="value in ctrl.measureTypes" ng-class="\'circle-bg-\'+ $index % 4"  ><div class="circle-value" ></div><div  class="circle-label" ><vr-label isValue="{{value}}">{{value.DisplayName}}</vr-label</div></vr-section>';

    }

    function BISummary(ctrl, $scope) {

        var summaryAPI;
        var measures = [];
        var units = [];

        function initializeController() {
            ctrl.dataSource = [];
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                    ctrl.title = payload.title;
                    ctrl.settings = payload.settings;
                    ctrl.filter = payload.filter;
                }

                var loadMeasuresPromise = loadMeasures();
                promises.push(loadMeasuresPromise);

                var checkPermissionsDeferred = UtilsService.createPromiseDeferred();
                promises.push(checkPermissionsDeferred.promise);

                loadMeasuresPromise.then(function () {
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
            api.retrieveData = retrieveData;
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function retrieveData(filter) {
            var fromDate = new Date(filter.fromDate.getTime() + filter.fromDate.getTimezoneOffset() * 60 * 1000);
            var toDate = new Date(filter.toDate.getTime() + filter.toDate.getTimezoneOffset() * 60 * 1000);
            if (!ctrl.isAllowed)
                return;
            ctrl.isGettingData = true;

            return VR_BI_BIAPIService.GetSummaryMeasureValues(fromDate, toDate,ctrl.settings.TimeEntity, ctrl.settings.MeasureTypes)
                .then(function (response) {

                    for (var i = 0; i < response.length; i++) {
                        ctrl.dataSource[i] = {
                            value: response[i].toFixed(2),
                            description: ctrl.measureTypes[i],

                        }
                    }
                })
                .finally(function () {

                    ctrl.isGettingData = false;
                });

        }

        function loadMeasures() {
            return VR_BI_BIConfigurationAPIService.GetMeasuresInfo()
                .then(function (response) {
                    if (ctrl.settings != undefined) {
                        for (var i = 0; i < ctrl.settings.MeasureTypes.length; i++) {
                            var value = UtilsService.getItemByVal(response, ctrl.settings.MeasureTypes[i], 'Name');

                            if (value != null) {
                                measures.push(value);
                            }

                        }
                    }

                });
        }

        this.initializeController = initializeController;
        this.defineAPI = defineAPI;
    }

    return directiveDefinitionObject;
}]);