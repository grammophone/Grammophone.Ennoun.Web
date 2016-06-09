"use strict";

var inferView = (function () {
	// Holds the array of Grammophone.Ennoun.Web.Models.Inference.SentenceResponseModel objects.
	var sentenceResponses = null;

	// The last clicked word, or null if no word is clicked yet.
	var selectedWord = null;

	/**
	Show the lemmata matching a given key.
	*/
	function showLexiconLemmataByKey(key) {
		var lexiconContainer = document.getElementById("lexiconContainer");

		ReactDOM.unmountComponentAtNode(lexiconContainer);
		ReactDOM.render(<LexiconLemmata form={key} searchMode="by-key"></LexiconLemmata>, lexiconContainer);
	}

	var LexiconText = React.createClass({
		showReference: function () {
			showLexiconLemmataByKey(this.props.item.Reference);
		},

		render: function () {
			var linkElement = null;

			if (this.props.item.Reference) {
				linkElement = <span> <button className="btn-link" onClick={this.showReference}>[See {this.props.item.Reference}]</button></span>
			}

			return (
				<p>
					{this.props.item.Description}{linkElement}
				</p>
			);
		}
	});

	var LemmaSenses = React.createClass({
		render: function () {
			if (this.props.senses.length == 0) return null;

			return (
				<ul>
					{this.props.senses.map(function (sense) {
						return (
							<li>
								<LexiconText item={sense}></LexiconText>
								<LemmaSenses senses={sense.Subsenses}></LemmaSenses>
							</li>
						);
					})}
				</ul>
			);
		}
	});

	// Root component for the lexicon lemmata.
	var LexiconLemmata = React.createClass({
		getEmptyState: function () {
			return {
				currentForm: null,
				lemmaResults: []
			};
		},

		getDefaultProps: function () {
			return {
				searchMode: "best-approximate" // Can be "approximate", "best-approximate" or "by-key".
			};
		},

		updateState: function () {
			var dis = this;

			var url;

			switch (dis.props.searchMode) {
				case "best-approximate":
					url = "/API/Inference/GetBestLemmata?" + $.param({ form: dis.props.form });
					break;

				case "approximate":
					url = "/API/Inference/GetLemmata?" + $.param({ form: dis.props.form });
					break;

				case "by-key":
					url = "/API/Inference/GetLemmataByKey?" + $.param({ key: dis.props.form });
					break;

				default:
					throw "Invalid searchMode specified in LemmaSenses component.";
			}

			$.ajax({
				url: url,
				method: "GET",
				dataType: "json",
				error: function (jqXHR, textStatus, errorThrown) {
					commonView.errorAlertForAjaxResult(jqXHR);
				},
				success: function (data, textStatus, jqXHR) {
					var lemmaResults = null;

					switch (dis.props.searchMode) {
						case "by-key":
							lemmaResults = data.map(function (lemma) {
								return {
									Lemma: lemma,
									Distance: 0.0
								};
							});
							break;

						default:
							lemmaResults = data;
							break;
					}

					var newState = {
						currentForm: dis.props.form,
						lemmaResults: lemmaResults
					};

					dis.setState(newState);
				}
			});
		},

		componentDidMount: function () {
			this.updateState();
		},

		getInitialState: function () {
			return this.getEmptyState();
		},

		componentDidUpdate: function () {
			if (this.props.form != this.state.currentForm) {
				if (this.props.form) {
					this.updateState();
				}
				else {
					this.setState(this.getEmptyState());
				}
			}
		},

		render: function () {

			if (this.props.form != this.state.currentForm) {
				return (
					<div className="h4">Loading... <span className="glyphicon-hourglass"></span></div>
				);
			}

			return (
				<div>
					{this.state.lemmaResults.map(function (lemmaResult) {
						var etymologyElement = null;

						if (lemmaResult.Lemma.Etymology) {

							etymologyElement = (
								<div>
									<h4>Etymology</h4>
									<p><LexiconText item={lemmaResult.Lemma.Etymology}></LexiconText></p>
								</div>
							);
						}

						var sensesElement = null;

						if (lemmaResult.Lemma.Senses.length > 0) {
							sensesElement = (
								<div>
									<h4>Sense</h4>
									<p><LemmaSenses senses={lemmaResult.Lemma.Senses}></LemmaSenses></p>
								</div>
							);
						}

						var notesElement = null;

						if (lemmaResult.Lemma.Notes && lemmaResult.Lemma.Notes.length) {
							notesElement = (
								<div>
									<h4>Notes</h4>
									<ul>
										{lemmaResult.Lemma.Notes.map(function (note) {
											return <li><p>{note}</p></li>
										})}
									</ul>
								</div>
							);
						}

						return (
							<dl>
								<dt className="h3 typography-classic text-danger">{lemmaResult.Lemma.Form}</dt>
								<dd>
									{etymologyElement}
									{sensesElement}
									{notesElement}
								</dd>
							</dl>
						);
					})}
				</div>);
		}
	});

	// Component for inflection lines.
	var InferredInflection = React.createClass({
		render: function () {
			return (
				<tr>
					<td>{this.props.inflection.Type}</td>
					<td>{this.props.inflection.Name}</td>
				</tr>
			);
		}
	});

	// Component for displaying word details.
	var InferredLemma = React.createClass({
		getEmptyState: function () {
			return { lemma: null };
		},

		getInitialState: function () {
			return this.getEmptyState();
		},

		initializePropertiesTable() {
			$('#propertiesContainer [data-toggle="popover"]').popover();
		},

		componentDidUpdate: function () {
			this.initializePropertiesTable();
		},

		componentDidMount: function () {
			this.initializePropertiesTable();
		},

		render: function () {

			var warningElement = (
				<button type="button" className="btn-link" aria-label="warning"
								role="button"
								data-toggle="popover"
								data-trigger="focus"
								title="Work in progress"
								data-content="Occasionaly there will be errors in the estimation. The accuracy will improve over time as the system evolves.">
					<span className="glyphicon glyphicon-warning-sign text-danger" aria-hidden="true"></span>
				</button>
			);

			var inflectionsHeaderElement = null, inflectionsBody = null;

			var lemmaInference = this.props.lemmaInference;

			if (lemmaInference.Tag && lemmaInference.Tag.Inflections.length > 0) {
				inflectionsHeaderElement =
					<tr>
						<th colSpan="2">Inflection {warningElement}</th>
					</tr>;

				inflectionsBody =
					lemmaInference.Tag.Inflections.map(function (inflection) {
						return <InferredInflection inflection={inflection}></InferredInflection>
					});
			}

			var lexiconSearchForm = lemmaInference.Lemma ? lemmaInference.Lemma : lemmaInference.Form;

			var lexiconElement = null;

			// Only show lexicon if the tag is not special, like punctuation.
			// Special tags have names within brackets.
			if (lemmaInference.Tag.Type.indexOf('[') === -1) {
				lexiconElement = <LexiconLemmata form={lexiconSearchForm }></LexiconLemmata>;
			}

			var messagesElement = null;

			if (this.props.messages && this.props.messages.length) {
				messagesElement = (
					<div>
						{this.props.messages.map(function (message) {
							switch (message.Level) {
								case "Information":
									return <div className="alert alert-info">{message.Text}</div>;

								case "Warning":
									return <div className="alert alert-warning">{message.Text}</div>;

								case "Error":
									return <div className="alert alert-danger">{message.Text}</div>;

								default:
									return null;
							}
						})}
					</div>
				);
			}

			return (
				<div>
					{messagesElement}
					<div id="propertiesContainer" className="panel panel-default">
						<div className="panel-heading typography-classic text-largest bg-info">
							{lemmaInference.Form}
						</div>
						<div className="panel-body" id="lexiconContainer">
							{lexiconElement}
						</div>
						<table id="analysisTable" className="table">
							<tbody>
								<tr>
									<th colSpan="2">Basic properties {warningElement}</th>
								</tr>
								<tr>
									<td style={{ width: "14em" }}>
										Part-of-speech
									</td>
									<td>{lemmaInference.Tag ? lemmaInference.Tag.Type: "[not found]"}</td>
								</tr>
								<tr>
									<td>Estimated lemma</td>
									<td>{lemmaInference.Lemma ? lemmaInference.Lemma : "[not found]"}</td>
								</tr>
								{inflectionsHeaderElement}
								{inflectionsBody}
							</tbody>
						</table>
					</div>
				</div>
			);
		}
	});

	// Component for a word in a sentence.
	var InferredWord = React.createClass({
		getInitialState: function () {
			return {
				selected: false
			}
		},
		onSelect: function () {
			this.state.selected = true;

			this.setState(this.state);

			if (selectedWord) {
				selectedWord.onDeselect();
			}

			var analysisContainer = document.getElementById("analysisContainer");

			ReactDOM.unmountComponentAtNode(analysisContainer);

			ReactDOM.render(
				<InferredLemma lemmaInference={this.props.lemmaInference} 
											 probability={this.props.probability}
											 messages={this.props.messages}></InferredLemma>,
				analysisContainer);

			selectedWord = this;
		},
		onDeselect: function () {
			this.state.selected = false;

			this.setState(this.state);
		},
		render: function () {
			if (this.props.lemmaInference.MinimumLexiconDistance >= 0.0) {
				var cssClass = this.state.selected ? "bg-info clickable text-primary" : "clickable text-primary";

				return (
					<span><span onClick={this.onSelect} className={cssClass }>{this.props.lemmaInference.Form}</span> </span>
				);
			}
			else {
				return <span>{this.props.lemmaInference.Form} </span>
			}
		}
	});

	// Component for a sentence.
	var InferredSentence = React.createClass({
		render: function () {
			var dis = this;

			return (
				<span>
					{this.props.sentenceInference.LemmaInferences.map(function (lemmaInference) {
						return (
							<InferredWord lemmaInference={lemmaInference} 
														probability={dis.props.sentenceInference.Probability}
														messages={dis.props.messages}></InferredWord>);
					})}
				</span>
			);
		}
	});

	// The root component for the inferred text.
	var InferredText = React.createClass({
		getInitialState: function () {
			return {
				sentenceResponses: sentenceResponses
			};
		},
		render: function () {
			return (
				<div className="well typography-classic text-large">
					{this.state.sentenceResponses.map(function (sentenceResponse) {
						return (<InferredSentence sentenceInference={sentenceResponse.SentenceInference}
																			messages={sentenceResponse.Messages}></InferredSentence>);
					})}
				</div>
			);
		}
	});

	// Initialization.
	$(function () {
		// Connect to SignalR hub.

		var inferenceHub = $.connection.inferenceHub;

		inferenceHub.client.SetProgress = function (percentage) {
			commonView.setProgressBarValue("#progressBar", percentage.toFixed(0));
		}

		$.connection.hub.start().done(function () {
			$.ajax({
				url: "/API/Inference/Infer",
				method: "POST",
				data: {
					Text: $("#textContainer").text(),
					ConnectionID: $.connection.hub.id
				},
				dataType: "json",
				error: function (jqXHR, textStatus, errorThrown) {
					commonView.errorAlertForAjaxResult(jqXHR);

					$("#progressIndicator").fadeOut(1000, function () {
						$("#errorPromptContainer").fadeIn(1000);
					});
				},
				success: function (data, textStatus, jqXHR) {
					sentenceResponses = data;

					$("#progressIndicator").fadeOut(1000, function () {
						$("#resultsContainer").fadeIn(1000);
					});

					ReactDOM.render(<InferredText></InferredText>, document.getElementById("inferredTextContainer"));
				}
			});
		});

	});

})();