'use strict';
app.directive('vrCommonCountrySelector', ['VRCommon_CountryAPIService', 'VRCommon_CountryService', 'UtilsService', 'VRUIUtilsService', function (VRCommon_CountryAPIService, VRCommon_CountryService, UtilsService, VRUIUtilsService) {

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
            hideremoveicon: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.datasource = [];

            ctrl.label = "Country";
            if ($attrs.ismultipleselection != undefined) {
                ctrl.label = "Countries";
            }


            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            $scope.addNewCountry = function () {
                var onCountryAdded = function (countryObj) {
                    ctrl.datasource.push(countryObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(countryObj.Entity);
                    else
                        ctrl.selectedvalues = countryObj.Entity;
                };
                VRCommon_CountryService.addCountry(onCountryAdded);
            };

            ctrl.haspermission = function () {
                return VRCommon_CountryAPIService.HasAddCountryPermission();
            };

            var ctor = new countryCtor(ctrl, $scope, $attrs);
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
            return getCountryTemplate(attrs);
        }

    };

    function getCountryTemplate(attrs) {

        var multipleselection = "";
        if (attrs.ismultipleselection != undefined) {
            multipleselection = "ismultipleselection";
        }
    

        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewCountry"';

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : undefined;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"    ><vr-select  on-ready="scopeModel.onSelectorReady" ' + multipleselection + '  datatextfield="Name" datavaluefield="CountryId" isrequired="ctrl.isrequired"'
            + ' label="{{ctrl.label}}" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Country" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" haspermission="ctrl.haspermission"' + hideremoveicon + '>'
               
            +'</vr-select></vr-columns>';
    }

    function countryCtor(ctrl, $scope, attrs) {

    	var selectorAPI;

        function initializeController() {

        	$scope.scopeModel = {};

        	$scope.scopeModel.onSelectorReady = function (api) {
        		selectorAPI = api;
        		defineAPI();
        	};

        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
            	selectorAPI.clearDataSource();

            	var selectedIds;
            	var filter;

            	if (payload != undefined) {

            	    if (payload.fieldTitle != undefined) {
            	        ctrl.label = payload.fieldTitle;
            	    }

                	selectedIds = payload.selectedIds;
                	filter = payload.filter;
                }
                return getCountriesInfo(attrs, ctrl, selectedIds, filter);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('CountryId', attrs, ctrl);
            };

            api.getAllCountries = function () {
                var allCountries;
                if (ctrl.datasource != undefined) {
                    allCountries = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        var country = ctrl.datasource[i];
                        allCountries.push(country);
                    }
                }
                return allCountries;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    function getCountriesInfo(attrs, ctrl, selectedIds, filter) {
        return VRCommon_CountryAPIService.GetCountriesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
            });
            if (selectedIds != undefined) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'CountryId', attrs, ctrl);
            }
        });
    }

    return directiveDefinitionObject;
}]);