(function (app) {

    'use strict';

    DataRecordSourceGridDirective.$inject = ['Analytic_RecordSearchService'];

    function DataRecordSourceGridDirective(Analytic_RecordSearchService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataRecordSourceGrid = new DataRecordSourceGrid($scope, ctrl, $attrs);
                dataRecordSourceGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/RecordSearch/SourceSettings/Templates/DataRecordSourceGridTemplate.html"
        };

        function DataRecordSourceGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var counter = 0;
            function initializeController() {
                ctrl.sources = [];
                ctrl.isSourceGridValid = function () {
                    if (ctrl.sources.length == 0) {
                        return 'At least one Souce must be added.'
                    }
                    return null;
                };
                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addSource = function () {
                    var onDataRecordSrouceAdded = function (sourceObj) {
                        counter++;
                        sourceObj.id = counter;
                        ctrl.sources.push(sourceObj);
                    };
                    Analytic_RecordSearchService.addDataRecordSource(onDataRecordSrouceAdded, getSourceNames(null));
                };

                ctrl.removeSource = function (source) {
                    ctrl.sources.splice(ctrl.sources.indexOf(source), 1);
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (query) {
                    ctrl.sources.length = 0;
                    if (query) {
                        if (query.sources && query.sources.length > 0) {
                            for (var y = 0; y < query.sources.length; y++) {
                                counter++;
                                var currentSource = query.sources[y];
                                currentSource.id = counter;
                                ctrl.sources.push(currentSource);
                            }
                        }
                    }
                };

                api.getData = function () {
                    return ctrl.sources;
                };
                return api;
            }

            function defineMenuActions() {
                ctrl.sourcesGridMenuActions = [{
                    name: 'Edit',
                    clicked: editSource
                }];
            }
            function editSource(source) {
                var onDataRecordSrouceUpdated = function (sourceObj) {
                    ctrl.sources[ctrl.sources.indexOf(source)] = sourceObj;
                };

                Analytic_RecordSearchService.editDataRecordSource(source, getSourceNames(source), onDataRecordSrouceUpdated);
            }

            function getSourceNames(excludedSource) {
                var existingSources = [];
                for (var x = 0; x < ctrl.sources.length; x++) {
                    var currentSource = ctrl.sources[x];
                    if (excludedSource == null || excludedSource.Title != currentSource.Title) {
                        existingSources.push(currentSource.Title.toLowerCase());
                    }
                }
                return existingSources;
            }
        }
    }

    app.directive('vrAnalyticDatarecordsourceGrid', DataRecordSourceGridDirective);

})(app);