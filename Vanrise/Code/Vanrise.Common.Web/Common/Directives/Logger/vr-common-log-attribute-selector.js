﻿'use strict';
app.directive('vrCommonLogAttributeSelector', ['VRCommon_LogAttributeAPIService', 'VRCommon_LogAttributeEnum', 'UtilsService', 'VRUIUtilsService',
    function (VRCommon_LogAttributeAPIService, VRCommon_LogAttributeEnum, UtilsService, VRUIUtilsService) {

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
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                $scope.addNewLogAttribute = function () {
                    var onLogAttributeAdded = function (logAttributeObj) {
                        ctrl.datasource.push(logAttributeObj.Entity);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(logAttributeObj.Entity);
                        else
                            ctrl.selectedvalues = logAttributeObj.Entity;
                    };
                    VRCommon_LogAttributeService.addLogAttribute(onLogAttributeAdded);
                }

                //ctrl.haspermission = function () {
                //    return VRCommon_LogAttributeAPIService.HasAddLogAttributePermission();
                //};

                var ctor = new LogAttributeCtor(ctrl, $scope, $attrs);
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
                return getLogAttributeTemplate(attrs);
            }

        };

        function getLogAttributeTemplate(attrs) {

            var multipleselection = "";
            var label = "LogAttribute";
            if (attrs.ismultipleselection != undefined) {
                label = "LogAttributes";
                multipleselection = "ismultipleselection";
            }
            if (attrs.label != undefined) {
                label = attrs.label;
            }
            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewLogAttribute"';

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-select ' + multipleselection + '  datatextfield="Description" datavaluefield="LogAttributeID" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="LogAttribute" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + '></vr-select>'
        }

        function LogAttributeCtor(ctrl, $scope, attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var attribute;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        attribute = payload.attribute;
                    }

                    return getLogAttributeInfo(attrs, ctrl, selectedIds, attribute);
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('LogAttributeID', attrs, ctrl);
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getLogAttributeInfo(attrs, ctrl, selectedIds, attribute) {
            return VRCommon_LogAttributeAPIService.GetSpecificLogAttribute(attribute).then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'LogAttributeID', attrs, ctrl);
                }
            });
        }

        return directiveDefinitionObject;
    }]);