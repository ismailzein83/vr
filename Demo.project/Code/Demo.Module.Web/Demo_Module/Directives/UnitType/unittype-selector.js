﻿'use strict';
app.directive('unittypeSelector', ['UnitTypeAPIService', 'UtilsService', 'VRUIUtilsService',
    function (UnitTypeAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                showaddbutton: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

              
                var typeSelector = new TypeSelector(ctrl, $scope, $attrs);
                typeSelector.initializeController();


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
                return getTypeTemplate(attrs);
            }

        };

        function getTypeTemplate(attrs) {

            var multipleselection = "";
            var label = "Unit Type";
            if (attrs.ismultipleselection != undefined) {
                label = "Unit Types";
                multipleselection = "ismultipleselection";
            }

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Description" datavaluefield="UnitTypeId" isrequired="ctrl.isrequired" '
                + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Unit Types" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }
        function TypeSelector(ctrl, $scope, attrs) {

            var selectorAPI;
            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                }
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('UnitTypeId', attrs, ctrl);
                }
                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    return UnitTypeAPIService.GetUnitTypesInfo().then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.datasource.push(item);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'UnitTypeId', attrs, ctrl);
                        }
                    });

                   
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }

        
        return directiveDefinitionObject;
    }]);

