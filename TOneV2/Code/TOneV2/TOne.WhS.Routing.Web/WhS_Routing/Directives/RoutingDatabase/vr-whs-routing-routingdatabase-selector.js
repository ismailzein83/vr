﻿'use strict';
app.directive('vrWhsRoutingRoutingdatabaseSelector', ['WhS_Routing_RoutingDatabaseAPIService', 'WhS_Routing_RoutingProcessTypeEnum', 'UtilsService', 'VRUIUtilsService', 'WhS_Routing_RoutingDatabaseTypeEnum',
    function (WhS_Routing_RoutingDatabaseAPIService, WhS_Routing_RoutingProcessTypeEnum, UtilsService, VRUIUtilsService, WhS_Routing_RoutingDatabaseTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                selectedvalues: '=',
                hideremoveicon: "@",
                getcustomerroutes: "@",
                getroutingproductroutes: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new routingDatabaseCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {
                    }
                };
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

        };


        function getTemplate(attrs) {
            var multipleselection = "";
            var label = "Database";
            if (attrs.ismultipleselection != undefined) {
                label = "Databases";
                multipleselection = "ismultipleselection";
            }
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Title" datavaluefield="RoutingDatabaseId" '
            + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"  onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" ' + hideremoveicon + '></vr-select>'
               + '</div>';
        }

        function routingDatabaseCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter;
                    var selectedIds;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    if (filter == undefined)
                        filter = {};

                    filter.ProcessType = $attrs.getcustomerroutes != undefined ?
                        WhS_Routing_RoutingProcessTypeEnum.CustomerRoute.value : WhS_Routing_RoutingProcessTypeEnum.RoutingProductRoute.value;

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_Routing_RoutingDatabaseAPIService.GetRoutingDatabaseInfo(serializedFilter).then(function (response) {
                        var hasCurrentDB = false;
                        angular.forEach(response, function (itm) {
                            if (itm.Type == WhS_Routing_RoutingDatabaseTypeEnum.Current.value)
                                hasCurrentDB = true;
                            ctrl.datasource.push(itm);
                        });

                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'RoutingDatabaseId', $attrs, ctrl);
                        else {
                            var currentRoutingDBTypeValue = WhS_Routing_RoutingDatabaseTypeEnum.Current.value;
                            var currentDBSelectedIds = ($attrs.ismultipleselection != undefined) ? [currentRoutingDBTypeValue] : currentRoutingDBTypeValue;
                            if (hasCurrentDB) {
                                VRUIUtilsService.setSelectedValues(currentDBSelectedIds, 'Type', $attrs, ctrl);
                            }
                            else {
                                if (ctrl.datasource.length > 0) {
                                    var firstItem = ctrl.datasource[0];
                                    var defaultSelectedIds = ($attrs.ismultipleselection != undefined) ? [firstItem.Type] : firstItem.Type;
                                    VRUIUtilsService.setSelectedValues(defaultSelectedIds, 'Type', $attrs, ctrl);
                                }
                            }
                        }

                        if (payload != undefined && payload.onLoadRoutingDatabaseInfo != undefined && typeof (payload.onLoadRoutingDatabaseInfo) == 'function') {
                            payload.onLoadRoutingDatabaseInfo(ctrl.selectedvalues);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('RoutingDatabaseId', $attrs, ctrl);
                };

                api.isDatabaseTypeCurrent = function () {
                    if (ctrl.selectedvalues == undefined) {
                        return false;
                    }

                    if (ctrl.selectedvalues instanceof Array) {
                        return false;
                    }

                    if (ctrl.selectedvalues.Type != WhS_Routing_RoutingDatabaseTypeEnum.Current.value) {
                        return false;
                    }

                    return true;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);