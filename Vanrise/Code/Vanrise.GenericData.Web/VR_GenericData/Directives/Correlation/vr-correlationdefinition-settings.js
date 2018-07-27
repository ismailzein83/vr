(function (app) {

	'use strict';

	VRCorrelationDefinitionSettings.$inject = ['UtilsService'];

	function VRCorrelationDefinitionSettings(UtilsService) {

		var directiveDefinitionObject = {
			restrict: 'E',
			scope: {
				onReady: '=',
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new VRCorrelationDefinitionSettingsCtor(ctrl, $scope, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			compile: function (element, attrs) {
				return {
					pre: function ($scope, iElem, iAttrs, ctrl) {

					}
				};
			},
			templateUrl: '/Client/Modules/GenericData/Directives/Correlation/Templates/VRCorrelationDefinitionSettings.html'
		};

		function VRCorrelationDefinitionSettingsCtor(ctrl, $scope, attrs) {
			this.initializeController = initializeController;

			var inputDataRecordTypeId;
			var inputDataRecordTypeSelectorAPI;
			var inputDataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
			var inputDataRecordStorageId;
			var inputDataRecordStorageSelectorAPI;
			var inputDataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var outputDataRecordTypeId;
			var outputDataRecordTypeSelectorAPI;
			var outputDataRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
			var outputDataRecordStorageId;
			var outputDataRecordStorageSelectorAPI;
			var outputDataRecordStorageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var callingNumberFieldName;
			var callingNumberFieldSelectorAPI;
			var callingNumberFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var calledNumberFieldName;
			var calledNumberFieldSelectorAPI;
			var calledNumberFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var durationFieldName;
			var durationFieldSelectorAPI;
			var durationFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var datetimeFieldName;
			var datetimeFieldSelectorAPI;
			var datetimeFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var mergeDataTransformationDefinitionId;
			var mergeDataTransformationAPI;
			var mergeDataTransformationSelectorReadyDeferred = UtilsService.createPromiseDeferred();

			var settings;

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.onInputRecordTypeSelectionChanged = function () {
					var promises = [];
					inputDataRecordTypeId = inputDataRecordTypeSelectorAPI.getSelectedIds();
					inputDataRecordStorageId = undefined;
					callingNumberFieldName = undefined;
					calledNumberFieldName = undefined;
					durationFieldName = undefined;
					datetimeFieldName = undefined;

					promises.push(loadInputDataRecordStorageSelector());
					promises.push(loadCallingNumberFieldSelector());
					promises.push(loadCalledNumberFieldSelector());
					promises.push(loadDurationFieldSelector());
					promises.push(loadDatetimeFieldSelector());
					return UtilsService.waitMultiplePromises(promises);
				};

				$scope.scopeModel.onOutputRecordTypeSelectionChanged = function () {
					var promises = [];
					outputDataRecordTypeId = outputDataRecordTypeSelectorAPI.getSelectedIds();
					outputDataRecordStorageId = undefined;

					promises.push(loadOutputDataRecordStorageSelector());
					return UtilsService.waitMultiplePromises(promises);
				};

				$scope.onInputDataRecordTypeSelectorReady = function (api) {
					inputDataRecordTypeSelectorAPI = api;
					inputDataRecordTypeSelectorReadyDeferred.resolve();
				};
				$scope.onInputDataRecordStorageSelectorReady = function (api) {
					inputDataRecordStorageSelectorAPI = api;
					inputDataRecordStorageSelectorReadyDeferred.resolve();
				};

				$scope.onOutputDataRecordTypeSelectorReady = function (api) {
					outputDataRecordTypeSelectorAPI = api;
					outputDataRecordTypeSelectorReadyDeferred.resolve();
				};
				$scope.onInputDataRecordStorageSelectorReady = function (api) {
					outputDataRecordStorageSelectorAPI = api;
					outputDataRecordStorageSelectorReadyDeferred.resolve();
				};

				$scope.scopeModel.onCallingNumberFieldSelectorDirectiveReady = function (api) {
					callingNumberFieldSelectorAPI = api;
					callingNumberFieldSelectorReadyDeferred.resolve();
				};

				$scope.scopeModel.onCalledNumberFieldSelectorDirectiveReady = function (api) {
					calledNumberFieldSelectorAPI = api;
					calledNumberFieldSelectorReadyDeferred.resolve();
				};

				$scope.scopeModel.onDurationFieldSelectorDirectiveReady = function (api) {
					durationFieldSelectorAPI = api;
					durationFieldSelectorReadyDeferred.resolve();
				};

				$scope.scopeModel.onDatetimeFieldSelectorDirectiveReady = function (api) {
					datetimeFieldSelectorAPI = api;
					datetimeFieldSelectorReadyDeferred.resolve();
				};

				$scope.scopeModel.onDataTransformationSelectorDirectiveReady = function (api) {
					mergeDataTransformationAPI = api;
					mergeDataTransformationSelectorReadyDeferred.resolve();
				}

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					if (payload != undefined && payload.componentType != undefined) {
						$scope.scopeModel.name = payload.componentType.Name;
						if (payload.componentType.Settings != undefined) {
							inputDataRecordTypeId = payload.componentType.Settings.InputDataRecordTypeId;
							inputDataRecordStorageId = payload.componentType.Settings.InputDataRecordTypeId;
							outputDataRecordTypeId = payload.componentType.Settings.OutputDataRecordTypeId;
							outputDataRecordStorageId = payload.componentType.Settings.OutputDataRecordStorageId;
							callingNumberFieldName = payload.componentType.Settings.CallingNumberFieldName;
							calledNumberFieldName = payload.componentType.Settings.CalledNumberFieldName;
							durationFieldName = payload.componentType.Settings.DurationFieldName;
							datetimeFieldName = payload.componentType.Settings.DatetimeFieldName;
							mergeDataTransformationDefinitionId = payload.componentType.Settings.MergeDataTransformationDefinitionId
						}
					}

					var promises = [];

					promises.push(loadInputDataRecordTypeSelector());
					promises.push(loadInputDataRecordStorageSelector());
					promises.push(loadOutputDataRecordTypeSelector());
					promises.push(loadOutputDataRecordStorageSelector());
					promises.push(loadCallingNumberFieldSelector());
					promises.push(loadCalledNumberFieldSelector());
					promises.push(loadDurationFieldSelector());
					promises.push(loadDatetimeFieldSelector());
					promises.push(loadMergeDataTransformationSelector());

					return UtilsService.waitMultiplePromises(promises);
				};

				api.getData = function () {
					return {
						Name: $scope.scopeModel.name,
						Settings:
							{
								$type: "Vanrise.GenericData.Entities.VRCorrelationDefinitionSettings, Vanrise.GenericData.Entities",
								InputDataRecordTypeId: (inputDataRecordTypeSelectorAPI != undefined) ? inputDataRecordTypeSelectorAPI.getData() : null,
								OutputDataRecordTypeId: (outputDataRecordTypeSelectorAPI != undefined) ? outputDataRecordTypeSelectorAPI.getData() : null,
								InputDataRecordStorageId: (inputDataRecordStorageSelectorAPI != undefined) ? inputDataRecordStorageSelectorAPI.getData() : null,
								OutputDataRecordStorageId: (outputDataRecordStorageSelectorAPI != undefined) ? outputDataRecordStorageSelectorAPI.getData() : null,
								CallingNumberFieldName: (callingNumberFieldSelectorAPI != undefined) ? callingNumberFieldSelectorAPI.getData() : null,
								CalledNumberFieldName: (calledNumberFieldSelectorAPI != undefined) ? calledNumberFieldSelectorAPI.getData() : null,
								DurationFieldName: (durationFieldSelectorAPI != undefined) ? durationFieldSelectorAPI.getData() : null,
								DatetimeFieldName: (datetimeFieldSelectorAPI != undefined) ? datetimeFieldSelectorAPI.getData() : null,
								MergeDataTransformationDefinitionId: (mergeDataTransformationAPI != undefined) ? mergeDataTransformationAPI.getData() : null
							}
					}
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);

			}

			function loadInputDataRecordTypeSelector() {

				var loadInputDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
				inputDataRecordTypeSelectorReadyDeferred.promise.then(function () {
					var directivePayload = { selectedIds: inputDataRecordTypeId };

					VRUIUtilsService.callDirectiveLoad(inputDataRecordTypeSelectorAPI, directivePayload, loadInputDataRecordTypeSelectorPromiseDeferred);
				});

				return loadInputDataRecordTypeSelectorPromiseDeferred.promise;
			}

			function loadInputDataRecordStorageSelector() {

				var loadInputDataRecordStorageSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
				inputDataRecordStorageSelectorReadyDeferred.promise.then(function () {
					var directivePayload = {
						DataRecordTypeId: inputDataRecordTypeId,
						selectedIds: inputDataRecordStorageId
					}
					VRUIUtilsService.callDirectiveLoad(inputDataRecordStorageSelectorAPI, directivePayload, loadInputDataRecordStorageSelectorPromiseDeferred);
				});

				return loadInputDataRecordStorageSelectorPromiseDeferred.promise;
			}

			function loadOutputDataRecordTypeSelector() {

				var loadOutputDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
				outputDataRecordTypeSelectorReadyDeferred.promise.then(function () {
					var directivePayload = { selectedIds: outputDataRecordTypeId };

					VRUIUtilsService.callDirectiveLoad(outputDataRecordTypeSelectorAPI, directivePayload, loadOutputDataRecordTypeSelectorPromiseDeferred);
				});

				return loadOutputDataRecordTypeSelectorPromiseDeferred.promise;
			}

			function loadOutputDataRecordStorageSelector() {

				var loadOutputDataRecordStorageSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
				outputDataRecordStorageSelectorReadyDeferred.promise.then(function () {
					var directivePayload = {
						DataRecordTypeId: settings.OutputDataRecordTypeId,
						selectedIds: settings.OutputDataRecordStorageId
					};

					VRUIUtilsService.callDirectiveLoad(outputDataRecordStorageSelectorAPI, directivePayload, loadOutputDataRecordStorageSelectorPromiseDeferred);
				});

				return loadOutputDataRecordStorageSelectorPromiseDeferred.promise;
			}

			function loadCallingNumberFieldSelector() {

				var loadCallingNumberFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
				callingNumberFieldSelectorReadyDeferred.promise.then(function () {
					var directivePayload = {
						dataRecordTypeId: settings.InputDataRecordTypeId,
						selectedIds: settings.CallingNumberFieldName
					};

					VRUIUtilsService.callDirectiveLoad(callingNumberFieldSelectorAPI, directivePayload, loadCallingNumberFieldSelectorPromiseDeferred);
				});

				return loadCallingNumberFieldSelectorPromiseDeferred.promise;
			}

			function loadCalledNumberFieldSelector() {

				var loadCalledNumberFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
				calledNumberFieldSelectorReadyDeferred.promise.then(function () {
					var directivePayload = {
						dataRecordTypeId: settings.InputDataRecordTypeId,
						selectedIds: settings.CalledNumberFieldName
					};

					VRUIUtilsService.callDirectiveLoad(calledNumberFieldSelectorAPI, directivePayload, loadCalledNumberFieldSelectorPromiseDeferred);
				});

				return loadCalledNumberFieldSelectorPromiseDeferred.promise;
			}

			function loadDurationFieldSelector() {

				var loadDurationFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
				durationFieldSelectorReadyDeferred.promise.then(function () {
					var directivePayload = {
						dataRecordTypeId: settings.InputDataRecordTypeId,
						selectedIds: settings.DurationFieldName
					};

					VRUIUtilsService.callDirectiveLoad(durationFieldSelectorAPI, directivePayload, loadDurationFieldSelectorPromiseDeferred);
				});

				return loadDurationFieldSelectorPromiseDeferred.promise;
			}

			function loadDatetimeFieldSelector() {

				var loadDatetimeFieldSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
				datetimeFieldSelectorReadyDeferred.promise.then(function () {
					var directivePayload = {
						dataRecordTypeId: settings.InputDataRecordTypeId,
						selectedIds: settings.DatetimeFieldName
					};

					VRUIUtilsService.callDirectiveLoad(datetimeFieldSelectorAPI, directivePayload, loadDatetimeFieldSelectorPromiseDeferred);
				});

				return loadDatetimeFieldSelectorPromiseDeferred.promise;
			}

			function loadMergeDataTransformationSelector() {

				var loadMergeDataTransformationSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
				mergeDataTransformationSelectorReadyDeferred.promise.then(function () {
					var directivePayload = {
						selectedIds: mergeDataTransformationDefinitionId
					};

					VRUIUtilsService.callDirectiveLoad(mergeDataTransformationAPI, directivePayload, loadMergeDataTransformationSelectorPromiseDeferred);
				});

				return loadMergeDataTransformationSelectorPromiseDeferred.promise;
			}
		}
		return directiveDefinitionObject;
	}
	app.directive('vrCorrelationdefinitionSettings', VRCorrelationDefinitionSettings);

})(app);
