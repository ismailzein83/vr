'use strict';
app.directive('vrCommonCountrySelector', ['VRCommon_CountryAPIService', 'VRCommon_CountryService', 'UtilsService', 'VRUIUtilsService',
    function (VRCommon_CountryAPIService, VRCommon_CountryService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "@",
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

                $scope.addNewCountry = function () {
                    var onCountryAdded = function (countryObj) {
                        return getAllCountries($attrs, ctrl, countryObj.Entity.CountryId); 
                    };
                    VRCommon_CountryService.addCountry(onCountryAdded);
                }


                var ctor = new countryCtor(ctrl, $scope, $attrs);
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
                return getCountryTemplate(attrs);
            }

        };


        function getCountryTemplate(attrs) {

            var multipleselection = "";
            var label = "Country";
            if (attrs.ismultipleselection != undefined) {
                label = "Countries";
                multipleselection = "ismultipleselection";
            }

            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";
            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewCountry"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="CountryId" '
            + required + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Country" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>'
        }

        function countryCtor(ctrl, $scope, attrs) {

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

                    return getAllCountries(attrs, ctrl, selectedIds);
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CountryId', attrs, ctrl);
                }

                api.getData = function () {
                    return $scope.selectedvalues;
                }
                api.getDataId = function () {
                    return $scope.selectedvalues.CountryId;
                }
                api.getIdsData = function () {
                    return getIdsList($scope.selectedvalues, "CountryId");
                }
                api.setData = function (selectedIds) {
                 
                    if (attrs.ismultipleselection != undefined) {
                        for (var i = 0; i < selectedIds.length; i++) {
                            var selectedCountryValue = UtilsService.getItemByVal(ctrl.datasource, selectedIds[i], "CountryId");
                            if (selectedCountryValue != null)
                                ctrl.selectedvalues.push(selectedCountryValue);
                        }
                    } else {
                        var selectedCountryValue = UtilsService.getItemByVal(ctrl.datasource, selectedIds, "CountryId");
                        if (selectedCountryValue != null)
                            ctrl.selectedvalues = selectedCountryValue;
                    }
                }
                function getIdsList(tab, attname) {
                    var list = [];
                    for (var i = 0; i < tab.length ; i++)
                        list[list.length] = tab[i][attname];
                    return list;

                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        function getAllCountries(attrs, ctrl, selectedIds) {
            return VRCommon_CountryAPIService.GetAllCountries().then(function (response) {
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