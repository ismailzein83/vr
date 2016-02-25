'use strict';
app.directive('vrDemoDestinationgroupSelector', ['Demo_DestinationGroupAPIService', 'UtilsService', 'VRUIUtilsService',
    function (Demo_DestinationGroupAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                isdisabled: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new destinationGroupCtor(ctrl, $scope, $attrs);
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
                return getDestinationGroupTemplate(attrs);
            }

        };

        function getDestinationGroupTemplate(attrs) {

            var multipleselection = "";
            var label = "Destination Group";
            if (attrs.ismultipleselection != undefined) {
                label = "Destination Groups";
                multipleselection = "ismultipleselection";
            }


            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="DestinationGroupId" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Destination Group" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>'
        }



        function destinationGroupCtor(ctrl, $scope, attrs) {

            var filter;
            var selectorApi;
            var isDirectiveLoaded = false;

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    return getDestinationGroupsInfo(attrs, ctrl, selectedIds);
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DestinationGroupId', attrs, ctrl);
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getDestinationGroupsInfo(attrs, ctrl, selectedIds) {
            return Demo_DestinationGroupAPIService.GetDestinationGroupsInfo().then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'DestinationGroupId', attrs, ctrl);
                }
            });
        }

        return directiveDefinitionObject;
    }]);