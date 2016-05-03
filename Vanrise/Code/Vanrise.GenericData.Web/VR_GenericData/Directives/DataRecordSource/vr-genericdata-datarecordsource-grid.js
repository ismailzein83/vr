(function (app) {

    'use strict';

    DataRecordSourceGridDirective.$inject = ['VR_GenericData_DataRecordSourceService'];

    function DataRecordSourceGridDirective(VR_GenericData_DataRecordSourceService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/DataRecordSource/Templates/DataRecordSourceGridTemplate.html'
        };

        function DataRecordSourceGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.sources = [];
                ctrl.isSourceGridValid = function () {
                    if (ctrl.sources.length == 0) {
                        return 'At least one Souce must be added.'
                    }
                    return null;
                }
                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addSource = function () {
                    var onDataRecordSrouceAdded = function (sourceObj) {
                        ctrl.sources.push(sourceObj);
                    }
                    VR_GenericData_DataRecordSourceService.addDataRecordSource(onDataRecordSrouceAdded);
                }

                ctrl.removeSource = function (source) {
                    ctrl.sources.splice(ctrl.sources.indexOf(source), 1);
                }

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.loadGrid = function (query) {

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
                    gridAPI.itemUpdated(sourceObj);
                }

                VR_GenericData_DataRecordSourceService.editDataRecordSource(source, onDataRecordSrouceUpdated);
            }
        }
    }

    app.directive('vrGenericdataDatarecordsourceGrid', DataRecordSourceGridDirective);

})(app);