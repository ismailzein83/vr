'use strict';


var app = angular.module('mainModule', ['appControllers', 'appRouting', 'ngCookies'])
.controller('mainCtrl', ['$scope','$rootScope','VR_Sec_MenuAPIService','SecurityService','BaseAPIService','VR_Sec_PermissionAPIService','notify','$animate','$cookies','$timeout','MenuItemTypeEnum','UtilsService','VRModalService',
    function mainCtrl($scope, $rootScope, VR_Sec_MenuAPIService, SecurityService, BaseAPIService, VR_Sec_PermissionAPIService, notify, $animate, $cookies, $timeout, MenuItemTypeEnum, UtilsService, VRModalService) {
    Waves.displayEffect();
    $rootScope.setCookieName = function (cookieName) {
        if (cookieName != undefined && cookieName != '')
            SecurityService.setAccessCookieName(cookieName);

        var userInfo = SecurityService.getLoggedInUserInfo();

        if (userInfo === undefined) {
            SecurityService.redirectToLoginPage();
            return;
        }
        $scope.userDisplayName = userInfo.UserDisplayName;
    };
    $rootScope.setLoginURL = function (loginURL) {
        if (loginURL != undefined && loginURL != '') {
            SecurityService.setLoginURL(loginURL);
            BaseAPIService.setLoginURL(loginURL);
        }
    };
    $scope.obj = {};
    $scope.obj.testValue = "";
    $scope.obj.testValidate = function () {
        if ($scope.obj.testval == undefined && $scope.obj.testval2 == undefined)
            return "req val";
        return null;
    };
    var testApi;
    $scope.onReadTest = function (api) {
        testApi = api;
    };
    
    $scope.getTestApiData = function () {
        console.log(testApi.getData());
    }

    $rootScope.onValidationMessageShown = function (e) {      
        var self = angular.element(e.currentTarget);
        var selfHeight = $(self).height();
        var TophasLable = $(self).parent().attr('label') != undefined ? 0 : (($(self).parents('.dropdown-container2').length > 0)) ? -10 : -15;
        var topVar = ($(self).parents('.dropdown-container2').length > 0) ? (selfHeight / 3) - 5 : (selfHeight / 3);
        var selfWidth = $(self).width();
        var selfOffset = $(self).offset();
        var elleft = selfOffset.left - $(window).scrollLeft() + $(self).width() ;
        var left = 0;
        var tooltip = self.parent().find('.tooltip-error');
        if (innerWidth - elleft  < 100) {
            elleft = elleft - (100 + $(self).width() + 10);
            $(tooltip).addClass('tooltip-error-right');
            $(tooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + topVar + TophasLable, left: elleft , width:100 })
        }
        else {
            $(tooltip).removeClass('tooltip-error-right');
            $(tooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + topVar + TophasLable, left: elleft })
        }
        e.stopPropagation();
    };
   
    VR_Sec_PermissionAPIService.GetEffectivePermissions().then(function (response) {
            $rootScope.effectivePermissionsWrapper = response;
        }
    );
    var dropdownHidingTimeoutHandlerc;
    $scope.showMenu = function (e) {
        var $this = angular.element(e.currentTarget);
        clearTimeout(dropdownHidingTimeoutHandlerc);
        if (!$this.hasClass('open')) {
            $('.dropdown-toggle', $this).dropdown('toggle');
            $($this).find('.dropdown-menu').first().stop(true, true).slideDown();
        }
    };
    $scope.hideMenu = function (e) {
        var $this = angular.element(e.currentTarget);
        dropdownHidingTimeoutHandlerc = setTimeout(function () {
            if ($this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
                $($this).find('.dropdown-menu').first().stop(true, true).slideUp();
            }
        }, 200);

    };
    $animate.enabled($('#sidebar-wrapper'));
    $animate.enabled(true, $('#collapsedmenu'));
    $scope.toogled = true;
    $scope.toggledpanel = function () {
        $scope.toogled = !$scope.toogled;
    };
    $scope.getPageName = function () {
        if ($scope.currentPage != null)
            return $scope.currentPage.Name;
        else
            return "Home Page";
    };
    $scope.pinned = true;
    $scope.pinpanel = function () {
        $scope.pinned = !$scope.pinned;
    };
    $scope.showToogled = function () {
        $scope.toogled = true;
        $rootScope.$broadcast("menu-full");
    };
    $scope.hideToogled = function () {
        $scope.toogled = false;
        $rootScope.$broadcast("menu-collapsed");

    };
    var timer;
    $scope.show = false;
    $scope.mouseover = function () {
        if ($scope.pinned == true)
            return;
        $timeout.cancel(timer);
        $scope.toogled = true;
        $rootScope.$broadcast("menu-full");
    };
    $scope.mouseout = function () {
        if ($scope.pinned == true)
            return;
       timer = $timeout(function () {
           $scope.toogled = false;
           $rootScope.$broadcast("menu-collapsed");
       }, 500);

    };
    $scope.logout = function () {
        SecurityService.deleteAccessCookie();
        SecurityService.redirectToLoginPage(true);
    };
    $scope.openSupportModal = function () {
        var modalSettings = {
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Support";
        };
        VRModalService.showModal('/Client/Modules/Common/Views/Support.html', null, modalSettings);
    };
    $scope.openResetPasswordModal = function () {

        var modalSettings = {
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Change my Password";
        };
        VRModalService.showModal('/Client/Modules/Security/Views/User/ChangePassword.html', null, modalSettings);
    };

    $scope.openEditProfileModal = function () {
        var modalSettings = {
        };
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit my profile";
            modalScope.onProfileUpdated = function (response) {
                $scope.userDisplayName = response.Name;
            };
        };
        VRModalService.showModal('/Client/Modules/Security/Views/User/EditProfile.html', null, modalSettings);
    };
   
    $scope.menuItemsCurrent = null;
   
    $scope.setIndex = function (item) {
        if ($scope.menuItemsCurrent != null && $scope.menuItemsCurrent.Id == item.Id) {
            $scope.menuItemsCurrent = null;

        }
        else {
            $scope.menuItemsCurrent = item;
        }
    };
    $scope.menusubItemsCurrent = null;
    $scope.setIndexSub = function (o) {
        if ($scope.menusubItemsCurrent != null && $scope.menusubItemsCurrent.Id == o.Id) {
            $scope.menusubItemsCurrent = null;
        }
        else {
            $scope.menusubItemsCurrent = o;
        }
    };
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
    };
    $rootScope.toggletime = function (e) {
        //   e.preventDefault();
        //   e.stopPropagation();
        $('.datetime-controle').data("DateTimePicker").toggle();
        $('.date-section').removeClass('in');
        $('.time-section').addClass('in');
    };

    $scope.clock = ""; // initialise the time variable
    $scope.tickInterval = 1000; //ms
    
    var tick = function () {
        $scope.clock = Date.now(); // get the current time
        setTimeout(tick, $scope.tickInterval); // reset the timer
    };
    setTimeout(tick, $scope.tickInterval);

    var allMenuItems = [];   
    VR_Sec_MenuAPIService.GetMenuItems().then(function (response) {

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
        $scope.spinner = false;
    });
    
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
   
    $scope.currentPage = null;
    function setMenuItemSelectedFlag(menuItem, isSelected) {
        $scope.menusubItemsCurrent = null;
        menuItem.isSelected = isSelected;       
        if (menuItem.parent != null)
            setMenuItemSelectedFlag(menuItem.parent, isSelected);
        if (menuItem.isSelected == true && menuItem.Childs == null) {
            $scope.currentPage = menuItem;
            $scope.currentPage.Title = ($scope.currentPage.Title != null) ? $scope.currentPage.Title : $scope.currentPage.Name;
        }
            $scope.currentPage = menuItem;
        if (menuItem.parent == null && isSelected == true) {
            $scope.menuItemsCurrent = menuItem;
        }
           
        if (menuItem.parent != null && menuItem.Childs != null && isSelected == true) {
            $scope.menusubItemsCurrent = menuItem;
            
        }
            
        
    }
   
    var pathArray = location.href.split('/');
    var protocol = pathArray[0];
    var host = pathArray[2];
    $scope.baseurl = protocol + '//' + host;
    $scope.operatorAccountSelectionOption = 1;   
    var numberReg = /^\d+$/;
    $scope.isNumber = function (s) {
        return String(s).search(numberReg) != -1;
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
    };
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
   
}]);

app.controller('loginCtrl', function loginCtrl($scope, SecurityService) {
    $scope.setCookieName = function (cookieName) {
        if (cookieName != undefined && cookieName != '')
            SecurityService.setAccessCookieName(cookieName);
    };
});

angular.module('mainModule')
.config(function ($popoverProvider) {
    angular.extend($popoverProvider.defaults, {
        animation: 'am-flip-x',
        trigger: 'hover',
        autoClose: true,
        delay: { show: 1, hide: 100000 }
    });
})