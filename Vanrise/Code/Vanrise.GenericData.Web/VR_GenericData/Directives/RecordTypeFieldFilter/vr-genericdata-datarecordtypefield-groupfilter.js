'use strict';
app.directive('vrGenericdataDatarecordtypefieldGroupfilter', ['VR_GenericData_DataRecordTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_RecordQueryLogicalOperatorEnum',
    function (VR_GenericData_DataRecordTypeAPIService, UtilsService, VRUIUtilsService, VR_GenericData_RecordQueryLogicalOperatorEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new recordTypeFieldGroupFilterCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/RecordTypeFieldFilter/Templates/DataRecordTypeFieldGroupFilter.html"

        };

        function recordTypeFieldGroupFilterCtor(ctrl, $scope, $attrs) {
            var filterObj;
            var context;
            ctrl.isValid = function () {
                if (ctrl.groups.length == 0 && ctrl.rules.length == 0)
                    return 'At least one item should be added';
                return null;
            }
            ctrl.addRule = function () {
                var rule = {
                    onRuleFilterReady: function (api) {
                        var payload = {
                            context: context
                        };
                        api.load(payload);
                        rule.api = api;
                    }
                };
                ctrl.rules.push(rule);
            };
            $scope.removeRule = function (rule) {
                ctrl.rules.splice(ctrl.rules.indexOf(rule), 1);
            };

            $scope.removeGroup = function (group) {
                ctrl.groups.splice(ctrl.groups.indexOf(group), 1);
            };

            ctrl.addGroup = function () {
                var group = {
                    onGroupFilterReady: function (api) {
                        var payload = {
                            context: context
                        };
                        api.load(payload);
                        group.api = api;
                    }
                };


                ctrl.groups.push(group);
            };

            ctrl.groups = [];
            ctrl.rules = [];

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    $scope.conditions = UtilsService.getArrayEnum(VR_GenericData_RecordQueryLogicalOperatorEnum);
                    if (payload != undefined) {
                        context = payload.context;

                        filterObj = payload.filterObj;
                        if (filterObj) {
                            buildData(filterObj.Filters, promises);
                            ctrl.condition = payload.filterObj.LogicalOperator;//UtilsService.getEnum(VR_GenericData_RecordQueryLogicalOperatorEnum, 'value', payload.filterObj.LogicalOperator).description;

                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                var buildData = function (items, promises) {
                    if (items) {
                        for (var x = 0; x < items.length; x++) {
                            var currentItem = items[x];

                            var payload = {
                                context: context,
                                filterObj: currentItem
                            };

                            var filterItem = {
                                payload: payload,
                                readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                loadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(filterItem.loadPromiseDeferred.promise);

                            if (currentItem.$type == 'Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities') {
                                buildGroup(filterItem);
                            }
                            else {
                                buildRule(filterItem);
                            }
                        }
                    }
                };

                var buildRule = function (filterItem) {
                    var rule = {};
                    var filterItemPayload = filterItem.payload;

                    rule.onRuleFilterReady = function (api) {
                        rule.api = api;
                        filterItem.readyPromiseDeferred.resolve();
                    };

                    filterItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(rule.api, filterItemPayload, filterItem.loadPromiseDeferred);
                        });

                    ctrl.rules.push(rule);
                };

                var buildGroup = function (filterItem) {

                    var group = {};
                    var filterItemPayload = filterItem.payload;

                    group.onGroupFilterReady = function (api) {
                        group.api = api;
                        filterItem.readyPromiseDeferred.resolve();
                    };

                    filterItem.readyPromiseDeferred.promise
                        .then(function () {
                            VRUIUtilsService.callDirectiveLoad(group.api, filterItemPayload, filterItem.loadPromiseDeferred);
                        });

                    ctrl.groups.push(group);
                };

                api.getData = function () {

                    var filters = [];
                    if (ctrl.rules.length > 0) {

                        for (var x = 0; x < ctrl.rules.length; x++) {
                            var currentRule = ctrl.rules[x];
                            filters.push(currentRule.api.getData());
                        }
                    }

                    if (ctrl.groups.length > 0) {
                        for (var y = 0; y < ctrl.groups.length; y++) {
                            var currentGroup = ctrl.groups[y];
                            filters.push(currentGroup.api.getData());
                        }
                    }
                    //var logicalOperator = UtilsService.getEnum(VR_GenericData_RecordQueryLogicalOperatorEnum, 'description', ctrl.condition);
                    var filterGroup = {
                        $type: "Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities",
                        LogicalOperator: ctrl.condition,//logicalOperator.value,
                        Filters: filters
                    };
                    return filterGroup;
                };

                api.getExpression = function () {
                    var logicalOperator = UtilsService.getEnum(VR_GenericData_RecordQueryLogicalOperatorEnum, 'value', ctrl.condition);

                    var expression = '';
                    if (ctrl.rules.length > 0) {
                        for (var x = 0; x < ctrl.rules.length; x++) {
                            var currentRule = ctrl.rules[x];
                            if (expression.length > 0) {
                                expression += ' ' + logicalOperator.description + ' ';
                            }
                            expression += currentRule.api.getExpression();
                        }
                    }

                    if (ctrl.groups.length > 0) {
                        for (var y = 0; y < ctrl.groups.length; y++) {
                            var currentGroup = ctrl.groups[y];
                            if (expression.length > 0) {
                                expression += ' ' + logicalOperator.description;
                            }
                            expression += ' (' + currentGroup.api.getExpression() + ')';
                        }
                    }

                    return expression;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

