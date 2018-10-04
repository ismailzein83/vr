﻿'use strict';
app.directive('businessprocessBpDefinitionSelector', ['BusinessProcess_BPDefinitionAPIService', 'UtilsService', 'VRUIUtilsService','$filter',
    function (BusinessProcess_BPDefinitionAPIService, UtilsService, VRUIUtilsService, $filter) {

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
                isdisabled: "=",
                showaddbutton: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new bpDefinitionCtor(ctrl, $scope, $attrs);
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
                return getBPDefinitionTemplate(attrs);
            }

        };


        function getBPDefinitionTemplate(attrs) {

            var multipleselection = "";
            var label = "Business Process";

            if (attrs.ismultipleselection != undefined) {
                label = "Business Processes";
                multipleselection = "ismultipleselection";
            }

            var hidelabel = "";
            if (attrs.hidelabel != undefined)
                hidelabel = "hidelabel";

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = 'hideremoveicon';

            var addCliked = '';

            // vr-disabled="ctrl.isdisabled"  is removed temporary
            return '<div vr-disabled="ctrl.isdisabled">'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="BPDefinitionID" isrequired="ctrl.isrequired"' + hideremoveicon 
                + ' label="' + label + '" ' + hidelabel + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"' 
                + 'onselectionchanged="ctrl.onselectionchanged" entityName="BPDefinition" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }

        function bpDefinitionCtor(ctrl, $scope, attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var serializedFilter = {};
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        if (payload.filter != undefined) {
                            serializedFilter = UtilsService.serializetoJson(payload.filter);
                        }
                    }
                    return getBPDefinitionsInfo(attrs, ctrl, selectedIds, serializedFilter);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('BPDefinitionID', attrs, ctrl);
                };

                api.setSelectedValues = function (selectedIds) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'BPDefinitionID', attrs, ctrl);
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getBPDefinitionsInfo(attrs, ctrl, selectedIds, serializedFilter) {
            return BusinessProcess_BPDefinitionAPIService.GetBPDefinitionsInfo(serializedFilter).then(function (response) {
                ctrl.datasource.length = 0;
                var data = $filter('orderBy')(response, 'Name');
                angular.forEach(data, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'BPDefinitionID', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);