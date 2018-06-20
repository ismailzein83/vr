'use strict';

app.service('SecurityService', ['$rootScope', 'Sec_CookieService', 'UtilsService', 'VR_Sec_PermissionFlagEnum', '$cookies', 'VR_Sec_SecurityAPIService', '$q', 'VRNotificationService', 'VRModalService',
function ($rootScope, Sec_CookieService, UtilsService, VR_Sec_PermissionFlagEnum, $cookies, VR_Sec_SecurityAPIService, $q, VRNotificationService, VRModalService) {

    function authenticate(securityProviderId, payload, reloginAfterPasswordActivation) {
        var deferred = $q.defer();

        var credentialsObject = {
            SecurityProviderId: securityProviderId,
            Payload: payload
        };
        VR_Sec_SecurityAPIService.Authenticate(credentialsObject).then(function (response) {
            if (VRNotificationService.notifyOnUserAuthenticated(response, onValidationNeeded, onExpiredPasswordChangeNeeded)) {

                Sec_CookieService.createAccessCookieFromAuthToken(response.AuthenticationObject);
                deferred.resolve();
            }
            else {
                deferred.reject();
            }
        }).catch(function () {
            deferred.reject();
        });


        function onValidationNeeded() {
            var onPasswordActivated = function (passwordAfterActivation) {
                if (reloginAfterPasswordActivation != undefined)
                    reloginAfterPasswordActivation(passwordAfterActivation);
            };
            var email = payload.Email;
            var password = payload.Password;
            activatePassword(email, password, onPasswordActivated, securityProviderId);
        }

        function onExpiredPasswordChangeNeeded() {
            var onExpiredPasswordChanged = function (passwordAfterChange) {
                if (reloginAfterPasswordActivation != undefined)
                    reloginAfterPasswordActivation(passwordAfterChange);
            };
            var email = payload.Email;
            var password = payload.Password;
            changeExpiredPassword(email, password, onExpiredPasswordChanged, securityProviderId);
        }

        return deferred.promise;
    }

    function activatePassword(email, tempPassword, onPasswordActivated, securityProviderId) {
        var modalParameters = {
            email: email,
            tempPassword: tempPassword,
            securityProviderId: securityProviderId
        };

        var modalSettings = {};
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.onPasswordActivated = onPasswordActivated;
        };

        VRModalService.showModal('/Client/Modules/Security/Views/User/ActivatePasswordEditor.html', modalParameters, modalSettings);
    }

    function changeExpiredPassword(email, oldPassword, onExpiredPasswordChanged, securityProviderId) {
        var modalParameters = {
            email: email,
            oldPassword: oldPassword,
            securityProviderId: securityProviderId
        };

        var modalSettings = {};
        modalSettings.onScopeReady = function (modalScope) {
            modalScope.onExpiredPasswordChanged = onExpiredPasswordChanged;
        };

        VRModalService.showModal('/Client/Modules/Security/Views/User/ChangeExpiredPasswordEditor.html', modalParameters, modalSettings);
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
        else {
            return undefined;
        }
    }

    var isRenewingToken = false;
    function renewTokenIfNeeded(userToken) {
        var cookieCreatedTime = userToken.cookieCreatedTime;
        if (cookieCreatedTime != undefined && typeof (cookieCreatedTime) != typeof (Date))
            cookieCreatedTime = new Date(cookieCreatedTime);
        var nbOfSecondsToCompareWith = (userToken.ExpirationIntervalInSeconds - 60);
        if (nbOfSecondsToCompareWith > 300)//max 5 minutes to renew token
            nbOfSecondsToCompareWith = 300;
        else if (nbOfSecondsToCompareWith < 60)//min 1 minute to renew token
            nbOfSecondsToCompareWith = 60;
        if (cookieCreatedTime == undefined || ((new Date()).getTime() - cookieCreatedTime.getTime()) / 1000 > nbOfSecondsToCompareWith) {
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

    function redirectToApplication(applicationURL) {
        var deferred = $q.defer();

        VR_Sec_SecurityAPIService.RedirectToApplication(applicationURL).then(function (response) {
            if (response != undefined) {
                Sec_CookieService.createAccessCookieFromAuthToken(response.AuthenticationToken, response.CookieName);
            }
            deferred.resolve();
            window.location.href = applicationURL;
        }).catch(function () {
            deferred.reject();
        });

        return deferred.promise;
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
        redirectToLoginPage: redirectToLoginPage,
        redirectToApplication: redirectToApplication
    });
}]);