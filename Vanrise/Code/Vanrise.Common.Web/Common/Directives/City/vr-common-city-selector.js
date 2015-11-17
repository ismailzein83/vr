'use strict';
app.directive('vrCommonCitySelector', ['VRCommon_CityAPIService', 'VRCommon_CityService', 'UtilsService', 'VRUIUtilsService',
    function (VRCommon_CityAPIService, VRCommon_CityService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                type: "=",
                onReady: '=',
                label: "@",
                ismultipleselection: "@",
                hideselectedvaluessection: '@',
                onselectionchanged: '=',
                isrequired: '@',
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
                $scope.onselectionchanged = function () {
                    ctrl.selectedvalues = ctrl.selectedvalues;
                    if (ctrl.onselectionchanged != undefined) {
                        var onvaluechangedMethod = $scope.$parent.$eval(ctrl.onselectionchanged);
                        if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                            onvaluechangedMethod();
                        }
                    }

                }

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

            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";
            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewCity"';

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="CityId" '
            + required + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="City" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
               + '</div>';
        }
        function City(ctrl, $scope, attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};


                api.getData = function () {
                    return ctrl.selectedvalues;
                }
                api.getDataId = function () {
                    return ctrl.selectedvalues.CityId;
                }
                api.getIdsData = function () {
                    return getIdsList(ctrl.selectedvalues, "CityId");
                }
               
                function getIdsList(tab, attname) {
                    var list = [];
                    for (var i = 0; i < tab.length ; i++)
                        list[list.length] = tab[i][attname];
                    return list;

                }
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CityId', $attrs, ctrl);
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

