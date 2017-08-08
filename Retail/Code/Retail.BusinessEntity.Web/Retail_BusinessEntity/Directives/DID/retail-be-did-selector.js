﻿'use strict';

app.directive('retailBeDidSelector', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_DIDAPIService', 'Retail_BE_DIDService',
    function (UtilsService, VRUIUtilsService, Retail_BE_DIDAPIService, Retail_BE_DIDService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '=',
                vrLoader: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new DIDCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function DIDCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectedIds;
            var filter;

            var selectorAPI;

            function initializeController() {

                ctrl.addNewDID = function () {
                    var onDIDAdded = function (didObj) {

                        if (attrs.ismultipleselection != undefined) {
                            var previousSelectedIds = VRUIUtilsService.getIdSelectedIds('DIDId', attrs, ctrl);
                            selectedIds = previousSelectedIds != undefined ? previousSelectedIds : [];
                            selectedIds.push(didObj.Entity.DIDId);
                        }
                        else {
                            selectedIds = didObj.Entity.DIDId;
                        }

                        ctrl.vrLoader = true;
                        loadSelector(selectedIds, filter).then(function () {
                            ctrl.vrLoader = false;
                        });
                    };
                    Retail_BE_DIDService.addDID(onDIDAdded);
                };

                ctrl.haspermission = function () {
                    return Retail_BE_DIDAPIService.HasAddDIDPermission();
                };

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return loadSelector(selectedIds, filter);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DIDId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadSelector(selectedIds, filter) {
                selectorAPI.clearDataSource();

                return Retail_BE_DIDAPIService.GetDIDsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'DIDId', attrs, ctrl);
                        }
                    }
                });
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "DID";

            if (attrs.ismultipleselection != undefined) {
                label = "DIDs";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="ctrl.addNewDID"';

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                        '<span vr-loader="ctrl.vrLoader">' +
                            '<vr-select ' + multipleselection + ' datatextfield="Number" datavaluefield="DIDId" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                                + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                                + ' hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate" ' + addCliked + ' haspermission="ctrl.haspermission">' +
                            '</vr-select>' +
                        '</span>' +
                    '</vr-columns>';
        }
    }]);