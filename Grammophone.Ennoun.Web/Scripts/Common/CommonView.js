"use strict";

var commonView = (function ($) {
	// Controller for turning pages.
	function PaginationCtrl(opt) {
		var options, ctrl = this;

		// defaults
		ctrl.options = {
			paging: new PagingModel(),
			endpointFunction: function () { return ''; },
			pageContainer: '',
			payloadFunction: function () {
				return ctrl.options.paging;
			}
		}

		$.extend(ctrl.options, opt);

		// Intended as event handler for turning pages.
		ctrl.onTurnPage = function (pageNumber) {
			ctrl.options.paging.CurrentPageNumber = pageNumber;

			$.ajax({
				url: ctrl.options.endpointFunction(),
				contentType: "application/json",
				data: JSON.stringify(ctrl.options.payloadFunction()),
				type: "POST",
				success: function (data) {
					switch (ctrl.options.paging.Type) {
						case 2: // ShowMore
						case "ShowMore":
						case 3: // ShowMoreButton
						case "ShowMoreButton":
							$("#" + ctrl.options.paging.ControlsContainer).remove();
							$("#" + ctrl.options.pageContainer).append(data);
							break;

						default:
							var pageContainer = $("#" + ctrl.options.pageContainer);
							pageContainer.html(data);
							commonView.scrollTo(pageContainer);
							break;
					}

					// If utils has been imported, fire a success event.
					// Usefull for notifying other components that the UI has been updated (e.g. Portfolio Viewer)
					window.hasOwnProperty('utils') && utils.publish('pagination-success', ctrl.options.paging.ControlsContainer);
				},
				error: function (result) {
					commonView.errorAlertForAjaxResult(result);
				}
			});
		}
	}

	// Constructor for PagingModel.
	function PagingModel() {
		this.CurrentPageNumber = 0;
		this.Type = "Numbers";
		this.ControlsContainer = "";
	}

	// Page initialization.
	$("#messageTemplatesContainer").load("/MessageTemplates.html");

	// Show an alert, HTML-escaping the message.
	function showAlert(alertName, message, timeout) {
		var alert = $("#" + alertName);

		var messageSpan = $("#" + alertName + "Message");

		messageSpan.text(message);

		alert.fadeIn();

		timeout && setTimeout(function () {
			alert.fadeOut();
		}, timeout);
	}

	// Show an alert, treating the message as HTML.
	function showAlertHtml(alertName, messageHtml, timeout) {
		var alert = $("#" + alertName);

		var messageSpan = $("#" + alertName + "Message");

		messageSpan.html(messageHtml);

		alert.fadeIn();

		timeout && setTimeout(function () {
			alert.fadeOut();
		}, timeout);
	}

	// Show the modal box for entering URLs.
	// Requires a callback function whose argument is the selected value.
	function promptBox(title, body, initialValue, imgLink, urlCallback) {
		var modal = $("#promptBox");

		var form = $("#promptBox form");

		// Reset the modal.
		$("#promptBox .modal-title").text(title);
		$("#promptBoxBody").text(body);
		$("#promptBoxImg").attr('src', imgLink);

		// for unknown reasons, initial value often contains uneccesary whitespace characters
		initialValue = initialValue ? initialValue.trim() : '';
		var promptInput = $("#promptBox input").val(initialValue);

		var submitHandler = function () {
			if (form.valid()) {
				urlCallback(promptInput.val());

				modal.modal("hide");
			}

			return false;
		}

		function onHide() {
			// Avoid adding the same listeners many times.
			form.off("submit", submitHandler);

			modal.off("hidden.bs.modal", onHide);
		}

		function onShow() {
			// Avoid adding the same listener many times.
			form.on("submit", submitHandler);

			modal.off("shown.bs.modal", onShow);

			modal.on("hidden.bs.modal", onHide);
		}

		form.validate({
			errorLabelContainer: "#promptBoxError"
		});

		modal.on("shown.bs.modal", onShow);

		modal.modal("show");
	}

	return {
		// Display a modal text box with a title and a body.
		messageBox: function (title, body) {
			$("#messageBoxTitle").text(title);
			$("#messageBoxBody").text(body);
			$("#messageBox").modal("show");
		},

		// Display an error alert.
		errorAlert: function (message, timeout) {
			showAlert("errorAlert", message, timeout);
		},

		// Display a warning alert.
		warningAlert: function (message, timeout) {
			showAlert("warningAlert", message, timeout);
		},

		// Display a success alert.
		successAlert: function (message, timeout) {
			showAlert("successAlert", message, timeout);
		},

		// Show an appropriate error alert for an error result obtained via $.ajax().
		errorAlertForAjaxResult: function (result) {
			if (result.responseJSON && result.responseJSON.DisplayMessage) {
				commonView.errorAlert(result.responseJSON.DisplayMessage);
			}
			else {
				if (result.responseJSON)
					console.error(JSON.stringify(result.responseJSON));

				commonView.errorAlert("An error has occured.");
			}
		},

		// Display an error alert with HTML content.
		errorAlertHtml: function (messageHtml, timeout) {
			showAlertHtml("errorAlert", messageHtml, timeout);
		},

		// Display a warning alert with HTML content.
		warningAlertHtml: function (messageHtml, timeout) {
			showAlertHtml("warningAlert", messageHtml, timeout);
		},

		// Display a success alert  with HTML content.
		successAlertHtml: function (messageHtml, timeout) {
			showAlertHtml("successAlert", messageHtml, timeout);
		},

		// Show an appropriate error alert with HTML content for an error result obtained via $.ajax().
		errorAlertForAjaxResultHtml: function (result) {
			if (result.responseJSON && result.responseJSON.DisplayMessage) {
				commonView.errorAlertHtml(result.responseJSON.DisplayMessage);
			}
			else {
				if (result.responseJSON)
					console.error(JSON.stringify(result.responseJSON));

				commonView.errorAlert("An error has occured.");
			}
		},

		// Display a confirmation box and invoke a callback upon accepting.
		confirmationBox: function (title, body, acceptCallback) {
			var acceptButton = $("#confirmationBoxAccept");

			function onAccept() {
				acceptButton.off("click", onAccept);

				acceptCallback();
			}

			acceptButton.on("click", onAccept);

			$("#confirmationBoxTitle").text(title);
			$("#confirmationBoxBody").text(body);

			$("#confirmationBox").modal("show");
		},

		// Attempt to get an ID from the URL.
		// The idPrefix parameter is what must precede the ID value in the URL.
		getIDFromUrl: function (idPrefix) {

			var browserUrl = document.location.href;

			var idPrefixLength = idPrefix.length;

			var lala = new String("5");

			var idPrefixPosition = browserUrl.toLowerCase().indexOf(idPrefix.toLowerCase());

			if (idPrefixPosition < 0) {
				window.alert("The current resource is not rooting from " + idPrefix);
				return 0;
			}

			var idPosition = idPrefixPosition + idPrefixLength;

			var suffix = browserUrl.substring(idPosition);

			var suffixSplit = suffix.split(/[#\/\?]/);

			if (suffixSplit.length == 0) {
				window.alert("No ID is specified in the URL");
				return 0;
			}

			return parseInt(suffixSplit[0]);
		},

		// Show an indicator element when an input element becomes valid upon change.
		// Set 'prevalidate' to true in order to force initial validation and show.
		showOnValid: function (inputElement, indicatorElement, prevalidate) {

			if (typeof prevalidate === "undefined") prevalidate = true;

			inputElement = $(inputElement);
			indicatorElement = $(indicatorElement);

			(function () {

				window.setTimeout(function () {
					var timerID = -1;

					if (prevalidate) {
						if (inputElement.valid())
							indicatorElement.show();
						else
							indicatorElement.hide();
					}

					inputElement.on("change keyup paste", function () {
						if (timerID >= 0) window.clearTimeout(timerID);

						timerID = window.setTimeout(function () {
							if (inputElement.valid())
								indicatorElement.show();
							else
								indicatorElement.hide();
						},
						300);
					});

				},
				100);

			})();

		},

		// Show the modal box for entering URLs.
		// Requires a callback function whose argument is the selected value.
		promptBox: promptBox,

		// Scroll to an element.
		scrollTo: function (element) {
			var offset;
			if (!element) {
				return;
			}
			element = $(element);
			offset = element.offset();
			offset && $('html, body').animate({ scrollTop: element.offset().top - 200 }, 'slow');
		},

		// Create a Paging model.
		createPagingModel: function () {
			return new PagingModel();
		},

		// Create a controller for turning pages.
		createPaginationCtrl: function (options) {
			return new PaginationCtrl(options);
		},

		/**
		Set the value of a progress-bar.
		*
		@param progressBarSelector The selector for a progress bar.
		@param {Number} value from 0 to 100.
		@param {String} [messasge] An optional message to display.
		*/
		setProgressBarValue: function (progressBarSelector, value, message) {
			var percent = value + "%";

			if (message) 
				message = percent + " - " + message;
			else 
				message = percent;

			var progressBar = $(progressBarSelector);

			progressBar.css({ width: percent });
			progressBar.attr("aria-valuenow", value);

			var messageContainer = progressBar.find("span");

			messageContainer.text(message);
		},

		// Make a form ajax-enabled.
		bindAjaxForm: function (form, successFunction, errorFunction, completeFunction) {
			form = $(form);

			form.submit(function (evt) {
				// Prevent the browsers default function.
				evt.preventDefault();

				// Grab the form and wrap it with jQuery if it's not already.
				// If client side validation fails, don't do anything.
				if (!form.valid()) return;

				// Send your ajax request.
				$.ajax({
					method: form.prop('method'),
					url: form.prop('action') + window.location.search,
					data: form.serialize(),
					traditional: true,
					error: function (jqXHR, textStatus, errorThrown) {
						if (errorFunction)
							errorFunction(jqXHR, textStatus, errorThrown);
						else
							commonView.errorAlertForAjaxResult(jqXHR);
					},
					success: function (data, textStatus, jqXHR) {
						if (successFunction) successFunction(data, textStatus, jqXHR);
					},
					complete: function (jqXHR, textStatus) {
						if (completeFunction) completeFunction(jqXHR, textStatus);
					}
				});
			});
		}

	}

})($);
