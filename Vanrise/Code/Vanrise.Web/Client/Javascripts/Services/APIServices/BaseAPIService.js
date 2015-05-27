
app.service('BaseAPIService', function ($http, $q, notify) {

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
                console.log(data);
                notify.closeAll();
                notify({ message: 'Error Occured while getting data!', classes: "alert alert-danger" });
                setTimeout(function () {
                    notify.closeAll();
                }, 3000);
                deferred.reject(data);
            });
        return deferred.promise;
    }

    function post(url, dataToSend, options) {
        var deferred = $q.defer();
        var data;
        if (dataToSend)
            data = dataToSend
        $http.post(url, data)
            .success(function (response, status, headers, config) {
                deferred.resolve(response);
            })
            .error(function (data, status, headers, config) {
                console.log(data);
                notify.closeAll();
                notify({ message: 'Error Occured while posting data!', classes: "alert alert-danger" });
                setTimeout(function () {
                    notify.closeAll();
                }, 3000);
                deferred.reject(data);
            });
        return deferred.promise;

    }

});