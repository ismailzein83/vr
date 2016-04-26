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
            var dataRecordTypeId;
            var filterObj;
            var context;

            ctrl.addRule = function () {
                var rule = {
                    onRuleFilterReady: function (api) {
                        var payload = {
                            dataRecordTypeId: dataRecordTypeId,
                            context: context
                        };
                        api.load(payload);
                        rule.api = api;
                    }
                };
                ctrl.rules.push(rule);
            }
            $scope.removeRule = function (rule) { 
                ctrl.rules.splice(ctrl.rules.indexOf(rule), 1);
            }

            $scope.removeGroup = function (group) {
                ctrl.groups.splice(ctrl.groups.indexOf(group), 1);
            }

            ctrl.addGroup = function () {
                var group = {
                    onGroupFilterReady: function (api) {
                        var payload = {
                            dataRecordTypeId: dataRecordTypeId,
                            context: context
                        };
                        api.load(payload);
                        group.api = api;
                    }
                };


                ctrl.groups.push(group);
            }

            ctrl.groups = [];
            ctrl.rules = [];

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.conditions = UtilsService.getArrayEnum(VR_GenericData_RecordQueryLogicalOperatorEnum);
                    if (payload != undefined) {
                        dataRecordTypeId = payload.dataRecordTypeId;
                        context = payload.context;
                        
                        filterObj = payload.filterObj;
                        if (filterObj) {
                            buildData(filterObj.Filters);
                            ctrl.condition = payload.filterObj.LogicalOperator;//UtilsService.getEnum(VR_GenericData_RecordQueryLogicalOperatorEnum, 'value', payload.filterObj.LogicalOperator).description;
                            
                        }
                    }
                }

                var buildData = function (items) {
                    if (items) {
                        
                        for (var x = 0; x < items.length; x++) {
                            var currentItem = items[x];
                            if (currentItem.$type == 'Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities') {
                                buildGroup(currentItem);
                            }
                            else {
                                buildRule(currentItem);
                            }
                        }
                    }
                }

                var buildRule = function (currentItem) {
                    var rule = {
                        onRuleFilterReady: function (api) {
                            var payload = {
                                dataRecordTypeId: dataRecordTypeId,
                                context: context,
                                filterObj: currentItem
                            };
                            api.load(payload);
                            rule.api = api;
                        }
                    };
                    ctrl.rules.push(rule);
                }

                var buildGroup = function (currentItem) {
                    var group = {
                        onGroupFilterReady: function (api) {
                            var payload = {
                                dataRecordTypeId: dataRecordTypeId,
                                context: context,
                                filterObj: currentItem
                            };
                            api.load(payload);
                            group.api = api;
                        }
                    };

                    ctrl.groups.push(group);
                }

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
                }

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
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);

