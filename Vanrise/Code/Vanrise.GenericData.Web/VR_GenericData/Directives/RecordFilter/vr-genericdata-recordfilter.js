(function (app) {

    'use strict';

    RecordfilterDirective.$inject = ['VR_GenericData_DataRecordTypeService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_RecordFilterAPIService'];

    function RecordfilterDirective(VR_GenericData_DataRecordTypeService, UtilsService, VRUIUtilsService, VR_GenericData_RecordFilterAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                validateedit: "=",
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new Recordfilter($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordFilter/Templates/RecordFilterTemplate.html"

        };

        function Recordfilter($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var filterObj;
            var context;

            function initializeController() {
                $scope.scopeModel = {};

                ctrl.expression;

                ctrl.addFilter = function () {
                    if (context != undefined) {
                        var onFilterAdded = function (filter, expression) {
                            filterObj = filter;
                            ctrl.expression = expression;
                        };
                        VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter(context.getFields(), filterObj, onFilterAdded);
                    }
                };

                ctrl.resetFilter = function () {
                    ctrl.expression = undefined;
                    filterObj = null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    ctrl.expression = undefined;
                    filterObj = undefined;

                    if (payload != undefined) {
                        context = payload.context;
                        filterObj = payload.FilterGroup;

                        var recordFilterFieldInfosByFieldName = {};

                        if (context != undefined) {
                            var fields = context.getFields();
                            for (var i = 0; i < fields.length; i++) {
                                var field = fields[i];
                                recordFilterFieldInfosByFieldName[field.FieldName] = { Name: field.FieldName, Title: field.FieldTitle, Type: field.Type };
                            }
                        }

                        if (filterObj != undefined)
                            promises.push(buildRecordFilterGroupExpression(recordFilterFieldInfosByFieldName, filterObj));

                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data =
                    {
                        filterObj: filterObj,
                        expression: ctrl.expression
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildRecordFilterGroupExpression(recordFilterFieldInfosByFieldName, filterObj) {
                return VR_GenericData_RecordFilterAPIService.BuildRecordFilterGroupExpression({ RecordFilterFieldInfosByFieldName: recordFilterFieldInfosByFieldName, FilterGroup: filterObj }).then(function (response) {
                    ctrl.expression = response
                });
            }
        }
    }

    app.directive('vrGenericdataRecordfilter', RecordfilterDirective);

})(app);