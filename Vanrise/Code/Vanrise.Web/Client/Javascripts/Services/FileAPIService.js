'use strict';
var serviceObj = function (BaseAPIService) {
    return ({
        DownloadFile: DownloadFile,
        PreviewImage: PreviewImage
    });


    function DownloadFile(fileId, moduleName) {
        return BaseAPIService.get('/api/VRCommon/File/DownloadFile', {
            fileId: fileId,
            moduleName: moduleName
        }, {
            returnAllResponseParameters: true,
            responseTypeAsBufferArray: true
        });
    }
    function PreviewImage(fileId) {
        return BaseAPIService.get('/api/VRCommon/File/PreviewImage', {
            fileId: fileId
        });
    }


};
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('FileAPIService', serviceObj);