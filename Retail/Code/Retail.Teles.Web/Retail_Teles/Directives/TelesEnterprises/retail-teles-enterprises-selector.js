﻿'use strict';

app.directive('retailTelesEnterprisesSelector', ['Retail_Teles_EnterpriseAPIService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_BusinessEntityDefinitionAPIService', function (Retail_Teles_EnterpriseAPIService, UtilsService, VRUIUtilsService, VR_GenericData_BusinessEntityDefinitionAPIService) {
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
            customvalidate: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var enterpriseSelector = new EnterpriseSelector(ctrl, $scope, $attrs);
            enterpriseSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function EnterpriseSelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        function initializeController() {
            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                selectorAPI.clearDataSource();

                var selectedIds;
                var filter;
                var vrConnectionId;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    vrConnectionId = payload.vrConnectionId;

                    if (payload.filter != undefined)
                        filter = payload.filter;

                    var loadPromise = UtilsService.createPromiseDeferred();
                    promises.push(loadPromise.promise);

                    if(payload.businessEntityDefinitionId != undefined)
                    {
                        VR_GenericData_BusinessEntityDefinitionAPIService.GetBusinessEntityDefinition(payload.businessEntityDefinitionId).then(function (response) {
                            if (response != undefined && response.Settings != undefined) {
                                vrConnectionId = response.Settings.VRConnectionId;
                                loadEnterPrisesInfo(attrs, ctrl, vrConnectionId, filter, selectedIds).then(function () {
                                    loadPromise.resolve();
                                });
                            }
                        });
                     
                    }else
                    {
                        loadEnterPrisesInfo(attrs, ctrl, vrConnectionId, filter, selectedIds).then(function () {
                            loadPromise.resolve();
                        });
                    }
                }
                return UtilsService.waitMultiplePromises(promises);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('TelesEnterpriseId', attrs, ctrl);
            };
            api.clearDataSource = function () {
                selectorAPI.clearDataSource();
                ctrl.datasource.length = 0;
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function loadEnterPrisesInfo(attrs, ctrl, vrConnectionId, filter, selectedIds)
        {
            return Retail_Teles_EnterpriseAPIService.GetEnterprisesInfo(vrConnectionId, UtilsService.serializetoJson(filter)).then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        ctrl.datasource.push(response[i]);
                    }

                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'TelesEnterpriseId', attrs, ctrl);
                    }
                }
            });
        }
    }

    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "Enterprise";

        if (attrs.ismultipleselection != undefined) {
            label = "Enterprises";
            multipleselection = "ismultipleselection";
        }

        return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + ' datatextfield="Name" datavaluefield="TelesEnterpriseId" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate"></vr-select></vr-columns>';
    }

}]);