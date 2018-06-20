'use strict';

app.service('Sec_CookieService', ['$cookies', 'VRNotificationService',
function ($cookies, VRNotificationService) {

    function createAccessCookie(userInfo, cookieName) {
        var accessCookieName = cookieName == undefined ? getAccessCookieName() : cookieName;
        $cookies.put(accessCookieName, userInfo, { path: '/', domain: location.hostname, expires: '', secure: false });
    }

    function createAccessCookieFromAuthToken(authToken, cookieName) {
        authToken.cookieCreatedTime = new Date();
        if (authToken.PasswordExpirationDaysLeft != null) {
            $cookies.put("passwordExpirationDays", authToken.PasswordExpirationDaysLeft, { path: '/', domain: location.hostname, expires: '', secure: false });
        }
        var userInfo = JSON.stringify(authToken);
        createAccessCookie(userInfo, cookieName);
    }

    function getAccessCookie() {
        return $cookies.get(getAccessCookieName());
    }

    function isAccessCookieAvailable() {
        return getAccessCookie() != undefined;
    }

    function getAccessCookieName() {
        return cookieName;
    }

    var cookieName = 'Vanrise_AccessCookie-' + location.origin;
    function setAccessCookieName(value) {
        if (value != undefined && value != '')
            cookieName = value;
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
        createAccessCookie: createAccessCookie,
        createAccessCookieFromAuthToken: createAccessCookieFromAuthToken,
        deleteAccessCookie: deleteAccessCookie,
        isAccessCookieAvailable: isAccessCookieAvailable,
        getLoggedInUserInfo: getLoggedInUserInfo,
        getUserToken: getUserToken,
        setAccessCookieName: setAccessCookieName
    });
}]);