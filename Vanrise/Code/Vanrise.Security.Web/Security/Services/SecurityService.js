'use strict';

app.service('SecurityService', ['$rootScope', 'UtilsService', 'VR_Sec_PermissionFlagEnum', '$cookies', 'VR_Sec_SecurityAPIService', function ($rootScope, UtilsService, VR_Sec_PermissionFlagEnum, $cookies, VR_Sec_SecurityAPIService) {

   

    function IsAllowed(requiredPermissions) {
        return VR_Sec_SecurityAPIService.IsAllowed(requiredPermissions);
    }

    function HasPermissionToActions(systemActionNames) {
        var dummyPromise = UtilsService.createPromiseDeferred();
        if (getUserToken() && location.pathname != '' &&  location.pathname.indexOf('/Security/Login') < 0) {
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
        if ($rootScope.effectivePermissionsWrapper != undefined)
        {
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
        if(effectivePermissionFlag != null)
        {
            var fullControlFlag = UtilsService.getItemByVal(effectivePermissionFlag.PermissionFlags, 'Full Control', 'FlagName');
            if (fullControlFlag != null)
            {
                if (fullControlFlag.FlagValue === VR_Sec_PermissionFlagEnum.Deny.value) {
                    return false;
                }
                else
                {
                    angular.forEach(requiredFlags, function (flag) {
                        allowedFlags.push(flag);
                    });
                }
            }
            else
            {
                for (var i = 0; i < requiredFlags.length; i++)
                {
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
            for(var j=0; j < requiredFlags.length; j++)
            {
                if (!UtilsService.contains(allowedFlags, requiredFlags[j]))
                    return false;
            }
        }

        return result;
    }

    function createAccessCookie(userInfo) {      
        $cookies.put(getAccessCookieName(), userInfo, { path: '/', domain: location.hostname, expires: '', secure: false });
    }

    function getAccessCookie() {
        return $cookies.get(getAccessCookieName());
    }

    function getAccessCookieName() {
        return cookieName;
    }

    var cookieName = 'Vanrise_AccessCookie-' + location.origin;
    function setAccessCookieName(value) {
        if (value != undefined && value != '')
            cookieName = value;
    }

    var loginURL = '/Security/Login';
    function setLoginURL(value)
    {
        if (value != undefined && value != '')
            loginURL = value;
    }

    function redirectToLoginPage(withoutAutoRedirect)
    {
        if (location.pathname.indexOf('/Security/Login') < 0) {
            var url = loginURL;
            if (!withoutAutoRedirect)
                url += '?redirectTo=' + encodeURIComponent(window.location.href);
            window.location.href = url;
        }
    }

    function getLoggedInUserInfo() {
        var accessCookie = getAccessCookie();
        if (accessCookie != undefined)
            return JSON.parse(accessCookie);
        else
            return undefined;
    }

    function getUserToken() {
        var accessCookie = getAccessCookie();
        if (accessCookie != undefined)
            return JSON.parse(accessCookie).Token;
        else
            return undefined;
    }

    function deleteAccessCookie() {
        $cookies.remove(getAccessCookieName());
    }

    return ({
        isAllowed: isAllowed,
        createAccessCookie: createAccessCookie,
        deleteAccessCookie: deleteAccessCookie,
        getLoggedInUserInfo: getLoggedInUserInfo,
        getUserToken: getUserToken,
        IsAllowed: IsAllowed,
        HasPermissionToActions: HasPermissionToActions,
        setAccessCookieName: setAccessCookieName,
        setLoginURL: setLoginURL,
        redirectToLoginPage: redirectToLoginPage
    });
}]);