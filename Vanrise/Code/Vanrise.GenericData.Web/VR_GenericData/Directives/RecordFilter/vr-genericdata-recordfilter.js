﻿(function (app) {

    'use strict';

    RecordfilterDirective.$inject = ['VR_GenericData_DataRecordTypeService', 'UtilsService', 'VRUIUtilsService'];

    function RecordfilterDirective(VR_GenericData_DataRecordTypeService, UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                validateedit:"=",
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
                    if (context != undefined)
                    {
                        var onFilterAdded = function (filter, expression) {
                            filterObj = filter;
                            ctrl.expression = expression;
                        }
                        VR_GenericData_DataRecordTypeService.addDataRecordTypeFieldFilter(context.getFields(), filterObj, onFilterAdded);
                    }
                 
                };
                ctrl.resetFilter = function () {
                    ctrl.expression = undefined;
                    filterObj = null;
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        context = payload.context;
                        if(payload != undefined )
                        {
                            filterObj = payload.FilterGroup;
                            ctrl.expression = payload.ConditionExpression;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = 
                    {
                        filterObj : filterObj,
                        expression : ctrl.expression
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('vrGenericdataRecordfilter', RecordfilterDirective);

})(app);