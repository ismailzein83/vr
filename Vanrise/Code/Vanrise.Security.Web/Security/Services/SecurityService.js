'use strict';

app.service('SecurityService', ['$rootScope', 'Sec_CookieService', 'UtilsService', 'VR_Sec_PermissionFlagEnum', '$cookies', 'VR_Sec_SecurityAPIService', '$q', 'VRNotificationService', 'VRModalService',
function ($rootScope, Sec_CookieService, UtilsService, VR_Sec_PermissionFlagEnum, $cookies, VR_Sec_SecurityAPIService,  $q, VRNotificationService, VRModalService) {

    function authenticate(email, password, reloginAfterPasswordActivation) {
        var deferred = $q.defer();
        var credentialsObject = {
            Email: email,
            Password: password
        };
        VR_Sec_SecurityAPIService.Authenticate(credentialsObject).then(function (response) {
            if (VRNotificationService.notifyOnUserAuthenticated(response, onValidationNeeded)) {

                Sec_CookieService.createAccessCookieFromAuthToken(response.AuthenticationObject);
                deferred.resolve();
            }
            else {
                deferred.reject();
            }
        }).catch(function () {
            deferred.reject();
        });


        function onValidationNeeded(userObj) {
            var onPasswordActivated = function (passwordAfterActivation) {
                if (reloginAfterPasswordActivation != undefined)
                    reloginAfterPasswordActivation(passwordAfterActivation);
            };
            activatePassword(email, userObj, password, onPasswordActivated);
        }


        return deferred.promise;
    }

    function activatePassword(email, userObj, tempPassword, onPasswordActivated) {
        var modalParameters = {
            email: email,
            userObj: userObj,
            tempPassword: tempPassword
        };

        var modalSettings = {};
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.onPasswordActivated = onPasswordActivated;
        };

        VRModalService.showModal('/Client/Modules/Security/Views/User/ActivatePasswordEditor.html', modalParameters, modalSettings);
    }

    function IsAllowed(requiredPermissions) {
        return VR_Sec_SecurityAPIService.IsAllowed(requiredPermissions);
    }

    function HasPermissionToActions(systemActionNames) {
        var dummyPromise = UtilsService.createPromiseDeferred();
        if (Sec_CookieService.getUserToken() && location.pathname != '' && location.pathname.indexOf('/Security/Login') < 0) {
            return VR_Sec_SecurityAPIService.HasPermissionToActions(systemActionNames);

        }
        else {
            dummyPromise.resolve(false);
            return dummyPromise.promise;
        }
    }

    function isAllowed(attrValue) {

        var arrayOfPermissions = attrValue.split('|');

        var requiredPermissions = [];

        angular.forEach(arrayOfPermissions, function (perm) {

            var keyValuesArray = perm.split(':');

            var flags = [];
            angular.forEach(keyValuesArray[1].split(','), function (flag) {
                flag = flag.trim();
                flags.push(flag);
            });

            var singlePermission = {
                requiredPath: keyValuesArray[0].trim(),
                requiredFlags: flags
            };

            requiredPermissions.push(singlePermission);
        });

        var result = true;

        //TODO: no need for this check and should be removed when the problem of synchronization is solved
        if ($rootScope.effectivePermissionsWrapper != undefined) {
            for (var i = 0; i < requiredPermissions.length; i++) {
                var allowedFlags = [];
                result = checkPermissions(requiredPermissions[i].requiredPath, requiredPermissions[i].requiredFlags,
                    $rootScope.effectivePermissionsWrapper.PermissionResults, $rootScope.effectivePermissionsWrapper.BreakInheritanceEntities, allowedFlags);
                if (result === false)
                    break;
            }
        }

        return result;
    }

    function checkPermissions(requiredPath, requiredFlags, effectivePermissions, breakInheritanceEntities, allowedFlags) {
        var result = true;

        var effectivePermissionFlag = UtilsService.getItemByVal(effectivePermissions, requiredPath, 'PermissionPath');
        if (effectivePermissionFlag != null) {
            var fullControlFlag = UtilsService.getItemByVal(effectivePermissionFlag.PermissionFlags, 'Full Control', 'FlagName');
            if (fullControlFlag != null) {
                if (fullControlFlag.FlagValue === VR_Sec_PermissionFlagEnum.Deny.value) {
                    return false;
                }
                else {
                    angular.forEach(requiredFlags, function (flag) {
                        allowedFlags.push(flag);
                    });
                }
            }
            else {
                for (var i = 0; i < requiredFlags.length; i++) {
                    var effectiveFlag = UtilsService.getItemByVal(effectivePermissionFlag.PermissionFlags, requiredFlags[i], 'FlagName');
                    if (effectiveFlag != null) {
                        if (effectiveFlag.FlagValue === VR_Sec_PermissionFlagEnum.Deny.value)
                            return false;
                        else
                            allowedFlags.push(requiredFlags[i]);
                    }
                }
            }
        }

        var index = requiredPath.lastIndexOf('/');
        if (index > 0 && !UtilsService.contains(breakInheritanceEntities, requiredPath)) {
            var oneLevelUp = requiredPath.slice(0, index);
            result = checkPermissions(oneLevelUp, requiredFlags, effectivePermissions, breakInheritanceEntities, allowedFlags);
        }
        else {
            for (var j = 0; j < requiredFlags.length; j++) {
                if (!UtilsService.contains(allowedFlags, requiredFlags[j]))
                    return false;
            }
        }

        return result;
    }

    var loginURL = '/Security/Login';
    function setLoginURL(value) {
        if (value != undefined && value != '')
            loginURL = value;
    }

    function redirectToLoginPage(withoutAutoRedirect) {
        if (location.pathname.indexOf('/Security/Login') < 0) {
            var url = loginURL;
            if (!withoutAutoRedirect)
                url += '?redirectTo=' + encodeURIComponent(window.location.href);
            window.location.href = url;
        }
    }

    function getUserToken() {
        var userToken = Sec_CookieService.getLoggedInUserInfo();
        if (userToken != undefined) {
            renewTokenIfNeeded(userToken);
            return userToken.Token;
        }
        else
        {
            return undefined;
        }
    }

    var isRenewingToken = false;
    function renewTokenIfNeeded(userToken) {
        var cookieCreatedTime = userToken.cookieCreatedTime;
        if (cookieCreatedTime != undefined && typeof (cookieCreatedTime) != typeof (Date))
            cookieCreatedTime = new Date(cookieCreatedTime);
        if (cookieCreatedTime == undefined || ((new Date()).getTime() - cookieCreatedTime.getTime()) / 1000 > (userToken.ExpirationIntervalInSeconds - 60)) {
            if (!isRenewingToken) {
                isRenewingToken = true;
                VR_Sec_SecurityAPIService.TryRenewCurrentSecurityToken().then(function (response) {
                    if (response != null && response.IsSucceeded) {
                        Sec_CookieService.createAccessCookieFromAuthToken(response.NewAuthenticationToken);
                    }
                }).finally(function () {
                    isRenewingToken = false;
                });
            }
        }
    }

    return ({
        authenticate: authenticate,
        isAllowed: isAllowed,
        createAccessCookie: Sec_CookieService.createAccessCookie,
        deleteAccessCookie: Sec_CookieService.deleteAccessCookie,
        getLoggedInUserInfo: Sec_CookieService.getLoggedInUserInfo,
        getUserToken: getUserToken,
        IsAllowed: IsAllowed,
        HasPermissionToActions: HasPermissionToActions,
        setAccessCookieName: Sec_CookieService.setAccessCookieName,
        setLoginURL: setLoginURL,
        redirectToLoginPage: redirectToLoginPage
    });
}]);