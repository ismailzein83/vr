
app.service('BaseAPIService', function ($http, $q) {

    return ({
        get: get,
        post: post
    });

    function get(url, params, options) {
        var deferred = $q.defer();

        var urlParameters;
        if (params)
            urlParameters = {
                params: params
            };
        
        $http.get(url, urlParameters)
            .success(function (response, status, headers, config) {
                deferred.resolve(response);
            })
            .error(function (data, status, headers, config) {
                console(data);
                deferred.reject(data);
            });
        return deferred.promise;
    }

    function post(url, data, options) {

    }

});