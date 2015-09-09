'use strict'
var serviceObj = function (BaseAPIService) {
    return ({
        DownloadFile: DownloadFile,
        PreviewImage: PreviewImage
    });
    
    function GetCarriers(userId) {
        return BaseAPIService.get('/api/AccountManager/GetCarriers', {
            userId: userId
        });
    }

  
    function DownloadFile(fileId) {
        return BaseAPIService.get('/api/VRFile/DownloadFile', {
            fileId: fileId
        },{
            returnAllResponseParameters: true,
            responseTypeAsBufferArray: true
        });
    }
    function PreviewImage(fileId) {
        return BaseAPIService.get('/api/VRFile/PreviewImage', {
            fileId: fileId
        });
    }

    
}
serviceObj.$inject = ['BaseAPIService'];
appControllers.service('FileAPIService', serviceObj);