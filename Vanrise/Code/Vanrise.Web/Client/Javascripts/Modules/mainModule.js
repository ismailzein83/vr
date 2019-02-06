'use strict';

var app = angular.module('mainModule', ['appControllers', 'appRouting', 'ngCookies'])
    .controller('mainCtrl', ['$scope', '$rootScope', 'VR_Sec_MenuAPIService', 'SecurityService', 'BaseAPIService', 'VR_Sec_PermissionAPIService', 'notify', '$cookies', '$timeout', 'MenuItemTypeEnum', 'UtilsService', 'VRModalService', 'VRNavigationService', 'UISettingsService', '$location', '$window', "VRLocalizationService", "Sec_CookieService", "VRNotificationService", "VR_Sec_RegisteredApplicationAPIService", "MobileService", "MasterLayoutMenuOptionEnum",
        function mainCtrl($scope, $rootScope, VR_Sec_MenuAPIService, SecurityService, BaseAPIService, VR_Sec_PermissionAPIService, notify, $cookies, $timeout, MenuItemTypeEnum, UtilsService, VRModalService, VRNavigationService, UISettingsService, $location, $window, VRLocalizationService, Sec_CookieService, VRNotificationService, VR_Sec_RegisteredApplicationAPIService, MobileService, MasterLayoutMenuOptionEnum) {


            (function () {
                document.getElementById("mainBodyContainer").style.display = "block";
            })();
            $scope.isMobile = MobileService.isMobile();
            if (!$scope.isMobile) {
                Waves.displayEffect();

            }

            $rootScope.addFocusPanelClass = function (e) {
                var clickedPanel = e.currentTarget;
                $('.panel-vr').each(function (index) {
                    if ($(this).prop('id') != $(clickedPanel).prop('id'))
                        $(this).removeClass('focus-panel');
                });
                $(clickedPanel).parent().addClass('focus-panel');
                $rootScope.$broadcast("hide-all-menu");
                $rootScope.$broadcast("hide-all-select");
                e.stopPropagation();
            };

            $rootScope.$on("$destroy", function () {
                $(window).off("resize.Viewport");
            });
            $scope.$on("$destroy", function () {
                $(window).off("resize.Viewport");
            });

            $rootScope.setCookieName = function (cookieName) {
                if (cookieName != undefined && cookieName != '')
                    SecurityService.setAccessCookieName(cookieName);

                var userInfo = SecurityService.getLoggedInUserInfo();

                if (userInfo === undefined) {
                    SecurityService.redirectToLoginPage();
                    return;
                }
                if ($cookies.get("passwordExpirationDays") != undefined) {
                    VRNotificationService.showWarning("Your password expires in " + userInfo.PasswordExpirationDaysLeft + " day(s).");
                    $cookies.put("passwordExpirationDays", undefined);
                }
                $scope.userDisplayName = userInfo.UserDisplayName;
                $scope.userPhotoFileId = userInfo.PhotoFileId;
                $scope.supportPasswordManagement = userInfo.SupportPasswordManagement;
                $scope.securityProviderId = userInfo.SecurityProviderId;
                //getRemoteRegisteredApplicationsInfo(userInfo);
            };

            //var getRemoteRegisteredApplicationsInfo = function (userInfo) {
            //    var securityProviderId = userInfo.SecurityProviderId;
            //    $scope.isLoadingRemoteApplications = true;

            //    VR_Sec_RegisteredApplicationAPIService.GetRemoteRegisteredApplicationsInfo(securityProviderId).then(function (response) {
            //        if (response != undefined) {
            //            $scope.remoteApplications = response;
            //        }
            //    }).catch(function (error) {
            //        VRNotificationService.notifyException(error, $scope);
            //    }).finally(function () {
            //        $scope.isLoadingRemoteApplications = false;
            //    });
            //};

            //$scope.redirectToApplication = function (remoteApplication) {
            //    return SecurityService.redirectToApplication(remoteApplication.URL);
            //}

            $rootScope.setLocalizationEnabled = function (isLocalizationEnabled) {
                if (isLocalizationEnabled != undefined) {
                    VRLocalizationService.setLocalizationEnabled(isLocalizationEnabled);
                }
            };
            $rootScope.setLocalizationRTL = function (isRTL) {
                if (isRTL != undefined) {
                    VRLocalizationService.setLocalizationRTL(isRTL);
                }
            };

            $rootScope.setLoginURL = function (loginURL) {
                if (loginURL != undefined && loginURL != '') {
                    SecurityService.setLoginURL(loginURL);
                    BaseAPIService.setLoginURL(loginURL);
                }
            };
            $rootScope.setVersionNumber = function (version) {
                $rootScope.version = version;
            };

            $rootScope.onValidationMessageShown = function (e) {
                var self = angular.element(e.currentTarget);
                var selfHeight = $(self).height();
                var TophasLable = $(self).parent().attr('label') != undefined ? 0 : (($(self).parents('.dropdown-container2').length > 0)) ? -10 : -15;
                var topVar = ($(self).parents('.dropdown-container2').length > 0) ? (selfHeight / 3) - 5 : (selfHeight / 3);
                var selfWidth = $(self).width();
                var selfOffset = $(self).offset();
                var elleft = selfOffset.left - $(window).scrollLeft() + $(self).width();
                var left = 0;
                var tooltip = self.parent().find('.tooltip-error');
                $(tooltip).removeClass('tooltip-error-right');
                if ($scope.isMobile) {
                    var initialTop = (selfOffset.top - $(window).scrollTop());
                    $(tooltip).css({ position: 'fixed', top: initialTop + selfHeight, left: selfOffset.left - $(window).scrollLeft() + (selfWidth / 3) });
                }
                else {
                    $(tooltip).removeClass('tooltip-error-right');
                    $(tooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + topVar + TophasLable, left: elleft });
                    if (innerWidth - elleft < 100 && !VRLocalizationService.isLocalizationRTL()) {
                        elleft = elleft - (100 + $(self).width() + 10);
                        $(tooltip).addClass('tooltip-error-right');
                        $(tooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + topVar + TophasLable, left: elleft, width: 100 });
                    }
                    else if (VRLocalizationService.isLocalizationRTL() && (selfOffset.left - $(window).scrollLeft()) > 100) {
                        elleft = elleft - (100 + $(self).width() + 10);
                        $(tooltip).addClass('tooltip-error-right');
                        $(tooltip).css({ position: 'fixed', top: selfOffset.top - $(window).scrollTop() + topVar + TophasLable, left: elleft, width: 100 });
                    }
                }

                e.stopPropagation();
            };

            VR_Sec_PermissionAPIService.GetEffectivePermissions().then(function (response) {
                $rootScope.effectivePermissionsWrapper = response;
            }
            );
            var dropdownHidingTimeoutHandlerc;
            $scope.currentMenuIndex = undefined;
            $scope.showMenu = function (e, id) {
                var $this = angular.element(e.currentTarget);
                $scope.currentMenuIndex = id;
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
                        $scope.currentMenuIndex = undefined;
                        $($this).find('.dropdown-menu').first().stop(true, true).slideUp();
                    }
                }, 200);
            };
            $scope.toogled = false;
            $scope.toggledpanel = function () {
                $scope.toogled = !$scope.toogled;
            };
            $scope.getPageName = function () {
                if ($scope.currentPage != null)
                    return $scope.currentPage.Title;
                else
                    return "Home Page";
            };
            $scope.pinned = true;
            $scope.pinpanel = function () {
                $scope.pinned = !$scope.pinned;
            };
            $rootScope.showToogled = function () {
                $scope.toogled = true;
                $rootScope.$broadcast("menu-full");
            };
            $rootScope.hideToogled = function () {
                $scope.toogled = false;
                $rootScope.$broadcast("menu-collapsed");

            };
            var timer;
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
                $('#mainNgView').off();
                $('#mainNgView').empty();
                $(window).off("resize.Viewport");
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

            $scope.openRemoteApplicationsModal = function () {
                var parameters = {
                    securityProviderId: $scope.securityProviderId
                };
                var modalSettings = {
                };
                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.title = "Switch Application";
                };
                VRModalService.showModal('/Client/Modules/Security/Views/Security/RemoteApplications.html', parameters, modalSettings);
            };
            $scope.openMobileUserActionPopup = function () {
                var modalSettings = {
                    autoclose: true
                };
                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.mainCtrl = $scope;
                };
                VRModalService.showModal('/Client/Modules/Security/Views/User/MobileUserActionPopup.html', null, modalSettings);
            };

            $scope.openEditProfileModal = function () {
                var modalSettings = {
                };
                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.title = "Edit my profile";
                    modalScope.onProfileUpdated = function (response) {
                        $scope.userDisplayName = response.Name;
                        $scope.userPhotoFileId = response.PhotoFileId;

                        var userInfo = SecurityService.getLoggedInUserInfo();

                        userInfo.PhotoFileId = response.PhotoFileId;
                        userInfo.UserDisplayName = response.Name;

                        Sec_CookieService.createAccessCookieFromAuthToken(userInfo);
                    };
                };
                VRModalService.showModal('/Client/Modules/Security/Views/User/EditProfile.html', null, modalSettings);
            };

            $scope.openEditMyLanguageModal = function () {
                var modalSettings = {
                };
                modalSettings.onScopeReady = function (modalScope) {
                    modalScope.title = "Edit My Language";
                    modalScope.onLanguageUpdated = function (languageId) {
                        var languageCookie = VRLocalizationService.getLanguageCookie();
                        if (languageCookie != languageId) {
                            VRLocalizationService.createOrUpdateLanguageCookie(languageId);
                            location.reload();
                        }
                    };
                };
                VRModalService.showModal('/Client/Modules/Security/Views/User/EditMyLanguage.html', null, modalSettings);
            };
            $scope.menuItemsCurrent = null;
            $scope.setIndex = function (item) {
                // $('.vr-menu-item').slideUp();
                if ($scope.menuItemsCurrent != null && $scope.menuItemsCurrent.Id == item.Id) {
                    $scope.menuItemsCurrent = null;
                    $('#collapse-' + item.Id).removeClass('vr-menu-item-selected');
                    $('#collapse-' + item.Id).removeAttr('style');
                }
                else {
                    $scope.menuItemsCurrent = item;
                    $('#collapse-' + item.Id).addClass('vr-menu-item-selected');
                    $('#collapse-' + item.Id).removeAttr('style');
                    $rootScope.seledtedModuleId = item.Id;
                    if (item.DefaultURL != null || $rootScope.showModuleTiles) {
                        $rootScope.goToModulePage(item);
                    }

                }
            };
            $scope.menusubItemsCurrent = null;
            $scope.setIndexSub = function (item) {
                if ($scope.menusubItemsCurrent != null && $scope.menusubItemsCurrent.Id == item.Id) {
                    $scope.menusubItemsCurrent = null;
                }
                else {
                    $scope.menusubItemsCurrent = item;
                    $rootScope.seledtedModuleId = item.Id;
                    if (item.DefaultURL != null || $rootScope.showModuleTiles) {
                        $rootScope.goToModulePage(item);
                    }
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
                $('.datetime-controle').data("DateTimePicker").toggle();
                $('.date-section').removeClass('in');
                $('.time-section').addClass('in');
            };
            var allMenuItems = [];
            (function () {
                if (!UISettingsService.isUISettingsHasValue()) {
                    UISettingsService.loadUISettings().then(function () {
                        var masterLayoutSettings = UISettingsService.getMasterLayoutSettings();
                        buildRootScopeMasterLayoutSettings(masterLayoutSettings);
                    });
                }
                else {
                    var masterLayoutSettings = UISettingsService.getMasterLayoutSettings();
                    buildRootScopeMasterLayoutSettings(masterLayoutSettings);
                }
            })();

            function buildRootScopeMasterLayoutSettings(obj) {
                $rootScope.masterLayoutSettings = obj;
                $scope.toogled = obj.ExpandedMenu;
                $rootScope.showApplicationTiles = obj.ShowApplicationTiles;
                $rootScope.showModuleTiles = obj.ShowModuleTiles;
                $rootScope.toTilesView = obj.TilesMode;
                $rootScope.toModuleTilesView = obj.ModuleTilesMode;
                $rootScope.evalApplicationViewToggle();
                switch (obj.MenuOption) {
                    case MasterLayoutMenuOptionEnum.FullMenu.value:
                        $rootScope.fullMenu = true;
                        $rootScope.hideToogleIcon = false;
                        $rootScope.showAllMenuItems = true;
                        break;
                    case MasterLayoutMenuOptionEnum.ModuleFilteredMenu.value:
                        $rootScope.moduleFilter = true;
                        $rootScope.hideToogleIcon = (window.location.href + "" == UISettingsService.getDefaultPageURl()) ? true : false;
                        $rootScope.showAllMenuItems = false;
                        break;
                    case MasterLayoutMenuOptionEnum.NoMenu.value:
                        $rootScope.noMenu = true;
                        $rootScope.hideToogleIcon = true;
                        $rootScope.showAllMenuItems = false;
                        $rootScope.hideToogled();
                        break;
                    default:
                        $rootScope.hideToogleIcon = false;
                        $rootScope.showAllMenuItems = true;
                        break;
                }
            }

            $rootScope.showApplicationViewToggle = true;
            $rootScope.showModuleViewToggle = true;
            $rootScope.evalApplicationViewToggle = function () {
                if (window.location.hash == "#/view/default")
                    return;
                $rootScope.showApplicationViewToggle = (
                    ($rootScope.showApplicationTiles == true && UISettingsService.getDefaultPageURl() != undefined && UISettingsService.getDefaultPageURl() != "" && window.location.href + "" == UISettingsService.getDefaultPageURl())
                    ||
                    ($rootScope.showApplicationTiles == true && UISettingsService.getDefaultPageURl() != undefined && UISettingsService.getDefaultPageURl() != "" && (window.location.hash + "") == "#/default")
                );
            };

            $rootScope.evalModuleViewToggle = function () {
                if (window.location.hash == "#/view/default")
                    return;

                $rootScope.showModuleViewToggle = (
                    ($rootScope.showModuleTiles == true && $rootScope.selectedtile && $rootScope.selectedtile.DefaultURL != undefined && window.location.hash.indexOf("ModuleDefault") > -1)
                    ||
                    ($rootScope.showModuleTiles == true && $rootScope.selectedtile && $rootScope.selectedtile.DefaultURL != undefined && decodeURIComponent(window.location.href) == VRNavigationService.getFullPageURl($rootScope.selectedtile.DefaultURL))
                        );
            };

            $rootScope.switchDefaultView = function (state) {
                if ($rootScope.toTilesView != state) {
                    $rootScope.toTilesView = state;
                    if (UISettingsService.getDefaultPageURl() && (!$rootScope.toTilesView || !$rootScope.showApplicationTiles)) {
                        window.location.href = UISettingsService.getDefaultPageURl();
                    }
                    else
                        VRNavigationService.goto("/default");
                }
            };

            $rootScope.switchModuleDefaultView = function (state) {
                if ($rootScope.toModuleTilesView != state) {
                    $rootScope.toModuleTilesView = state;
                    if ($rootScope.toModuleTilesView) {
                        var parameters = {
                            moduleId: $rootScope.selectedtile.Id
                        };
                        VRNavigationService.goto("/Security/Views/ModuleDefault", parameters);
                    }
                    else if ($rootScope.selectedtile.DefaultURL && (!$rootScope.toModuleTilesView || !$rootScope.showModuleTiles))
                        window.location.href = $rootScope.selectedtile.DefaultURL;
                }
            };

            VR_Sec_MenuAPIService.GetMenuItems().then(function (response) {
                angular.forEach(response, function (value, key, itm) {
                    value.keyclass = key % 16;
                    value.isSelected = false;
                    value.parent = null;
                    value.tileIcon = buildTileIconPath(value.Icon)
                    allMenuItems.push(value);
                    matchParentNode(value);

                    $rootScope.mainMenuReady = true;
                });
                $rootScope.menuItems = response;
                if (currentURL != undefined)
                    setSelectedMenuFromURL();
                $scope.spinner = false;
            });

            function matchParentNode(obj) {
                if (obj.Childs != null) {
                    angular.forEach(obj.Childs, function (value, key, itm) {
                        value.parent = obj;
                        value.isSelected = false;
                        allMenuItems.push(value);
                        matchParentNode(value);
                    });
                }

            }
            function buildTileIconPath(icon) {
                if (icon == null) return "";
                var iconpath = icon.substring(0, icon.lastIndexOf("."));
                var iconFileExtension = icon.substring(icon.lastIndexOf("."));
                return iconpath + "-tile" + iconFileExtension;
            }

            var selectedMenuItem;
            var currentURL;
            $rootScope.$on('$locationChangeStart', function (event, next, current) {
                if (returnToDefaultPage(next)) {
                    if (!UISettingsService.isUISettingsHasValue()) {
                        UISettingsService.loadUISettings().then(function () {
                            redirectToDefaultIfExist();
                        });
                    }
                    else
                        redirectToDefaultIfExist();
                }
                $('.vr-menu-item').removeClass('vr-menu-item-selected');
                $(window).off("resize.Viewport");
            });

            $rootScope.$on('$viewContentLoaded', function (event) {
                checkGoogleTracking();
                $rootScope.evalModuleViewToggle();
                // $('.navbar-toggle').click();
            });
            function checkGoogleTracking() {
                var status = UISettingsService.getGoogleTrackingStatus();
                if (status != undefined && status == true) {
                    $window.ga('send', 'pageview', $location.path());
                }
            }

            $rootScope.$on('$locationChangeSuccess', function (evnt, newURL) {
                var decodedURL = decodeURIComponent(newURL);
                currentURL = decodedURL.substring(decodedURL.indexOf('#'), decodedURL.length);
                setSelectedMenuFromURL();
                if (UISettingsService.isUISettingsHasValue()) {
                    $rootScope.evalApplicationViewToggle();
                }

            });
            function returnToDefaultPage(url) {
                var pathArray = location.href.split("/");
                var protocol = pathArray[0];
                var host = pathArray[2];
                var defaultUrl = protocol + "//" + host + "/#/default";
                return defaultUrl == url;
            }

            function redirectToDefaultIfExist() {
               
            }

            $rootScope.goToHomePage = function () {
                $scope.currentPage = null;
                $scope.menuItemsCurrent = null;
                $scope.menusubItemsCurrent = null;
                $rootScope.selectedtile = null;
                $rootScope.selectedMenu = null;
                $rootScope.seledtedModuleId = undefined;
                $rootScope.toTilesView = UISettingsService.getMasterLayoutDefaultViewTileState();
                $rootScope.toModuleTilesView = UISettingsService.getMasterLayoutModuleDefaultViewTileState();
                if ($rootScope.moduleFilter) {
                    $rootScope.hideToogleIcon = true;
                    $scope.toogled = false;
                }
                if (UISettingsService.getDefaultPageURl() && (!$rootScope.toTilesView || !$rootScope.showApplicationTiles)) {
                    window.location.href = UISettingsService.getDefaultPageURl();
                }
                else
                    VRNavigationService.goto("/default");
            };

            $rootScope.goToModulePage = function (item) {
                $scope.currentPage = null;
                $scope.menuItemsCurrent = null;
                $scope.menusubItemsCurrent = null;
                $rootScope.seledtedModuleId = undefined;
                if ($rootScope.moduleFilter) {
                    $rootScope.hideToogleIcon = false;
                }
                $scope.toogled = UISettingsService.getMasterLayoutMenuToogledState();
                if (item.Childs && item.Childs.length > 0) {
                    $rootScope.selectedtile = item;
                    $rootScope.seledtedModuleId = item.Id;
                    $rootScope.toModuleTilesView = UISettingsService.getMasterLayoutModuleDefaultViewTileState();
                    var parameters = {
                        moduleId: item.Id
                    };
                    if (item.parent === null)
                        $scope.menuItemsCurrent = item;
                    if (item.parent != null) {
                        $scope.menuItemsCurrent = item.parent;
                        $scope.menusubItemsCurrent = item
                    }
                    if (($rootScope.toModuleTilesView && $rootScope.showModuleTiles) || item.DefaultURL == null) {
                        var parameters = {
                            moduleId: item.Id
                        };
                        VRNavigationService.goto("/Security/Views/ModuleDefault", parameters);
                    }
                    else if (item.DefaultURL && (!$rootScope.toModuleTilesView || !$rootScope.showModuleTiles)) {
                        if ($rootScope.moduleFilter) {
                            $rootScope.hideToogleIcon = false;
                        }
                        window.location.href = item.DefaultURL;
                    }
                }
                else
                    window.location.href = item.Location;
            };
            $rootScope.menuFilter = function (item) {
                if ($rootScope.showAllMenuItems) return true;
                return (($rootScope.selectedMenu && $rootScope.selectedMenu.Id === item.Id) || ($rootScope.selectedMenu && $rootScope.selectedMenu.parent != null && $rootScope.selectedMenu.parent.Id === item.Id));
            };
            function setSelectedMenuFromURL() {
                if (currentURL == undefined || allMenuItems.length == 0)
                    return;
                var matchMenuItem = UtilsService.getItemByVal(allMenuItems, currentURL, "Location");
                var matchModule = UtilsService.getItemByVal(allMenuItems, currentURL, "DefaultURL");
                if (selectedMenuItem != undefined)
                    setMenuItemSelectedFlag(selectedMenuItem, false);
                if (matchModule != null) {
                    selectedMenuItem = matchModule;
                    $rootScope.selectedtile = matchModule;
                    $rootScope.selectedMenu = matchModule;
                    setMenuItemSelectedFlag(selectedMenuItem, true);
                    $rootScope.setSelectedMenuTile();
                }
                if (matchMenuItem != null) {
                    selectedMenuItem = matchMenuItem;
                    $rootScope.selectedMenu = matchMenuItem.parent;
                    setMenuItemSelectedFlag(selectedMenuItem, true);
                    $rootScope.setSelectedMenuTile();
                    //$rootScope.hideToogleIcon = false;
                }
                if ($('#navbar').attr("aria-expanded") == 'true') {
                    $('.navbar-toggle').click();
                }

            }
            $rootScope.setSelectedMenuTile = function () {
                if ($rootScope.seledtedModuleId != undefined) {
                    $scope.menuItemsCurrent = null;
                    $scope.menusubItemsCurrent = null;
                    var selectedModule = UtilsService.getItemByVal(allMenuItems, $rootScope.seledtedModuleId, "Id");
                    $rootScope.selectedMenu = selectedModule;
                    $rootScope.selectedtile = selectedModule;
                    if (selectedModule.parent === null)
                        $scope.menuItemsCurrent = selectedModule;
                    if (selectedModule.parent != null) {
                        $scope.menuItemsCurrent = selectedModule.parent;
                        $scope.menusubItemsCurrent = selectedModule
                    }
                }
                $rootScope.evalModuleViewToggle();
            };
            $scope.currentPage = null;

            function setMenuItemSelectedFlag(menuItem, isSelected) {
                $scope.menusubItemsCurrent = null;
                menuItem.isSelected = isSelected;
                if (menuItem.parent != null) {
                    setMenuItemSelectedFlag(menuItem.parent, isSelected);
                }

                if (menuItem.isSelected == true && menuItem.Childs == null) {
                    $scope.currentPage = menuItem;

                }

                if (menuItem.parent == null && isSelected == true) {
                    $scope.menuItemsCurrent = menuItem;
                    $('#collapse-' + menuItem.Id).addClass('vr-menu-item-selected');
                }

                if (menuItem.parent != null && menuItem.Childs != null && isSelected == true) {
                    $scope.menusubItemsCurrent = menuItem;
                }

            }

            var pathArray = location.href.split("/");
            var protocol = pathArray[0];
            var host = pathArray[2];
            $scope.baseurl = protocol + "//" + host;
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
            };

        }]);

app.controller('loginCtrl', ["$scope", "SecurityService", "$rootScope", "VRLocalizationService", "MobileService", function loginCtrl($scope, SecurityService, $rootScope, VRLocalizationService, MobileService) {
    $scope.isMobile = MobileService.isMobile();
    $scope.setCookieName = function (cookieName) {
        if (cookieName != undefined && cookieName != '')
            SecurityService.setAccessCookieName(cookieName);
    };
    $scope.setLocalizationEnabled = function (isLocalizationEnabled) {
        if (isLocalizationEnabled != undefined) {
            VRLocalizationService.setLocalizationEnabled(isLocalizationEnabled);
        }
    };

    $scope.setLocalizationRTL = function (isRTL) {
        if (isRTL != undefined) {
            VRLocalizationService.setLocalizationRTL(isRTL);
        }
    };
}]);
app.controller('DocumentCtrl', ['$scope', function documentCtrl($scope) {
    $scope.downloadDocument = function (file) {
        window.open(file);
    };
}]);
app.animation('.slideanimation', function () {
    return {
        enter: function (element, done) {
            element.hide().slideDown();
            return function (cancelled) {
            };
        },
        leave: function (element, done) {
            element.slideUp(function () {
                element.remove();
            });
        }
    };
});
angular.module('mainModule')
    .config(['$popoverProvider', function ($popoverProvider) {
        angular.extend($popoverProvider.defaults, {
            animation: 'am-flip-x',
            trigger: 'hover',
            autoClose: true,
            delay: { show: 1, hide: 100000 }
        });
    }]);
app.run(function ($rootScope, $window, UISettingsService) {
    if (!UISettingsService.isUISettingsHasValue()) {
        UISettingsService.loadUISettings().then(function () {
            $rootScope.mainViewReady = true;
            registerToGoogleAccount();
        }).catch(function () {
            $rootScope.mainViewError = true;
        });
    }
    else {
        $rootScope.mainViewReady = true;
        registerToGoogleAccount();
    }
    function registerToGoogleAccount() {
        var account = UISettingsService.getGoogleTrackingAccount();
        if (account != undefined && account != null)
            $window.ga('create', account, 'auto');
    }
});