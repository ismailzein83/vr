
app.service('BaseAPIService', function ($http, $q) {

    return ({
        get: get,
        post: post
    });

    var _pathArray = location.href.split('/');
    var _protocol = pathArray[0];
    var _host = pathArray[2];
    var _baseurl = protocol + '//' + host;

    function get(url, options) {
        
    }

    function post(url, options) {

    }

});