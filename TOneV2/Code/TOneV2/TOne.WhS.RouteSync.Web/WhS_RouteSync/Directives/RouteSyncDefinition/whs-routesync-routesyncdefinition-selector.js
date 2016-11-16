'use strict';
app.directive('whsRoutesyncRoutesyncdefinitionSelector', ['WhS_RouteSync_RouteSyncDefinitionAPIService', 'VRUIUtilsService',
    function (WhS_RouteSync_RouteSyncDefinitionAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                var routeSyncDefinitionSelectorCtor = new RouteSyncDefinitionSelectorCtor(ctrl, $scope, $attrs);
                routeSyncDefinitionSelectorCtor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
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
                return getRouteSyncDefinitionTemplate(attrs);
            }

        };


        function getRouteSyncDefinitionTemplate(attrs) {
            var label = "Route Sync Definition";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Route Sync Definitions";
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                   + '<vr-select on-ready="ctrl.onSelectorReady"'
                   + '  selectedvalues="ctrl.selectedvalues"'
                   + '  onselectionchanged="ctrl.onselectionchanged"'
                   + '  datasource="ctrl.datasource"'
                   + '  datavaluefield="RouteSyncDefinitionId"'
                   + '  datatextfield="Name"'
                   + '  ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
                   + '  label="' + label + '"'
                   + ' entityName="' + label + '"'
                   + '  >'
                   + '</vr-select>'
                   + '</vr-columns>';
        }

        function RouteSyncDefinitionSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var filter;
            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(defineAPI());
                    }
                };


                ctrl.search = function (nameFilter) {
                    return WhS_RouteSync_RouteSyncDefinitionAPIService.GetRouteSyncDefinitionsInfo(nameFilter);
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return WhS_RouteSync_RouteSyncDefinitionAPIService.GetRouteSyncDefinitionsInfo(undefined).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'RouteSyncDefinitionId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('RouteSyncDefinitionId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;

            }

        }

        return directiveDefinitionObject;

    }]);