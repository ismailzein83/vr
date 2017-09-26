(function (app) {

    "use strict";

    VRDateTimeService.$inject = ['UISettingsService'];

    function VRDateTimeService(UISettingsService) {

        function getNowDateTime() {

            var serverSideOffsetInMinutes = (-1) * UISettingsService.getServerUtcOffsetInMinutes();

            var clientDate = new Date();
            var clientTimezoneOffsetInMinutes = clientDate.getTimezoneOffset();
            var serverClientTimezoneOffsetInMilliseconds = (clientTimezoneOffsetInMinutes - serverSideOffsetInMinutes) * 60000;
            var clientSideDate = new Date(clientDate.getTime() + serverClientTimezoneOffsetInMilliseconds);

            return clientSideDate;
        }

        function getCurrentDateWithoutMilliseconds() {
            var date = getNowDateTime();
            date.setHours(date.getHours(), date.getMinutes(), date.getSeconds(), 0);
            return date;
        }

        return ({
            getNowDateTime: getNowDateTime,
            getCurrentDateWithoutMilliseconds: getCurrentDateWithoutMilliseconds

        });
    }

    app.service('VRDateTimeService', VRDateTimeService);

})(app);


