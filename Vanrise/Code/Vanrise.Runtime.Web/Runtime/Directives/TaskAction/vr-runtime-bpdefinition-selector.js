﻿'use strict';
app.directive('vrRuntimeBpdefinitionSelector', ['BusinessProcessAPIService', 'UtilsService', 'VRUIUtilsService',
    function (BusinessProcessAPIService, UtilsService, VRUIUtilsService) {

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
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

              

                var ctor = new bpCtor(ctrl, $scope, $attrs);
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
                return getBPTemplate(attrs);
            }

        };


        function getBPTemplate(attrs) {

            var multipleselection = "";
            var label = "Workflow Name";
            if (attrs.ismultipleselection != undefined) {
                label = "Workflow Name";
                multipleselection = "ismultipleselection";
            }

        
            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Title" datavaluefield="BPDefinitionID" isrequired="ctrl.isrequired"'
               + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Bp definition" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>'
        }

        function bpCtor(ctrl, $scope, attrs) {

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
                    
                    return getBPDefenitionsInfo(attrs, ctrl, selectedIds);
                    
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('BPDefinitionID', attrs, ctrl);
                }
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getBPDefenitionsInfo(attrs, ctrl, selectedIds) {
            return BusinessProcessAPIService.GetDefinitions().then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'BPDefinitionID', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);