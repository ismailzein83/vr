'use strict';
var serviceObj = function (BaseAPIService) {
    return ({
        DownloadRemoteFile: DownloadRemoteFile,
    });


    function DownloadRemoteFile(connectionId, fileId, moduleName) {
        return BaseAPIService.get('/api/VRCommon/RemoteFile/DownloadRemoteFile', {
            connectionId : connectionId,
            fileId: fileId,
            moduleName: moduleName
        }, {
            returnAllResponseParameters: true,
            responseTypeAsBufferArray: true
        });
    }
};
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('RemoteFileAPIService', serviceObj);