'use strict';


var app = angular.module('mainModule', ['appControllers', 'appRouting'])
.controller('mainCtrl', function mainCtrl($scope, $rootScope, notify, $animate) {
    Waves.displayEffect();
    var dropdownHidingTimeoutHandlerc;

    $animate.enabled($('#sidebar-wrapper'));
    $animate.enabled(false, $('#sidebar-wrapper'));
    $animate.enabled(false, $('#collapsedmenu'));

    $scope.toogled = true;
    $scope.toggledpanel = function () {
        $scope.toogled = !$scope.toogled;


    }

    $scope.showMenu = function (e) {

        var $this = angular.element(e.currentTarget);
        clearTimeout(dropdownHidingTimeoutHandlerc);
        if (!$this.hasClass('open')) {
            $('.dropdown-toggle', $this).dropdown('toggle');
            $($this).find('.dropdown-menu').first().stop(true, true).slideDown();
        }
    }
    $scope.hideMenu = function (e) {
        var $this = angular.element(e.currentTarget);
        dropdownHidingTimeoutHandlerc = setTimeout(function () {
            if ($this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
                $($this).find('.dropdown-menu').first().stop(true, true).slideUp();
            }
        }, 200);

    }
    $(window).resize(function () {
        var w = window.innerWidth;
        if (w >= 1200)
            $scope.toogled = true;
        else
            $scope.toogled = false;
    });
    $scope.menuItemsCurrent = -1;
    $scope.setIndex = function (i, e) {
        $('.panel-heading').removeClass('active-menu-parent');
        if ($scope.menuItemsCurrent == i) {
            $scope.menuItemsCurrent = -1;
        }
        else {
            $scope.menuItemsCurrent = i;
            var $this = angular.element(e.currentTarget);
            $this.addClass('active-menu-parent');
        }

    }
    $scope.menusubItemsCurrent = -1;
    $scope.setIndexSub = function (i, e) {
        if ($scope.menusubItemsCurrent == i) {
            $scope.menusubItemsCurrent = -1;
        }
        else {
            $scope.menusubItemsCurrent = i;

        }

    }
    $scope.parent = null;
    $scope.child = null;
    $scope.setActiveClass = function (e, p, c) {
        if (e != null) {
            $('.menu-item-list-ligth').removeClass('active-menu');
            var $this = angular.element(e.currentTarget);
            $this.addClass('active-menu');
        }
        $scope.parent = p;
        $scope.child = c;

    }
    $rootScope.hisnav = [];
    $rootScope.showsubview = function (page) {
        $rootScope.$broadcast(page + "-show");

    }
    $rootScope.hidesubview = function (page, $index) {
        if ($index == 0) {
            $rootScope.hisnav.length = 0;
        }
        else {
            $rootScope.hisnav.pop();
        }
        setTimeout(function () {
            $rootScope.$broadcast(page);
        }, 10)

    }
    $rootScope.ishideView = function (val) {
        var obj = $scope.findExsiteObj($rootScope.hisnav, val, 'name')
        if (obj != null) {
            return obj.show;
        }
        else return false;

    }
    $scope.menuItems = [
        {
            name: "Routing", location: '', icon: 'glyphicon-certificate', childs: [
               { name: "Rule Management", location: '#/RouteRuleManager' },
               { name: "Rate Plan", location: '#/RatePlanning' },
               { name: "Route Manager", location: '#/RouteManager' }

            ]
        },
        {
            name: "BI", location: '', icon: 'glyphicon-cog', childs: [
               { name: "Top Management Dashboard", location: '#/BI/TopManagementDashboard' },
                { name: "Senior Management Dashboard", location: '#/BI/SeniorManagementDashboard' },
                { name: "Top Destination Dashboard", location: '#/BI/ZoneDashboard' },
                { name: "Dynamic Dashboard", location: '#/BI/DynamicDashboard' },
                 { name: "Entity Report", location: '#/BI/EntityReport' }
            ]
        },
        {
            name: "BusinessEntity", location: '', icon: 'glyphicon-cog', childs: [
               { name: "DumySwitchs", location: '#/BusinessEntity/DumySwitchs' },
            { name: "Switch Managments", location: '#/BusinessEntity/Switch Managments' },
            { name: "CarrierAccount Managments", location: '#/BusinessEntity/CarrierAccount Managments' }
            ]
        },
        {
            name: "NOC", location: '', icon: 'glyphicon-flash', childs: [
               { name: "Zone Monitor", location: '#/NOC/ZoneMonitor' },
               { name: "Variation Reports", location: '#/NOC/VariationReports' }
            ]
        },
        {
            name: "Security", location: '', icon: 'glyphicon-flash', childs: [
               { name: "Roles", location: '#/Security/RoleManagement' }
            ]
        },
        {
            name: "Others", icon: 'glyphicon-pencil', location: '', childs: [
               { name: "Default", location: '#/Default' },
                { name: "Test View", location: '#/TestView' },
                { name: "Strategy Management", location: '#/Strategy' },
                { name: "ZingChart", location: '#/ZingChart' },
                { name: "HighChart", location: '#/HighChart' },
                { name: "HighChartSparkline", location: '#/HighChartSparkline' },
                { name: "FusionChart", location: '#/FusionChart' },
                { name: "CanvasJSChart", location: '#/CanvasJSChart' },
                { name: "AMChart", location: '#/AMChart' },
                { name: "Tree", location: '#/Tree' },
                {
                    name: "For test", location: '', childs: [
                         { name: "test1", location: '#/CanvasJSChart' },
                         { name: "test2", location: '#/AMChart' },
                    ]
                },
                 {
                     name: "For test 22", location: '', childs: [
                          { name: "an other 1", location: '#/CanvasJSChart' },
                          { name: "an other 2 ", location: '#/AMChart' },
                     ]
                 }
            ]
        }
    ];

    var pathArray = location.href.split('/');
    var protocol = pathArray[0];
    var host = pathArray[2];
    $scope.baseurl = protocol + '//' + host;
    $scope.carrierAccountSelectionOption = 1;

    $scope.findExsite = function (arr, value, attname) {
        var index = -1;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i][attname] == value) {
                index = i
            }
        }
        return index;
    }
    $scope.findExsiteObj = function (arr, value, attname) {
        var obj = null;
        for (var i = 0; i < arr.length; i++) {
            if (arr[i][attname] == value) {
                obj = arr[i];
            }
        }
        return obj;
    }
    var numberReg = /^\d+$/;
    $scope.isNumber = function (s) {
        return String(s).search(numberReg) != -1
    };
    $scope.dateToString = function (date) {
        var dateString = '';
        if (date) {

            var day = "" + (parseInt(date.getDate()));
            if (day.length == 1)
                dateString += "0" + day;
            else
                dateString += day;
            var month = "" + (parseInt(date.getMonth()) + 1);
            if (month.length == 1)
                dateString += "/0" + month;
            else
                dateString += "/" + month;
            dateString += "/" + date.getFullYear();
        }
        return dateString;
    }
    var dateReg = /^(0?[1-9]|[12][0-9]|3[01])\/(0?[1-9]|1[012])\/((199\d)|([2-9]\d{3}))$/;
    $scope.isDate = function (s) {
        var d = "";
        if (s && (s instanceof Date)) {
            var d = $scope.dateToString(s);
        }
        else d = s;
        var res = String(d).search(dateReg) != -1;
        return res;
    }
    $scope.testDate = function (s) {
        var res;
        var d = "";
        if (s == '' || s == null) {
            return 0
        }
        else if (s != '' || s == undefined) {
            // alert(s)
            if (s && (s instanceof Date)) {
                var d = $scope.dateToString(s);
            }
            else d = s;
            var test = String(d).search(dateReg) != -1;
            if (test)
                return 1;
            else
                return 2
        }



    }



});
angular.module('mainModule')
.config(function ($timepickerProvider) {
    angular.extend($timepickerProvider.defaults, {
        timeFormat: 'HH:mm:ss',
        length: 7,
        minuteStep: 1,
        animation: ""
    });
})
.config(function ($datepickerProvider) {
    angular.extend($datepickerProvider.defaults, {
        dateFormat: 'dd/MM/yyyy',
        startWeek: 1,
        animation: ""
    });
})
.config(function ($popoverProvider) {
    angular.extend($popoverProvider.defaults, {
        animation: 'am-flip-x',
        trigger: 'hover',
        autoClose: true,
        delay: { show: 1, hide: 100000 }
    });
})
