'use strict';
app.directive('vrCommonCityySelector', ['VRCommon_CityAPIService', 'VRCommon_CityService', 'UtilsService', 'VRUIUtilsService',
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
                    VRCommon_CityService.addCity(onCityAdded, countryId);
                }

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

            return '<div><vr-common-country-selector  onselectionchanged="ctrl.onCountrySelectionChanged" ng-show="ctrl.showCountrySelector" on-ready="ctrl.onCountrySelectorReady"> </vr-common-country-selector>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" ng-show="ctrl.showCitiesSelector" datavaluefield="CityId" isrequired="ctrl.isrequired" '
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="ctrl.onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="City" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>';
        }
        function City(ctrl, $scope, attrs) {

            var countrySelectorAPI;
            var countrySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var selectorAPI;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            ctrl.showCitiesSelector = false;
           

            function initializeController() {
                ctrl.onCountrySelectorReady = function (api) {
                    countrySelectorAPI = api;
                    countrySelectorReadyPromiseDeferred.resolve();
                };

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyPromiseDeferred.resolve();

                };

                UtilsService.waitMultiplePromises([countrySelectorReadyPromiseDeferred.promise, selectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            
                ctrl.onCountrySelectionChanged = function (item) {
                    var selectedIds;
                    var filter = {};
                    if (item != undefined) {
                        ctrl.showCitiesSelector = true;
                        filter.CountryId = item.CountryId;
                        var serializedFilter = {};
                        serializedFilter = UtilsService.serializetoJson(filter);
                        getCitiesInfo(attrs, ctrl, selectedIds, serializedFilter);
                    }
                }
               

            }


            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter;
                    var selectedIds;
                    if (payload != undefined && payload.countryId != null) {
                        ctrl.showCountrySelector = false;
                        filter = payload;
                        selectedIds = payload.selectedIds;
                        var serializedFilter = {};
                        if (filter != undefined) {
                            serializedFilter = UtilsService.serializetoJson(filter);
                        }
                        var loadSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        selectorReadyPromiseDeferred.promise.then(function () {
                            ctrl.showCitiesSelector = true;
                            getCitiesInfo(attrs, ctrl, selectedIds, serializedFilter).then(function () {
                                loadSelectorPromiseDeferred.resolve();
                            });
                        })
                        return loadSelectorPromiseDeferred.promise;
                    }
                    else if(payload != undefined && payload.countryId == null) {
                        ctrl.showCountrySelector = false;
                    }
                    else if (payload == undefined) {
                        ctrl.showCountrySelector = true;
                        var loadCountrySelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        countrySelectorReadyPromiseDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, undefined, loadCountrySelectorPromiseDeferred);
                        });
                        return loadCountrySelectorPromiseDeferred.promise;
                    }
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CityId', attrs, ctrl);
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
                return api;
            }

            this.initializeController = initializeController;

        }

        function getCitiesInfo(attrs, ctrl, selectedIds, serializedFilter) {

            return VRCommon_CityAPIService.GetCitiesInfo(serializedFilter).then(function (response) {
                ctrl.datasource.length = 0;
                ctrl.selectedvalues = (attrs.ismultipleselection != undefined) ? [] : undefined;
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

