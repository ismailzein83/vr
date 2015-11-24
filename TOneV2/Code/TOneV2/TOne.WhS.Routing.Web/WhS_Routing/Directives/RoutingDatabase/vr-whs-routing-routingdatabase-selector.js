﻿'use strict';
app.directive('vrWhsRoutingRoutingdatabaseSelector', ['WhS_Routing_RoutingDatabaseAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_Routing_RoutingDatabaseAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                selectedvalues: '=',
                hideremoveicon: "@"
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
                }
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }

        };


        function getTemplate(attrs) {
            var multipleselection = "";
            var label = "Routing Database";
            if (attrs.ismultipleselection != undefined) {
                label = "Routing Databases";
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
               + '</div>'
        }

        function routingDatabaseCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter = {};
                    var selectedIds;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return WhS_Routing_RoutingDatabaseAPIService.GetRoutingDatabaseInfo().then(function (response) {
                        angular.forEach(response, function (itm) {
                            ctrl.datasource.push(itm);
                        });
                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'RoutingDatabaseId', $attrs, ctrl);

                    });
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('RoutingDatabaseId', $attrs, ctrl);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);