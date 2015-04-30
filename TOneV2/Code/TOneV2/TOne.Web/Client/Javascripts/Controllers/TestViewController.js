'use strict'

var TestViewController = function ($scope, CarriersService, ZonesService, RoutingAPIService) {
    var ctrl = this;
    load();
    loadZone();
    //loadCarriers();

    ctrl.selectedRoutes = function (items, item, data) {

    }

    ctrl.onsearch = function (text) {
        return ZonesService.getSalesZones(text);
    }

    ctrl.onready = function (api) {
        //ZonesService.getSalesZones("Lebanon").then(function (items) {
        //    api.setSelectedValues(items);
        //}, function (msg) {
        //    console.log(msg);
        //});

        
    }
    $scope.gridOptionsRouteRule = {
        expandableRowTemplate: '<div style="height:20px;"> test</div>',
        expandableRowHeight: 20,
        enableVerticalScrollbar: 2,
        //subGridVariable will be available in subGrid scope
        expandableRowScope: {
            subGridVariable: 'subGridScopeVariable'
        },
        columnDefs: [
          {
              name: 'Carrier Account', field: 'CarrierAccountDescription', height: 40, enableHiding: false, enableColumnMenu: false, width: 300,
              cellTooltip: function (row, col) {
                  return row.entity.CarrierAccountDescription;
              },
          },
          {
              name: 'Code Set', enableColumnMenu: false, field: 'CodeSetDescription', height: 40, width: 300, enableHiding: false,
              cellTooltip: function (row, col) {
                  return row.entity.CodeSetDescription;
              },
          },
          { name: 'Rule Type', enableColumnMenu: false, field: 'ActionDescription', height: 40, width: 100, enableHiding: false },
          { name: 'Start Date', enableColumnMenu: false, field: 'BeginEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 100, enableHiding: false },
          { name: 'End Date', enableColumnMenu: false, field: 'EndEffectiveDate', cellFilter: 'date:"yyyy-MM-dd "', height: 40, width: 100, enableHiding: false }//,
        ],
        enableColumnResizing: true,
        enableSorting: false,
        onRegisterApi: function (gridApi) {
            //gridApi.expandable.on.rowExpandedStateChanged($scope, function (row) {
            //    var h = angular.element(document.getElementById('gridtest')).css('height');
            //    var realheigth = parseInt(h.substring(0, h.length - 2));
            //    alert(realheigth)
            //    if (row.isExpanded) {
            //        realheigth = realheigth + 20;
            //    }
            //    else {
            //        realheigth = realheigth - 20;
            //    }

            //    angular.element(document.getElementById('gridtest')).css('height', realheigth + 'px');
            //});
        }

    };
    var pageSize = 40;
    var page = 0;
    var pageUp = 0;
    var last = false;

    $scope.columns = [
        { displayname: 'last', name: 'EndEffectiveDate' },
        { displayname: 'Carrier Account', name: 'CarrierAccountDescription' },
        { displayname: 'Code Set', name: 'CodeSetDescription' },
        { displayname: 'last', name: 'EndEffectiveDate' },
        { displayname: 'Carrier Account', name: 'CarrierAccountDescription' },
        { displayname: 'Code Set', name: 'CodeSetDescription' },
         { displayname: 'last', name: 'EndEffectiveDate' },
        { displayname: 'Carrier Account', name: 'CarrierAccountDescription' },
        { displayname: 'Code Set', name: 'CodeSetDescription' }
    ]
    $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
    $scope.tabdata = []
    RoutingAPIService.getAllRouteRule(page, pageSize)
        .then(function (data) {             
            $scope.gridOptionsRouteRule.data = data;
            $scope.tabdata = data;
        })
    var lastScrollTop = 0;
    var datainloded = false;
    $(".grid-body-container").scroll(function () {

        var st = $(this).scrollTop();
        var scrollPercentage = 100 * st / ($('#grid-canvas').height() - $(this).height());

        if (st > lastScrollTop) {
            if (scrollPercentage > 60 && !datainloded)
                loaddata()
        } else {
            console.log('u' +st +" // " + scrollPercentage)
        }
        lastScrollTop = st;
    });
    function loaddata() {
        if (last == false) {
            page = page + 1;
            datainloded = true;
            RoutingAPIService.getAllRouteRule(page, pageSize)
            .then(function (data) {
                $scope.tabdata = $scope.tabdata.concat(data);
                last = (data.length < pageSize) ? true : false;
                datainloded = false;

            })
        }

    }

    $scope.getwidth = function () {             
        var gridwidth = $('#gridtest').width();
        return (gridwidth / $scope.columns.length);
    }
    //$scope.increaseWidth = function (c) {
       
    //    var w = c.colwidth * 1.5;
    //    var gridwidth = $('#gridtest').width() - w ;
    //    var ow = (gridwidth / ($scope.columns.length - 1));
    //    console.log(ow + ' // ' + w)
    //    angular.forEach($scope.columns, function (col) {
    //        if (col.name != c.name)
    //            col.colwidth = ow;
            

    //    });
    //    col.colwidth = w;
    //}

    function load() {
        ctrl.model = 'Test View model';
        ctrl.Input1 = '';
        ctrl.Input2 = '';
        ctrl.data = [{ name: "Moroni", age: 50 },
                    { name: "Tiancum", age: 43 },
                    { name: "Jacob", age: 27 }];

        ctrl.routes = [{ name: "Moroni", value: 50 },
            { name: "Tiancum", value: 43 },
            { name: "Jacob", value: 27 }];

        ctrl.routesOptions = {
            lastselectedvalue: {},
            datasource: [{ name: "Moroni", value: 50 },
            { name: "Tiancum", value: 43 },
            { name: "Jacob", value: 27 }]
        };

        ctrl.options = {
            selectedvalues: [],
            lastselectedvalue: {},
            datasource: []
        };

        ctrl.customvalidate1 = function (length) {
            console.log(length);
            var isvalid = false;
            if (length == 2) isvalid = true;

            if (isvalid) return '';
            return 'Maximum length : 2';
        }

        ctrl.customvalidate2 = function (api) {

            var isvalid = false;
            if (api == undefined || api.lastselectedvalue == undefined) isvalid = false;
            else if (api.lastselectedvalue.Name == 'TEST') isvalid = true;

            if (isvalid) return '';
            return 'Name : TEST';
        }

        ctrl.validationGroup = [];

        ctrl.optionsFilter = {
            selectedvalues: [],
            datasource: ['5', '10', '15'],
            onselectionchanged: function (selectedvalues, datasource) {
                console.log(selectedvalues);
                console.log(datasource);
            }
        };

        ctrl.optionsZone = {
            selectedvalues: { ZoneId: 3, Name: "Afghanistan-Mobile" },
            datasource: [],
            onselectionchanged: function (selectedvalues, datasource) {
                console.log(selectedvalues[0]);
                console.log(datasource);
            }
        };

        ctrl.optionsZoneRemote = {
            selectedvalues: [],
            datasource: function (text) {
                console.log(text);
                return ZonesService.getSalesZones(text);
            },
            onselectionchanged: function (selectedvalues, datasource) {
                console.log(selectedvalues);
                console.log(datasource);
            }
        };

        ctrl.alertMsg = function () {

        };
        
        ctrl.validateForm = function () {
            if (ctrl.api == undefined || ctrl.api.isvalid == undefined) return true;
            return ! ctrl.api.isvalid();
        };
        
    }
    
    function loadZone() {
        ZonesService.getSalesZones("L").then(function (items) {
            ctrl.optionsZone.datasource = items;
        }, function (msg) {
            console.log(msg);
        });
    }

    function loadCarriers() {
        return CarriersService.getCustomers().then(function (customers) {
            ctrl.options.datasource = customers;
        }, function (msg) {
            console.log(msg);
        });
    }
    
}

TestViewController.$inject = ['$scope', 'CarriersService', 'ZonesService', 'RoutingAPIService'];

appControllers.controller('TestViewController', TestViewController);