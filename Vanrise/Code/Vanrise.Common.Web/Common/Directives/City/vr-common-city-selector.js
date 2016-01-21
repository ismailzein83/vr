﻿'use strict';
app.directive('vrCommonCitySelector', ['VRCommon_CityAPIService', 'VRCommon_CityService', 'UtilsService', 'VRUIUtilsService',
    function (VRCommon_CityAPIService, VRCommon_CityService, UtilsService, VRUIUtilsService) {

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
                ctrl.filter;
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                $scope.addNewCity = function () {
                    var onCityAdded = function (cityObj) {
                        ctrl.datasource.push(cityObj.Entity);
                        if ($attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(cityObj.Entity);
                        else
                            ctrl.selectedvalues = cityObj.Entity;               
                    };
                   
                    if (ctrl.filter != undefined)
                        var countryId = ctrl.filter.CountryId;
                    VRCommon_CityService.addCity(onCityAdded, countryId );
                }
                $scope.datasource = [];
                var beCity = new City(ctrl, $scope, $attrs);
                beCity.initializeController();
               

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
                return getBeCityTemplate(attrs);
            }

        };
        function getBeCityTemplate(attrs) {

            var multipleselection = "";
            var label = "City";
            if (attrs.ismultipleselection != undefined) {
                label = "Cities";
                multipleselection = "ismultipleselection";
            }

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewCity"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="CityId" isrequired="ctrl.isrequired" '
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="City" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>';
        }
        function City(ctrl, $scope, attrs) {

            var selectorAPI;

            function initializeController() {
                $scope.onSelectorReady = function(api)
                {
                    selectorAPI = api;
                    defineAPI();
                }
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CityId', attrs, ctrl);
                }
                api.load = function (payload) {
                    var filter;
                    var selectedIds;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }
                    var serializedFilter = {};
                    ctrl.filter =  undefined
                    if (filter != undefined) {
                        ctrl.filter = filter;
                        serializedFilter = UtilsService.serializetoJson(filter);
                    }
                       
                  
                      
                    return getCitiesInfo(attrs, ctrl, selectedIds, serializedFilter);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }

        function getCitiesInfo(attrs, ctrl, selectedIds, serializedFilter) {
            
            return VRCommon_CityAPIService.GetCitiesInfo(serializedFilter).then(function (response) {
                ctrl.datasource.length = 0;
                angular.forEach(response, function (itm) {
                    ctrl.datasource.push(itm);
                });

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'CityId', attrs, ctrl);
                }
            });
        }
        return directiveDefinitionObject;
    }]);

