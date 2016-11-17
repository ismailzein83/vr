'use strict';
app.directive('vrCommonCitySelector', ['VRCommon_CityAPIService', 'VRCommon_CityService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService',
    function (VRCommon_CityAPIService, VRCommon_CityService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                showaddbutton: '@',
                normalColNum: '@',
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var cityCtrl = new CityCtrl(ctrl, $scope, $attrs);
                cityCtrl.initializeController();


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
                return getCityTemplate(attrs);
            }

        };
        function getCityTemplate(attrs) {

            var multipleselection = "";

            var label = "City";
            if (attrs.ismultipleselection != undefined) {
                label = "Cities";
                multipleselection = "ismultipleselection";
            }

            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="ctrl.addNewCity"';

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-common-country-selector  onselectionchanged="ctrl.onCountrySelectionChanged"  ng-show="ctrl.showCountrySelector" on-ready="ctrl.onCountrySelectorReady" ' + hideremoveicon + '> </vr-common-country-selector> </vr-columns>'
                + '<vr-columns colnum="{{ctrl.normalColNum}}" vr-loader="ctrl.isLoadingCities"><vr-select ' + multipleselection + '  datatextfield="Name"   datavaluefield="CityId" isrequired="ctrl.isrequired" '
            + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="ctrl.onSelectorReady" onselectionchanged="ctrl.onselectionchanged" entityName="City" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission" ' + hideremoveicon + '></vr-select>'
               + '</vr-columns>';
        }

        function CityCtrl(ctrl, $scope, attrs) {

            var countrySelectorAPI;
            var countrySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var selectorAPI;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var countryId;

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

                ctrl.onCountrySelectionChanged = function () {

                    selectorAPI.clearDataSource();

                    var countryId = countrySelectorAPI.getSelectedIds();
                    if (countryId != undefined) {
                        ctrl.isLoadingCities = true;
                        getCitiesInfo(attrs, ctrl, undefined, countryId).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            ctrl.isLoadingCities = false;
                        });
                    }
                };

                ctrl.addNewCity = function () {
                    var onCityAdded = function (cityObj) {
                        ctrl.datasource.push(cityObj.Entity);
                        if (attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(cityObj.Entity);
                        else
                            ctrl.selectedvalues = cityObj.Entity;
                    };

                    VRCommon_CityService.addCity(onCityAdded, countryId != undefined ? countryId : countrySelectorAPI.getSelectedIds());
                };
                ctrl.haspermission = function () {
                    return VRCommon_CityAPIService.HasAddCityPermission();
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    selectorAPI.clearDataSource();
                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        countryId = payload.countryId;
                    }

                    if (countryId != undefined) {
                        ctrl.showCountrySelector = false;
                        return getCitiesInfo(attrs, ctrl, selectedIds, payload.countryId)
                    }
                    else {
                        ctrl.showCountrySelector = true;

                        if (selectedIds != undefined) {
                            var selectedCityIds = [];

                            if (attrs.ismultipleselection != undefined)
                                selectedCityIds = selectedIds;
                            else
                                selectedCityIds.push(selectedIds);

                            var loadAllSectionPromiseDeferred = UtilsService.createPromiseDeferred();

                            countrySelectorReadyPromiseDeferred.promise.then(function () {

                                var promises = [];
                                var loadCountrySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                                promises.push(loadCountrySelectorPromiseDeferred.promise);

                                var setSelectedCityPromiseDeferred = UtilsService.createPromiseDeferred();
                                promises.push(setSelectedCityPromiseDeferred.promise);

                                VRCommon_CityAPIService.GetDistinctCountryIdsByCityIds(selectedCityIds).then(function (response) {

                                    var selectedCountryIds = [];

                                    for (var i = 0 ; i < response.length < 0 ; i++) {
                                        selectedCountryIds.push(response[i]);
                                    }

                                    var countryPayload = {
                                        selectedIds: selectedCountryIds
                                    };
                                    VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countryPayload, loadCountrySelectorPromiseDeferred);

                                    loadCountrySelectorPromiseDeferred.promise.then(function () {
                                        VRUIUtilsService.setSelectedValues(selectedIds, 'CityId', attrs, ctrl);
                                        setSelectedCityPromiseDeferred.resolve();
                                    });

                                });

                                UtilsService.waitMultiplePromises(promises).then(function () {
                                    loadAllSectionPromiseDeferred.resolve();
                                }).catch(function (error) {
                                    loadAllSectionPromiseDeferred.reject(error);
                                });
                            });

                            return loadAllSectionPromiseDeferred.promise;
                        }
                        else {
                            var loadCountrySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                            countrySelectorReadyPromiseDeferred.promise.then(function () {
                                VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, undefined, loadCountrySelectorPromiseDeferred);
                            });
                            return loadCountrySelectorPromiseDeferred.promise;
                        }
                    }
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CityId', attrs, ctrl);
                };

                api.clearDataSource = function () {
                    selectorAPI.clearDataSource();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;
            }

            this.initializeController = initializeController;

        }

        function getCitiesInfo(attrs, ctrl, selectedIds, countryId) {
            return VRCommon_CityAPIService.GetCitiesInfo(countryId).then(function (response) {
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

