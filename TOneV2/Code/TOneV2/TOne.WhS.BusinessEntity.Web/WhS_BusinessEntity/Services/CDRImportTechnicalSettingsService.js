
app.service('WhS_BE_CDRImportTechnicalSettingsService', ['VRModalService', 'UtilsService',
    function (vrModalService, utilsService) {
        function getRuleDefinitionType() {
            var promiseDeffered = utilsService.createPromiseDeferred();
            promiseDeffered.resolve("ae91755c-f573-4add-8dba-7733193384af");
            return promiseDeffered.promise;
        }
        return ({
            getRuleDefinitionType: getRuleDefinitionType
        });
    }]);
