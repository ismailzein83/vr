'use strict';

app.service('SecurityService', ['$rootScope', 'UtilsService', 'PermissionFlagEnum', '$cookies', function ($rootScope, UtilsService, PermissionFlagEnum, $cookies) {

    return ({
        isAllowed: isAllowed,
        createAccessCookie: createAccessCookie,
        getAccessCookie: getAccessCookie,
        deleteAccessCookie: deleteAccessCookie
    });

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
                if (fullControlFlag.FlagValue === PermissionFlagEnum.Deny.value) {
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
                        if (effectiveFlag.FlagValue === PermissionFlagEnum.Deny.value)
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
        var expiresDate = new Date(new Date().getTime() + parseInt(30) * 1000 * 60 * 60 * 24);
        $cookies.put('Vanrise_AccessCookie-' + location.port, userInfo, { path: '/', domain: location.hostname, expires: expiresDate, secure: false });
    }

    function getAccessCookie() {
        return $cookies.get('Vanrise_AccessCookie-' + location.port);
    }

    function deleteAccessCookie() {
        $cookies.remove('Vanrise_AccessCookie-' + location.port);
    }
}]);