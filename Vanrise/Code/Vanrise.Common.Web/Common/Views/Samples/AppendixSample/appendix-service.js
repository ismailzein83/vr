(function (appControllers) {

    "use strict";
    Common_AppendixSample_Service.$inject = ['UtilsService'];

    function Common_AppendixSample_Service( UtilsService) {

        var dataIndex = 1;
        function getRemoteData(timeOut) {
            var promiseDeferred = UtilsService.createPromiseDeferred();
            setTimeout(function () {
                var data = [];
                var dataCount = dataIndex + 10;
                for (var i = dataIndex; i <= dataCount; i++) {
                    data.push({ value: i, description: 'Item ' + i });
                    dataIndex = i;
                }
                promiseDeferred.resolve(data);
            }, timeOut);
            return promiseDeferred.promise;
        }
       
        return ({
            getRemoteData: getRemoteData
        });
    }

    appControllers.service('Common_AppendixSample_Service', Common_AppendixSample_Service);

})(appControllers);