﻿'use strict';


var app = angular.module('mainModule', ['appControllers', 'appRouting', 'ngCookies'])
.controller('mainCtrl', function mainCtrl($scope, $rootScope, MenuAPIService, PermissionAPIService, notify, $animate, $cookies, $timeout, MenuItemTypeEnum, UtilsService) {
    
    var cookieUserToken = $cookies['TOne_LoginTokenCookie'];

    if (cookieUserToken == undefined)
    {
        window.location.href = '/Security/Login';
    }

    PermissionAPIService.GetEffectivePermissions().then(function (response) {
        $rootScope.effectivePermissionsWrapper = response;
    }
    );

    $scope.userDisplayName = $cookies['TOne_LoginUserDisplayNameCookie'];

    Waves.displayEffect();
    var dropdownHidingTimeoutHandlerc;

    $animate.enabled($('#sidebar-wrapper'));
    //$animate.enabled(false, $('#sidebar-wrapper'));
    //$animate.enabled(true, $('#collapsedmenu'));

    $scope.toogled = true;
    $scope.toggledpanel = function () {
        $scope.toogled = !$scope.toogled;


    }
    $scope.logout = function () {
        window.location.href = '/Security/Login';
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
    $scope.menuItemsCurrent = null;
    $scope.setIndex = function (item, e) {
        var $this = angular.element(e.currentTarget);
         
        if ($scope.menuItemsCurrent != null && $scope.menuItemsCurrent.Id == item.Id) {
            $($this).parent().find('.panel-body').first().stop(true, true).slideUp(500);
            $scope.menuItemsCurrent = null;

        }
        else {            
            $($this).parent().find('.panel-body').first().stop(true, true).slideDown(500);
            $scope.menuItemsCurrent = item;
        }

    }
    $scope.menusubItemsCurrent = null;
    $scope.setIndexSub = function (o) {
        if ($scope.menusubItemsCurrent != null && $scope.menusubItemsCurrent.Id == o.Id) {
            $scope.menusubItemsCurrent = null;
        }
        else {
            $scope.menusubItemsCurrent = o;
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
        var obj = $scope.findExisteObj($rootScope.hisnav, val, 'name')
        if (obj != null) {
            return obj.show;
        }
        else return false;

    }  
    $scope.clock = ""; // initialise the time variable
    $scope.tickInterval = 1000 //ms

    var tick = function () {
        $scope.clock = Date.now() // get the current time
        $timeout(tick, $scope.tickInterval); // reset the timer
    }
    $timeout(tick, $scope.tickInterval);

    var allMenuItems = [];   
    MenuAPIService.GetMenuItems().then(function (response) {
        angular.forEach(response, function (value, key, itm) {
            value.keyclass = key % 16;
            value.isSelected = false;
            value.parent = null;
            allMenuItems.push(value);
            matchParentNode(value);
          
        });    
        $scope.menuItems = response;
        if (currentURL != undefined)
            setSelectedMenuFromURL();
    })
    function matchParentNode(obj) {
       
        if (obj.Childs != null) {
            angular.forEach(obj.Childs, function (value, key, itm) {               
                value.parent = obj ;
                value.isSelected = false;
                allMenuItems.push(value);
                matchParentNode(value);               
            });
          

        }       

    }

    var selectedMenuItem;
    var currentURL;
    $rootScope.$on('$locationChangeSuccess', function (evnt, newURL) {
        var decodedURL = decodeURIComponent(newURL);
        currentURL = decodedURL.substring(decodedURL.indexOf('#'), decodedURL.length);
        setSelectedMenuFromURL();
    });

    function setSelectedMenuFromURL() {
        if (currentURL == undefined || allMenuItems.length == 0)
            return;
        var matchMenuItem = UtilsService.getItemByVal(allMenuItems, currentURL, "Location");
        if (selectedMenuItem != undefined)
            setMenuItemSelectedFlag(selectedMenuItem, false);
        if (matchMenuItem != null) {
            selectedMenuItem = matchMenuItem;
            setMenuItemSelectedFlag(selectedMenuItem, true);
        }
    }
    $scope.breadcrumb = {};

    function setMenuItemSelectedFlag(menuItem, isSelected) {
        $scope.menusubItemsCurrent = null;
        menuItem.isSelected = isSelected;       
        if (menuItem.parent != null)
            setMenuItemSelectedFlag(menuItem.parent, isSelected);
        if (menuItem.isSelected == true)
            $scope.breadcrumb.leaf = menuItem;
        if (menuItem.parent == null && isSelected == true) {
            $scope.menuItemsCurrent = menuItem;
            $scope.breadcrumb.parent = menuItem;
        }
           
        if (menuItem.parent != null && menuItem.Childs != null && isSelected == true) {
            $scope.menusubItemsCurrent = menuItem;
            $scope.breadcrumb.subparent = menuItem;
        }
            
        
    }
   
    var pathArray = location.href.split('/');
    var protocol = pathArray[0];
    var host = pathArray[2];
    $scope.baseurl = protocol + '//' + host;
    $scope.carrierAccountSelectionOption = 1;   
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
   
});

app.controller('loginCtrl', function loginCtrl($scope) {
    
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
