//(function (app) {

//    'use strict';

//    ReportTypeSelectorDirective.$inject = ['VRCommon_VRComponentTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

//    function ReportTypeSelectorDirective(VRCommon_VRComponentTypeAPIService, UtilsService, VRUIUtilsService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                ismultipleselection: "@",
//                isrequired: "=",
//                onselectionchanged: '=',
//                normalColNum: '@',
//                customlabel: "@"
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                ctrl.datasource = [];
//                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

//                var reportTypeSelector = new ReportTypeSelector(ctrl, $scope, $attrs);
//                reportTypeSelector.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {
//                return {
//                    pre: function ($scope, iElem, iAttrs, ctrl) {

//                    }
//                };
//            },
//            template: function (element, attrs) {
//                return getDirectiveTemplate(attrs);
//            }
//        };

//        function ReportTypeSelector(ctrl, $scope, attrs) {
//            this.initializeController = initializeController;

//            var filter;
//            var selectedIds;
//            var selectFirstItem;

//            var componentTypeSelectorAPI;
//            var componentTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                ctrl.onComponenetTypeSelectorReady = function (api) {
//                    componentTypeSelectorAPI = api;
//                    componentTypeSelectorReadyDeferred.resolve();
//                };

//                UtilsService.waitMultiplePromises([componentTypeSelectorReadyDeferred.promise]).then(function () {
//                    defineAPI();
//                });
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];
                    
//                    if (payload != undefined) {
//                        filter = payload.filter;
//                        if (filter == undefined)
//                            filter = {};
//                        selectedIds = payload.selectedIds;
//                        selectFirstItem = payload.selectFirstItem != undefined && payload.selectFirstItem == true;
//                    }

//                    initialPromises.push(loadComponentTypeSelector());

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];
//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };
                    
//                };

//                api.getSelectedIds = function () {
//                    return componentTypeSelectorAPI.getSelectedIds();
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }

//            function loadComponentTypeSelector() {
//                var loadComponentTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
//                componentTypeSelectorReadyDeferred.promise.then(function () {
//                    filter.Filters = [{
//                        $type: "Vanrise.Analytic.Entities.ReportTypeFilter, Vanrise.Analytic.Entities"
//                    }];
//                    var payloadSelector = {
//                        selectedIds: selectedIds,
//                        filter: filter
//                    };
//                    VRUIUtilsService.callDirectiveLoad(componentTypeSelectorAPI, payloadSelector, loadComponentTypeSelectorPromiseDeferred);
//                });
//                return loadComponentTypeSelectorPromiseDeferred.promise;
//            }
//        }

//        function getDirectiveTemplate(attrs) {

//            var multipleselection = '';

//            var label = 'Report Type';
//            if (attrs.ismultipleselection != undefined) {
//                label = 'Report Types';
//                multipleselection = 'ismultipleselection';
//            }

//            if (attrs.customlabel != undefined) {
//                label = attrs.customlabel;
//            }

//            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
//                + '<vr-common-componenttype-selector on-ready="ctrl.onComponenetTypeSelectorReady"'
//                + ' isrequired="ctrl.isrequired"'
//                + ' onselectionchanged="ctrl.onselectionchanged"'
//                + ' label="' + label + '"'
//                + ' ' + multipleselection + '>'
//                + '</vr-common-componenttype-selector>'
//                + '</vr-columns>';
//        }
//    }

//    app.directive('vrAnalyticReporttypeSelector', ReportTypeSelectorDirective);

//})(app);