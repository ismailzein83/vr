
app.service('WhS_BE_SaleAreaTechnicalSettingsService', ['VRModalService', 'UtilsService',
    function (vrModalService, utilsService) {
        function getRuleDefinitionType() {
            var promiseDeffered = utilsService.createPromiseDeferred();
            promiseDeffered.resolve("b2061c48-a2c9-4494-a707-0e84a195b5e5");
            return promiseDeffered.promise;
        }
        return ({
            getRuleDefinitionType: getRuleDefinitionType
        });
    }]);
