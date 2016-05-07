"use strict";

// View support for the common layout.
var layoutView = (function () {

	// Update the inference engine state indicator.
	function setEngineState(engineState) {
		var glyphClasses = null, title = null, description = null;

		switch (engineState) {
			case "NotStarted":
			case "Starting":
				title = "Inference engine loading";
				description = "Inference will be available in a few minutes, once this sign changes to OK."
				glyphClasses = "glyphicon glyphicon-hourglass text-danger";
				break;

			case "Started":
				title = "Inference engine is loaded";
				description = "Inference is now available for analyzing texts."
				glyphClasses = "glyphicon glyphicon-ok text-success";
				break;

			case "Error":
				title = "Inference not available";
				description = "The inference engine is not available for analyzing texts."
				glyphClasses = "glyphicon glyphicon-remove text-danger";
				break;

			default:
				return;
		}

		var indicator = $("#engineIndicator");

		indicator.attr({
			"aria-label": title,
			"data-original-title": title,
			"title": title,
			"data-content": description
		});

		var glyphContainer = indicator.find("span");

		glyphContainer.removeClass();
		glyphContainer.addClass(glyphClasses);

		indicator.popover();

		indicator.show(1000);
	}

	// Initialization.
	$(function () {
		// Connect to engine state SignalR hub.

		var engineStateHub = $.connection.engineStateHub;

		engineStateHub.client.SetEngineState = setEngineState;

		$.connection.hub.start().done(function () {
			$.ajax({
				url: "/API/Inference/EngineState",
				method: "GET",
				dataType: "json",
				success: function (data, textStatus, jqXHR) {
					setEngineState(data);
				}
			});
		});
	});

})();