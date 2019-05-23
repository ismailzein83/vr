(function (appControllers) {

    'use strict';

    function UtilsService() {
        function getLegendContent() {
            return '<div style="font-size:12px; margin:10px">' +
                '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FF0000; margin: 0px 3px"></div> Blocked </div>' +
                '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FFA500; margin: 0px 3px"></div> Lossy </div>' +
                '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #0000FF; margin: 0px 3px"></div> Forced </div>' +
                '<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #28A744; margin: 0px 3px"></div> Market Price </div>' +
                '</div>';
        }

        return ({
            getLegendContent: getLegendContent
        });
    }

    appControllers.service('WhS_Routing_UtilsService', UtilsService);

})(appControllers);
